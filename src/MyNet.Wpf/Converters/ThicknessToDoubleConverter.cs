// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public enum ThicknessSideType
    {
        Left,
        Top,
        Right,
        Bottom
    }

    [ValueConversion(typeof(Thickness), typeof(double), ParameterType = typeof(ThicknessSideType))]
    public class ThicknessToDoubleConverter(ThicknessSideType side) : IValueConverter
    {
        private readonly ThicknessSideType _side = side;

        public static readonly ThicknessToDoubleConverter Left = new(ThicknessSideType.Left);
        public static readonly ThicknessToDoubleConverter Top = new(ThicknessSideType.Top);
        public static readonly ThicknessToDoubleConverter Right = new(ThicknessSideType.Right);
        public static readonly ThicknessToDoubleConverter Bottom = new(ThicknessSideType.Bottom);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is Thickness thickness
                ? (_side switch
                {
                    ThicknessSideType.Left => thickness.Left,
                    ThicknessSideType.Top => thickness.Top,
                    ThicknessSideType.Right => thickness.Right,
                    ThicknessSideType.Bottom => thickness.Bottom,
                    _ => default,
                })
                : (object)default(double);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
    }
}
