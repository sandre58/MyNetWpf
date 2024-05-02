// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyNet.Utilities;

namespace MyNet.Wpf.Parameters;

public static class CommandsAssist
{
    #region DoubleClickCommand

    public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached(
        "DoubleClickCommand",
        typeof(object),
        typeof(CommandsAssist),
        new PropertyMetadata(OnDoubleClickCommandChanged));

    public static ICommand GetDoubleClickCommand(UIElement item) => (ICommand)item.GetValue(DoubleClickCommandProperty);

    public static void SetDoubleClickCommand(UIElement item, ICommand value) => item.SetValue(DoubleClickCommandProperty, value);

    private static void OnDoubleClickCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        if (target is Control control)
        {
            if (e.NewValue != null && e.OldValue == null)
                control.MouseDoubleClick += OnDoubleClick;
            else if (e.NewValue == null && e.OldValue != null)
                control.MouseDoubleClick -= OnDoubleClick;
        }
    }

    private static void OnDoubleClick(object? sender, RoutedEventArgs e) => ExecuteCommand(sender, DoubleClickCommandProperty);

    #endregion DoubleClickCommand

    #region ClickCommand

    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.RegisterAttached(
        "ClickCommand",
        typeof(object),
        typeof(CommandsAssist),
        new PropertyMetadata(OnClickCommandChanged));

    public static ICommand GetClickCommand(UIElement item) => (ICommand)item.GetValue(ClickCommandProperty);

    public static void SetClickCommand(UIElement item, ICommand value) => item.SetValue(ClickCommandProperty, value);

    private static void OnClickCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        if (target is Control control)
        {
            if (e.NewValue != null && e.OldValue == null)
                control.MouseLeftButtonUp += OnClick;
            else if (e.NewValue == null && e.OldValue != null)
                control.MouseLeftButtonUp -= OnClick;
        }
    }

    private static void OnClick(object? sender, RoutedEventArgs e) => ExecuteCommand(sender, ClickCommandProperty);


    private static void ExecuteCommand(object? sender, DependencyProperty dpCommand)
    {
        if (sender is Control control)
        {
            var command = (ICommand)control.GetValue(dpCommand);
            var commandParameter = GetCommandParameter(control);
            command.Execute(commandParameter);
        }
    }

    #endregion ClickCommand

    #region EnterCommand

    public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.RegisterAttached(
        "EnterCommand",
        typeof(object),
        typeof(CommandsAssist),
        new PropertyMetadata(OnEnterCommandChanged));

    public static ICommand GetEnterCommand(UIElement item) => (ICommand)item.GetValue(EnterCommandProperty);

    public static void SetEnterCommand(UIElement item, ICommand value) => item.SetValue(EnterCommandProperty, value);

    private static void OnEnterCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        if (target is Control control)
        {
            if (e.NewValue != null && e.OldValue == null)
                control.PreviewKeyDown += OnPreviewKeyDown;
            else if (e.NewValue == null && e.OldValue != null)
                control.PreviewKeyDown -= OnPreviewKeyDown;
        }
    }

    private static void OnPreviewKeyDown(object? sender, KeyEventArgs e) => (e.Key == Key.Enter).IfTrue(() => ExecuteCommand(sender, EnterCommandProperty));

    #endregion EnterCommand

    #region CommandParameter

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
        "CommandParameter",
        typeof(object),
        typeof(CommandsAssist),
        new PropertyMetadata(null));

    public static object GetCommandParameter(UIElement item) => item.GetValue(CommandParameterProperty);

    public static void SetCommandParameter(UIElement item, object value) => item.SetValue(CommandParameterProperty, value);

    #endregion CommandParameter
}
