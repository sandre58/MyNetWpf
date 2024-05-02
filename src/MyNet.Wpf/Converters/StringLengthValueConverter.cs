// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MyNet.Wpf.Converters;

internal class StringLengthValueConverter : IValueConverter
{
    public static readonly StringLengthValueConverter Default = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is string stringValue ? stringValue.Length : (object)0;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
