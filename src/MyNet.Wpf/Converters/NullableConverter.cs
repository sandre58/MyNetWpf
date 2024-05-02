// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class NullableConverter : IValueConverter
    {
        public static IValueConverter Default { get; } = new NullableConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.ToString() == "NaN" ? null : value;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrEmpty(value?.ToString()) ? null : value;
    }
}
