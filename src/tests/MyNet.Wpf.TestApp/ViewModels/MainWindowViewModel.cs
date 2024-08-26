// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Extensions;
using MyNet.UI.Locators;
using MyNet.UI.Navigation;
using MyNet.UI.Theming;
using MyNet.UI.Toasting;
using MyNet.Utilities;
using MyNet.Utilities.Localization;
using MyNet.Wpf.TestApp.Properties;
using MyNet.Wpf.TestApp.Resources;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class MainWindowViewModel : LocalizableObject
    {
        public bool IsDark { get; set; }

        public INavigationService NavigationService { get; }

        public HomeViewModel HomeViewModel { get; }

        public CultureInfo? SelectedCulture { get; set; }

        public ObservableCollection<CultureInfo?> Cultures { get; private set; } = [];

        public ICommand IsDarkCommand { get; }

        public ICommand IsLightCommand { get; }

        public MainWindowViewModel(INavigationService navigationService, IViewModelLocator viewModelLocator)
        {
            HomeViewModel = viewModelLocator.Get<HomeViewModel>();
            NavigationService = navigationService;
            IsDarkCommand = CommandsManager.Create(() => IsDark = true);
            IsLightCommand = CommandsManager.Create(() => IsDark = false);

            using (PropertyChangedSuspender.Suspend())
            {
                Cultures.AddRange(GlobalizationService.Current.SupportedCultures);
                IsDark = ThemeManager.CurrentTheme?.Base == ThemeBase.Dark;
                SelectedCulture = string.IsNullOrEmpty(GlobalizationService.Current.Culture.Name) ? null : GetSelectedCulture(CultureInfo.GetCultureInfo(GlobalizationService.Current.Culture.Name));
            }
        }

        protected void OnIsDarkChanged()
        {
            if (PropertyChangedSuspender.IsSuspended) return;

            var theme = IsDark ? ThemeBase.Dark : ThemeBase.Light;
            ThemeManager.ApplyBase(theme);
            Settings.Default.ThemeBase = theme.ToString();

            Settings.Default.Save();

            ToasterManager.ShowSuccess(TestAppResources.SettingsHaveBeenSaved);
        }

        private CultureInfo? GetSelectedCulture(CultureInfo culture) => Cultures.Contains(culture) ? culture : culture.Parent is not null ? GetSelectedCulture(culture.Parent) : null;

        protected virtual void OnSelectedCultureChanged()
        {
            var cultureInfo = SelectedCulture ?? CultureInfo.InstalledUICulture;
            GlobalizationService.Current.SetCulture(cultureInfo);
            Settings.Default.Language = cultureInfo.ToString();

            Settings.Default.Save();
        }

        protected override void OnCultureChanged()
        {
            base.OnCultureChanged();
            SelectedCulture = string.IsNullOrEmpty(GlobalizationService.Current.Culture.Name) ? null : GetSelectedCulture(CultureInfo.GetCultureInfo(GlobalizationService.Current.Culture.Name));
        }
    }
}
