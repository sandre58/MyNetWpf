// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class EnumToVisibilityConverter(bool isHidden) : IValueConverter
    {
        private readonly bool _isHidden = isHidden;
        /// <summary>
        /// Return a unique instance of <see cref="EnumToVisibilityConverter"/>.
        /// </summary>
        public static readonly EnumToVisibilityConverter CollapsedIfNotAny = new(false);

        public static readonly EnumToVisibilityConverter HiddenIfNotAny = new(true);

        #region Public Methods and Operators

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convert = EnumToBooleanConverter.Any.Convert(value, targetType, parameter, culture);
            var isVisible = convert != null && (bool)convert;
            return _isHidden
                ? BooleanToVisibilityConverter.HiddenIfFalse.Convert(isVisible, targetType, parameter, culture)
                : BooleanToVisibilityConverter.CollapsedIfFalse.Convert(isVisible, targetType, parameter, culture);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertBack = _isHidden ?
                BooleanToVisibilityConverter.HiddenIfFalse.ConvertBack(value, targetType, parameter, culture) :
                BooleanToVisibilityConverter.CollapsedIfFalse.ConvertBack(value, targetType, parameter, culture);
            var isVisible = convertBack != null && (bool)convertBack;
            return EnumToBooleanConverter.Any.ConvertBack(isVisible, targetType, parameter, culture);
        }

        #endregion Public Methods and Operators
    }
}
