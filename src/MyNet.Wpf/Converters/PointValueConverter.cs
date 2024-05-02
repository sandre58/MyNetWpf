// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class PointValueConverter : IMultiValueConverter
    {
        public static readonly PointValueConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values?.Length == 2 && values[0] != null && values[1] != null && double.TryParse(values[0].ToString(), out var x) &&
                    double.TryParse(values[1].ToString(), out var y)
                ? new Point(x, y)
                : (object)new Point();

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => value is Point point ? ([point.X, point.Y]) : Array.Empty<object>();
    }
}
