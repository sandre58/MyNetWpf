// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a boolean value to a font weight (false: normal, true: bold)
    /// </summary>
    public class EqualityOrNullToVisibilityConverter(Visibility equalVisibility = Visibility.Visible, Visibility notEqualVisibility = Visibility.Collapsed)
                : IValueConverter, IMultiValueConverter
    {
        private readonly Visibility _equalVisibility = equalVisibility;
        private readonly Visibility _notEqualVisibility = notEqualVisibility;

        public static readonly EqualityOrNullToVisibilityConverter CollapsedIfEqual = new(Visibility.Collapsed, Visibility.Visible);
        public static readonly EqualityOrNullToVisibilityConverter CollapsedIfNotEqual = new(Visibility.Visible, Visibility.Collapsed);
        public static readonly EqualityOrNullToVisibilityConverter HiddenIfEqual = new(Visibility.Hidden, Visibility.Visible);
        public static readonly EqualityOrNullToVisibilityConverter HiddenIfNotEqual = new(Visibility.Visible, Visibility.Hidden);

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
             => value is null || parameter != null && value.ToString() is string str && str.Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase) ? _equalVisibility : _notEqualVisibility;

        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => Convert(values[0], targetType, values[1], culture);

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

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
