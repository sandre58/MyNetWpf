﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    [ValueConversion(typeof(double), typeof(double), ParameterType = typeof(Orientation))]
    internal class SliderValueLabelPositionConverter : IValueConverter
    {
        public static readonly SliderValueLabelPositionConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Orientation orientation && value is double width)
            {
                const double halfGripWidth = 9.0;
                const double margin = 4.0;
                return orientation switch
                {
                    Orientation.Horizontal => -width * 0.5 + halfGripWidth,
                    Orientation.Vertical => -width - margin,
                    _ => throw new NotImplementedException()
                };
            }

            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
