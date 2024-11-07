// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class NullToBooleanConverter(bool nullValue = true)
                : IValueConverter
    {
        private readonly bool _nullValue = nullValue;

        public static readonly NullToBooleanConverter TrueIfNull = new();
        public static readonly NullToBooleanConverter FalseIfNull = new(false);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = value == null;
            if (value is string str)
            {
                flag = string.IsNullOrEmpty(str);
            }

            if (value is double dbl)
            {
                flag = double.IsNaN(dbl);
            }

            if (value is Array arr)
            {
                flag = arr.Length == 0;
            }

            if (value is DateTime date)
            {
                flag = date == DateTime.MinValue;
            }

            return flag ? _nullValue : !_nullValue;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
