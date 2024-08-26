// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;
using MyNet.Humanizer;

namespace MyNet.Wpf.Converters
{
    internal sealed class CalendarYearMonthConverter : IMultiValueConverter
    {
        public static readonly CalendarYearMonthConverter Default = new();

        public object? Convert(object?[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            var ticks = long.MaxValue;
            foreach (var value in values ?? Enumerable.Empty<object?>())
            {
                if (value is DateTime dt)
                    ticks = dt.Ticks;
                else if (value is XmlLanguage language)
                    culture = language.GetSpecificCulture();
            }
            return ticks == long.MaxValue ? null : new DateTime(ticks).ToString(culture.DateTimeFormat.YearMonthPattern, culture).ToTitle();
        }

        public object?[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
