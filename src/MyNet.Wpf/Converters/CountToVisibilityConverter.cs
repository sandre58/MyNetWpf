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
    public class CountToVisibilityConverter
        : IValueConverter
    {
        private readonly ComparaisonToVisibilityConverter _converter;
        private readonly int _parameter;

        public static readonly CountToVisibilityConverter CollapsedIfAny = new(ComparaisonToVisibilityConverter.CollapsedIfIsGreaterThanTo, 0);
        public static readonly CountToVisibilityConverter CollapsedIfNotAny = new(ComparaisonToVisibilityConverter.CollapsedIfIsLessThanTo, 1);
        public static readonly CountToVisibilityConverter CollapsedIfMany = new(ComparaisonToVisibilityConverter.CollapsedIfIsGreaterThanTo, 1);
        public static readonly CountToVisibilityConverter CollapsedIfNotMany = new(ComparaisonToVisibilityConverter.CollapsedIfIsLessThanTo, 2);
        public static readonly CountToVisibilityConverter CollapsedIfOne = new(ComparaisonToVisibilityConverter.CollapsedIfIsEqualsTo, 1);
        public static readonly CountToVisibilityConverter CollapsedIfNotOne = new(ComparaisonToVisibilityConverter.CollapsedIfIsNotEqualsTo, 1);
        public static readonly CountToVisibilityConverter HiddenIfAny = new(ComparaisonToVisibilityConverter.HiddenIfIsGreaterThanTo, 0);
        public static readonly CountToVisibilityConverter HiddenIfNotAny = new(ComparaisonToVisibilityConverter.HiddenIfIsLessThanTo, 1);
        public static readonly CountToVisibilityConverter HiddenIfMany = new(ComparaisonToVisibilityConverter.HiddenIfIsGreaterThanTo, 1);
        public static readonly CountToVisibilityConverter HiddenIfNotMany = new(ComparaisonToVisibilityConverter.HiddenIfIsLessThanTo, 2);

        private CountToVisibilityConverter(ComparaisonToVisibilityConverter converter, int parameter)
        {
            _converter = converter;
            _parameter = parameter;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => _converter.Convert(value, targetType, _parameter, culture);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => _converter.ConvertBack(value, targetType, _parameter, culture);
    }
}
