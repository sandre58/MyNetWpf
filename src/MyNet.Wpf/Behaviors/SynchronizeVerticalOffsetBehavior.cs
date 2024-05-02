// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using Microsoft.Xaml.Behaviors;
using MyNet.Wpf.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Behaviors
{
    public class SynchronizeVerticalOffsetBehavior : Behavior<FrameworkElement>
    {
        public static ScrollViewer? GetScrollViewer(FrameworkElement element) => element == null ? null : element is ScrollViewer viewer ? viewer : element.FindVisualChild<ScrollViewer>();

        public bool Vertical
        {
            get => (bool)GetValue(VerticalProperty);
            set => SetValue(VerticalProperty, value);
        }

        public static readonly DependencyProperty VerticalProperty = DependencyProperty.RegisterAttached(nameof(Vertical), typeof(bool), typeof(SynchronizeVerticalOffsetBehavior));

        public bool Horizontal
        {
            get => (bool)GetValue(HorizontalProperty);
            set => SetValue(HorizontalProperty, value);
        }

        public static readonly DependencyProperty HorizontalProperty = DependencyProperty.RegisterAttached(nameof(Horizontal), typeof(bool), typeof(SynchronizeVerticalOffsetBehavior));

        public FrameworkElement Source
        {
            get => (FrameworkElement)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(nameof(Source), typeof(FrameworkElement), typeof(SynchronizeVerticalOffsetBehavior), new PropertyMetadata(null, SourceChangedCallBack));

        private static void SourceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SynchronizeVerticalOffsetBehavior synchronizeVerticalOffsetBehavior && e.NewValue is FrameworkElement newSource)
            {
                var newScrollViewer = GetScrollViewer(newSource);
                AddBehavior(synchronizeVerticalOffsetBehavior, newScrollViewer, GetScrollViewer(synchronizeVerticalOffsetBehavior.AssociatedObject));
            }
        }

        private static void AddBehavior(SynchronizeVerticalOffsetBehavior behavior, ScrollViewer? sourceScrollViewer, ScrollViewer? targetScrollViewer)
            => behavior.AddBehavior(sourceScrollViewer, targetScrollViewer);

        private void AddBehavior(ScrollViewer? sourceScrollViewer, ScrollViewer? targetScrollViewer)
        {
            if (sourceScrollViewer != null)
            {
                sourceScrollViewer.ScrollChanged += (sender, e) => UpdateTargetViewAccordingToSource(sourceScrollViewer, targetScrollViewer);
                UpdateTargetViewAccordingToSource(sourceScrollViewer, targetScrollViewer);
            }
        }

        private void UpdateTargetViewAccordingToSource(ScrollViewer? sourceScrollViewer, ScrollViewer? targetScrollViewer)
        {
            if (sourceScrollViewer != null && targetScrollViewer != null)
            {
                if (Vertical) targetScrollViewer.ScrollToVerticalOffset(sourceScrollViewer.VerticalOffset);
                if (Horizontal) targetScrollViewer.ScrollToHorizontalOffset(sourceScrollViewer.HorizontalOffset);
                targetScrollViewer.UpdateLayout();
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AddBehavior(GetScrollViewer(AssociatedObject), GetScrollViewer(Source));
            AddBehavior(GetScrollViewer(Source), GetScrollViewer(AssociatedObject));
        }
    }
}
