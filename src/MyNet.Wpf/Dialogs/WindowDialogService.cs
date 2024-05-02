// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using MyNet.UI.Dialogs.Models;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Dialogs
{
    public class WindowDialogService : DialogService
    {
        /// <inheritdoc />
        public override Task ShowAsync(object view, IDialogViewModel viewModel)
        {
            GetWindow(view, viewModel, false).Show();

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override Task<bool?> ShowDialogCoreAsync(object view, IDialogViewModel viewModel)
        {
            var result = GetWindow(view, viewModel, true).ShowDialog();

            return Task.FromResult(result);
        }

        private Window GetWindow(object view, IDialogViewModel viewModel, bool isDialog)
        {
            var dialog = CreateWindow();
            PrepareWindow(dialog);

            dialog.Content = view;
            dialog.DataContext = viewModel;
            dialog.Title = viewModel.Title ?? string.Empty;

            if (view is ContentDialog contentDialog)
                contentDialog.ShowCloseButton = false;

            // Load view Model on openning control
            dialog.Loaded += OnWindowLoaded;

            // Close control when view Model request
            viewModel.CloseRequest += (sender, e) => Schedulers.WpfScheduler.Current.Schedule(() => dialog.Close());

            // Manage control closing by view Model
            if (isDialog) dialog.Closing += OnWindowClosingAsync;

            // Hide Control
            dialog.Closed += OnWindowClosedAsync;

            return dialog;
        }

        protected virtual Window CreateWindow() => new ExtendedWindowDialog();

        protected virtual void PrepareWindow(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.WindowState = WindowState.Normal;
            window.ResizeMode = ResizeMode.NoResize;
            window.ShowInTaskbar = false;
            window.Height = double.NaN;
            window.Width = double.NaN;
            window.MinWidth = 150;
            window.MinHeight = 100;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Focusable = true;

            if (window is ExtendedWindow extendedWindow)
            {
                extendedWindow.ShowMaxRestoreButton = false;
                extendedWindow.ShowIconOnTitleBar = false;
                extendedWindow.ShowMinButton = false;
            }
        }

        private void OnWindowLoaded(object? sender, RoutedEventArgs e)
        {
            var dialogViewModel = ((sender as Window)!.DataContext as IDialogViewModel);

            if (dialogViewModel != null && dialogViewModel.LoadWhenDialogOpening)
                dialogViewModel.Load();
        }

        private async void OnWindowClosingAsync(object? sender, CancelEventArgs e)
        {
            (sender as Window)!.DialogResult = ((sender as Window)!.DataContext as IDialogViewModel)!.DialogResult;

            e.Cancel = !await ((sender as Window)!.DataContext as IDialogViewModel)!.CanCloseAsync().ConfigureAwait(false);
        }

        private void OnWindowClosedAsync(object? sender, EventArgs e)
        {
            (sender as Window)!.Loaded -= OnWindowLoaded;
            (sender as Window)!.Closing -= OnWindowClosingAsync;
            (sender as Window)!.Closed -= OnWindowClosedAsync;
        }
    }
}
