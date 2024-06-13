// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Models;
using MyNet.UI.Dialogs.Settings;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities.Extensions;
using MyNet.Utilities.IO.FileExtensions;
using MyNet.Wpf.TestApp.Services;
using MyNet.Wpf.TestApp.ViewModels.Contents;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class DialogsWindowsViewModel : NavigableWorkspaceViewModel
    {
        public ICommand OpenMessageDialogCommand { get; set; }

        public ICommand OpenFileDialogCommand { get; set; }

        public ICommand OpenCustomDialogCommand { get; set; }

        public ICommand OpenCustomNonDialogCommand { get; set; }

        public ICommand OpenPerfNonDialogCommand { get; set; }

        public ICommand OpenPerfDialogCommand { get; set; }

        public DialogsWindowsViewModel()
        {
            OpenMessageDialogCommand = CommandsManager.Create<string>(async x =>
            {
                var result = MessageBoxResult.None;
                switch (x)
                {
                    case "success":
                        result = await WindowDialogManager.ShowSuccessAsync("You are successfully logged in your account.", MessageBoxResultOption.Ok).ConfigureAwait(false);
                        break;

                    case "info":
                        result = await WindowDialogManager.ShowInformationAsync("If you close the next window without saving, your changes will the lost..", MessageBoxResultOption.OkCancel).ConfigureAwait(false);
                        break;

                    case "warning":
                        result = await WindowDialogManager.ShowWarningAsync("The file could not be deleted at this times !.", MessageBoxResultOption.Ok).ConfigureAwait(false);
                        break;

                    case "error":
                        result = await WindowDialogManager.ShowErrorAsync("An error occured. The systeme will shut down now.", MessageBoxResultOption.None).ConfigureAwait(false);
                        break;

                    case "question":
                        result = await WindowDialogManager.ShowQuestionAsync("Please confirm before proceed. Do you want to Continue ?.").ConfigureAwait(false);
                        break;

                    case "custom":
                        result = await WindowDialogManager.ShowMessageAsync("Ceci est un message personnalisé.", "Custom", MessageBoxResultOption.YesNoCancel, MessageSeverity.Custom, MessageBoxResult.No).ConfigureAwait(false);
                        break;

                    case "custom2":
                        result = await WindowDialogManager.ShowMessageBoxAsync(new CustomMessageBox()).ConfigureAwait(false);
                        break;
                }

                ToasterManager.ShowInformation($"The result of message dialog : {result}.");
            });

            OpenFileDialogCommand = CommandsManager.Create<string>(async x =>
            {
                var path = string.Empty;
                switch (x)
                {
                    case "folder":
                        var result = await WindowDialogManager.ShowFolderBrowserDialogAsync(OpenFolderDialogSettings.Default).ConfigureAwait(false);
                        path = result.selectedPath;
                        break;

                    case "file":
                        var result1 = await WindowDialogManager.ShowOpenFileDialogAsync().ConfigureAwait(false);
                        path = result1.filename;
                        break;

                    case "images":
                        var result2 = await ShowOpenImagesDialogAsync().ConfigureAwait(false);
                        path = result2.filename;
                        break;

                    case "save":
                        var result3 = await WindowDialogManager.ShowSaveFileDialogAsync(new SaveFileDialogSettings
                        {
                            Filters = FileExtensionInfoProvider.Excel.GetFileFilters(),
                        }).ConfigureAwait(false);
                        path = result3.filename;
                        break;
                }

                ToasterManager.ShowInformation($"The result of file dialog : {path}.");
            });

            OpenCustomDialogCommand = CommandsManager.Create(async () =>
            {
                var vm = new LoginDialogViewModel();
                await WindowDialogManager.ShowDialogAsync(vm).ConfigureAwait(false);

                ShowToasterResult(vm);
            });

            OpenCustomNonDialogCommand = CommandsManager.Create(async () =>
            {
                var vm = new LoginDialogViewModel();
                await WindowDialogManager.ShowAsync(vm, x => ShowToasterResult(x)).ConfigureAwait(false);
            });

            OpenPerfNonDialogCommand = CommandsManager.Create(async () =>
            {
                using var vm = new PerfDialogViewModel();
                await WindowDialogManager.ShowAsync(vm).ConfigureAwait(false);
            });

            OpenPerfDialogCommand = CommandsManager.Create(async () =>
            {
                using var vm = new PerfDialogViewModel();
                await WindowDialogManager.ShowDialogAsync(vm).ConfigureAwait(false);
            });
        }

        private static void ShowToasterResult(LoginDialogViewModel viewModel)
        {
            if (!viewModel.DialogResult.HasValue)
                ToasterManager.ShowWarning("No result.");
            else if (viewModel.DialogResult.Value)
                ToasterManager.ShowSuccess("Dialog has been validated.");
            else
                ToasterManager.ShowError("Dialog has been cancelled");

            ToasterManager.ShowInformation($"Login : {viewModel.Login} ; Password : {viewModel.Password}");
        }

        private static async Task<(bool? result, string filename)> ShowOpenImagesDialogAsync(bool multiselect = false, string initialDirectory = "") => await WindowDialogManager.ShowOpenFileDialogAsync(new OpenFileDialogSettings
        {
            Multiselect = multiselect,
            InitialDirectory = initialDirectory,
            Filters = FileExtensionInfoProvider.AllImages.GetFileFilters(x => x.Translate())
        }).ConfigureAwait(false);
    }

    internal class CustomMessageBox : IMessageBox
    {
        public string? Title { get; set; } = "Custom";

        public MessageBoxResultOption Buttons { get; set; } = MessageBoxResultOption.Ok;

        public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.OK;

    }
}
