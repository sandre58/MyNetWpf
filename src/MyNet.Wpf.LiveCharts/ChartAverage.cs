// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using LiveCharts;
using MyNet.Observable;
using MyNet.Utilities;

namespace MyNet.Wpf.LiveCharts
{
    public class ChartAverage<T> : ObservableObject
    {
        public IChartValues Values { get; }

        public double Average { get; private set; }

        public ChartAverage() => Values = new ChartValues<T>();

        public void Update(T value, double count)
        {
            Values.Clear();
            _ = Values.Add(value);
            Average = !count.NearlyEqual(0) ? double.Parse(value?.ToString() ?? string.Empty, CultureInfo.CurrentCulture) / count : 0;
        }

        public void Reset() => Values.Clear();
    }
}
