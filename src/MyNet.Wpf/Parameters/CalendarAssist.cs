// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public enum CalendarOrientation
    {
        Vertical,
        Horizontal
    }
    public static class CalendarAssist
    {
        #region Header Visibility
        public static readonly DependencyProperty IsHeaderVisibleProperty = DependencyProperty.RegisterAttached(
            "IsHeaderVisible", typeof(bool), typeof(CalendarAssist), new PropertyMetadata(true));

        public static bool GetIsHeaderVisible(DependencyObject element) => (bool)element.GetValue(IsHeaderVisibleProperty);
        public static void SetIsHeaderVisible(DependencyObject element, bool value) => element.SetValue(IsHeaderVisibleProperty, value);
        #endregion

        #region Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached(
            "Orientation", typeof(CalendarOrientation), typeof(CalendarAssist), new FrameworkPropertyMetadata(default(CalendarOrientation)));

        public static CalendarOrientation GetOrientation(DependencyObject element) => (CalendarOrientation)element.GetValue(OrientationProperty);
        public static void SetOrientation(DependencyObject element, CalendarOrientation value) => element.SetValue(OrientationProperty, value);
        #endregion
    }
}
