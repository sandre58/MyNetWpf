// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// The math operations which can be used at the <see cref="MathConverter"/>
    /// </summary>
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Percent,
        PercentToValue,
        Pow,
        Modulo
    }

    /// <summary>
    /// MathConverter provides a value converter which can be used for math operations.
    /// It can be used for normal binding or multi binding as well.
    /// If it is used for normal binding the given parameter will be used as operands with the selected operation.
    /// If it is used for multi binding then the first and second binding will be used as operands with the selected operation.
    /// This class cannot be inherited.
    /// </summary>
    public class MathConverter(MathOperation operation) : IValueConverter, IMultiValueConverter
    {
        public MathOperation Operation { get; set; } = operation;

        public static MathConverter Add => new(MathOperation.Add);

        public static MathConverter Subtract => new(MathOperation.Subtract);

        public static MathConverter Multiply => new(MathOperation.Multiply);

        public static MathConverter Divide => new(MathOperation.Divide);

        public static MathConverter Percent => new(MathOperation.Percent);

        public static MathConverter PercentToValue => new(MathOperation.PercentToValue);

        public static MathConverter Pow => new(MathOperation.Pow);

        public static MathConverter Modulo => new(MathOperation.Modulo);

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => DoConvert([value, parameter], Operation);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values == null || values.Length < 2 ? Binding.DoNothing : DoConvert(values, Operation);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

        private static object DoConvert(IEnumerable<object> values, MathOperation operation)
        {
            try
            {
                var validValues = values.NotNull().Select(x => System.Convert.ToDouble(x, CultureInfo.InvariantCulture));

                return operation switch
                {
                    MathOperation.Add => validValues.Aggregate((x, y) => x + y),
                    MathOperation.Divide => validValues.Aggregate((x, y) => y.NearlyEqual(0) ? 0 : x / y),
                    MathOperation.Multiply => validValues.Aggregate((x, y) => x * y),
                    MathOperation.Subtract => validValues.Aggregate((x, y) => x - y),
                    MathOperation.Percent => validValues.Aggregate((x, y) => y.NearlyEqual(0) ? 0 : x / y * 100.00),
                    MathOperation.PercentToValue => validValues.Aggregate((x, y) => x * y / 100.00),
                    MathOperation.Pow => validValues.Aggregate(Math.Pow),
                    MathOperation.Modulo => validValues.Aggregate((x, y) => x % y),
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
