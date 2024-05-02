// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using DynamicData;
using MyNet.UI.Theming;
using MyNet.Wpf.Extensions;
using MyNet.Utilities;

namespace MyNet.Wpf.Theming
{
    public class ThemeDictionary : ResourceDictionary
    {
        private static readonly string ColorPattern = "MyNet.Colors.{0}";
        private static readonly string BrushPattern = "MyNet.Brushes.{0}";
        private static readonly string ThemeBaseResourceDictionaryPattern = "pack://application:,,,/MyNet.Wpf;component/Styles/Themes/Theme.{0}.xaml";
        private static readonly string MyThemeResourceDictionary = "pack://application:,,,/MyNet.Wpf;component/Themes/MyNet.Theme.xaml";

        private ThemeBase _theme = ThemeBase.Light;
        private ColorPair _primary = new("#1DB6EB".ToColor()!.Value);
        private ColorPair _accent = new("#FFD801".ToColor()!.Value);
        private object? _themeResourcePatterns;

        public ThemeDictionary()
        {
            SetThemeBase(_theme);
            SetPrimaryColors(_primary);
            SetAccentColors(_accent);
            MergedDictionaries.Add(new SharedResourceDictionary { Source = new Uri(MyThemeResourceDictionary, UriKind.Absolute) });
        }

        public ThemeBase Theme
        {
            get => _theme;
            set
            {
                if (value != _theme)
                {
                    SetThemeBase(value);
                }
            }
        }

        public ColorPair Primary
        {
            get => _primary;
            set
            {
                _primary = value;
                SetPrimaryColors(value);
            }
        }

        public ColorPair Accent
        {
            get => _accent;
            set
            {
                _accent = value;
                SetPrimaryColors(value);
            }
        }

        public object? ThemeResourcePatterns
        {
            get => _themeResourcePatterns;
            set
            {
                _themeResourcePatterns = value;
                SetThemeBase(Theme);
            }
        }

        public Theme ToTheme() => new()
        {
            Base = Theme,
            PrimaryColor = _primary.Color.ToString(),
            PrimaryForegroundColor = _primary.GetForegroundColor().ToString(),
            AccentColor = _accent.Color.ToString(),
            AccentForegroundColor = _accent.GetForegroundColor().ToString()
        };

        public void SetThemeBase(ThemeBase @base)
        {
            _theme = @base;
            var themeBase = @base == ThemeBase.Inherit ? ThemeBase.Dark : @base;

            var listOfResourcePatterns = new List<string> { ThemeBaseResourceDictionaryPattern };

            if (_themeResourcePatterns is IEnumerable<string> resourcePatterns)
                listOfResourcePatterns.AddRange(resourcePatterns);

            var listOfOldResourceUris = Enum.GetNames(typeof(ThemeBase)).SelectMany(x => listOfResourcePatterns.Select(y => new Uri(y.FormatWith(x), UriKind.Absolute))).ToList();
            var listOfNewResourceUris = listOfResourcePatterns.Select(y => new Uri(y.FormatWith(themeBase), UriKind.Absolute)).ToList();

            MergedDictionaries.RemoveMany(MergedDictionaries.Where(x => listOfOldResourceUris.Contains(x.Source)).ToList());
            MergedDictionaries.AddRange(listOfNewResourceUris.Select(x => new SharedResourceDictionary { Source = x }));
        }

        public void SetPrimaryColors(ColorPair pair)
        {
            _primary = pair;
            SetColors("Primary", pair.Color, pair.ForegroundColor);
        }

        public void SetAccentColors(ColorPair pair)
        {
            _accent = pair;
            SetColors("Accent", pair.Color, pair.ForegroundColor);
        }

        private void SetColors(string colorName, Color color, Color? foreground)
        {
            SetSolidColorBrush(colorName, color);
            SetSolidColorBrush($"{colorName}.Foreground", foreground ?? color.ContrastingForegroundColor());

            var light = new ColorPair(color.Lighten(), foreground);
            SetSolidColorBrush($"{colorName}.Light", light.Color);
            SetSolidColorBrush($"{colorName}.Light.Foreground", light.GetForegroundColor());

            var dark = new ColorPair(color.Darken(), foreground);
            SetSolidColorBrush($"{colorName}.Dark", dark.Color);
            SetSolidColorBrush($"{colorName}.Dark.Foreground", dark.GetForegroundColor());
        }

        private void SetSolidColorBrush(string name, Color value)
        {
            ArgumentNullException.ThrowIfNull(name);

            this[ColorPattern.FormatWith(name)] = value;

            if (this[BrushPattern.FormatWith(name)] is SolidColorBrush brush)
            {
                if (brush.Color == value) return;

                if (!brush.IsFrozen)
                {
                    var animation = new ColorAnimation
                    {
                        From = brush.Color,
                        To = value,
                        Duration = new Duration(TimeSpan.FromMilliseconds(300))
                    };
                    brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
                    return;
                }
            }

            var newBrush = new SolidColorBrush(value);
            newBrush.Freeze();
            this[BrushPattern.FormatWith(name)] = newBrush;
        }
    }
}
