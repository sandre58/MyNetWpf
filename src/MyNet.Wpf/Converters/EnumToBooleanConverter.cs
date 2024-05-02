// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Return a unique instance of <see cref="EnumToBooleanConverter"/>.
        /// </summary>
        public static readonly EnumToBooleanConverter Any = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || value == null)
            {
                return false;
            }

            var val = parameter is IEnumerable parameters
                ? parameters.Cast<object>().Any(parameter2 =>
                    System.Convert.ToInt32(parameter2, culture) == System.Convert.ToInt32(value, culture))
                : System.Convert.ToInt32(parameter, culture) == System.Convert.ToInt32(value, culture);

            return val;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null && ((parameter == null) || !(bool)value)
                ? DependencyProperty.UnsetValue
                : (parameter is IEnumerable parameters) ? parameters.Cast<object>().FirstOrDefault() : parameter;
    }
}
