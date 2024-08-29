// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Converters
{
    public enum DateTimeConversion
    {
        None,

        CurrentToLocal,

        CurrentToUtc,

        LocalToCurrent,

        LocalToUtc,

        UtcToCurrent,

        UtcToLocal,
    }

    public sealed class DateTimeConverter : IValueConverter
    {
        private readonly DateTimeConversion _conversion;

        public static readonly DateTimeConverter Default = new(DateTimeConversion.None);
        public static readonly DateTimeConverter CurrentToLocal = new(DateTimeConversion.CurrentToLocal);
        public static readonly DateTimeConverter CurrentToUtc = new(DateTimeConversion.CurrentToUtc);
        public static readonly DateTimeConverter LocalToCurrent = new(DateTimeConversion.LocalToCurrent);
        public static readonly DateTimeConverter LocalToUtc = new(DateTimeConversion.LocalToUtc);
        public static readonly DateTimeConverter UtcToCurrent = new(DateTimeConversion.UtcToCurrent);
        public static readonly DateTimeConverter UtcToLocal = new(DateTimeConversion.UtcToLocal);

        public DateTimeConverter(DateTimeConversion conversion) => _conversion = conversion;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is DateTime date
                ? Convert(date, _conversion)
                : value is DateOnly dateOnly
                ? dateOnly.BeginningOfDay()
                : null;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is DateTime date
                ? (targetType == typeof(DateOnly) ? date.ToDate() : Convert(date, _conversion))
                : null;

        private static DateTime Convert(DateTime date, DateTimeConversion conversion)
            => conversion switch
            {
                DateTimeConversion.CurrentToLocal => GlobalizationService.Current.ConvertToTimeZone(date, TimeZoneInfo.Local),
                DateTimeConversion.CurrentToUtc => GlobalizationService.Current.ConvertToUtc(date),
                DateTimeConversion.LocalToCurrent => GlobalizationService.Current.Convert(date),
                DateTimeConversion.LocalToUtc => date.ToUniversalTime(),
                DateTimeConversion.UtcToCurrent => GlobalizationService.Current.Convert(date),
                DateTimeConversion.UtcToLocal => date.ToLocalTime(),
                _ => date,
            };
    }
}
