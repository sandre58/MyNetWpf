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
using MyNet.Observable.Deferrers;
using MyNet.UI.Busy;
using MyNet.UI.Busy.Models;
using MyNet.Utilities;
using MyNet.Utilities.Collections;
using MyNet.Wpf.Busy;

namespace MyNet.Wpf.Behaviors
{
    public class ItemsControlAsyncBehavior : Behavior<ItemsControl>
    {
        private readonly IDisposable _disposable;
        private readonly ThreadSafeObservableCollection<object> _cacheItems = [];
        private readonly ObservableCollection<object> _items = [];
        private readonly SingleTaskDeferrer _loadItemsDeferrer;

        public ItemsControlAsyncBehavior()
        {
            _loadItemsDeferrer = new(async x => await SynchronizeItemsAsync(x).ConfigureAwait(false), null, () => Dispatcher.BeginInvoke(() => GetBusyService(AssociatedObject).Resume()));
            _disposable = _cacheItems.ToObservableChangeSet().Throttle(200.Milliseconds()).Subscribe(_ => _loadItemsDeferrer.AskRefresh());
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
            "BusyService", typeof(IBusyService), typeof(ItemsControlAsyncBehavior), new PropertyMetadata(null));

        public static void SetBusyService(DependencyObject element, IBusyService value) => element.SetValue(BusyServiceProperty, value);

        public static IBusyService GetBusyService(DependencyObject element)
        {
            var busy = (IBusyService)element.GetValue(BusyServiceProperty);

            if (busy is null)
                SetBusyService(element, new BusyService());

            return (IBusyService)element.GetValue(BusyServiceProperty);
        }

        #endregion

        private static void OnItemsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ItemsControlAsyncBehavior behavior) return;

            behavior.OnItemsChanged(args);
        }

        private void OnItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged newCollection)
            {
                if (_cacheItems.Count > 0)
                    _cacheItems.Clear();
                newCollection.CollectionChanged -= Items_CollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newCollection1)
            {
                newCollection1.CollectionChanged += Items_CollectionChanged;
                _cacheItems.AddRange(((IEnumerable)e.NewValue).OfType<object>());
            }
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => _cacheItems.Set(((IEnumerable?)sender)?.OfType<object>());

        private async Task<bool> RemoveItemsAsync(IList items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Dispatcher.BeginInvoke(() => _items.Remove(item));
            }

            return true;
        }

        private async Task<bool> AddItemsAsync(IList items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Dispatcher.BeginInvoke(() => _items.Add(item));
                await Task.Delay(5.Milliseconds(), cancellationToken).ConfigureAwait(false);
            }

            return true;
        }

        private async Task SynchronizeItemsAsync(CancellationToken cancellationToken)
        => await Dispatcher.Invoke(() => GetBusyService(AssociatedObject)).WaitAsync<IndeterminateBusy>(async x =>
        {
            try
            {
                var sourceList = _cacheItems.ToList();
                var destinationList = Dispatcher.Invoke(() => _items.OfType<object>().ToList());
                var predicate = new Func<object, object, bool>(Equals);

                // Delete
                var toDelete = destinationList.Where(x => !sourceList.Exists(y => predicate(y, x))).ToList();
                _ = await RemoveItemsAsync(toDelete, cancellationToken).ConfigureAwait(false);

                // Add
                var toAdd = sourceList.Where(x => !destinationList.Any(y => predicate(x, y))).ToList();
                _ = await AddItemsAsync(toAdd, cancellationToken).ConfigureAwait(false);

                // Move
                for (var i = 0; i < _cacheItems.Count; i++)
                {
                    var cacheItem = _cacheItems[i];
                    var indexOfItem = Dispatcher.Invoke(() => _items.IndexOf(cacheItem));

                    if (indexOfItem >= 0 && i < _items.Count && indexOfItem != i)
                        await Dispatcher.BeginInvoke(() => _items.Move(indexOfItem, i));
                }
            }
            catch (OperationCanceledException)
            {
                // Noting
            }
        }).ConfigureAwait(false);

        protected override void OnAttached()
        {
            base.OnAttached();
            VirtualizingPanel.SetIsVirtualizing(AssociatedObject, false);
            AssociatedObject.SetValue(ItemsControl.ItemsSourceProperty, _items);
        }
    }
}
