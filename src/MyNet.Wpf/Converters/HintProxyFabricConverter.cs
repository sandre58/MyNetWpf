// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using MyNet.Wpf.Controls.HintProxy;

namespace MyNet.Wpf.Converters;

/// <summary>
/// Converter for <see cref="SmartHint"/> control. Can be extended by <see cref="HintProxyFabric.RegisterBuilder(Func{Control, bool}, Func{Control, IHintProxy})"/> method.
/// </summary>
public class HintProxyFabricConverter : IValueConverter
{
    private static readonly Lazy<HintProxyFabricConverter> Instance = new();

    public static HintProxyFabricConverter Default => Instance.Value;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => HintProxyFabric.Get(value as Control);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
