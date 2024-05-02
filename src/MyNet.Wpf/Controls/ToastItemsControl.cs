// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Controls
{
    internal class ToastItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ShouldReverseItemsProperty = DependencyProperty.Register(nameof(ShouldReverseItems), typeof(bool), typeof(ToastItemsControl), new FrameworkPropertyMetadata(default(bool), ShouldReverseItemsPropertyChanged));

        public bool ShouldReverseItems
        {
            get => (bool)GetValue(ShouldReverseItemsProperty);
            set => SetValue(ShouldReverseItemsProperty, value);
        }

        public ToastItemsControl() => Loaded += OnLoaded;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) => PrepareItemsControl(this, ShouldReverseItems);

        private static void ShouldReverseItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ToastItemsControl itemsControl)
                return;

            var shouldReverse = (bool)e.NewValue;

            PrepareItemsControl(itemsControl, shouldReverse);
        }

        private static void PrepareItemsControl(ItemsControl itemsControl, bool reverse)
        {
            var itemPanel = itemsControl.FindVisualChild<ItemsPresenter>()?.FindVisualChild<Panel>();
            if (itemPanel == null)
                return;

            var scaleY = reverse ? -1 : 1;

            itemPanel.LayoutTransform = new ScaleTransform(1, scaleY);
            var itemContainerStyle = itemsControl.ItemContainerStyle == null ? new Style() : CopyStyle(itemsControl.ItemContainerStyle);
            var setter = new Setter
            {
                Property = LayoutTransformProperty,
                Value = new ScaleTransform(1, scaleY)
            };
            itemContainerStyle.Setters.Add(setter);
            itemsControl.ItemContainerStyle = itemContainerStyle;
        }

        private static Style CopyStyle(Style style)
        {
            var styleCopy = new Style();
            foreach (var currentSetter in style.Setters)
            {
                styleCopy.Setters.Add(currentSetter);
            }
            foreach (var currentTrigger in style.Triggers)
            {
                styleCopy.Triggers.Add(currentTrigger);
            }
            return styleCopy;
        }

        protected override DependencyObject GetContainerForItemOverride() => new ToastItem();
    }
}
