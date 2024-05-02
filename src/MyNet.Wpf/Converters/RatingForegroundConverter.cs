// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    internal class RatingForegroundConverter : IMultiValueConverter
    {
        private static byte SemiTransparent => 0x42; // ~26% opacity

        public static RatingForegroundConverter Default { get; } = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 4
                && values[0] is SolidColorBrush brush
                && values[1] is Orientation orientation
                && values[2] is double value
                && values[3] is int buttonValue)
            {
                if (value >= buttonValue)
                    return brush;

                var originalColor = brush.Color;
                var semiTransparent = Color.FromArgb(SemiTransparent, brush.Color.R, brush.Color.G, brush.Color.B);

                return value > buttonValue - 1.0
                    ? new LinearGradientBrush
                    {
                        StartPoint = orientation == Orientation.Horizontal ? new Point(0, 0.5) : new Point(0.5, 0),
                        EndPoint = orientation == Orientation.Horizontal ? new Point(1, 0.5) : new Point(0.5, 1),
                        GradientStops = new()
                            {
                                new GradientStop { Color = originalColor, Offset = value - buttonValue + 1},
                                new GradientStop { Color = semiTransparent, Offset = value - buttonValue + 1}
                            }
                    }
                    : new SolidColorBrush(semiTransparent);
            }

            // This should never happen (returning actual brush to avoid the compilers squiggly line warning)
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
