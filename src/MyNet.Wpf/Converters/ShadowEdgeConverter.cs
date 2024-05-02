// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Converters
{
    public class ShadowEdgeConverter : IMultiValueConverter
    {
        public static readonly ShadowEdgeConverter Default = new();

        public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            static double? getValidSize(object? value)
                => value is double d && !double.IsNaN(d) && !double.IsInfinity(d) ? d : null;

            static DropShadowEffect? getDropShadow(object? value) => value switch
            {
                Elevation elevation => ElevationAssist.GetDropShadow(elevation),
                _ => null
            };

            if (values is null
                || values.Length < 3
                || getValidSize(values[0]) is not { } width
                || getValidSize(values[1]) is not { } height
                || getDropShadow(values[2]) is not { } dropShadow)
            {
                return null;
            }

            var blurRadius = dropShadow.BlurRadius;

            Rect rect;

            if (values.Length > 3 && values[3] is ShadowEdges edges and not ShadowEdges.All)
            {
                rect = new Rect(0, 0, width, height);

                if (edges.HasFlag(ShadowEdges.Left))
                {
                    rect = rect with { X = -blurRadius, Width = width + blurRadius };
                }

                if (edges.HasFlag(ShadowEdges.Top))
                {
                    rect = rect with { Y = -blurRadius, Height = height + blurRadius };
                }

                if (edges.HasFlag(ShadowEdges.Right))
                {
                    rect = rect with { Width = rect.Width + blurRadius };
                }

                if (edges.HasFlag(ShadowEdges.Bottom))
                {
                    rect = rect with { Height = rect.Height + blurRadius };
                }
            }
            else
            {
                rect = new Rect(
                    -blurRadius,
                    -blurRadius,
                    width + blurRadius + blurRadius,
                    height + blurRadius + blurRadius);
            }

            var drawing = new GeometryDrawing(Brushes.White, null, new RectangleGeometry(rect));
            return new DrawingBrush(drawing)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                Viewport = rect,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = rect,
                ViewboxUnits = BrushMappingMode.Absolute
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
