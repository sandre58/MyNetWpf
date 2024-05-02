// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Automation;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    [Localizability(LocalizationCategory.Menu)]
    public class Submenu : MenuBase
    {
        static Submenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Submenu), new FrameworkPropertyMetadata(typeof(Submenu)));
            IsTabStopProperty.OverrideMetadata(typeof(Submenu), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Submenu), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(Submenu), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Submenu), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        protected override AutomationPeer OnCreateAutomationPeer() => new SubmenuAutomationPeer(this);
    }
}
