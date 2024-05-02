// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using MyNet.UI.Theming;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Theming
{
    public class ThemeService : IThemeService
    {
        private ThemeDictionary _themeDictionary = null!;

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        private ThemeDictionary ThemeDictionary
            => _themeDictionary ??= Application.Current.Resources.MergedDictionaries.OfType<ThemeDictionary>().FirstOrDefault() ?? throw new InvalidOperationException("You must add a <my:ThemeDictionary /> in App.xaml.");

        public Theme CurrentTheme => ThemeDictionary.ToTheme();

        public IThemeService AddBaseExtension(IThemeExtension extension) => this;

        public IThemeService AddPrimaryExtension(IThemeExtension extension) => this;

        public IThemeService AddAccentExtension(IThemeExtension extension) => this;

        public void ApplyTheme(Theme theme)
        {
            if (theme.Base is not null)
            {
                ThemeDictionary.SetThemeBase(theme.Base.Value);
            }

            if (theme.PrimaryColor is not null)
            {
                ThemeDictionary.SetPrimaryColors(new(theme.PrimaryColor.ToColor() ?? default, theme.PrimaryForegroundColor?.ToColor()));
            }

            if (theme.AccentColor is not null)
            {
                ThemeDictionary.SetAccentColors(new(theme.AccentColor.ToColor() ?? default, theme.AccentForegroundColor?.ToColor()));
            }

            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(CurrentTheme));
        }
    }
}
