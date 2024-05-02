// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class EnumToIntConverter : IValueConverter
    {
        public static readonly EnumToIntConverter Default = new();

        #region Public Methods and Operators

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not Enum val ? null : System.Convert.ToInt32(val, culture);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !int.TryParse(value?.ToString(), out var val) ? null : Enum.ToObject(targetType, val);

        #endregion Public Methods and Operators
    }
}
