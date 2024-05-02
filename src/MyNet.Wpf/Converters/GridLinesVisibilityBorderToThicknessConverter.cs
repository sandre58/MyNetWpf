// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    internal class GridLinesVisibilityBorderToThicknessConverter : IValueConverter
    {
        public static readonly GridLinesVisibilityBorderToThicknessConverter Default = new();

        private const double GridLinesThickness = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DataGridGridLinesVisibility visibility)
                return Binding.DoNothing;

            var thickness = parameter as double? ?? GridLinesThickness;

            return visibility switch
            {
                DataGridGridLinesVisibility.All => new Thickness(0, 0, thickness, thickness),
                DataGridGridLinesVisibility.Horizontal => new Thickness(0, 0, 0, thickness),
                DataGridGridLinesVisibility.Vertical => new Thickness(0, 0, thickness, 0),
                DataGridGridLinesVisibility.None => new Thickness(0),
                _ => throw new NotSupportedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
