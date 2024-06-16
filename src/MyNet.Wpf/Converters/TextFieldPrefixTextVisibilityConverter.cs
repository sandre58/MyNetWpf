// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class TextFieldPrefixTextVisibilityConverter : IMultiValueConverter
    {
        public static readonly TextFieldPrefixTextVisibilityConverter Default = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var prefixText = (string)values[1];
            if (string.IsNullOrEmpty(prefixText))
                return Visibility.Collapsed;

            if (values.Length > 2)
            {
                var hint = (string)values[2];
                if (string.IsNullOrEmpty(hint))
                    return Visibility.Visible;
            }

            var isHintInFloatingPosition = (bool)values[0];
            return isHintInFloatingPosition ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
