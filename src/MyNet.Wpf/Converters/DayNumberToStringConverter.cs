// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities;
using MyNet.Humanizer;

namespace MyNet.Wpf.Converters
{
    public sealed class DayNumberToStringConverter : IValueConverter
    {
        private readonly LetterCasing _casing;

        public static readonly DayNumberToStringConverter ToUpper = new(LetterCasing.AllCaps);

        public static readonly DayNumberToStringConverter ToLower = new(LetterCasing.LowerCase);

        public static readonly DayNumberToStringConverter ToTitle = new(LetterCasing.Title);

        private DayNumberToStringConverter(LetterCasing casing) => _casing = casing;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime date) return Binding.DoNothing;

            var result = date.Day.ToString(CultureInfo.CurrentCulture.DateTimeFormat);
            if (date == date.FirstDayOfMonth() || date == date.LastDayOfMonth())
            {
                result += " " + date.ToMonthAbbreviated(CultureInfo.CurrentCulture)?.ApplyCase(_casing);
            }

            if (date == date.FirstDayOfYear())
            {
                result += " " + date.Year.ToString(CultureInfo.CurrentCulture.DateTimeFormat);
            }

            return result;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
    }
}
