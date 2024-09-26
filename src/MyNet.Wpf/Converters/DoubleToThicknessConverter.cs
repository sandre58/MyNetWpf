// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters;

public class DoubleToThicknessConverter : IValueConverter
{
    private enum Mode
    {
        All,

        Top,

        Left,

        Right,

        Bottom,

        Horizontal,

        Vertical,
    }

    public static readonly DoubleToThicknessConverter All = new(Mode.All);

    public static readonly DoubleToThicknessConverter Top = new(Mode.Top);

    public static readonly DoubleToThicknessConverter Left = new(Mode.Left);

    public static readonly DoubleToThicknessConverter Right = new(Mode.Right);

    public static readonly DoubleToThicknessConverter Bottom = new(Mode.Bottom);

    public static readonly DoubleToThicknessConverter Horizontal = new(Mode.Horizontal);

    public static readonly DoubleToThicknessConverter Vertical = new(Mode.Vertical);

    private readonly Mode _mode;

    private DoubleToThicknessConverter(Mode mode) => _mode = mode;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var padding = value is double val ? val : 0;
        return _mode switch
        {
            Mode.Left => new Thickness(padding, 0, 0, 0),
            Mode.Right => new Thickness(0, 0, padding, 0),
            Mode.Top => new Thickness(0, padding, 0, 0),
            Mode.Bottom => new Thickness(0, 0, 0, padding),
            Mode.Horizontal => new Thickness(padding, 0, padding, 0),
            Mode.Vertical => new Thickness(0, padding, 0, padding),
            _ => new Thickness(padding)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
