// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Xaml.Behaviors;
using PropertyChanged;

namespace MyNet.Wpf.Behaviors;

[DoNotNotify]
public class BehaviorCollection : FreezableCollection<Behavior>
{
    protected override Freezable CreateInstanceCore() => new BehaviorCollection();
}
