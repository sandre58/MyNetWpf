// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using LiveCharts;

namespace MyNet.Wpf.LiveCharts.Converters
{
    public class DoubleToChartValuesConverter : IValueConverter
    {
        public static DoubleToChartValuesConverter Default { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is double dbl ? new ChartValues<double> { dbl } : (object)new ChartValues<double>();
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
