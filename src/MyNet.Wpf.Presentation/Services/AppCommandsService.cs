// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.Services;

namespace MyNet.Wpf.Presentation.Services
{
    public class AppCommandsService : IAppCommandsService
    {
        public void Exit() => System.Windows.Application.Current.Shutdown();
    }
}
