// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class ConcatStringsConverter
        : IMultiValueConverter
    {
        public static readonly ConcatStringsConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values is null ? Binding.DoNothing : string.Join(parameter?.ToString(), values);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
