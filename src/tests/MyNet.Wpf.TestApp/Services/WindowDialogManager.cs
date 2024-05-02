// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Messages;
using MyNet.UI.Dialogs.Models;
using MyNet.UI.Dialogs.Settings;
using MyNet.UI.Locators;
using MyNet.UI.Resources;
using MyNet.Utilities.Messaging;
using MyNet.Wpf.Dialogs;

namespace MyNet.Wpf.TestApp.Services
{
    /// <summary>
    /// Provides methods and properties to display a window dialog.
    /// </summary>
    public static class WindowDialogManager
    {
        #region Fields

        private static IMessageBoxFactory? _messageBoxFactory;
        private static IViewResolver? _viewResolver;
        private static IViewLocator? _viewLocator;
        private static IViewModelLocator? _viewModelLocator;

        #endregion Fields

        #region Members

        public static IList<IDialogViewModel>? OpenedDialogs => DialogService?.OpenedDialogs;

        public static bool HasOpenedDialogs => OpenedDialogs != null && OpenedDialogs.Any();

        public static WindowDialogService DialogService { get; } = new();

        #endregion Members

        public static void Initialize(
            IMessageBoxFactory messageBoxFactory,
            IViewResolver viewResolver,
            IViewLocator viewLocator,
            IViewModelLocator viewModelLocator) => (_messageBoxFactory, _viewResolver, _viewLocator, _viewModelLocator) = (messageBoxFactory, viewResolver, viewLocator, viewModelLocator);

        #region Show

