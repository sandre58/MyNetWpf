// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using MyNet.Humanizer;

namespace MyNet.Wpf.Converters
{
    public class IntegerToOrdinalizeConverter
        : IValueConverter
    {
        public static readonly IntegerToOrdinalizeConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value?.ToString()?.Ordinalize();

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
    }
}
