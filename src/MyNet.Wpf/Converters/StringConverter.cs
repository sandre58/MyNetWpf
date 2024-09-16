// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Humanizer;
using MyNet.Utilities;
using MyNet.Utilities.Extensions;
using MyNet.Utilities.Units;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts string values.
    /// </summary>
    public class StringConverter(LetterCasing casing, bool pluralize = false, bool abbreviate = false)
                : IValueConverter, IMultiValueConverter
    {
        private readonly LetterCasing _casing = casing;
        private readonly bool _pluralize = pluralize;
        private readonly bool _abbreviate = abbreviate;

        public static StringConverter ToUpper { get; } = new StringConverter(LetterCasing.AllCaps);
        public static StringConverter ToLower { get; } = new StringConverter(LetterCasing.LowerCase);
        public static StringConverter ToTitle { get; } = new StringConverter(LetterCasing.Title);
        public static StringConverter Default { get; } = new StringConverter(LetterCasing.Normal);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            // Value
            var result = value switch
            {
                Color color => color.ToName() == color.ToHex() ? color.ToHex() : $"{color.ToName()} ({color.ToHex()})",
                Enum enumValue => enumValue.Humanize(_abbreviate, culture),
                IEnumeration enumValue => enumValue.Humanize(_abbreviate, culture),
                TimeSpan timespan => timespan.Humanize(1, TimeUnit.Year, TimeUnit.Day, culture: culture),
                _ => value.ToString(),
            };

            // Format
            if (parameter is string p)
            {
                if (double.TryParse(result, out var res) && !string.IsNullOrEmpty(result))
                {
                    if (double.IsNaN(res)) return null;

                    var format = _pluralize ? p.TranslateWithCount(res, _abbreviate, culture) : _abbreviate ? p.TranslateAbbreviated(culture) : p.Translate(culture);
                    result = res.ToString(format, culture);
                }
                else if (value is string str && !string.IsNullOrEmpty((string)value))
                {
                    var format = _abbreviate ? p.TranslateAbbreviated(culture) : p.Translate(culture);
                    var translation = str.Translate(culture);
                    result = format.FormatWith(culture, translation);
                }
                else if (value is DateTime date)
                {
                    result = DateTimeToStringConverter.Default.Convert(date, targetType, p, culture)?.ToString();
                }
                else if (value is DateOnly date1)
                {
                    result = DateTimeToStringConverter.Default.Convert(date1, targetType, p, culture)?.ToString();
                }
                else if (value is TimeOnly time)
                {
                    result = DateTimeToStringConverter.Default.Convert(time, targetType, p, culture)?.ToString();
                }
                else if (value is TimeSpan && int.TryParse(p, out var number))
                {
                    var split = result?.Split(" ");
                    if (split != null && number <= split.Length)
                    {
                        result = split[number - 1];
                    }
                }
            }

            // Casing
            return result?.ApplyCase(_casing);
        }

        public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => Convert(values.Length > 0 ? values[0] : null, targetType, values.Length > 1 ? values[1] : null, culture);

        public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
