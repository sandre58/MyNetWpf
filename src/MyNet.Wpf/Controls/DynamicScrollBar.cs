// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;

using DynamicScrollBarBase = Wpf.Ui.Controls.DynamicScrollBar;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class DynamicScrollBar : DynamicScrollBarBase
{
    static DynamicScrollBar() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicScrollBar), new FrameworkPropertyMetadata(typeof(DynamicScrollBar)));
}
