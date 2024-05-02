// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class TabControlAssist
    {
        #region IndicatorBrush

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.RegisterAttached(
            "IndicatorBrush",
            typeof(Brush),
            typeof(TabControlAssist),
            new PropertyMetadata(null));

        public static Brush GetIndicatorBrush(DependencyObject item) => (Brush)item.GetValue(IndicatorBrushProperty);

        public static void SetIndicatorBrush(DependencyObject item, Brush value) => item.SetValue(IndicatorBrushProperty, value);

        #endregion

        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.RegisterAttached(
            "HeaderBackground",
            typeof(Brush),
            typeof(TabControlAssist),
            new PropertyMetadata(null));

        public static Brush GetHeaderBackground(DependencyObject item) => (Brush)item.GetValue(HeaderBackgroundProperty);

        public static void SetHeaderBackground(DependencyObject item, Brush value) => item.SetValue(HeaderBackgroundProperty, value);

        #endregion

        #region ShowHeader

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.RegisterAttached(
            "ShowHeader",
            typeof(bool),
            typeof(TabControlAssist),
            new PropertyMetadata(true));

        public static bool GetShowHeader(DependencyObject item) => (bool)item.GetValue(ShowHeaderProperty);

        public static void SetShowHeader(DependencyObject item, bool value) => item.SetValue(ShowHeaderProperty, value);

        #endregion

        #region HeaderForeground

        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.RegisterAttached(
            "HeaderForeground",
            typeof(Brush),
            typeof(TabControlAssist),
            new PropertyMetadata(null));

        public static Brush GetHeaderForeground(DependencyObject item) => (Brush)item.GetValue(HeaderForegroundProperty);

        public static void SetHeaderForeground(DependencyObject item, Brush value) => item.SetValue(HeaderForegroundProperty, value);

        #endregion

        #region MoreContent

        public static readonly DependencyProperty MoreContentProperty = DependencyProperty.RegisterAttached(
            "MoreContent",
            typeof(object),
            typeof(TabControlAssist),
            new PropertyMetadata(null));

        public static object GetMoreContent(DependencyObject item) => item.GetValue(MoreContentProperty);

        public static void SetMoreContent(DependencyObject item, object value) => item.SetValue(MoreContentProperty, value);

        #endregion

        #region MoreContentAlignment

        public static readonly DependencyProperty MoreContentHorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
            "MoreContentHorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(TabControlAssist),
            new PropertyMetadata(HorizontalAlignment.Right));

        public static HorizontalAlignment GetMoreContentHorizontalAlignment(DependencyObject item) => (HorizontalAlignment)item.GetValue(MoreContentHorizontalAlignmentProperty);

        public static void SetMoreContentHorizontalAlignment(DependencyObject item, HorizontalAlignment value) => item.SetValue(MoreContentHorizontalAlignmentProperty, value);

        public static readonly DependencyProperty MoreContentVerticalAlignmentProperty = DependencyProperty.RegisterAttached(
            "MoreContentVerticalAlignment",
            typeof(VerticalAlignment),
            typeof(TabControlAssist),
            new PropertyMetadata(VerticalAlignment.Center));

        public static VerticalAlignment GetMoreContentVerticalAlignment(DependencyObject item) => (VerticalAlignment)item.GetValue(MoreContentVerticalAlignmentProperty);

        public static void SetMoreContentVerticalAlignment(DependencyObject item, VerticalAlignment value) => item.SetValue(MoreContentVerticalAlignmentProperty, value);

        #endregion

        #region TabItemHeight

        public static readonly DependencyProperty TabItemHeightProperty = DependencyProperty.RegisterAttached(
            "TabItemHeight", typeof(double), typeof(TabControlAssist), new PropertyMetadata(48.0));

        public static void SetTabItemHeight(DependencyObject element, double value) => element.SetValue(TabItemHeightProperty, value);

        public static double GetTabItemHeight(DependencyObject element) => (double)element.GetValue(TabItemHeightProperty);

        #endregion

        #region HasUniformTabWidth

        public static readonly DependencyProperty HasUniformTabWidthProperty = DependencyProperty.RegisterAttached(
            "HasUniformTabWidth", typeof(bool), typeof(TabControlAssist), new PropertyMetadata(false));

        public static void SetHasUniformTabWidth(DependencyObject element, bool value) => element.SetValue(HasUniformTabWidthProperty, value);

        public static bool GetHasUniformTabWidth(DependencyObject element) => (bool)element.GetValue(HasUniformTabWidthProperty);

        #endregion
    }
}
