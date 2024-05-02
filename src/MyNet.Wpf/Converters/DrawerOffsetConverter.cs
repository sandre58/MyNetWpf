// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class DrawerOffsetConverter : IValueConverter
    {
        public static readonly DrawerOffsetConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = value as double? ?? 0;
            if (double.IsInfinity(d) || double.IsNaN(d)) d = 0;

            var dock = parameter is Dock p ? p : Dock.Left;
            return dock switch
            {
                Dock.Top => new Thickness(0, 0 - d, 0, 0),
                Dock.Bottom => new Thickness(0, 0, 0, 0 - d),
                Dock.Right => new Thickness(0, 0, 0 - d, 0),
                _ => (object)new Thickness(0 - d, 0, 0, 0),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
