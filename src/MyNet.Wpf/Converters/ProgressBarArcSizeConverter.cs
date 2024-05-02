// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class ProgressBarArcSizeConverter : IValueConverter
    {
        public static readonly ProgressBarArcSizeConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is double @double && (@double > 0.0) ? new Size(@double / 2, @double / 2) : (object)new Size();

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
