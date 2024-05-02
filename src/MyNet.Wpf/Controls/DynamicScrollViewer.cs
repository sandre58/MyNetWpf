// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;

using DynamicScrollViewerBase = Wpf.Ui.Controls.DynamicScrollViewer;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class DynamicScrollViewer : DynamicScrollViewerBase
{
    static DynamicScrollViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicScrollViewer), new FrameworkPropertyMetadata(typeof(DynamicScrollViewer)));
}
