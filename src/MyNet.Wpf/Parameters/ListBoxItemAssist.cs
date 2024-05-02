// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class ListBoxItemAssist
    {
        #region IsMouseOverBackground

        public static readonly DependencyProperty IsMouseOverBackgroundProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverBackground",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverBackground(UIElement item) => (Brush)item.GetValue(IsMouseOverBackgroundProperty);

        public static void SetIsMouseOverBackground(UIElement item, Brush value) => item.SetValue(IsMouseOverBackgroundProperty, value);

        #endregion IsMouseOverBackground

        #region IsMouseOverBorderBrush

        public static readonly DependencyProperty IsMouseOverBorderBrushProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverBorderBrush",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverBorderBrush(UIElement item) => (Brush)item.GetValue(IsMouseOverBorderBrushProperty);

        public static void SetIsMouseOverBorderBrush(UIElement item, Brush value) => item.SetValue(IsMouseOverBorderBrushProperty, value);

        #endregion IsMouseOverBackground

        #region IsMouseOverForeground

        public static readonly DependencyProperty IsMouseOverForegroundProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverForeground",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverForeground(UIElement item) => (Brush)item.GetValue(IsMouseOverForegroundProperty);

        public static void SetIsMouseOverForeground(UIElement item, Brush value) => item.SetValue(IsMouseOverForegroundProperty, value);

        #endregion IsMouseOverForeground

        #region IsSelectedBackground

        public static readonly DependencyProperty IsSelectedBackgroundProperty = DependencyProperty.RegisterAttached(
            "IsSelectedBackground",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsSelectedBackground(UIElement item) => (Brush)item.GetValue(IsSelectedBackgroundProperty);

        public static void SetIsSelectedBackground(UIElement item, Brush value) => item.SetValue(IsSelectedBackgroundProperty, value);

        #endregion IsSelectedBackground

        #region IsSelectedBorderBrush

        public static readonly DependencyProperty IsSelectedBorderBrushProperty = DependencyProperty.RegisterAttached(
            "IsSelectedBorderBrush",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsSelectedBorderBrush(UIElement item) => (Brush)item.GetValue(IsSelectedBorderBrushProperty);

        public static void SetIsSelectedBorderBrush(UIElement item, Brush value) => item.SetValue(IsSelectedBorderBrushProperty, value);

        #endregion IsSelectedBorderBrush

        #region IsSelectedForeground

        public static readonly DependencyProperty IsSelectedForegroundProperty = DependencyProperty.RegisterAttached(
            "IsSelectedForeground",
            typeof(Brush),
            typeof(ListBoxItemAssist),
            new PropertyMetadata(null));

        public static Brush GetIsSelectedForeground(UIElement item) => (Brush)item.GetValue(IsSelectedForegroundProperty);

        public static void SetIsSelectedForeground(UIElement item, Brush value) => item.SetValue(IsSelectedForegroundProperty, value);

        #endregion IsSelectedForeground

        #region ShowSelection

        public static bool GetShowSelection(DependencyObject element)
            => (bool)element.GetValue(ShowSelectionProperty);
        public static void SetShowSelection(DependencyObject element, bool value)
            => element.SetValue(ShowSelectionProperty, value);

        public static readonly DependencyProperty ShowSelectionProperty =
            DependencyProperty.RegisterAttached("ShowSelection", typeof(bool), typeof(ListBoxItemAssist), new PropertyMetadata(true));

        #endregion
    }
}
