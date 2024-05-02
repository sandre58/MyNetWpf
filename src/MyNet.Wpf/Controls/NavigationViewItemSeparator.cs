// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    [ToolboxItem(true)]
    public class NavigationViewItemSeparator : Separator
    {
        static NavigationViewItemSeparator() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItemSeparator), new FrameworkPropertyMetadata(typeof(NavigationViewItemSeparator)));
    }
}
