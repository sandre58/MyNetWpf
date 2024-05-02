// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class ProgressBarRotateTransformCentreConverter : IValueConverter
    {
        public static readonly ProgressBarRotateTransformCentreConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            //value == actual width
            (double)value / 2;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
