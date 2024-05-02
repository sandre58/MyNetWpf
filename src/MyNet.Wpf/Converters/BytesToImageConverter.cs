// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Am implementation of <see cref="IValueConverter"/> to convert Image to byte array and vice versa.
    /// </summary>
    public class BytesToImageConverter : IValueConverter
    {

        #region Public Properties

        /// <summary>
        /// Return a unique instance of <see cref="BytesToImageConverter"/>.
        /// </summary>
        public static readonly BytesToImageConverter Default = new();

        #endregion Public Properties

        #region Implementation of IValueConverter

        /// <summary>
        /// Converts byte[] to Image source.
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is byte[] bytes ? bytes.ToBitmap() : null;

        /// <summary>
        /// Converts ImageSource to byte[]
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is BitmapImage renderTargetBitmap ? renderTargetBitmap.ToBytes() : null;

        #endregion Implementation of IValueConverter
    }
}
