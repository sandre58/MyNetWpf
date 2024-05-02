// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters;

internal class SliderToolTipConverter : IMultiValueConverter
{
    public static readonly SliderToolTipConverter Default = new();

    public object? Convert(object?[]? values, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (values?.Length >= 2 && values[1] is string format && !string.IsNullOrEmpty(format))
        {
            try
            {
                return string.Format(culture, format, values[0]);
            }
            catch (FormatException)
            {
                // Nothing
            }
        }
        return values?.Length >= 1 && targetType is not null
            ? System.Convert.ChangeType(values[0], targetType, culture)
            : DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
