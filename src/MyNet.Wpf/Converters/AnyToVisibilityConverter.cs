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
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class AnyToVisibilityConverter(Visibility notAnyVisibility = Visibility.Visible, Visibility anyVisibility = Visibility.Collapsed)
                : IValueConverter
    {
        private readonly Visibility _notAnyVisibility = notAnyVisibility;
        private readonly Visibility _anyVisibility = anyVisibility;

        public static readonly AnyToVisibilityConverter CollapsedIfAny = new(Visibility.Collapsed, Visibility.Visible);
        public static readonly AnyToVisibilityConverter HiddenIfAny = new(Visibility.Visible, Visibility.Hidden);
        public static readonly AnyToVisibilityConverter HiddenIfNotAny = new(Visibility.Hidden, Visibility.Visible);

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
            if (value is IEnumerable items)
            {
                flag = items.OfType<object>().Any();
            }

            return (flag ? _anyVisibility : _notAnyVisibility);
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
