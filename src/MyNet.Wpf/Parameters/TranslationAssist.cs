// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using MaterialDesignThemes.Wpf;
using MyNet.Wpf.Controls;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Parameters
{
    public static class TranslationAssist
    {
        #region UseCulture

        private static readonly DependencyProperty UpdateOnCultureChangedProperty =
    DependencyProperty.RegisterAttached("UpdateOnCultureChanged", typeof(bool), typeof(TranslationAssist), new PropertyMetadata(false, OnUpdateOnCultureChanged));

        public static bool GetUpdateOnCultureChanged(FrameworkElement dp) => (bool)dp.GetValue(UpdateOnCultureChangedProperty);

        public static void SetUpdateOnCultureChanged(FrameworkElement dp, bool value) => dp.SetValue(UpdateOnCultureChangedProperty, value);

        private static void OnUpdateOnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element) return;
            if (CultureInfoService.Current is null) return;

            if ((bool)e.NewValue)
            {
                UpdateControl(element);
                CultureInfoService.Current.CultureChanged += onCultureChanged;
                element.Loaded += Element_Loaded;
            }
            else
            {
                CultureInfoService.Current.CultureChanged -= onCultureChanged;
                element.Loaded -= Element_Loaded;
            }

            void onCultureChanged(object? sender, EventArgs e) => UpdateControl(element);
        }

        private static void Element_Loaded(object? sender, RoutedEventArgs e) => UpdateControl((FrameworkElement?)sender);

        private static void UpdateControl(FrameworkElement? element)
        {
            if (element is null) return;

            UpdateLanguage(element);

            if (element is Controls.TimePicker tp)
                UpdateTimeFormat(tp);
        }

        private static void UpdateLanguage(FrameworkElement element) => element.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);

        private static void UpdateTimeFormat(Controls.TimePicker timePicker) => timePicker.Is24Hours = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains("HH", StringComparison.InvariantCulture);

        #endregion UseCulture
    }
}
