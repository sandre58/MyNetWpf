// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Win32;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Models;
using MyNet.UI.Dialogs.Settings;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Dialogs
{
    public abstract class DialogService : IDialogService
    {
        public ObservableCollection<IDialogViewModel> OpenedDialogs { get; private set; } = [];

        /// <inheritdoc />
        public abstract Task ShowAsync(object view, IDialogViewModel viewModel);

        /// <inheritdoc />
        public virtual async Task<bool?> ShowDialogAsync(object view, IDialogViewModel viewModel)
        {
            OpenedDialogs.Add(viewModel);

            var result = await ShowDialogCoreAsync(view, viewModel).ConfigureAwait(false);

            _ = OpenedDialogs.Remove(viewModel);

            return result;
        }

        protected abstract Task<bool?> ShowDialogCoreAsync(object view, IDialogViewModel viewModel);

        public async Task<MessageBoxResult?> ShowMessageBoxAsync(IMessageBox viewModel)
        {
            var view = new MessageView(viewModel);

            _ = await ShowDialogAsync(view, view).ConfigureAwait(false);

            return view.MessageBoxResult;
        }

        /// <inheritdoc />
        public virtual Task<bool?> ShowOpenFileDialogAsync(OpenFileDialogSettings settings)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = settings.DefaultExtension,
                Filter = settings.Filters,
                FileName = settings.FileName,
                CheckFileExists = settings.CheckFileExists,
                CheckPathExists = settings.CheckPathExists,
                InitialDirectory = settings.InitialDirectory,
                Multiselect = settings.Multiselect,
                Title = settings.Title
            };

            var dialogResult = openFileDialog.ShowDialog();

            settings.FileName = openFileDialog.FileName;
            settings.FileNames = openFileDialog.FileNames;
            return Task.FromResult(dialogResult);
        }

        public virtual Task<bool?> ShowSaveFileDialogAsync(SaveFileDialogSettings settings)
        {
            var saveFileDialog = new SaveFileDialog
            {
                CheckFileExists = settings.CheckFileExists,
                CheckPathExists = settings.CheckPathExists,
                CreatePrompt = settings.CreatePrompt,
                DefaultExt = settings.DefaultExtension,
                FileName = settings.FileName,
                Filter = settings.Filters,
                InitialDirectory = settings.InitialDirectory,
                OverwritePrompt = settings.OverwritePrompt,
                Title = settings.Title
            };

            var dialogResult = saveFileDialog.ShowDialog();

            settings.FileName = saveFileDialog.FileName;

            return Task.FromResult(dialogResult);
        }

        /// <inheritdoc />
        public virtual Task<bool?> ShowFolderDialogAsync(OpenFolderDialogSettings settings)
        {
            var openFolderDialog = new OpenFolderDialog
            {
                InitialDirectory = settings.InitialDirectory,
                Title = settings.Title,
                FolderName = settings.Folder,
            };

            var dialogResult = openFolderDialog.ShowDialog();

            settings.Folder = openFolderDialog.FolderName;

            return Task.FromResult(dialogResult);
        }
    }
}
