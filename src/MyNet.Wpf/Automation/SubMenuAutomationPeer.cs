// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Controls;
using System.Windows.Automation.Peers;

namespace MyNet.Wpf.Automation
{
    public class SubmenuAutomationPeer(Submenu owner) : FrameworkElementAutomationPeer(owner)
    {
        [System.Diagnostics.Contracts.Pure]
        protected override string GetClassNameCore() => "Submenu";

        [System.Diagnostics.Contracts.Pure]
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Menu;
    }
}
