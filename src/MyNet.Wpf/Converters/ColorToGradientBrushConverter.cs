// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Converters
{
    public class ColorToGradientBrushConverter : IValueConverter
    {
        private enum Mode
        {
            Darken,

            Lighten
        }

        public static readonly ColorToGradientBrushConverter Darken = new(Mode.Darken);
        public static readonly ColorToGradientBrushConverter Lighten = new(Mode.Lighten);

        private readonly Mode _mode;

        private ColorToGradientBrushConverter(Mode mode) => _mode = mode;


        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush && value is not Color && value is not string) return Binding.DoNothing;

            var color = value is SolidColorBrush b ? b.Color : value is Color c ? c : value is string s ? s.ToColor().GetValueOrDefault() : Colors.White;

            return new LinearGradientBrush()
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 1),
                GradientStops =
            [
                new GradientStop(color, 0),
                new GradientStop(_mode == Mode.Lighten ? color.Lighten() : color.Darken(), 0.5),
                new GradientStop(color, 1)
            ]
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is SolidColorBrush brush ? brush.Color : (object)default(Color);
    }
}
