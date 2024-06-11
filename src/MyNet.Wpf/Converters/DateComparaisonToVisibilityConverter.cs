// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// MathConverter provides a value converter which can be used for math operations.
    /// It can be used for normal binding or multi binding as well.
    /// If it is used for normal binding the given parameter will be used as operands with the selected operation.
    /// If it is used for multi binding then the first and second binding will be used as operands with the selected operation.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class DateComparaisonToVisibilityConverter : IValueConverter, IMultiValueConverter
    {
        private DateComparaisonForConverter Comparaison { get; set; }

        private readonly Visibility _falseVisibility;
        private readonly Visibility _trueVisibility;

        private DateComparaisonToVisibilityConverter(DateComparaisonForConverter operation, Visibility trueVisibility, Visibility falseVisibility)
        {
            Comparaison = operation;
            _trueVisibility = trueVisibility;
            _falseVisibility = falseVisibility;
        }

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsEqualsTo = new(DateComparaisonForConverter.IsEqualsTo, Visibility.Collapsed, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsNotEqualsTo = new(DateComparaisonForConverter.IsEqualsTo, Visibility.Visible, Visibility.Collapsed);

        public static readonly DateComparaisonToVisibilityConverter HiddenIfIsNotEqualsTo = new(DateComparaisonForConverter.IsEqualsTo, Visibility.Visible, Visibility.Hidden);

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsGreaterThanTo = new(DateComparaisonForConverter.IsGreaterThan, Visibility.Collapsed, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter HiddenIfIsGreaterThanTo = new(DateComparaisonForConverter.IsGreaterThan, Visibility.Hidden, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsLessThanTo = new(DateComparaisonForConverter.IsLessThan, Visibility.Collapsed, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter HiddenIfIsLessThanTo = new(DateComparaisonForConverter.IsLessThan, Visibility.Hidden, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsBetweenThanTo = new(DateComparaisonForConverter.IsBetween, Visibility.Collapsed, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter HiddenIfIsBetweenThanTo = new(DateComparaisonForConverter.IsBetween, Visibility.Hidden, Visibility.Visible);

        public static readonly DateComparaisonToVisibilityConverter CollapsedIfIsNotBetweenThanTo = new(DateComparaisonForConverter.IsBetween, Visibility.Visible, Visibility.Collapsed);

        public static readonly DateComparaisonToVisibilityConverter HiddenIfIsNotBetweenThanTo = new(DateComparaisonForConverter.IsBetween, Visibility.Visible, Visibility.Hidden);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => DoConvert(value, parameter, null, Comparaison, _trueVisibility, _falseVisibility);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values == null || values.Length < 2
                ? Binding.DoNothing
                : DoConvert(values[0], values[1], values.Length > 2 ? values[2] : null, Comparaison, _trueVisibility, _falseVisibility);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

        private static object DoConvert(object firstValue, object secondValue, object? thirdValue, DateComparaisonForConverter operation, Visibility trueVisibility, Visibility falseVisibility)
        {
            var result = DateComparaisonToBooleanConverter.DoConvert(firstValue, secondValue, thirdValue, operation);

            return result is not bool boolean ? result : boolean ? trueVisibility : falseVisibility;
        }
    }
}
