// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    public class RGBAToGradientBrushConverter : IMultiValueConverter
    {
        private enum Mode
        {
            Red,

            Green,

            Blue,

            Alpha
        }

        private enum Orientation
        {
            Horizontal,

            Vertical
        }

        public static readonly RGBAToGradientBrushConverter Red = new(Mode.Red, Orientation.Horizontal);
        public static readonly RGBAToGradientBrushConverter Green = new(Mode.Green, Orientation.Horizontal);
        public static readonly RGBAToGradientBrushConverter Blue = new(Mode.Blue, Orientation.Horizontal);
        public static readonly RGBAToGradientBrushConverter Alpha = new(Mode.Alpha, Orientation.Horizontal);
        public static readonly RGBAToGradientBrushConverter RedVertical = new(Mode.Red, Orientation.Vertical);
        public static readonly RGBAToGradientBrushConverter GreenVertical = new(Mode.Green, Orientation.Vertical);
        public static readonly RGBAToGradientBrushConverter BlueVertical = new(Mode.Blue, Orientation.Vertical);
        public static readonly RGBAToGradientBrushConverter AlphaVertical = new(Mode.Alpha, Orientation.Vertical);

        private readonly Mode _mode;
        private readonly Orientation _orientation;

        private RGBAToGradientBrushConverter(Mode mode, Orientation orientation)
        {
            _mode = mode;
            _orientation = orientation;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var v1 = values.Length > 0 && byte.TryParse(values[0].ToString(), out var val1) ? val1 : (byte)255;
            var v2 = values.Length > 1 && byte.TryParse(values[1].ToString(), out var val2) ? val2 : (byte)255;
            var v3 = values.Length > 2 && byte.TryParse(values[2].ToString(), out var val3) ? val3 : (byte)255;

            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(_orientation == Orientation.Horizontal ? 0 : 0.5, _orientation == Orientation.Horizontal ? 0.5 : 0),
                EndPoint = new Point(_orientation == Orientation.Horizontal ? 1 : 0.5, _orientation == Orientation.Horizontal ? 0.5 : 1)
            };

            switch (_mode)
            {
                case Mode.Red:
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 0, v1, v2), _orientation == Orientation.Horizontal ? 0 : 1));
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, v1, v2), _orientation == Orientation.Horizontal ? 1 : 0));
                    break;
                case Mode.Green:
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, v1, 0, v2), _orientation == Orientation.Horizontal ? 0 : 1));
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, v1, 255, v2), _orientation == Orientation.Horizontal ? 1 : 0));
                    break;
                case Mode.Blue:
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, v1, v2, 0), _orientation == Orientation.Horizontal ? 0 : 1));
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, v1, v2, 255), _orientation == Orientation.Horizontal ? 1 : 0));
                    break;
                case Mode.Alpha:
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, v1, v2, v3), _orientation == Orientation.Horizontal ? 0 : 1));
                    brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, v1, v2, v3), _orientation == Orientation.Horizontal ? 1 : 0));
                    break;
                default:
                    break;
            }

            brush.Freeze();
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => targetTypes.Select(t => Binding.DoNothing).ToArray();

    }
}
