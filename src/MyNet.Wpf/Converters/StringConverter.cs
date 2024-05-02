// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Extensions;
using MyNet.Utilities;
using MyNet.Humanizer;
using MyNet.Utilities.Units;
using MyNet.Utilities.Extensions;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts string values.
    /// </summary>
    public class StringConverter(LetterCasing casing, bool convertPlural = false, bool abbreviation = false, bool initials = false)
                : IValueConverter
    {
        private readonly LetterCasing _casing = casing;
        private readonly bool _convertPlural = convertPlural;
        private readonly bool _abbreviation = abbreviation;
        private readonly bool _initials = initials;

        public static StringConverter ToUpper { get; } = new StringConverter(LetterCasing.AllCaps);
        public static StringConverter ToLower { get; } = new StringConverter(LetterCasing.LowerCase);
        public static StringConverter ToTitle { get; } = new StringConverter(LetterCasing.Title);
        public static StringConverter Default { get; } = new StringConverter(LetterCasing.Normal);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var result = value switch
            {
                Color color => color.ToName() == color.ToHex() ? color.ToHex() : $"{color.ToName()} ({color.ToHex()})",
                Enum enumValue => enumValue.Humanize(_abbreviation),
                IEnumeration enumValue => enumValue.Humanize(_abbreviation),
                TimeSpan timespan => timespan.Humanize(1, TimeUnit.Year, TimeUnit.Day),
                _ => value.ToString(),
            };

            if (parameter is string p)
            {
                if (double.TryParse(result, out var res) && !string.IsNullOrEmpty(result))
                {
                    if (double.IsNaN(res)) return null;

                    var format = _convertPlural ? p.Translate(res) : p.Translate(_abbreviation);
                    result = res.ToString(format, CultureInfo.CurrentCulture);
                }
                else if (value is string str && !string.IsNullOrEmpty((string)value))
                {
                    var format = p.Translate(_abbreviation);
                    var translation = str.Translate() ?? string.Empty;
                    result = format?.FormatWith(CultureInfo.CurrentCulture, translation);
                }
                else if (value is DateTime date)
                {
                    var format = p.TranslateDatePattern();
                    result = date.ToLocalTime().ToString(format, culture);
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

            if (_initials)
                result = result?.GetInitials();

            return result?.ApplyCase(_casing);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
