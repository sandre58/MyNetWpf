// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class SliderAssist
    {
        public static readonly DependencyProperty OnlyShowFocusVisualWhileDraggingProperty
            = DependencyProperty.RegisterAttached(
                "OnlyShowFocusVisualWhileDragging",
                typeof(bool),
                typeof(SliderAssist),
                new PropertyMetadata(false));

        public static void SetOnlyShowFocusVisualWhileDragging(RangeBase element, bool value)
            => element.SetValue(OnlyShowFocusVisualWhileDraggingProperty, value);

        public static bool GetOnlyShowFocusVisualWhileDragging(RangeBase element)
            => (bool)element.GetValue(OnlyShowFocusVisualWhileDraggingProperty);

        public static readonly DependencyProperty ToolTipFormatProperty
            = DependencyProperty.RegisterAttached(
                "ToolTipFormat",
                typeof(string),
                typeof(SliderAssist),
                new PropertyMetadata(null));

        public static void SetToolTipFormat(RangeBase element, string value)
            => element.SetValue(ToolTipFormatProperty, value);

        public static string GetToolTipFormat(RangeBase element)
            => (string)element.GetValue(ToolTipFormatProperty);

        #region BarSize

        public static readonly DependencyProperty BarSizeProperty
            = DependencyProperty.RegisterAttached(
                "BarSize",
                typeof(double),
                typeof(SliderAssist),
                new PropertyMetadata(4.0));

        public static void SetBarSize(RangeBase element, double value)
            => element.SetValue(BarSizeProperty, value);

        public static double GetBarSize(RangeBase element)
            => (double)element.GetValue(BarSizeProperty);

        #endregion

        #region ActiveBarSize

        public static readonly DependencyProperty ActiveBarSizeProperty
            = DependencyProperty.RegisterAttached(
                "ActiveBarSize",
                typeof(double),
                typeof(SliderAssist),
                new PropertyMetadata(6.0));

        public static void SetActiveBarSize(RangeBase element, double value)
            => element.SetValue(ActiveBarSizeProperty, value);

        public static double GetActiveBarSize(RangeBase element)
            => (double)element.GetValue(ActiveBarSizeProperty);

        #endregion

        #region ThumbBackground

        public static readonly DependencyProperty ThumbBackgroundProperty = DependencyProperty.RegisterAttached(
            "ThumbBackground",
            typeof(Brush),
            typeof(SliderAssist),
            new PropertyMetadata(null));

        public static Brush GetThumbBackground(UIElement item) => (Brush)item.GetValue(ThumbBackgroundProperty);

        public static void SetThumbBackground(UIElement item, Brush value) => item.SetValue(ThumbBackgroundProperty, value);

        #endregion ThumbBackground

        #region ShowTransparencyBackground

        public static readonly DependencyProperty ShowTransparencyBackgroundProperty = DependencyProperty.RegisterAttached(
            "ShowTransparencyBackground",
            typeof(bool),
            typeof(SliderAssist),
            new PropertyMetadata(false));

        public static bool GetShowTransparencyBackground(UIElement item) => (bool)item.GetValue(ShowTransparencyBackgroundProperty);

        public static void SetShowTransparencyBackground(UIElement item, bool value) => item.SetValue(ShowTransparencyBackgroundProperty, value);

        #endregion ShowTransparencyBackground
    }
}
