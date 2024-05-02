// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.ViewModels.Workspace;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class InputsRichTextEditorViewModel : NavigableWorkspaceViewModel
    {
        public string? Text { get; set; } = "Ceci est un test";
    }
}
