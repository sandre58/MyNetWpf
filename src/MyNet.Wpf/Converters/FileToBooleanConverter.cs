// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class FileToBooleanConverter
        : IValueConverter
    {
        private readonly bool _existsValue;

        public static readonly FileToBooleanConverter TrueIfExist = new(true);
        public static readonly FileToBooleanConverter FalseIfExist = new(false);

        private FileToBooleanConverter(bool existsValue) => _existsValue = existsValue;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return !_existsValue;
            }

            var file = new FileInfo(value.ToString().OrEmpty());

            return file.Exists ? _existsValue : !_existsValue;
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
