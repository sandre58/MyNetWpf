// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;
using MyNet.UI.Busy;
using MyNet.UI.Dialogs;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class ExtendedWindow : MetroWindow
{
    public ExtendedWindow() => SetResourceReference(StyleProperty, typeof(ExtendedWindow));

    static ExtendedWindow() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedWindow), new FrameworkPropertyMetadata(typeof(ExtendedWindow)));

    #region DialogService

    public static readonly DependencyProperty DialogServiceProperty = DependencyProperty.Register(nameof(DialogService), typeof(IDialogService), typeof(ExtendedWindow), new UIPropertyMetadata(null));

    public IDialogService DialogService
    {
        get => (IDialogService)GetValue(DialogServiceProperty);
        set => SetValue(DialogServiceProperty, value);
    }

    #endregion DialogService

    #region BusyService

    public static readonly DependencyProperty BusyServiceProperty = DependencyProperty.Register(nameof(BusyService), typeof(IBusyService), typeof(ExtendedWindow), new UIPropertyMetadata(null));

    public IBusyService BusyService
    {
        get => (IBusyService)GetValue(BusyServiceProperty);
        set => SetValue(BusyServiceProperty, value);
    }

    #endregion BusyService
}
