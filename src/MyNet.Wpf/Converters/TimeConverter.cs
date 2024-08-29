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
    public class TimeConverter : IValueConverter
    {
        private readonly DateTimeConversion _conversion;

        public static readonly TimeConverter Default = new(DateTimeConversion.None);
        public static readonly TimeConverter CurrentToLocal = new(DateTimeConversion.CurrentToLocal);
        public static readonly TimeConverter CurrentToUtc = new(DateTimeConversion.CurrentToUtc);
        public static readonly TimeConverter LocalToCurrent = new(DateTimeConversion.LocalToCurrent);
        public static readonly TimeConverter LocalToUtc = new(DateTimeConversion.LocalToUtc);
        public static readonly TimeConverter UtcToCurrent = new(DateTimeConversion.UtcToCurrent);
        public static readonly TimeConverter UtcToLocal = new(DateTimeConversion.UtcToLocal);

        public TimeConverter(DateTimeConversion conversion) => _conversion = conversion;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var time = value is TimeOnly timeOnly ? timeOnly : (TimeOnly?)(value is TimeSpan ts && ts != TimeSpan.MinValue ? TimeOnly.FromTimeSpan(ts) : null);

            if (!time.HasValue) return null;

            var date = _conversion switch
            {
                DateTimeConversion.LocalToCurrent => DateTime.Today.At(time.Value),
                DateTimeConversion.LocalToUtc => DateTime.Today.At(time.Value),
                DateTimeConversion.UtcToCurrent => DateTime.UtcNow.At(time.Value),
                DateTimeConversion.UtcToLocal => DateTime.UtcNow.At(time.Value),
                _ => GlobalizationService.Current.Convert(DateTime.UtcNow).At(time.Value)
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

            return new DateTimeConverter(conversion).ConvertBack(date, targetType, parameter, culture) is DateTime convertedDate ? (targetType == typeof(TimeOnly) || targetType == typeof(TimeOnly?) ? convertedDate.ToTime() : convertedDate.TimeOfDay) : null;
        }
    }
}
