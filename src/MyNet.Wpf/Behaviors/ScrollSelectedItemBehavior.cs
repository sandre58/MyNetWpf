// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Behaviors
{
    public sealed class ScrollSelectedItemBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty ItemProperty =
    DependencyProperty.Register(nameof(Item), typeof(object),
        typeof(ScrollSelectedItemBehavior));

        public object Item
        {
            get => GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.IsLoaded)
                AddBehavior();
            else
                AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, EventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AddBehavior();
        }

        private void AddBehavior()
        {
            var scrollViewer = AssociatedObject.FindVisualChild<ScrollViewer>();
            var index = AssociatedObject.Items.IndexOf(Item);

            if (scrollViewer is null || AssociatedObject.Items.Count == 0 || index <= -1) return;

            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight * index / AssociatedObject.Items.Count);
        }
    }
}
