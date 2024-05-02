// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Media;

namespace MyNet.Wpf.Converters
{
    public class HSVToGradientBrushConverter : IMultiValueConverter
    {
        private enum Mode
        {
            Saturation,

            Value
        }

        private enum Orientation
        {
            Horizontal,

            Vertical
        }

        public static readonly HSVToGradientBrushConverter Saturation = new(Mode.Saturation, Orientation.Horizontal);
        public static readonly HSVToGradientBrushConverter Value = new(Mode.Value, Orientation.Horizontal);
        public static readonly HSVToGradientBrushConverter SaturationVertical = new(Mode.Saturation, Orientation.Vertical);
        public static readonly HSVToGradientBrushConverter ValueVertical = new(Mode.Value, Orientation.Vertical);

        private readonly Mode _mode;
        private readonly Orientation _orientation;

        private HSVToGradientBrushConverter(Mode mode, Orientation orientation)
        {
            _mode = mode;
            _orientation = orientation;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var h = values.Length > 0 && double.TryParse(values[0].ToString(), out var hue) ? hue : 255.0;
            var saturationOrValue = values.Length > 1 && double.TryParse(values[1].ToString(), out var val) ? val : 1.0;

            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(_orientation == Orientation.Horizontal ? 0 : 0.5, _orientation == Orientation.Horizontal ? 0.5 : 0),
                EndPoint = new Point(_orientation == Orientation.Horizontal ? 1 : 0.5, _orientation == Orientation.Horizontal ? 0.5 : 1)
            };
            brush.GradientStops.Add(new GradientStop(new HSVColor(h, _mode == Mode.Saturation ? 0 : saturationOrValue, _mode == Mode.Value ? 0 : saturationOrValue).ToColor(), _orientation == Orientation.Horizontal ? 0 : 1));
            brush.GradientStops.Add(new GradientStop(new HSVColor(h, _mode == Mode.Saturation ? 1 : saturationOrValue, _mode == Mode.Value ? 1 : saturationOrValue).ToColor(), _orientation == Orientation.Horizontal ? 1 : 0));

            brush.Freeze();
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

    }
}
