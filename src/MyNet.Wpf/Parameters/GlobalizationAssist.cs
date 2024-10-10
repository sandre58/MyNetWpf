// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;
using MyNet.Utilities.Localization;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Parameters
{
    public static class GlobalizationAssist
    {
        #region UseCulture

        private static readonly DependencyProperty UpdateOnCultureChangedProperty =
                DependencyProperty.RegisterAttached("UpdateOnCultureChanged", typeof(bool), typeof(GlobalizationAssist), new PropertyMetadata(false, OnUpdateOnCultureChangedCallback));

        public static bool GetUpdateOnCultureChanged(FrameworkElement dp) => (bool)dp.GetValue(UpdateOnCultureChangedProperty);

        public static void SetUpdateOnCultureChanged(FrameworkElement dp, bool value) => dp.SetValue(UpdateOnCultureChangedProperty, value);

        private static void OnUpdateOnCultureChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element) return;
            if (GlobalizationService.Current is null) return;

            if ((bool)e.NewValue)
            {

                d.OnLoading<FrameworkElement>(x =>
                {
                    UpdateControl(x);
                    GlobalizationService.Current.CultureChanged += onCultureChanged;
                },
                x => GlobalizationService.Current.CultureChanged -= onCultureChanged);
            }
            else
            {
                GlobalizationService.Current.CultureChanged -= onCultureChanged;
            }

            void onCultureChanged(object? sender, EventArgs e) => UpdateControl(element);
        }

        private static void UpdateControl(FrameworkElement? element)
        {
            if (element is null) return;

            UpdateLanguage(element);

            if (element is Controls.TimePicker tp)
                UpdateTimeFormat(tp);
        }

        private static void UpdateLanguage(FrameworkElement element) => element.Language = XmlLanguage.GetLanguage(GlobalizationService.Current.Culture.Name);

        private static void UpdateTimeFormat(Controls.TimePicker timePicker) => timePicker.Is24Hours = GlobalizationService.Current.Culture.DateTimeFormat.ShortTimePattern.Contains("HH", StringComparison.InvariantCulture);

        #endregion UseCulture
    }
}
