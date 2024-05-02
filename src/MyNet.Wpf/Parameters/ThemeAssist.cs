// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class ThemeAssist
    {
        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static CornerRadius GetCornerRadius(UIElement item) => (CornerRadius)item.GetValue(CornerRadiusProperty);

        public static void SetCornerRadius(UIElement item, CornerRadius value) => item.SetValue(CornerRadiusProperty, value);

        #endregion CornerRadius

        #region UniformCornerRadius

        public static readonly DependencyProperty UniformCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "UniformCornerRadius",
            typeof(double),
            typeof(ThemeAssist),
            new PropertyMetadata(2.0d));

        public static double GetUniformCornerRadius(DependencyObject item) => (double)item.GetValue(UniformCornerRadiusProperty);

        public static void SetUniformCornerRadius(DependencyObject item, double value) => item.SetValue(UniformCornerRadiusProperty, value);

        #endregion UniformCornerRadius

        #region IsMouseOverBackground

        public static readonly DependencyProperty IsMouseOverBackgroundProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverBackground",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverBackground(UIElement item) => (Brush)item.GetValue(IsMouseOverBackgroundProperty);

        public static void SetIsMouseOverBackground(UIElement item, Brush value) => item.SetValue(IsMouseOverBackgroundProperty, value);

        #endregion IsMouseOverBackground

        #region IsMouseOverBorderBrush

        public static readonly DependencyProperty IsMouseOverBorderBrushProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverBorderBrush",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverBorderBrush(UIElement item) => (Brush)item.GetValue(IsMouseOverBorderBrushProperty);

        public static void SetIsMouseOverBorderBrush(UIElement item, Brush value) => item.SetValue(IsMouseOverBorderBrushProperty, value);

        #endregion IsMouseOverBackground

        #region IsMouseOverForeground

        public static readonly DependencyProperty IsMouseOverForegroundProperty = DependencyProperty.RegisterAttached(
            "IsMouseOverForeground",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsMouseOverForeground(UIElement item) => (Brush)item.GetValue(IsMouseOverForegroundProperty);

        public static void SetIsMouseOverForeground(UIElement item, Brush value) => item.SetValue(IsMouseOverForegroundProperty, value);

        #endregion IsMouseOverForeground

        #region IsCheckedBackground

        public static readonly DependencyProperty IsCheckedBackgroundProperty = DependencyProperty.RegisterAttached(
            "IsCheckedBackground",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsCheckedBackground(UIElement item) => (Brush)item.GetValue(IsCheckedBackgroundProperty);

        public static void SetIsCheckedBackground(UIElement item, Brush value) => item.SetValue(IsCheckedBackgroundProperty, value);

        #endregion IsCheckedBackground

        #region IsCheckedBorderBrush

        public static readonly DependencyProperty IsCheckedBorderBrushProperty = DependencyProperty.RegisterAttached(
            "IsCheckedBorderBrush",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsCheckedBorderBrush(UIElement item) => (Brush)item.GetValue(IsCheckedBorderBrushProperty);

        public static void SetIsCheckedBorderBrush(UIElement item, Brush value) => item.SetValue(IsCheckedBorderBrushProperty, value);

        #endregion IsCheckedBorderBrush

        #region IsCheckedForeground

        public static readonly DependencyProperty IsCheckedForegroundProperty = DependencyProperty.RegisterAttached(
            "IsCheckedForeground",
            typeof(Brush),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Brush GetIsCheckedForeground(UIElement item) => (Brush)item.GetValue(IsCheckedForegroundProperty);

        public static void SetIsCheckedForeground(UIElement item, Brush value) => item.SetValue(IsCheckedForegroundProperty, value);

        #endregion IsCheckedForeground

        #region Style

        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
            "Style",
            typeof(Style),
            typeof(ThemeAssist),
            new PropertyMetadata(null));

        public static Style GetStyle(UIElement item) => (Style)item.GetValue(StyleProperty);

        public static void SetStyle(UIElement item, Style value) => item.SetValue(StyleProperty, value);

        #endregion Style
    }
}
