// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts boolean to visibility values.
    /// </summary>
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        private readonly DateTimeConverterKind _source;
        private readonly DateTimeConverterKind _target;

        public static readonly TimeSpanToDateTimeConverter Default = new(DateTimeConverterKind.Current, DateTimeConverterKind.Current);
        public static readonly TimeSpanToDateTimeConverter CurrentToLocal = new(DateTimeConverterKind.Current, DateTimeConverterKind.Local);
        public static readonly TimeSpanToDateTimeConverter CurrentToUtc = new(DateTimeConverterKind.Current, DateTimeConverterKind.Utc);
        public static readonly TimeSpanToDateTimeConverter LocalToCurrent = new(DateTimeConverterKind.Local, DateTimeConverterKind.Current);
        public static readonly TimeSpanToDateTimeConverter LocalToUtc = new(DateTimeConverterKind.Local, DateTimeConverterKind.Utc);
        public static readonly TimeSpanToDateTimeConverter UtcToCurrent = new(DateTimeConverterKind.Utc, DateTimeConverterKind.Current);
        public static readonly TimeSpanToDateTimeConverter UtcToLocal = new(DateTimeConverterKind.Utc, DateTimeConverterKind.Local);

        public TimeSpanToDateTimeConverter(DateTimeConverterKind source, DateTimeConverterKind target)
        {
            _source = source;
            _target = target;
        }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not TimeSpan ts || ts == TimeSpan.MinValue) return null;

            var utcDate = _source switch
            {
                DateTimeConverterKind.Local => DateTime.Today.ToUtc(ts),
                DateTimeConverterKind.Utc => DateTime.UtcNow.ToUtc(ts),
                _ => GlobalizationService.Current.ConvertToUtc(new DateTime(ts.Ticks, DateTimeKind.Unspecified)),
            };

            return new DateTimeConverter(DateTimeConverterKind.Utc, _target).Convert(utcDate, targetType, parameter, culture);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not DateTime dt || dt == DateTime.MinValue) return null;

            var utcDate = _target switch
            {
                DateTimeConverterKind.Local => dt.ToUtc(),
                DateTimeConverterKind.Utc => dt.ToUtc(),
                _ => GlobalizationService.Current.ConvertToUtc(dt),
            };

            return new DateTimeConverter(_source, DateTimeConverterKind.Utc).ConvertBack(utcDate, targetType, parameter, culture) is DateTime date ? date.TimeOfDay : Binding.DoNothing;
        }
    }
}
