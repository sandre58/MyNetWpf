// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace MyNet.Wpf.Parameters
{
    public static class PopupAssist
    {
        #region Background

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
            "Background",
            typeof(Brush),
            typeof(PopupAssist),
            new PropertyMetadata(null));

        public static Brush GetBackground(UIElement item) => (Brush)item.GetValue(BackgroundProperty);

        public static void SetBackground(UIElement item, Brush value) => item.SetValue(BackgroundProperty, value);

        #endregion Background

        #region Foreground

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
            "Foreground",
            typeof(Brush),
            typeof(PopupAssist),
            new PropertyMetadata(null));

        public static Brush GetForeground(UIElement item) => (Brush)item.GetValue(ForegroundProperty);

        public static void SetForeground(UIElement item, Brush value) => item.SetValue(ForegroundProperty, value);

        #endregion Foreground

        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content",
            typeof(object),
            typeof(PopupAssist),
            new PropertyMetadata(null));

        public static object GetContent(UIElement item) => item.GetValue(ContentProperty);

        public static void SetContent(UIElement item, object value) => item.SetValue(ContentProperty, value);

        #endregion Content

        #region ContentTemplate

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.RegisterAttached(
            "ContentTemplate",
            typeof(DataTemplate),
            typeof(PopupAssist),
            new PropertyMetadata(null));

        public static DataTemplate GetContentTemplate(UIElement item) => (DataTemplate)item.GetValue(ContentProperty);

        public static void SetContentTemplate(UIElement item, DataTemplate value) => item.SetValue(ContentProperty, value);

        #endregion Content

        #region PlacementMode

        public static readonly DependencyProperty PlacementModeProperty = DependencyProperty.RegisterAttached(
            "PlacementMode",
            typeof(PopupBoxPlacementMode?),
            typeof(PopupAssist),
            new PropertyMetadata(null));

        public static PopupBoxPlacementMode? GetPlacementMode(UIElement item) => (PopupBoxPlacementMode?)item.GetValue(PlacementModeProperty);

        public static void SetPlacementMode(UIElement item, PopupBoxPlacementMode? value) => item.SetValue(PlacementModeProperty, value);

        #endregion PlacementMode

        #region ShowIndicator

        public static readonly DependencyProperty ShowIndicatorProperty = DependencyProperty.RegisterAttached(
            "ShowIndicator",
            typeof(bool),
            typeof(PopupAssist),
            new PropertyMetadata(false));

        public static bool GetShowIndicator(UIElement item) => (bool)item.GetValue(ShowIndicatorProperty);

        public static void SetShowIndicator(UIElement item, bool? value) => item.SetValue(ShowIndicatorProperty, value);

        #endregion ShowIndicator
    }
}
