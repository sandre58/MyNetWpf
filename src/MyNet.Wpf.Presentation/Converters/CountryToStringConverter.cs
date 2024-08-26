// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Humanizer;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Geography.Extensions;

namespace MyNet.Wpf.Presentation.Converters
{
    public class CountryToStringConverter : IValueConverter
    {
        private enum Display
        {
            Alpha2,

            Alpha3,

            DisplayName,

            Iso
        }

        private readonly Display _display;

        public static CountryToStringConverter ToAlpha2 { get; } = new CountryToStringConverter(Display.Alpha2);

        public static CountryToStringConverter ToAlpha3 { get; } = new CountryToStringConverter(Display.Alpha3);

        public static CountryToStringConverter ToDisplayName { get; } = new CountryToStringConverter(Display.DisplayName);

        public static CountryToStringConverter ToIso { get; } = new CountryToStringConverter(Display.Iso);

        private CountryToStringConverter(Display display) => _display = display;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Country country) return string.Empty;

            switch (_display)
            {
                case Display.Alpha2:
                    return country.Alpha2.ApplyCase(LetterCasing.AllCaps);

                case Display.Alpha3:
                    return country.Alpha3.ApplyCase(LetterCasing.AllCaps);

                case Display.DisplayName:
                    return country.GetDisplayName();

                case Display.Iso:
                    return country.Iso.ToString(culture);

                default:
                    break;
            }

            return string.Empty;

        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }
}
