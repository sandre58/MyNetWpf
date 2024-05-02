// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using VirtualizingWrapPanelBase = Wpf.Ui.Controls.VirtualizingWrapPanel;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class VirtualizingWrapPanel : VirtualizingWrapPanelBase
{
    static VirtualizingWrapPanel() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingWrapPanel), new FrameworkPropertyMetadata(typeof(VirtualizingWrapPanel)));
}
