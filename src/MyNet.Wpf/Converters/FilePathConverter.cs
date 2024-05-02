// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class FilePathConverter(FilePathConverter.FilePathResult result)
                : IValueConverter
    {
        public enum FilePathResult
        {
            Filename
        }

        private readonly FilePathResult _result = result;

        public static readonly FilePathConverter ToFilename = new(FilePathResult.Filename);

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
            if (value == null)
            {
                return string.Empty;
            }

            var file = new FileInfo(value.ToString() ?? string.Empty);
            return _result switch
            {
                FilePathResult.Filename => file.Name,
                _ => value,
            };
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
