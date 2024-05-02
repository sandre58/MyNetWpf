// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

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
    }
}
