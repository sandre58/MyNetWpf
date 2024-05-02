// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Converters
{
    public sealed class DateTimeToStringConverter : IValueConverter
    {

        public static readonly DateTimeToStringConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not TimeSpan time ? Binding.DoNothing : DateTime.Today.SetTime(time).ToString(parameter?.ToString(), culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DateTime.TryParse(value?.ToString(), culture, out var date) ? date.TimeOfDay : Binding.DoNothing;
    }
}
