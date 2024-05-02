// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    public class StringToGeometryConverter : IValueConverter
    {
        public static readonly StringToGeometryConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => Geometry.Parse(value?.ToString());

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
