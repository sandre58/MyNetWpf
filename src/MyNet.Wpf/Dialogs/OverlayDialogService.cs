// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MyNet.UI.Dialogs.Models;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Dialogs
{
    public class OverlayDialogService : DialogService
    {
        private Grid? _container;
        private UIElement? _associatedControl;

        public void SetContainer(Grid container) => _container = container;

        public void SetAssociatedControl(UIElement? associatedControl) => _associatedControl = associatedControl;

        /// <inheritdoc />
        public override Task ShowAsync(object view, IDialogViewModel viewModel)
        {
            var control = GetOverlayDialogControl(view, viewModel);
            AddToContainer(control);

            control.Show();

            return Task.CompletedTask;
        }

        protected override async Task<bool?> ShowDialogCoreAsync(object view, IDialogViewModel viewModel)
        {
            var control = GetOverlayDialogControl(view, viewModel);

            AddToContainer(control);

            return await control.ShowDialogAsync().ConfigureAwait(false);
        }

        private void AddToContainer(OverlayDialogControl control)
        {
            if (_container != null && control.Parent == null && !_container.Children.Contains(control))
            {
                if (_container.ColumnDefinitions.Count > 0)
                {
                    Grid.SetColumnSpan(control, _container.ColumnDefinitions.Count);
                }

                if (_container.RowDefinitions.Count > 0)
                {
                    Grid.SetRowSpan(control, _container.RowDefinitions.Count);
                }

                foreach (var previous in _container.Children)
                    if (previous is FrameworkElement frameworkElement)
                        frameworkElement.IsEnabled = false;

                _ = _container.Children.Add(control);
            }
        }

        private OverlayDialogControl GetOverlayDialogControl(object view, IDialogViewModel viewModel)
        {
            var control = CreateOverlayDialogControl();

            control.Content = view;
            control.DataContext = viewModel;
            control.Header = viewModel.Title ?? string.Empty;
            control.AssociatedControl = _associatedControl;

            var overlayDialogObject = viewModel is IOverlayDialog overlayDialogViewModel ? overlayDialogViewModel : view is IOverlayDialog overlayDialogView ? overlayDialogView : null;
            control.CloseOnClickAway = overlayDialogObject?.CloseOnClickAway ?? false;
            control.FocusOnShow = overlayDialogObject?.FocusOnShow ?? true;

            // Load view Model on openning control
            control.Opened += OnControlOpened;

            // Close control when view Model request
            viewModel.CloseRequest += (sender, e) => Schedulers.WpfScheduler.Current.Schedule(async () => await control.CloseAsync(viewModel.DialogResult).ConfigureAwait(false));

            // Hide Control
            control.Closed += OnControlClosedAsync;
            return control;
        }

        private void OnControlOpened(object sender, RoutedEventArgs e)
        {
            if ((sender as OverlayDialogControl)!.DataContext is IDialogViewModel dialogViewModel && dialogViewModel.LoadWhenDialogOpening)
                dialogViewModel.Load();
        }

        private async void OnControlClosedAsync(object sender, RoutedEventArgs e)
        {
            (sender as OverlayDialogControl)!.Opened -= OnControlOpened;
            (sender as OverlayDialogControl)!.Closed -= OnControlClosedAsync;


            if (_container is not null)
                foreach (var previous in _container.Children)
                    if (previous is FrameworkElement frameworkElement)
                        frameworkElement.IsEnabled = true;

            // Wait animation is completed
            await Task.Delay(400).ConfigureAwait(false);

            Schedulers.WpfScheduler.Current.Schedule(() => _container?.Children.Remove(sender as UIElement));
        }

        protected virtual OverlayDialogControl CreateOverlayDialogControl() => new();
    }
}
