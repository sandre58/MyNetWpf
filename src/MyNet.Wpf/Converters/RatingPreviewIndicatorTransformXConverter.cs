// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Converters
{
    internal class RatingPreviewIndicatorTransformXConverter : IMultiValueConverter
    {
        public static RatingPreviewIndicatorTransformXConverter Default { get; } = new();

        private static double Margin => 2.0;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 4
                && values[0] is double ratingBarButtonActualWidth
                && values[1] is double previewValueActualWidth
                && values[2] is int ratingButtonValue
                && values[4] is RatingBar ratingBar)
            {
                var ratingBarOrientation = ratingBar.Orientation;
                var isFractionalValueEnabled = ratingBar.IsFractionalValueEnabled;
                var previewValue = ratingBar.PreviewValue ?? 0.0d;
                if (!isFractionalValueEnabled)
                {
                    return ratingBarOrientation switch
                    {
                        Orientation.Horizontal => (ratingBarButtonActualWidth - previewValueActualWidth) / 2,
                        Orientation.Vertical => -previewValueActualWidth - Margin,
                        _ => throw new ArgumentOutOfRangeException(nameof(values))
                    };
                }

                // Special handling of edge cases due to the inaccuracy of how double values are stored
                var percent = previewValue % 1;
                if (Math.Abs(ratingButtonValue - previewValue) <= double.Epsilon)
                    percent = 1.0;
                else if (percent <= double.Epsilon)
                    percent = 0.0;

                return ratingBarOrientation switch
                {
                    Orientation.Horizontal => percent * ratingBarButtonActualWidth - (previewValueActualWidth / 2),
                    Orientation.Vertical => -previewValueActualWidth - Margin,
                    _ => throw new ArgumentOutOfRangeException(nameof(values))
                };
            }
            return 1.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
