// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Converters
{
    internal class FloatingHintOffsetCalculationConverter : IMultiValueConverter
    {
        public static readonly FloatingHintOffsetCalculationConverter Default = new();

        public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Length > 3 &&
                values[3] is Point floatingOffset &&
                IsType<Point>(targetType) &&
                floatingOffset != HintAssist.DefaultFloatingOffset)
            {
                return floatingOffset;
            }

            var fontFamily = (FontFamily)values[0]!;
            var fontSize = (double)values[1]!;
            var floatingScale = (double)values[2]!;

            var hintHeight = fontFamily.LineSpacing * fontSize;
            var floatingHintHeight = hintHeight * floatingScale;

            var offset = (values.Length > 4 ? values[4] : null) switch
            {
                Thickness padding => floatingHintHeight / 2 + padding.Top,
                double parentHeight => (parentHeight - hintHeight + floatingHintHeight) / 2,
                _ => floatingHintHeight
            };

            return IsType<Point>(targetType)
                ? new Point(0, -offset)
                : (object)new Thickness(0, offset, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();

        private static bool IsType<T>(Type type) => type == typeof(T);
    }
}
