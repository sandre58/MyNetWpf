// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class Plane3D : MaterialDesignThemes.Wpf.Plane3D
{
    static Plane3D() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Plane3D), new FrameworkPropertyMetadata(typeof(Plane3D)));
}
