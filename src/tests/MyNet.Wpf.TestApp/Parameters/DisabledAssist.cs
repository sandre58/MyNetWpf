// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;

namespace MyNet.Wpf.TestApp.Parameters
{
    public static class EnabledAssist
    {
        private static readonly List<UIElement> Elements = [];

        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(EnabledAssist), new PropertyMetadata(false, OnIsEnabledChangedCallback));

        public static bool GetIsEnabled(DependencyObject element) => (bool)element.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) => Elements.ForEach(x => x.IsEnabled = !GetCanBeDisabled(x) || (bool)e.NewValue);

        #endregion

        #region CanBeDisabled

        public static readonly DependencyProperty CanBeDisabledProperty = DependencyProperty.RegisterAttached(
            "CanBeDisabled", typeof(bool), typeof(EnabledAssist), new PropertyMetadata(false, OnCanBeDisabledChangedCallback));

        public static bool GetCanBeDisabled(DependencyObject element) => (bool)element.GetValue(CanBeDisabledProperty);
        public static void SetCanBeDisabled(DependencyObject element, bool value) => element.SetValue(CanBeDisabledProperty, value);

        private static void OnCanBeDisabledChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d is UIElement element)
                Elements.Add(element);
        }

        #endregion
    }
}
