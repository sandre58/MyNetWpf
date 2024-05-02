// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using VirtualizingItemsControlBase = Wpf.Ui.Controls.VirtualizingItemsControl;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class VirtualizingItemsControl : VirtualizingItemsControlBase
{
    static VirtualizingItemsControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingItemsControl), new FrameworkPropertyMetadata(typeof(VirtualizingItemsControl)));
}
