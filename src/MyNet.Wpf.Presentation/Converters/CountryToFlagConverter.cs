// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities.Geography;
using MyNet.Utilities.Geography.Extensions;

namespace MyNet.Wpf.Presentation.Converters
{
    public class CountryToFlagConverter(FlagSize size) : IValueConverter
    {
        private readonly FlagSize _size = size;

        public static CountryToFlagConverter To16 { get; } = new CountryToFlagConverter(FlagSize._16);

        public static CountryToFlagConverter To24 { get; } = new CountryToFlagConverter(FlagSize._24);

        public static CountryToFlagConverter To32 { get; } = new CountryToFlagConverter(FlagSize._32);

        public static CountryToFlagConverter To48 { get; } = new CountryToFlagConverter(FlagSize._48);

        public static CountryToFlagConverter To64 { get; } = new CountryToFlagConverter(FlagSize._64);

        public static CountryToFlagConverter To128 { get; } = new CountryToFlagConverter(FlagSize._128);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not Country country ? null : country.GetFlag(_size);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    }
}
