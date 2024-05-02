// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Controls
{
    public class PopupEx : MaterialDesignThemes.Wpf.PopupEx
    {
        static PopupEx() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupEx), new FrameworkPropertyMetadata(typeof(PopupEx)));
    }
}
