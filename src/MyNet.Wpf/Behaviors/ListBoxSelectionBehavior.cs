// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using MyNet.Utilities;

namespace MyNet.Wpf.Behaviors
{
    public class ListBoxSelectionBehavior : Behavior<ListBox>
    {
        private bool _updatingSelectedItems;
        private bool _updatingSelectedValues;
        private bool _updatingListBoxSelectedItems;

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable),
                typeof(ListBoxSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemsChanged));

        public IEnumerable? SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.Register(nameof(SelectedValues), typeof(IEnumerable),
                typeof(ListBoxSelectionBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedValuesChanged));

        public IEnumerable? SelectedValues
        {
            get => (IEnumerable)GetValue(SelectedValuesProperty);
            set => SetValue(SelectedValuesProperty, value);
        }

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ListBoxSelectionBehavior behavior) return;

            behavior.UpdateCollectionChangedCallbackOnSelectedItems(args.OldValue, args.NewValue);
            behavior.OnSelectedItemsChanged();
        }

        private static void OnSelectedValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ListBoxSelectionBehavior behavior) return;

            behavior.UpdateCollectionChangedCallbackOnSelectedValues(args.OldValue, args.NewValue);
            behavior.OnSelectedValuesChanged();
        }

        private void UpdateCollectionChangedCallbackOnSelectedValues(object? oldValue, object? newValue)
        {
            if (oldValue is INotifyCollectionChanged obs) obs.CollectionChanged -= SelectedValues_CollectionChanged;
            if (newValue is INotifyCollectionChanged obs1) obs1.CollectionChanged += SelectedValues_CollectionChanged;
        }

        private void SelectedValues_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => OnSelectedValuesChanged();

        private void UpdateCollectionChangedCallbackOnSelectedItems(object? oldValue, object? newValue)
        {
            if (oldValue is INotifyCollectionChanged obs) obs.CollectionChanged -= SelectedItems_CollectionChanged;
            if (newValue is INotifyCollectionChanged obs1) obs1.CollectionChanged += SelectedItems_CollectionChanged;
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => OnSelectedItemsChanged();

        private void OnSelectedItemsChanged()
        {
            if (_updatingSelectedItems) return;

            SelectedItemsToValues();
            SelectItems();
        }

        private void OnSelectedValuesChanged()
        {
            if (_updatingSelectedValues) return;

            SelectedValuesToItems();
            SelectItems();
        }

        // Update SelectedValues based on SelectedItems
        private void SelectedItemsToValues()
        {
            if (AssociatedObject == null) return;

            _updatingSelectedValues = true;
            SelectedValues = SelectedItems?.Cast<object>()
                        .Select(x => x.GetDeepPropertyValue(AssociatedObject.SelectedValuePath))
                        .ToList();
            _updatingSelectedValues = false;
        }

        // Update SelectedItems based on SelectedValues
        private void SelectedValuesToItems()
        {
            if (AssociatedObject == null) return;

            _updatingSelectedItems = true;
            SelectedItems = SelectedValues == null
                ? null
                : AssociatedObject?.Items.Cast<object>()
                        .Where(x => SelectedValues.Cast<object>().Contains(x.GetDeepPropertyValue(AssociatedObject.SelectedValuePath)))
                        .ToList();
            _updatingSelectedItems = false;
        }

        // Propagate selected items from model to view
        private void SelectItems()
        {
            if (AssociatedObject == null || AssociatedObject.SelectionMode == SelectionMode.Single || _updatingListBoxSelectedItems) return;

            AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
            AssociatedObject.SelectedItems.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    _ = AssociatedObject.SelectedItems.Add(item);
                }
            }
            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;
        }

        // Propagate selected items from view to model
        private void OnListBoxSelectionChanged(object? sender, SelectionChangedEventArgs args)
        {
            if (AssociatedObject == null || AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            _updatingListBoxSelectedItems = true;
            _updatingSelectedItems = true;
            SelectedItems = AssociatedObject?.SelectedItems.Cast<object>().ToList();
            SelectedItemsToValues();
            _updatingSelectedItems = false;
            _updatingListBoxSelectedItems = false;
        }

        // Re-select items when the set of items changes
        private void OnListBoxItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (AssociatedObject == null || AssociatedObject.Items.SourceCollection == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(AssociatedObject.SelectedValuePath))
            {
                SelectedValuesToItems();
            }

            SelectItems();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SelectionChanged += OnListBoxSelectionChanged;
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += OnListBoxItemsChanged;

            OnSelectedValuesChanged();
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged -= OnListBoxSelectionChanged;
                ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= OnListBoxItemsChanged;
            }
        }
    }
}
