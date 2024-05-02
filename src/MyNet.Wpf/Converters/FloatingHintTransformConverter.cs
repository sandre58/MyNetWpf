// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    internal class FloatingHintTransformConverter : IMultiValueConverter
    {
        public static readonly FloatingHintTransformConverter Default = new();

        public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null
                || values.Length != 4
                || Array.Exists(values, v => v == null)
                || !double.TryParse(values[0]?.ToString(), out var scale)
                || !double.TryParse(values[1]?.ToString(), out var lower)
                || !double.TryParse(values[2]?.ToString(), out var upper)
                || values[3] is not Point floatingOffset)
            {
                return Transform.Identity;
            }

            var result = upper + (lower - upper) * scale;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform
            {
                ScaleX = result,
                ScaleY = result
            });
            transformGroup.Children.Add(new TranslateTransform
            {
                X = scale * floatingOffset.X,
                Y = scale * floatingOffset.Y
            });
            return transformGroup;
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
