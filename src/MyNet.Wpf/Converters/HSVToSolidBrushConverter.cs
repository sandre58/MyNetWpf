// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Media;

namespace MyNet.Wpf.Converters
{
    public class HSVToSolidBrushConverter : IMultiValueConverter
    {
        public static readonly HSVToSolidBrushConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var h = values.Length > 0 && double.TryParse(values[0].ToString(), out var hue) ? hue : 255.0;
            var s = values.Length > 1 && double.TryParse(values[1].ToString(), out var saturation) ? saturation : 1.0;
            var v = values.Length > 2 && double.TryParse(values[2].ToString(), out var val) ? val : 1.0;
            var hsl = new HSVColor(h, s, v);

            var brush = new SolidColorBrush(hsl.ToColor());

            brush.Freeze();
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

    }
}
