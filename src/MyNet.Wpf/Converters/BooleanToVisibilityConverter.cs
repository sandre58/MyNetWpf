// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts boolean to visibility values.
    /// </summary>
    public class BooleanToVisibilityConverter(Visibility trueVisibility = Visibility.Visible, Visibility falseVisibility = Visibility.Collapsed) : IValueConverter
    {
        private readonly Visibility _falseVisibility = falseVisibility;
        private readonly Visibility _trueVisibility = trueVisibility;

        public static readonly BooleanToVisibilityConverter CollapsedIfFalse = new();
        public static readonly BooleanToVisibilityConverter CollapsedIfTrue = new(Visibility.Collapsed, Visibility.Visible);
        public static readonly BooleanToVisibilityConverter HiddenIfFalse = new(Visibility.Visible, Visibility.Hidden);
        public static readonly BooleanToVisibilityConverter HiddenIfTrue = new(Visibility.Hidden, Visibility.Visible);

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
            var flag = false;
            if (value is bool boolean)
            {
                flag = boolean;
            }

            return (flag ? _trueVisibility : _falseVisibility);
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
