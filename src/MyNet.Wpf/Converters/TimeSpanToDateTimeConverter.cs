// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualBasic;
using MyNet.Utilities;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts boolean to visibility values.
    /// </summary>
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        private readonly DateTimeConversion _conversion;

        public static readonly TimeSpanToDateTimeConverter Default = new(DateTimeConversion.None);
        public static readonly TimeSpanToDateTimeConverter CurrentToLocal = new(DateTimeConversion.CurrentToLocal);
        public static readonly TimeSpanToDateTimeConverter CurrentToUtc = new(DateTimeConversion.CurrentToUtc);
        public static readonly TimeSpanToDateTimeConverter LocalToCurrent = new(DateTimeConversion.LocalToCurrent);
        public static readonly TimeSpanToDateTimeConverter LocalToUtc = new(DateTimeConversion.LocalToUtc);
        public static readonly TimeSpanToDateTimeConverter UtcToCurrent = new(DateTimeConversion.UtcToCurrent);
        public static readonly TimeSpanToDateTimeConverter UtcToLocal = new(DateTimeConversion.UtcToLocal);

        public TimeSpanToDateTimeConverter(DateTimeConversion conversion) => _conversion = conversion;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TimeSpan ts || ts == TimeSpan.MinValue) return null;

            var date = _conversion switch
            {
                DateTimeConversion.LocalToCurrent => DateTime.Today.SetTime(ts),
                DateTimeConversion.LocalToUtc => DateTime.Today.SetTime(ts),
                DateTimeConversion.UtcToCurrent => DateTime.UtcNow.SetTime(ts),
                DateTimeConversion.UtcToLocal => DateTime.UtcNow.SetTime(ts),
                _ => GlobalizationService.Current.Convert(DateTime.UtcNow).SetTime(ts)
            };

            return new DateTimeConverter(_conversion).Convert(date, targetType, parameter, culture);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime date || date == DateTime.MinValue) return null;

            var conversion = _conversion switch
            {
                DateTimeConversion.CurrentToLocal => DateTimeConversion.UtcToCurrent,
                DateTimeConversion.CurrentToUtc => DateTimeConversion.LocalToCurrent,
                DateTimeConversion.LocalToCurrent => DateTimeConversion.CurrentToLocal,
                DateTimeConversion.LocalToUtc => DateTimeConversion.UtcToLocal,
                DateTimeConversion.UtcToCurrent => DateTimeConversion.CurrentToUtc,
                DateTimeConversion.UtcToLocal => DateTimeConversion.LocalToUtc,
                _ => DateTimeConversion.None
            };

            return new DateTimeConverter(conversion).ConvertBack(date, targetType, parameter, culture) is DateTime convertedDate ? convertedDate.TimeOfDay : Binding.DoNothing;
        }
    }
}
