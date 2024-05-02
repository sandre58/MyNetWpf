// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using MyNet.Wpf.Resources;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Logging;
using MyNet.Utilities.Resources;
using MyNet.UI.Resources;

namespace MyNet.Wpf
{
    public static class ResourceLocator
    {
        private static bool _isInitialized;

        public static Dictionary<Color, string> ColorResourcesDictionary { get; private set; } = [];

        public static void Initialize()
        {
            if (_isInitialized) return;

            // Common Resources
            TranslationService.RegisterResources(nameof(UiResources), UiResources.ResourceManager);
            TranslationService.RegisterResources(nameof(MessageResources), MessageResources.ResourceManager);
            TranslationService.RegisterResources(nameof(FormatResources), FormatResources.ResourceManager);
            TranslationService.RegisterResources(nameof(WpfResources), WpfResources.ResourceManager);
            TranslationService.RegisterResources(nameof(ColorResources), ColorResources.ResourceManager);

            Humanizer.ResourceLocator.Initialize();

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(TranslationService.Current.Culture.Name)));
            UpdateUiLanguage();
            CultureInfoService.Current.CultureChanged += (sender, e) =>
            {
                FillColorResourcesDictionary();
                UpdateUiLanguage();
            };

            _isInitialized = true;
        }

        private static void UpdateUiLanguage()
        {
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name);
            }
        }

        private static void FillColorResourcesDictionary()
        {
            ColorResourcesDictionary = [];
            var resourceSet = ColorResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            if (resourceSet is not null)
            {
                foreach (var entry in resourceSet.OfType<DictionaryEntry>())
                {
                    try
                    {
                        if (ColorConverter.ConvertFromString(entry.Key.ToString()) is Color color)
                        {
                            ColorResourcesDictionary.Add(color, entry.Value!.ToString()!);
                        }
                    }
                    catch (Exception)
                    {
                        LogManager.Warning($"{entry.Key} is not a valid color key");
                    }
                }
            }
        }
    }
}
