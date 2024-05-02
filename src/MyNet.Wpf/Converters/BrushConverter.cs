// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Converters
{
    public class BrushConverter : IValueConverter
    {
        private enum Mode
        {
            None,

            Contrast,

            Opacity,

            Darken,

            Lighten
        }

        public static readonly BrushConverter Default = new(Mode.None);
        public static readonly BrushConverter Contrast = new(Mode.Contrast);
        public static readonly BrushConverter Opacity = new(Mode.Opacity);
        public static readonly BrushConverter Darken = new(Mode.Darken);
        public static readonly BrushConverter Lighten = new(Mode.Lighten);

        private readonly Mode _mode;

        private BrushConverter(Mode mode) => _mode = mode;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush && value is not Color && value is not string) return Binding.DoNothing;

            var color = value is SolidColorBrush b ? b.Color : value is Color c ? c : value.ToString().ToColor() ?? Colors.White;
            var opacity = value is SolidColorBrush brush ? brush.Opacity : 1.0D;

            switch (_mode)
            {
                case Mode.Contrast:
                    color = IdealTextColor(Color.FromArgb(System.Convert.ToByte(255 * opacity), color.R, color.G, color.B));
                    break;
                case Mode.Opacity:
                    opacity = parameter is not null ? System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture) : 1.0D;
                    break;
                case Mode.Darken:
                    color = color.Darken(parameter is not null ? System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture) : 1);
                    break;
                case Mode.Lighten:
                    color = color.Lighten(parameter is not null ? System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture) : 1);
                    break;
                default:
                    break;
            }

            var newBrush = new SolidColorBrush(color) { Opacity = opacity };
            newBrush.Freeze();
            return newBrush;
        }

        public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
            => Binding.DoNothing;

        private static Color IdealTextColor(Color bg)
        {
            const int nThreshold = 86;
            var bgDelta = System.Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + (bg.B * 0.114));
            var foreColor = (255 - bgDelta < nThreshold) ? Colors.Black : Colors.White;
            return foreColor;
        }
    }
}
