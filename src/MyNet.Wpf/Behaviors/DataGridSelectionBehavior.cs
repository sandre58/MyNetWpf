// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace MyNet.Wpf.Behaviors
{
    public class DataGridSelectionBehavior : Behavior<DataGrid>
    {
        private bool _updatingSelectedItems;
        private bool _updatingDataGridSelectedItems;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable),
                typeof(DataGridSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        public IEnumerable? SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not DataGridSelectionBehavior behavior) return;

            behavior.UpdateCollectionChangedCallbackOnSelectedItems(args.OldValue, args.NewValue);
            behavior.OnSelectedItemsChanged();
        }

        private void UpdateCollectionChangedCallbackOnSelectedItems(object? oldValue, object? newValue)
        {
            if (oldValue is INotifyCollectionChanged obs) obs.CollectionChanged -= SelectedItems_CollectionChanged;
            if (newValue is INotifyCollectionChanged obs1) obs1.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => OnSelectedItemsChanged();

        private void OnSelectedItemsChanged()
        {
            if (_updatingSelectedItems) return;

            SelectItems();
        }

        // Propagate selected items from model to view
        private void SelectItems()
        {
            if (AssociatedObject == null || AssociatedObject.SelectionMode == DataGridSelectionMode.Single || AssociatedObject.SelectionUnit == DataGridSelectionUnit.Cell || _updatingDataGridSelectedItems) return;

            AssociatedObject.SelectionChanged -= OnDataGridSelectionChanged;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    _ = AssociatedObject.SelectedItems.Add(item);
                }
            }
            AssociatedObject.SelectionChanged += OnDataGridSelectionChanged;
        }

        // Propagate selected items from view to model
        private void OnDataGridSelectionChanged(object? sender, SelectionChangedEventArgs args)
        {
            if (AssociatedObject == null || AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            _updatingDataGridSelectedItems = true;
            _updatingSelectedItems = true;
            SelectedItems = AssociatedObject?.SelectedItems.Cast<object>().ToList();
            _updatingSelectedItems = false;
            _updatingDataGridSelectedItems = false;
        }

        // Re-select items when the set of items changes
        private void OnDataGridItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (AssociatedObject == null || AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            SelectItems();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnDataGridSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnDataGridItemsChanged;

            SelectItems();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= OnDataGridSelectionChanged;
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnDataGridItemsChanged;
            }
        }
    }
}
