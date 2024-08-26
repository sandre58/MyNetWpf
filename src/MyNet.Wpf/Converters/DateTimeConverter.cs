// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Converters
{
    public enum DateTimeConverterKind
    {
        Current,

        Local,

        Utc,
    }

    public sealed class DateTimeConverter : IValueConverter
    {
        private readonly DateTimeConverterKind _source;
        private readonly DateTimeConverterKind _target;

        public static readonly DateTimeConverter CurrentToLocal = new(DateTimeConverterKind.Current, DateTimeConverterKind.Local);
        public static readonly DateTimeConverter CurrentToUtc = new(DateTimeConverterKind.Current, DateTimeConverterKind.Utc);
        public static readonly DateTimeConverter LocalToCurrent = new(DateTimeConverterKind.Local, DateTimeConverterKind.Current);
        public static readonly DateTimeConverter LocalToUtc = new(DateTimeConverterKind.Local, DateTimeConverterKind.Utc);
        public static readonly DateTimeConverter UtcToCurrent = new(DateTimeConverterKind.Utc, DateTimeConverterKind.Current);
        public static readonly DateTimeConverter UtcToLocal = new(DateTimeConverterKind.Utc, DateTimeConverterKind.Local);

        public DateTimeConverter(DateTimeConverterKind source, DateTimeConverterKind target)
        {
            _source = source;
            _target = target;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is not DateTime date
                ? Binding.DoNothing
                : Convert(date, _source, _target);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is not DateTime date
                ? Binding.DoNothing
                : Convert(date, _target, _source);

        private static DateTime Convert(DateTime date, DateTimeConverterKind source, DateTimeConverterKind target)
            => target switch
            {
                DateTimeConverterKind.Local => source switch
                {
                    DateTimeConverterKind.Local => date,
                    DateTimeConverterKind.Utc => date.ToLocalTime(),
                    _ => GlobalizationService.Current.ConvertToTimeZone(date, TimeZoneInfo.Local),
                },
                DateTimeConverterKind.Utc => source switch
                {
                    DateTimeConverterKind.Utc => date,
                    DateTimeConverterKind.Local => date.ToUniversalTime(),
                    _ => GlobalizationService.Current.ConvertToUtc(date),
                },
                _ => source switch
                {
                    DateTimeConverterKind.Current => date,
                    _ => GlobalizationService.Current.Convert(date),
                },
            };
    }
}
