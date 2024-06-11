// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// MathConverter provides a value converter which can be used for math operations.
    /// It can be used for normal binding or multi binding as well.
    /// If it is used for normal binding the given parameter will be used as operands with the selected operation.
    /// If it is used for multi binding then the first and second binding will be used as operands with the selected operation.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ComparaisonToBooleanConverter : IValueConverter, IMultiValueConverter
    {
        private MathComparaisonForConverter Comparaison { get; set; }

        private ComparaisonToBooleanConverter(MathComparaisonForConverter operation) => Comparaison = operation;

        public static readonly ComparaisonToBooleanConverter IsEqualsTo = new(MathComparaisonForConverter.IsEqualsTo);

        public static readonly ComparaisonToBooleanConverter IsGreaterThan = new(MathComparaisonForConverter.IsGreaterThan);

        public static readonly ComparaisonToBooleanConverter IsLessThan = new(MathComparaisonForConverter.IsLessThan);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => DoConvert(value, parameter, Comparaison);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values == null || values.Length < 2 ? Binding.DoNothing : DoConvert(values[0], values[1], Comparaison);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

        internal static object DoConvert(object firstValue, object secondValue, MathComparaisonForConverter operation)
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
                var value1 = (firstValue as double?).GetValueOrDefault(System.Convert.ToDouble(firstValue, firstCulture));
                var value2 = (secondValue as double?).GetValueOrDefault(System.Convert.ToDouble(secondValue, secondCulture));

                return operation switch
                {
                    MathComparaisonForConverter.IsEqualsTo => value1.NearlyEqual(value2),
                    MathComparaisonForConverter.IsGreaterThan => value1 > value2,
                    MathComparaisonForConverter.IsLessThan => value1 < value2,
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
