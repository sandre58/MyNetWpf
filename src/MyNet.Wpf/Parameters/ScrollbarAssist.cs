// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Parameters
{
    public static class ScrollBarAssist
    {
        #region ButtonsVisibility

        public static readonly DependencyProperty ButtonsVisibilityProperty =
            DependencyProperty.RegisterAttached("ButtonsVisibility", typeof(Visibility), typeof(ScrollBarAssist), new PropertyMetadata(Visibility.Visible));

        public static void SetButtonsVisibility(DependencyObject element, Visibility value) => element.SetValue(ButtonsVisibilityProperty, value);

        public static Visibility GetButtonsVisibility(DependencyObject element) => (Visibility)element.GetValue(ButtonsVisibilityProperty);

        #endregion

        #region ThumbCornerRadius

        public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "ThumbCornerRadius", typeof(CornerRadius), typeof(ScrollBarAssist), new PropertyMetadata(default(CornerRadius)));

        public static void SetThumbCornerRadius(DependencyObject element, CornerRadius value) => element.SetValue(ThumbCornerRadiusProperty, value);

        public static CornerRadius GetThumbCornerRadius(DependencyObject element) => (CornerRadius)element.GetValue(ThumbCornerRadiusProperty);

        #endregion

        #region ThumbWidth

        public static readonly DependencyProperty ThumbWidthProperty = DependencyProperty.RegisterAttached(
            "ThumbWidth", typeof(double), typeof(ScrollBarAssist), new PropertyMetadata(double.NaN));

        public static void SetThumbWidth(DependencyObject element, double value) => element.SetValue(ThumbWidthProperty, value);

        public static double GetThumbWidth(DependencyObject element) => (double)element.GetValue(ThumbWidthProperty);

        #endregion

        #region ThumbHeight

        public static readonly DependencyProperty ThumbHeightProperty = DependencyProperty.RegisterAttached(
            "ThumbHeight", typeof(double), typeof(ScrollBarAssist), new PropertyMetadata(double.NaN));

        public static void SetThumbHeight(DependencyObject element, double value) => element.SetValue(ThumbHeightProperty, value);

        public static double GetThumbHeight(DependencyObject element) => (double)element.GetValue(ThumbHeightProperty);

        #endregion
    }
}
