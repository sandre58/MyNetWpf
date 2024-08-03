// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public sealed class DateToYearConverter : IValueConverter
    {
        public static readonly DateToYearConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not DateTime date ? Binding.DoNothing : date.Year;

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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => int.TryParse(value?.ToString(), out var year) ? new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Local) : Binding.DoNothing;
    }
}