        /// <summary>
        /// Displays a modal dialog.
        /// </summary>
        public static async Task ShowAsync<T>(Action<T>? closeAction = null) where T : class, IDialogViewModel
        {
            var vm = GetViewModel<T>();

            if (vm != null)
            {
                await ShowAsync(vm, closeAction).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Displays a modal dialog.
        /// </summary>
        /// <param name="typeViewModel">The view to include in workspace dialog.</param>
        /// <param name="closeAction"></param>
        public static async Task ShowAsync(Type typeViewModel, Action<IDialogViewModel>? closeAction = null)
        {
            var vm = GetViewModel<IDialogViewModel>(typeViewModel);

            if (vm != null)
            {
                await ShowAsync(vm, closeAction).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="viewModel">The view to include in workspace dialog.</param>
        /// <param name="closeAction"></param>
        public static async Task ShowAsync<T>(T viewModel, Action<T>? closeAction = null) where T : IDialogViewModel
        {
            if (DialogService is null) return;

            var view = GetViewFromViewModel(viewModel.GetType());

            if (view != null)
            {
                if (closeAction is not null)
                    viewModel.CloseRequest += (sender, e) => closeAction(viewModel);

                Messenger.Default.Send(new OpenDialogMessage(DialogType.Dialog, viewModel));

                await DialogService.ShowAsync(view, viewModel).ConfigureAwait(false);
            }
        }

        #endregion Show

        #region ShowDialog

        /// <summary>
        /// Displays a modal dialog.
        /// </summary>
        public static async Task<bool?> ShowDialogAsync<TViewModel>() where TViewModel : class, IDialogViewModel => await ShowDialogAsync(typeof(TViewModel)).ConfigureAwait(false);

        /// <summary>
        /// Displays a modal dialog.
        /// </summary>
        /// <param name="typeViewModel">The view to include in workspace dialog.</param>
        public static async Task<bool?> ShowDialogAsync(Type typeViewModel) => GetViewModel<IDialogViewModel>(typeViewModel) is not IDialogViewModel vm ? false : await ShowDialogAsync(vm).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="viewModel">The view to include in workspace dialog.</param>
        public static async Task<bool?> ShowDialogAsync<T>(T viewModel) where T : IDialogViewModel
        {
            if (DialogService is null) return null;

            Messenger.Default.Send(new OpenDialogMessage(DialogType.ModalDialog, viewModel));

            var view = GetViewFromViewModel(viewModel.GetType());

            return view is null ? false : await DialogService.ShowDialogAsync(view, viewModel).ConfigureAwait(false);
        }

        #endregion ShowDialog

        #region MessageBox

        public static async Task<MessageBoxResult> ShowSuccessAsync(string message,
            MessageBoxResultOption buttons = MessageBoxResultOption.Ok) => await ShowMessageAsync(message, UiResources.Success, buttons, MessageSeverity.Success, MessageBoxResult.OK).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="buttons">Buttons of window.</param>
        public static async Task<MessageBoxResult> ShowInformationAsync(string message,
            MessageBoxResultOption buttons = MessageBoxResultOption.Ok) => await ShowMessageAsync(message, UiResources.Information, buttons, MessageSeverity.Information, MessageBoxResult.OK).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="buttons">Buttons of window.</param>
        public static async Task<MessageBoxResult> ShowErrorAsync(string message,
            MessageBoxResultOption buttons = MessageBoxResultOption.Ok) => await ShowMessageAsync(message, UiResources.Error, buttons, MessageSeverity.Error, MessageBoxResult.OK).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="buttons">Buttons of window.</param>
        public static async Task<MessageBoxResult> ShowWarningAsync(string message,
            MessageBoxResultOption buttons = MessageBoxResultOption.Ok) => await ShowMessageAsync(message, UiResources.Warning, buttons, MessageSeverity.Warning, MessageBoxResult.OK).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        public static async Task<MessageBoxResult> ShowQuestionAsync(string message) => await ShowMessageAsync(message, UiResources.Question, MessageBoxResultOption.YesNo, MessageSeverity.Question, MessageBoxResult.Yes).ConfigureAwait(false);

        /// <summary>
        /// Displays a message dialog.
        /// </summary>
        /// <param name="message">Message.</param>
        public static async Task<MessageBoxResult> ShowQuestionWithCancelAsync(string message) => await ShowMessageAsync(message, UiResources.Question, MessageBoxResultOption.YesNoCancel, MessageSeverity.Question, MessageBoxResult.Yes).ConfigureAwait(false);

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and
        /// that accepts a default message box result and returns a result.
        /// </summary>
        /// <param name="message">
        /// A <see cref="string"/> that specifies the text to display.
        /// </param>
        /// <param name="caption">
        /// A <see cref="string"/> that specifies the title bar caption to display. Default value
        /// is an empty string.
        /// </param>
        /// <param name="button">
        /// A <see cref="MessageBoxButton"/> value that specifies which button or buttons to
        /// display. Default value is <see cref="MessageBoxButton.OK"/>.
        /// </param>
        /// <param name="severity">
        /// A <see cref="MessageBoxImage"/> value that specifies the icon to display. Default value
        /// is <see cref="MessageBoxImage.None"/>.
        /// </param>
        /// <param name="defaultResult">
        /// A <see cref="MessageBoxResult"/> value that specifies the default result of the
        /// message box. Default value is <see cref="MessageBoxResult.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="MessageBoxResult"/> value that specifies which message box button is
        /// clicked by the user.
        /// </returns>
        public static async Task<MessageBoxResult> ShowMessageAsync(
            string message,
            string? caption = "",
            MessageBoxResultOption button = MessageBoxResultOption.Ok,
            MessageSeverity severity = MessageSeverity.Information,
            MessageBoxResult defaultResult = MessageBoxResult.None)
        {
            var vm = _messageBoxFactory?.Create(message, caption, severity, button, defaultResult);
            return vm is null ? MessageBoxResult.None : await ShowMessageBoxAsync(vm).ConfigureAwait(false);
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and
        /// that accepts a default message box result and returns a result.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>
        /// A <see cref="MessageBoxResult"/> value that specifies which message box button is
        /// clicked by the user.
        /// </returns>
        public static async Task<MessageBoxResult> ShowMessageBoxAsync(IMessageBox viewModel)
        {
            if (DialogService is null) return MessageBoxResult.None;

            Messenger.Default.Send(new OpenDialogMessage(DialogType.MessageBox, viewModel));

            return await DialogService.ShowMessageBoxAsync(viewModel).ConfigureAwait(false) ?? MessageBoxResult.None;
        }

        #endregion MessageBox

        #region Files

        /// <summary>
        /// Displays the <see cref="OpenFileDialog"/>.
        /// </summary>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned;
        /// otherwise false.
        /// </returns>
        public static async Task<(bool? result, string filename)> ShowOpenFileDialogAsync()
        {
            if (DialogService is null) return (false, string.Empty);

            Messenger.Default.Send(new OpenDialogMessage(DialogType.FileDialog, null));

            var settings = new OpenFileDialogSettings();
            var result = await DialogService!.ShowOpenFileDialogAsync(settings).ConfigureAwait(false);
            return (result, settings.FileName);
        }

        /// <summary>
        /// Displays the <see cref="OpenFileDialog"/>.
        /// </summary>
        /// <param name="settings">The settings for the open file dialog.</param>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned;
        /// otherwise false.
        /// </returns>
        public static async Task<(bool? result, string filename)> ShowOpenFileDialogAsync(OpenFileDialogSettings settings)
        {
            if (DialogService is null) return (false, string.Empty);

            Messenger.Default.Send(new OpenDialogMessage(DialogType.FileDialog, null));

            var result = await DialogService!.ShowOpenFileDialogAsync(settings).ConfigureAwait(false);
            return (result, settings.FileName);
        }

        /// <summary>
        /// Displays the <see cref="SaveFileDialog"/>.
        /// </summary>
        /// <param name="settings">The settings for the save file dialog.</param>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned;
        /// otherwise false.
        /// </returns>
        public static async Task<(bool? result, string filename)> ShowSaveFileDialogAsync(SaveFileDialogSettings settings)
        {
            if (DialogService is null) return (false, string.Empty);

            Messenger.Default.Send(new OpenDialogMessage(DialogType.FileDialog, null));

            var result = await DialogService!.ShowSaveFileDialogAsync(settings).ConfigureAwait(false);
            return (result, settings.FileName);
        }

        /// <summary>
        /// Displays the <see cref="FolderBrowserDialog"/>.
        /// </summary>
        /// <param name="settings">The settings for the folder browser dialog.</param>
        /// <returns>
        /// If the user clicks the OK button of the dialog that is displayed, true is returned;
        /// otherwise false.
        /// </returns>
        public static async Task<(bool? result, string? selectedPath)> ShowFolderBrowserDialogAsync(OpenFolderDialogSettings settings)
        {
            if (DialogService is null) return (false, string.Empty);

            Messenger.Default.Send(new OpenDialogMessage(DialogType.FileDialog, null));

            var result = await DialogService!.ShowFolderDialogAsync(settings).ConfigureAwait(false);
            return (result, settings.Folder);
        }

        #endregion Files

        private static T? GetViewModel<T>() where T : class => GetViewModel<T>(typeof(T));

        private static T? GetViewModel<T>(Type typeViewModel) where T : class => (T?)_viewModelLocator?.Get(typeViewModel);

        private static object? GetViewFromViewModel(Type viewModelType)
        {
            var viewType = _viewResolver?.Resolve(viewModelType);

            if (viewType == null) throw new InvalidOperationException($"{viewType} has not been resolved.");

            var view = _viewLocator?.Get(viewType);

            return view;
        }
    }
}
