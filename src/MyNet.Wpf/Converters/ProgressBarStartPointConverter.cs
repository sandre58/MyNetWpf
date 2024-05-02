// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class ProgressBarStartPointConverter : IValueConverter
    {
        public static readonly ProgressBarStartPointConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is double @double && (@double > 0.0) ? new Point(@double / 2, 0) : (object)new Point();

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
