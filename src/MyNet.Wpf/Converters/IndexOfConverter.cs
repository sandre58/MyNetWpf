// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MyNet.Wpf.Converters
{
    public class IndexOfConverter : IValueConverter
    {
        public static IndexOfConverter Default { get; } = new IndexOfConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is IList list && int.TryParse(parameter?.ToString(), out var index) && list.Count > index ? list[index] : Binding.DoNothing;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
