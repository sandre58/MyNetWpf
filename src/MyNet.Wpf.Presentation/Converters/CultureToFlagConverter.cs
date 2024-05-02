// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities.Localization.Extensions;

namespace MyNet.Wpf.Presentation.Converters
{
    public class CultureToFlagConverter : IValueConverter
    {
        public static CultureToFlagConverter Default { get; } = new CultureToFlagConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is CultureInfo valueAsCulture ? valueAsCulture.GetImage() : null;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
