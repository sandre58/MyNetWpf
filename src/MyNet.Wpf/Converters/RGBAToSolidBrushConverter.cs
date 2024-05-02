// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    public class RGBAToSolidBrushConverter : IMultiValueConverter
    {
        public static readonly RGBAToSolidBrushConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var r = values.Length > 0 && byte.TryParse(values[0].ToString(), out var red) ? red : (byte)255;
            var g = values.Length > 1 && byte.TryParse(values[1].ToString(), out var green) ? green : (byte)255;
            var b = values.Length > 2 && byte.TryParse(values[2].ToString(), out var blue) ? blue : (byte)255;
            var a = values.Length > 3 && byte.TryParse(values[3].ToString(), out var transparency) ? transparency : (byte)255;

            var brush = new SolidColorBrush(Color.FromArgb(a, r, g, b));

            brush.Freeze();
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

    }
}
