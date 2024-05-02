// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Parameters
{
    public static class LayoutAssist
    {
        #region InnerPadding

        public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.RegisterAttached(
            "InnerPadding",
            typeof(Thickness),
            typeof(LayoutAssist),
            new PropertyMetadata(new Thickness(0)));

        public static Thickness GetInnerPadding(UIElement item) => (Thickness)item.GetValue(InnerPaddingProperty);

        public static void SetInnerPadding(UIElement item, Thickness value) => item.SetValue(InnerPaddingProperty, value);

        #endregion InnerPadding

        #region InnerMargin

        public static readonly DependencyProperty InnerMarginProperty = DependencyProperty.RegisterAttached(
            "InnerMargin",
            typeof(Thickness),
            typeof(LayoutAssist),
            new PropertyMetadata(new Thickness(0)));

        public static Thickness GetInnerMargin(UIElement item) => (Thickness)item.GetValue(InnerMarginProperty);

        public static void SetInnerMargin(UIElement item, Thickness value) => item.SetValue(InnerMarginProperty, value);

        #endregion InnerMargin
    }
}
