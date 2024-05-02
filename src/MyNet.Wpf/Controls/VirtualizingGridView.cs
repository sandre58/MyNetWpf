// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using VirtualizingGridViewBase = Wpf.Ui.Controls.VirtualizingGridView;

namespace MyNet.Wpf.Controls;

public class VirtualizingGridView : VirtualizingGridViewBase
{
    static VirtualizingGridView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualizingGridView), new FrameworkPropertyMetadata(typeof(VirtualizingGridView)));
}
