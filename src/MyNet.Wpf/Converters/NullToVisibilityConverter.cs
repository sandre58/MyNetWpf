// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class NullToVisibilityConverter(Visibility notNullVisibility = Visibility.Visible, Visibility nullVisibility = Visibility.Collapsed)
                : IValueConverter
    {
        private readonly Visibility _notNullVisibility = notNullVisibility;
        private readonly Visibility _nullVisibility = nullVisibility;

        public static readonly NullToVisibilityConverter CollapsedIfNull = new();
        public static readonly NullToVisibilityConverter CollapsedIfNotNull = new(Visibility.Collapsed, Visibility.Visible);
        public static readonly NullToVisibilityConverter HiddenIfNull = new(Visibility.Visible, Visibility.Hidden);
        public static readonly NullToVisibilityConverter HiddenIfNotNull = new(Visibility.Hidden, Visibility.Visible);

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

            return flag ? _nullVisibility : _notNullVisibility;
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
