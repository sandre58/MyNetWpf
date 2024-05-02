// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Observable.Translatables;

namespace MyNet.Wpf.Converters
{
    public class EnumToTranslatableEnumConverter : IValueConverter
    {
        public static readonly EnumToTranslatableEnumConverter Default = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not Enum val ? null : new TranslatableEnum(val);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is not TranslatableEnum val ? null : val.Value;
    }
}
