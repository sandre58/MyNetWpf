// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DynamicData;
using DynamicData.Binding;
using Microsoft.Xaml.Behaviors;
using MyNet.UI.Busy;
using MyNet.UI.Busy.Models;
using MyNet.Utilities;
using MyNet.Utilities.Collections;
using MyNet.Utilities.Threading;
using MyNet.Wpf.Busy;

namespace MyNet.Wpf.Behaviors
{
    public class ItemsControlAsyncBehavior : Behavior<ItemsControl>
    {
        private readonly IDisposable _disposable;
        private readonly ThreadSafeObservableCollection<object> _cacheItems = [];
        private readonly ObservableCollection<object> _items = [];
        private readonly SingleTaskRunner _loadItems;

        public ItemsControlAsyncBehavior()
        {
            _loadItems = new(async x => await SynchronizeItemsAsync(x).ConfigureAwait(false), null, () => Dispatcher.BeginInvoke(() => GetBusyService(AssociatedObject).Resume()));
            _disposable = _cacheItems.ToObservableChangeSet().Throttle(200.Milliseconds()).Subscribe(_ => LoadItems());
        }

        ~ItemsControlAsyncBehavior() => _disposable.Dispose();

        public static readonly DependencyProperty ItemsProperty
            = DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(ItemsControlAsyncBehavior), new FrameworkPropertyMetadata(OnItemsChangedCallback));

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        #region BusyService

        public static readonly DependencyProperty BusyServiceProperty = DependencyProperty.RegisterAttached(
            "BusyService", typeof(IBusyService), typeof(ItemsControlAsyncBehavior), new PropertyMetadata(new BusyService()));

        public static void SetBusyService(DependencyObject element, IBusyService value) => element.SetValue(BusyServiceProperty, value);

        public static IBusyService GetBusyService(DependencyObject element) => (IBusyService)element.GetValue(BusyServiceProperty);

        #endregion

        private static void OnItemsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ItemsControlAsyncBehavior behavior) return;

            behavior.OnItemsChanged(args);
        }

        private void OnItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                _cacheItems.Clear();
                newCollection.CollectionChanged -= Items_CollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection1)
            {
                newCollection1.CollectionChanged += Items_CollectionChanged;
                _cacheItems.AddRange(((IEnumerable)e.NewValue).OfType<object>());
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.IfNotNull(x => _cacheItems.AddRange(x.OfType<object>().Where(y => !_cacheItems.Contains(y))));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.IfNotNull(x => _cacheItems.RemoveMany(x.OfType<object>()));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (((IEnumerable?)sender)?.OfType<object>().Count() == 0)
                        _cacheItems.Clear();
                    break;
                default:
                    break;
            }
        }

        private void LoadItems()
        {
            _loadItems.Cancel();
            _loadItems.Run();
        }

        private async Task RemoveItemsAsync(IList items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Dispatcher.BeginInvoke(() => _items.Remove(item));
            }
        }

        private async Task AddItemsAsync(IList items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Dispatcher.BeginInvoke(() => _items.Add(item));
                await Task.Delay(5.Milliseconds(), cancellationToken);
            }
        }

        private async Task SynchronizeItemsAsync(CancellationToken cancellationToken)
        => await Dispatcher.Invoke(() => GetBusyService(AssociatedObject)).WaitAsync<IndeterminateBusy>(async _ =>
        {
            try
            {
                var sourceList = _cacheItems.ToList();
                var destinationList = Dispatcher.Invoke(() => _items.OfType<object>().ToList());
                var predicate = new Func<object, object, bool>(Equals);

                // Delete
                var toDelete = destinationList.Where(x => !sourceList.Exists(y => predicate(y, x))).ToList();
                await RemoveItemsAsync(toDelete, cancellationToken).ConfigureAwait(false);

                // Add
                var toAdd = sourceList.Where(x => !destinationList.Any(y => predicate(x, y))).ToList();
                await AddItemsAsync(toAdd, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Noting
            }
        }).ConfigureAwait(false);

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SetValue(ItemsControl.ItemsSourceProperty, _items);
        }
    }
}
