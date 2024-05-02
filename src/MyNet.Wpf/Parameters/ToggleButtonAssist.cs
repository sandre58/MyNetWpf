// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Parameters
{
    public static class ToggleButtonAssist
    {
        private static readonly DependencyPropertyKey HasOnContentPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "HasOnContent", typeof(bool), typeof(ToggleButtonAssist),
                new PropertyMetadata(false));

        public static readonly DependencyProperty HasOnContentProperty = HasOnContentPropertyKey.DependencyProperty;

        private static void SetHasOnContent(DependencyObject element, object value)
            => element.SetValue(HasOnContentPropertyKey, value);

        /// <summary>
        /// Framework use only.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool GetHasOnContent(DependencyObject element)
            => (bool)element.GetValue(HasOnContentProperty);

        /// <summary>
        /// Allows on (IsChecked) content to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        public static readonly DependencyProperty OnContentProperty = DependencyProperty.RegisterAttached(
            "OnContent", typeof(object), typeof(ToggleButtonAssist), new PropertyMetadata(default, OnContentPropertyChangedCallback));

        private static void OnContentPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            => SetHasOnContent(dependencyObject, dependencyPropertyChangedEventArgs.NewValue != null);

        /// <summary>
        /// Allows on (IsChecked) content to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetOnContent(DependencyObject element, object value)
            => element.SetValue(OnContentProperty, value);

        /// <summary>
        /// Allows on (IsChecked) content to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        public static object GetOnContent(DependencyObject element)
            => element.GetValue(OnContentProperty);

        /// <summary>
        /// Allows an on (IsChecked) template to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        public static readonly DependencyProperty OnContentTemplateProperty = DependencyProperty.RegisterAttached(
            "OnContentTemplate", typeof(DataTemplate), typeof(ToggleButtonAssist), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Allows an on (IsChecked) template to be provided on supporting <see cref="ToggleButton"/> styles.
        /// </summary>
        public static void SetOnContentTemplate(DependencyObject element, DataTemplate value)
            => element.SetValue(OnContentTemplateProperty, value);
    }
}
