// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Converters;
using MyNet.Humanizer;

namespace MyNet.Wpf.Parameters;

public static class NumericUpDownAssist
{
    public static readonly DependencyProperty AcceptableValueProperty = DependencyProperty.RegisterAttached(
        "AcceptableValue", typeof(object), typeof(NumericUpDownAssist), new FrameworkPropertyMetadata(null, HandleAcceptableValueChanged));
    public static void SetAcceptableValue(DependencyObject element, string value) => element.SetValue(AcceptableValueProperty, value);
    public static object GetAcceptableValue(DependencyObject element) => element.GetValue(AcceptableValueProperty);

    private static void HandleAcceptableValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is NumericUpDown numericUpDown && e.NewValue is not null)
        {
            if (e.NewValue.GetType().GetProperty("Value") is not null)
                numericUpDown.SetBinding(NumericUpDown.ValueProperty, new Binding { Source = e.NewValue, Path = new PropertyPath(path: "Value"), Converter = NullableConverter.Default });

            if (e.NewValue.GetType().GetProperty("Min") is not null)
                numericUpDown.SetBinding(NumericUpDown.MinimumProperty, new Binding { Source = e.NewValue, Path = new PropertyPath(path: "Min"), Converter = NullableConverter.Default });

            if (e.NewValue.GetType().GetProperty("Max") is not null)
                numericUpDown.SetBinding(NumericUpDown.MaximumProperty, new Binding { Source = e.NewValue, Path = new PropertyPath(path: "Max"), Converter = NullableConverter.Default });

            if (e.NewValue.GetType().GetProperty("Unit") is not null)
                numericUpDown.SetBinding(TextFieldAssist.SuffixTextProperty, new Binding { Source = e.NewValue, Path = new PropertyPath(path: "Unit"), Converter = new StringConverter(LetterCasing.Normal, false, true, false) });
        }
    }
}
