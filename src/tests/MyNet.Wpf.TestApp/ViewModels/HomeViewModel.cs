// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.TestApp.Resources;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class HomeViewModel : NavigableWorkspaceViewModel
    {
        protected override string CreateTitle() => TestAppResources.Home;
    }
}
