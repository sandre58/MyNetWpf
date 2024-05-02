// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class ExpanderRotateAngleConverter : IValueConverter
    {
        public static readonly ExpanderRotateAngleConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var factor = 1.0;
            if (parameter is { } parameterValue && !double.TryParse(parameterValue.ToString(), out factor))
            {
                factor = 1.0;
            }
            return value switch
            {
                ExpandDirection.Left => 90 * factor,
                ExpandDirection.Right => -90 * factor,
                _ => 0
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
