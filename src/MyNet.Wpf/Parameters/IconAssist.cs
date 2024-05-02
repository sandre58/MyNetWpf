// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class IconAssist
    {
        #region Icon

        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
            "Icon",
            typeof(object),
            typeof(IconAssist),
            new PropertyMetadata(null));

        public static object GetIcon(DependencyObject item) => item.GetValue(IconProperty);

        public static void SetIcon(DependencyObject item, object value) => item.SetValue(IconProperty, value);

        #endregion Icon

        #region Opacity

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.RegisterAttached(
            "Opacity",
            typeof(double),
            typeof(IconAssist),
            new PropertyMetadata(0.8));

        public static double GetOpacity(DependencyObject item) => (double)item.GetValue(OpacityProperty);

        public static void SetOpacity(DependencyObject item, double value) => item.SetValue(OpacityProperty, value);

        #endregion Opacity

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.RegisterAttached(
            "Alignment",
            typeof(Alignment),
            typeof(IconAssist),
            new PropertyMetadata(Alignment.Left));

        public static Alignment GetAlignment(DependencyObject item) => (Alignment)item.GetValue(AlignmentProperty);

        public static void SetAlignment(DependencyObject item, Alignment value) => item.SetValue(AlignmentProperty, value);

        #endregion Alignment

        #region Margin

        public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached(
            "Margin",
            typeof(Thickness),
            typeof(IconAssist),
            new PropertyMetadata(new Thickness(0, 0, 5, 0)));

        public static Thickness GetMargin(DependencyObject item) => (Thickness)item.GetValue(MarginProperty);

        public static void SetMargin(DependencyObject item, Thickness value) => item.SetValue(MarginProperty, value);

        #endregion Margin
    }
}
