// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public static ColorToBrushConverter Default { get; } = new ColorToBrushConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not SolidColorBrush && value is not Color && value is not string) return Binding.DoNothing;

            var color = value is SolidColorBrush b ? b.Color : value is Color c ? c : value is string s ? s.ToColor().GetValueOrDefault() : Colors.White;

            return new SolidColorBrush(color);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is SolidColorBrush brush ? brush.Color : (object)default(Color);
    }
}
