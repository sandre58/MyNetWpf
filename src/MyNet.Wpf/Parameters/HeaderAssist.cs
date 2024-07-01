// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class HeaderAssist
    {
        #region Background

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
            "Background",
            typeof(Brush),
            typeof(HeaderAssist),
            new PropertyMetadata(null));

        public static Brush GetBackground(DependencyObject item) => (Brush)item.GetValue(BackgroundProperty);

        public static void SetBackground(DependencyObject item, Brush value) => item.SetValue(BackgroundProperty, value);

        #endregion Background

        #region Foreground

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(HeaderAssist),
            new PropertyMetadata(null));

        public static Brush GetForeground(DependencyObject item) => (Brush)item.GetValue(ForegroundProperty);

        public static void SetForeground(DependencyObject item, Brush value) => item.SetValue(ForegroundProperty, value);

        #endregion Foreground

        #region Padding

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.RegisterAttached(
            "Padding",
            typeof(Thickness),
            typeof(HeaderAssist),
            new PropertyMetadata(new Thickness(0)));

        public static Thickness GetPadding(DependencyObject item) => (Thickness)item.GetValue(PaddingProperty);

        public static void SetPadding(DependencyObject item, Thickness value) => item.SetValue(PaddingProperty, value);

        #endregion Padding

        #region FontSize

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.RegisterAttached(
            "FontSize",
            typeof(double),
            typeof(HeaderAssist),
            new PropertyMetadata(12.0));

        public static double GetFontSize(DependencyObject item) => (double)item.GetValue(FontSizeProperty);

        public static void SetFontSize(DependencyObject item, double value) => item.SetValue(FontSizeProperty, value);

        #endregion FontSize

        #region FontWeight

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.RegisterAttached(
            "FontWeight",
            typeof(FontWeight),
            typeof(HeaderAssist),
            new PropertyMetadata(new FontWeight()));

        public static FontWeight GetFontWeight(DependencyObject item) => (FontWeight)item.GetValue(FontWeightProperty);

        public static void SetFontWeight(DependencyObject item, FontWeight value) => item.SetValue(FontWeightProperty, value);

        #endregion FontWeight

        #region Alignment

        public static readonly DependencyProperty AlignmentProperty = DependencyProperty.RegisterAttached(
            "Alignment",
            typeof(Alignment),
            typeof(HeaderAssist),
            new PropertyMetadata(Alignment.Top));

        public static Alignment GetAlignment(DependencyObject item) => (Alignment)item.GetValue(AlignmentProperty);

        public static void SetAlignment(DependencyObject item, Alignment value) => item.SetValue(AlignmentProperty, value);

        #endregion Alignment

        #region HorizontalAlignment

        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
            "HorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(HeaderAssist),
            new PropertyMetadata(HorizontalAlignment.Stretch));

        public static HorizontalAlignment GetHorizontalAlignment(DependencyObject item) => (HorizontalAlignment)item.GetValue(HorizontalAlignmentProperty);

        public static void SetHorizontalAlignment(DependencyObject item, HorizontalAlignment value) => item.SetValue(HorizontalAlignmentProperty, value);

        #endregion HorizontalAlignment

        #region VerticalAlignment

        public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.RegisterAttached(
            "VerticalAlignment",
            typeof(VerticalAlignment),
            typeof(HeaderAssist),
            new PropertyMetadata(VerticalAlignment.Center));

        public static VerticalAlignment GetVerticalAlignment(DependencyObject item) => (VerticalAlignment)item.GetValue(VerticalAlignmentProperty);

        public static void SetVerticalAlignment(DependencyObject item, VerticalAlignment value) => item.SetValue(VerticalAlignmentProperty, value);

        #endregion VerticalAlignment

        #region Opacity

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.RegisterAttached(
            "Opacity",
            typeof(double),
            typeof(HeaderAssist),
            new PropertyMetadata(1.0));

        public static double GetOpacity(DependencyObject item) => (double)item.GetValue(OpacityProperty);

        public static void SetOpacity(DependencyObject item, double value) => item.SetValue(OpacityProperty, value);

        #endregion Opacity

        #region Capitals

        public static readonly DependencyProperty CapitalsProperty = DependencyProperty.RegisterAttached(
            "Capitals",
            typeof(FontCapitals),
            typeof(HeaderAssist),
            new PropertyMetadata(FontCapitals.AllSmallCaps));

        public static FontCapitals GetCapitals(DependencyObject item) => (FontCapitals)item.GetValue(CapitalsProperty);

        public static void SetCapitals(DependencyObject item, FontCapitals value) => item.SetValue(CapitalsProperty, value);

        #endregion CommandParameter

        #region Size

        public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached(
            "Size",
            typeof(GridLength),
            typeof(HeaderAssist),
            new PropertyMetadata(GridLength.Auto));

        public static GridLength GetSize(DependencyObject item) => (GridLength)item.GetValue(SizeProperty);

        public static void SetSize(DependencyObject item, GridLength value) => item.SetValue(SizeProperty, value);

        #endregion Size

        #region Height

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
            "Height",
            typeof(double),
            typeof(HeaderAssist),
            new PropertyMetadata(35d));

        public static double GetHeight(DependencyObject item) => (double)item.GetValue(HeightProperty);

        public static void SetHeight(DependencyObject item, double value) => item.SetValue(HeightProperty, value);

        #endregion Height

        #region Width

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
            "Width",
            typeof(double),
            typeof(HeaderAssist),
            new PropertyMetadata(50d));

        public static double GetWidth(DependencyObject item) => (double)item.GetValue(WidthProperty);

        public static void SetWidth(DependencyObject item, double value) => item.SetValue(WidthProperty, value);

        #endregion Width

        #region Header

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
            "Header",
            typeof(object),
            typeof(HeaderAssist),
            new PropertyMetadata(null));

        public static object GetHeader(UIElement item) => item.GetValue(HeaderProperty);

        public static void SetHeader(UIElement item, object value) => item.SetValue(HeaderProperty, value);

        #endregion Header

        #region HeaderTemplate

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(HeaderAssist),
            new PropertyMetadata(null));

        public static DataTemplate GetHeaderTemplate(UIElement item) => (DataTemplate)item.GetValue(HeaderProperty);

        public static void SetHeaderTemplate(UIElement item, DataTemplate value) => item.SetValue(HeaderProperty, value);

        #endregion Header
    }
}
