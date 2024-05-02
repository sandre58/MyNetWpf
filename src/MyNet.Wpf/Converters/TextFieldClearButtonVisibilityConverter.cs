// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class TextFieldClearButtonVisibilityConverter : IMultiValueConverter
    {
        public static readonly TextFieldClearButtonVisibilityConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => !(bool)values[0]
                ? Visibility.Collapsed
                : (object)((bool)values[1] // Hint.IsContentNullOrEmpty
                ? Visibility.Hidden
                : Visibility.Visible);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
