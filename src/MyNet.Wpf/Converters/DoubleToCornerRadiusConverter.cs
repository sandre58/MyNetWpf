// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters;

public class DoubleToCornerRadiusConverter : IValueConverter
{
    private enum Mode
    {
        All,

        Top,

        Left,

        Right,

        Bottom
    }

    public static readonly DoubleToCornerRadiusConverter All = new(Mode.All);

    public static readonly DoubleToCornerRadiusConverter Top = new(Mode.Top);

    public static readonly DoubleToCornerRadiusConverter Left = new(Mode.Left);

    public static readonly DoubleToCornerRadiusConverter Right = new(Mode.Right);

    public static readonly DoubleToCornerRadiusConverter Bottom = new(Mode.Bottom);

    private readonly Mode _mode;

    private DoubleToCornerRadiusConverter(Mode mode) => _mode = mode;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => _mode switch
    {
        Mode.Left => new CornerRadius(Math.Max(0, (double)value), 0, 0, Math.Max(0, (double)value)),
        Mode.Right => new CornerRadius(0, Math.Max(0, (double)value), Math.Max(0, (double)value), 0),
        Mode.Top => new CornerRadius(Math.Max(0, (double)value), Math.Max(0, (double)value), 0, 0),
        Mode.Bottom => new CornerRadius(0, 0, Math.Max(0, (double)value), Math.Max(0, (double)value)),
        _ => new CornerRadius(Math.Max(0, (double)value))
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
