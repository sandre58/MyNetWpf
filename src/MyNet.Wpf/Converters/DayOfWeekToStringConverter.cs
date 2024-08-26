// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Humanizer;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts string values.
    /// </summary>
    public class DayOfWeekStringConverter
        : IValueConverter
    {

        private enum NameType
        {
            Full,

            Abbreviation,

            FirstLetter
        }

        private readonly NameType _type;
        private readonly LetterCasing _casing;

        private DayOfWeekStringConverter(NameType type, LetterCasing casing)
        {
            _type = type;
            _casing = casing;
        }

        public static readonly DayOfWeekStringConverter ToAbbreviationToUpper = new(NameType.Abbreviation, LetterCasing.AllCaps);

        public static readonly DayOfWeekStringConverter ToAbbreviationToLower = new(NameType.Abbreviation, LetterCasing.LowerCase);

        public static readonly DayOfWeekStringConverter ToAbbreviationToTitle = new(NameType.Abbreviation, LetterCasing.Title);

        public static readonly DayOfWeekStringConverter ToUpper = new(NameType.Full, LetterCasing.AllCaps);

        public static readonly DayOfWeekStringConverter ToLower = new(NameType.Full, LetterCasing.LowerCase);

        public static readonly DayOfWeekStringConverter ToTitle = new(NameType.Full, LetterCasing.Title);

        public static readonly DayOfWeekStringConverter ToFirstLetterToUpper = new(NameType.FirstLetter, LetterCasing.AllCaps);

        public static readonly DayOfWeekStringConverter ToFirstLetterToLower = new(NameType.FirstLetter, LetterCasing.LowerCase);

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
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DayOfWeek day;
            if (value is DayOfWeek dayValue)
                day = dayValue;
            else if (value is DateTime date)
                day = date.DayOfWeek;
            else
                return Binding.DoNothing;

            var dayStr = culture.DateTimeFormat.GetDayName(day);

            if (_type == NameType.Abbreviation)
            {
                dayStr = culture.DateTimeFormat.GetShortestDayName(day);
            }

            if (_type == NameType.FirstLetter)
            {
                dayStr = dayStr[..1];
            }

            return dayStr.ApplyCase(_casing);
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
