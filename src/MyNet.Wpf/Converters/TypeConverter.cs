// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    [ValueConversion(typeof(object), typeof(Type))]
    public class TypeConverter : IValueConverter
    {
        public static IValueConverter Default { get; } = new TypeConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? value.GetType() : (object?)null;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
