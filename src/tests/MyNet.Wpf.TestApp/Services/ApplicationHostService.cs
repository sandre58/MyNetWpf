// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using MyNet.Wpf.TestApp.Resources;
using MyNet.Humanizer;
using MyNet.UI.Busy;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Locators;
using MyNet.UI.Navigation;
using MyNet.UI.Theming;
using MyNet.UI.Toasting;
using MyNet.Utilities.Geography.Extensions;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Logging;
using MyNet.Wpf.TestApp.Properties;
using MyNet.Wpf.TestApp.ViewModels;
using MyNet.Wpf.TestApp.Views;

namespace MyNet.Wpf.TestApp.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService : IHostedService
{
    public ApplicationHostService(
        IThemeService themeService,
        INavigationService navigationService,
        IToasterService toasterService,
        IDialogService dialogService,
        IViewModelResolver viewModelResolver,
        IViewModelLocator viewModelLocator,
        IViewResolver viewResolver,
        IViewLocator viewLocator,
        IBusyServiceFactory busyServiceFactory,
        IMessageBoxFactory messageBoxFactory,
        ICommandFactory commandFactory,
        IScheduler uiScheduler,
        ILogger logger)
    {
        LogManager.Initialize(logger);
        ViewModelManager.Initialize(viewModelResolver, viewModelLocator);
        ViewManager.Initialize(viewResolver, viewLocator);
        ThemeManager.Initialize(themeService);
        NavigationManager.Initialize(navigationService, viewModelLocator);
        ToasterManager.Initialize(toasterService);
        DialogManager.Initialize(dialogService, messageBoxFactory, viewResolver, viewLocator, viewModelLocator);
        WindowDialogManager.Initialize(messageBoxFactory, viewResolver, viewLocator, viewModelLocator);
        BusyManager.Initialize(busyServiceFactory);
        CommandsManager.Initialize(commandFactory);
        UI.Threading.Scheduler.Initialize(uiScheduler);

        TranslationService.RegisterResources(nameof(CountryResources), CountryResources.ResourceManager);
        TranslationService.RegisterResources(nameof(TestAppResources), TestAppResources.ResourceManager);
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        InitializeAplication();

        var window = new MainWindow();
        Application.Current.MainWindow = window;
        window.Closed += OnWindowsClosed;
        window.Show();

        await Task.CompletedTask;

        NavigationManager.NavigateTo<HomeViewModel>();
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

    private static void InitializeAplication()
    {
        if (!string.IsNullOrEmpty(Settings.Default.Language))
            CultureInfoService.Current.SetCulture(Settings.Default.Language);

        ThemeManager.ApplyTheme(new Theme
        {
            Base = Settings.Default.ThemeBase.DehumanizeTo<ThemeBase>(),
            PrimaryColor = Settings.Default.ThemePrimaryColor,
            AccentColor = Settings.Default.ThemeAccentColor
        });
    }

    private void OnWindowsClosed(object? sender, EventArgs e) => Application.Current.Shutdown();
}
