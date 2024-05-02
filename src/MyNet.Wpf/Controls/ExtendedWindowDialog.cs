// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class ExtendedWindowDialog : ExtendedWindow
{
    /// <summary>
    /// Routed command to be used inside dialog content to close a dialog. Use a <see cref="Button.CommandParameter"/> to indicate the result of the parameter.
    /// </summary>
    public static readonly RoutedCommand CloseDialogCommand = new();

    public ExtendedWindowDialog()
    {
        SetResourceReference(StyleProperty, typeof(ExtendedWindowDialog));
        CommandBindings.Add(new CommandBinding(CloseDialogCommand, (sender, e) => Close()));
        InputBindings.Add(new KeyBinding(CloseDialogCommand, Key.Escape, ModifierKeys.None));
    }

    static ExtendedWindowDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedWindowDialog), new FrameworkPropertyMetadata(typeof(ExtendedWindowDialog)));
}
