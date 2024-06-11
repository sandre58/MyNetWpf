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
    public sealed class DateComparaisonToBooleanConverter : IValueConverter, IMultiValueConverter
    {
        private DateComparaisonForConverter Comparaison { get; set; }

        private DateComparaisonToBooleanConverter(DateComparaisonForConverter operation) => Comparaison = operation;

        public static readonly DateComparaisonToBooleanConverter IsEqualsTo = new(DateComparaisonForConverter.IsEqualsTo);

        public static readonly DateComparaisonToBooleanConverter IsGreaterThan = new(DateComparaisonForConverter.IsGreaterThan);

        public static readonly DateComparaisonToBooleanConverter IsLessThan = new(DateComparaisonForConverter.IsLessThan);

        public static readonly DateComparaisonToBooleanConverter IsBetween = new(DateComparaisonForConverter.IsBetween);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => DoConvert(value, parameter, null, Comparaison);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values == null || values.Length < 2 ? Binding.DoNothing : DoConvert(values[0], values[1], values.Length > 2 ? values[2] : null, Comparaison);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

        internal static object DoConvert(object firstValue, object secondValue, object? thirdValue, DateComparaisonForConverter operation)
        {
            if (firstValue == null
                || secondValue == null
                || firstValue == DependencyProperty.UnsetValue
                || secondValue == DependencyProperty.UnsetValue
                || firstValue == DBNull.Value
                || secondValue == DBNull.Value)
            {
                return Binding.DoNothing;
            }

            try
            {
                var firstCulture = firstValue is string ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
                var secondCulture = secondValue is string ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
                var thirdCulture = thirdValue is string ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
                var value1 = (firstValue as DateTime?).GetValueOrDefault(System.Convert.ToDateTime(firstValue, firstCulture));
                var value2 = (secondValue as DateTime?).GetValueOrDefault(System.Convert.ToDateTime(secondValue, secondCulture));
                var value3 = (thirdValue as DateTime?).GetValueOrDefault(System.Convert.ToDateTime(thirdValue, thirdCulture));

                return operation switch
                {
                    DateComparaisonForConverter.IsEqualsTo => value1 == value2,
                    DateComparaisonForConverter.IsGreaterThan => value1 > value2,
                    DateComparaisonForConverter.IsLessThan => value1 < value2,
                    DateComparaisonForConverter.IsBetween => value1 >= value2 && value1 <= value3,
                    _ => Binding.DoNothing,
                };
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }
    }
}
