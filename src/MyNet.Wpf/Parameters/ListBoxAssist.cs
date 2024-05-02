// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class ListBoxAssist
    {
        #region Orientation

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached(
            "Orientation",
            typeof(Orientation),
            typeof(ListBoxAssist),
            new PropertyMetadata(Orientation.Horizontal));

        public static Orientation GetOrientation(UIElement item) => (Orientation)item.GetValue(OrientationProperty);

        public static void SetOrientation(UIElement item, Orientation value) => item.SetValue(OrientationProperty, value);

        #endregion Orientation

        #region ScrollSelectedItem

        public static void SetScrollSelectedItem(ListBox listBox, object value) => listBox.SetValue(ScrollSelectedItemProperty, value);

        public static readonly DependencyProperty ScrollSelectedItemProperty =
            DependencyProperty.RegisterAttached("ScrollSelectedItem", typeof(object), typeof(ListBoxAssist),
                                                new UIPropertyMetadata(false, OnScrollSelectedIntoViewChanged));

        private static void OnScrollSelectedIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            if (e.NewValue is not null)
            {
                _ = listBox.Dispatcher.BeginInvoke(
                    () =>
                    {
                        listBox.UpdateLayout();
                        listBox.ScrollIntoView(e.NewValue);
                    });
            }
        }

        #endregion

        #region IsToggle

        public static readonly DependencyProperty IsToggleProperty = DependencyProperty.RegisterAttached(
            "IsToggle", typeof(bool), typeof(ListBoxAssist), new FrameworkPropertyMetadata(default(bool)));

        public static void SetIsToggle(DependencyObject element, bool value)
            => element.SetValue(IsToggleProperty, value);

        public static bool GetIsToggle(DependencyObject element)
            => (bool)element.GetValue(IsToggleProperty);

        #endregion
    }
}
