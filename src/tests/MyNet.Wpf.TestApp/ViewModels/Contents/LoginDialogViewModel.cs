// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.ViewModels.Dialogs;

namespace MyNet.Wpf.TestApp.ViewModels.Contents
{
    internal class LoginDialogViewModel : DialogViewModel
    {
        public string? Login { get; set; }

        public string? Password { get; set; }

        protected override string CreateTitle() => "Login";
    }
}
