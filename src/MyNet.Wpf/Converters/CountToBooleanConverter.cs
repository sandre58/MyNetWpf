// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class CountToBooleanConverter
        : IValueConverter
    {
        private readonly ComparaisonToBooleanConverter _converter;
        private readonly int _parameter;

        public static readonly CountToBooleanConverter Any = new(ComparaisonToBooleanConverter.IsGreaterThan, 0);
        public static readonly CountToBooleanConverter NotAny = new(ComparaisonToBooleanConverter.IsLessThan, 1);
        public static readonly CountToBooleanConverter Many = new(ComparaisonToBooleanConverter.IsGreaterThan, 1);
        public static readonly CountToBooleanConverter NotMany = new(ComparaisonToBooleanConverter.IsLessThan, 2);
        public static readonly CountToBooleanConverter One = new(ComparaisonToBooleanConverter.IsEqualsTo, 1);

        private CountToBooleanConverter(ComparaisonToBooleanConverter converter, int parameter)
        {
            _converter = converter;
            _parameter = parameter;
        }
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => _converter.Convert(value, targetType, _parameter, culture);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => _converter.ConvertBack(value, targetType, _parameter, culture);
    }
}
