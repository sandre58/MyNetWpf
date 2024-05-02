// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Configurations;
using MyNet.Observable;

namespace MyNet.Wpf.LiveCharts
{
    public class ChartSerie<T> : ObservableObject
    {
        public string Label { get; set; }

        public bool IsVisible { get; set; }

        public ChartValues<T> Values { get; private set; }

        public CartesianMapper<T>? Mapper { get; }

        public ChartSerie(string label = "", bool isVisible = true)
        {
            IsVisible = isVisible;
            Label = label;
            Values = [];
        }

        public ChartSerie(CartesianMapper<T> mapper, string label = "", bool isVisible = true) : this(label, isVisible) => Mapper = mapper;

        public void Update(IEnumerable<T> items)
        {
            Values.Clear();
            Values.AddRange(items);
        }
    }
}
