// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class NullableDateTimeToCurrentDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is DateTime ? value : DateTime.Now.Date;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
