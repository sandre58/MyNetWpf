// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Wpf.TestApp.Resources;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class TranslationViewModel : NavigableWorkspaceViewModel
    {
        protected override string CreateTitle() => TestAppResources.Translation;

        public ObservableCollection<string> Resources { get; } = [];

        public TranslationViewModel()
        {
        }
    }
}
