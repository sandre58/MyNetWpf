// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Notifications;
using MyNet.UI.Resources;
using MyNet.UI.Toasting.Settings;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Wpf.TestApp.Resources;
using MyNet.Wpf.Toasting;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class NotificationsViewModel : NavigableWorkspaceViewModel
    {
        private ToasterService? _toasterService;

        public bool IsUnique { get; set; }

        public double DurationInSeconds { get; set; } = 3.5;

        public int MaxItems { get; set; } = 50;

        public ToasterPosition Position { get; set; } = ToasterPosition.BottomRight;

        public double Width { get; set; } = 300;

        public double OffsetY { get; set; } = 10;

        public double OffsetX { get; set; } = 10;

        public ICommand ShowSuccessCommand { get; set; }

        public ICommand ShowErrorCommand { get; set; }

        public ICommand ShowWarningCommand { get; set; }

        public ICommand ShowInformationCommand { get; set; }

        public ICommand ShowNoneCommand { get; set; }

        public ICommand ShowCustomCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand ApplySettingsCommand { get; set; }

        public NotificationsViewModel()
        {
            ResetToasterService();

            ShowSuccessCommand = CommandsManager.Create(() => _toasterService?.Show(new MessageNotification("You have win world cup !", UiResources.Success, NotificationSeverity.Success), SettingsFromSeverity(NotificationSeverity.Success, ToastClosingStrategy.AutoClose), IsUnique));

            ShowErrorCommand = CommandsManager.Create(() => _toasterService?.Show(new MessageNotification("You have an error. Click on me for more information.", UiResources.Error, NotificationSeverity.Error), SettingsFromSeverity(NotificationSeverity.Error, ToastClosingStrategy.CloseButton), IsUnique, x => _toasterService?.Show(new MessageNotification("You are click on error.", UiResources.Information, NotificationSeverity.Information), SettingsFromSeverity(NotificationSeverity.Information, ToastClosingStrategy.AutoClose), IsUnique)));

            ShowWarningCommand = CommandsManager.Create(() => _toasterService?.Show(new MessageNotification("You have a warning. Click on me for more information.", UiResources.Warning, NotificationSeverity.Warning), SettingsFromSeverity(NotificationSeverity.Warning, ToastClosingStrategy.Both), IsUnique, x => _toasterService?.Show(new MessageNotification("You are click on error.", UiResources.Information, NotificationSeverity.Information), SettingsFromSeverity(NotificationSeverity.Information, ToastClosingStrategy.AutoClose), IsUnique)));

            ShowInformationCommand = CommandsManager.Create(() => _toasterService?.Show(new MessageNotification("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.t has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.", UiResources.Information, NotificationSeverity.Information), SettingsFromSeverity(NotificationSeverity.Information, ToastClosingStrategy.AutoClose), IsUnique));

            ShowNoneCommand = CommandsManager.Create(() => _toasterService?.Show(new MessageNotification("It's a standard message.", "Header", NotificationSeverity.None), ToastSettings.Default, IsUnique));

            ApplySettingsCommand = CommandsManager.Create(() =>
            {
                ResetToasterService();

                _toasterService?.Show(new MessageNotification(TestAppResources.SettingsHaveBeenSaved, UiResources.Success, NotificationSeverity.Success), SettingsFromSeverity(NotificationSeverity.Success, ToastClosingStrategy.AutoClose));
            });

            ShowCustomCommand = CommandsManager.Create(() => _toasterService?.Show(new CustomNotification(), ToastSettings.Default, IsUnique));

            ClearCommand = CommandsManager.Create(() => _toasterService?.Clear());
        }

        private void ResetToasterService()
        {
            (_toasterService as IDisposable)?.Dispose();
            _toasterService = new ToasterService(new ToasterSettings()
            {
                Duration = TimeSpan.FromSeconds(DurationInSeconds),
                MaxItems = MaxItems,
                OffsetX = OffsetX,
                OffsetY = OffsetY,
                Position = Position,
                Width = Width,
            });
        }

        public static ToastSettings SettingsFromSeverity(NotificationSeverity severity, ToastClosingStrategy toastClosingStrategy)
        {
            var settings = new ToastSettings() { ClosingStrategy = toastClosingStrategy };

            switch (severity)
            {
                case NotificationSeverity.Error:
                    settings.Style.Add("Background", "MyNet.Brushes.Negative");
                    settings.Style.Add("BorderBrush", "MyNet.Brushes.Negative");
                    break;

                case NotificationSeverity.Success:
                    settings.Style.Add("Background", "MyNet.Brushes.Positive");
                    settings.Style.Add("BorderBrush", "MyNet.Brushes.Positive");
                    break;

                case NotificationSeverity.Warning:
                    settings.Style.Add("Background", "MyNet.Brushes.Warning");
                    settings.Style.Add("BorderBrush", "MyNet.Brushes.Warning");
                    break;

                case NotificationSeverity.Information:
                    settings.Style.Add("Background", "MyNet.Brushes.Information");
                    settings.Style.Add("BorderBrush", "MyNet.Brushes.Information");
                    break;
                default:
                    break;
            }

            return settings;
        }
    }

    internal class CustomNotification : ObservableObject, INotification
    {
        public string Category => string.Empty;

        public Guid Id => Guid.NewGuid();
    }
}
