// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Theming;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Wpf.Helpers;
using MyNet.Wpf.TestApp.Properties;
using MyNet.Wpf.TestApp.Resources;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class ThemeViewModel : NavigableWorkspaceViewModel
    {
        private static readonly ObservableCollection<OpacityData> ControlOpacitiesCollection =
        [
            new OpacityData("Default", "Application.Background"),
            new OpacityData("Secondary", "Application.Background"),
            new OpacityData("Medium", "Application.Background"),
            new OpacityData("Disabled", "Application.Background"),
            new OpacityData("Border", "Application.Background"),
            new OpacityData("Checked", "Application.Background"),
            new OpacityData("Hovered", "Application.Background"),
            new OpacityData("Filled", "Application.Background"),
        ];

        private static readonly ObservableCollection<OpacityData> PrimaryOpacitiesCollection =
        [
            new OpacityData("Default", "Primary"),
            new OpacityData("Secondary", "Primary"),
            new OpacityData("Medium", "Primary"),
            new OpacityData("Disabled", "Primary"),
            new OpacityData("Border", "Primary"),
            new OpacityData("Checked", "Primary"),
            new OpacityData("Hovered", "Primary"),
            new OpacityData("Filled", "Primary"),
        ];

        private static readonly ObservableCollection<OpacityData> AccentOpacitiesCollection =
        [
            new OpacityData("Default", "Accent"),
            new OpacityData("Secondary", "Accent"),
            new OpacityData("Medium", "Accent"),
            new OpacityData("Disabled", "Accent"),
            new OpacityData("Border", "Accent"),
            new OpacityData("Checked", "Accent"),
            new OpacityData("Hovered", "Accent"),
            new OpacityData("Filled", "Accent"),
        ];

        private static readonly ObservableCollection<BrushData> ColorBrushesCollection =
        [
            new BrushData("Primary.Light", "Primary.Light.Foreground"),
            new BrushData("Primary", "Primary.Foreground"),
            new BrushData("Primary.Dark", "Primary.Dark.Foreground"),
            new BrushData("Accent.Light", "Accent.Light.Foreground"),
            new BrushData("Accent", "Accent.Foreground"),
            new BrushData("Accent.Dark", "Accent.Dark.Foreground"),
        ];

        private static readonly ObservableCollection<BrushData> ThemeBrushesCollection =
        [
            new BrushData("Application.Background", "Application.Foreground"),
            new BrushData("Application.Background.Light"),
            new BrushData("Application.Background.Dark"),
            new BrushData("Application.Background.Inverse"),
            new BrushData("Application.Border"),
            new BrushData("Application.Border.Inactive"),
            new BrushData("Application.Foreground"),
            new BrushData("Application.Foreground.Secondary"),
            new BrushData("Application.Foreground.Tertiary"),

            new BrushData("Popup.Background"),
            new BrushData("ScrollBar.Background"),

            new BrushData("Control.Background", "Application.Foreground", 1),
            new BrushData("Control.Border"),
            new BrushData("Control.Border.Secondary"),
            new BrushData("Control.Border.Focus"),

            new BrushData("Shadow"),

            new BrushData("None"),
            new BrushData("Positive"),
            new BrushData("Negative"),
            new BrushData("Warning"),
            new BrushData("Information"),

            new BrushData("Validation.Error"),
        ];

        public ReadOnlyObservableCollection<BrushData> ColorBrushes { get; } = new(ColorBrushesCollection);

        public ReadOnlyObservableCollection<BrushData> ThemeBrushes { get; } = new(ThemeBrushesCollection);

        public ReadOnlyObservableCollection<OpacityData> ControlOpacities { get; } = new(ControlOpacitiesCollection);

        public ReadOnlyObservableCollection<OpacityData> PrimaryOpacities { get; } = new(PrimaryOpacitiesCollection);

        public ReadOnlyObservableCollection<OpacityData> AccentOpacities { get; } = new(AccentOpacitiesCollection);

        public Color PrimaryColor { get; set; }

        public Color AccentColor { get; set; }

        public ICommand SaveInSettingsCommand { get; }

        public ThemeViewModel()
        {
            using (PropertyChangedSuspender.Suspend())
            {
                PrimaryColor = (Color)ColorConverter.ConvertFromString(ThemeManager.CurrentTheme?.PrimaryColor);
                AccentColor = (Color)ColorConverter.ConvertFromString(ThemeManager.CurrentTheme?.AccentColor);
                SaveInSettingsCommand = CommandsManager.Create(SaveInSettings);
            }
        }

        protected override string CreateTitle() => TestAppResources.Theme;

        protected void OnPrimaryColorChanged()
        {
            if (PropertyChangedSuspender.IsSuspended) return;
            UpdatePrimaryColor();
        }

        protected void OnAccentColorChanged()
        {
            if (PropertyChangedSuspender.IsSuspended) return;
            UpdateAccentColor();
        }

        private void UpdatePrimaryColor() => ThemeManager.ApplyPrimaryColor(PrimaryColor.ToString(CultureInfo.InvariantCulture));

        private void UpdateAccentColor() => ThemeManager.ApplyAccentColor(AccentColor.ToString(CultureInfo.InvariantCulture));

        private void SaveInSettings()
        {
            Settings.Default.ThemePrimaryColor = PrimaryColor.ToString(CultureInfo.InvariantCulture);
            Settings.Default.ThemeAccentColor = AccentColor.ToString(CultureInfo.InvariantCulture);

            Settings.Default.Save();

            ToasterManager.ShowSuccess(TestAppResources.SettingsHaveBeenSaved);
        }
    }

    internal class BrushData : ObservableObject
    {
        public BrushData(string displayName, string? foregroundDisplayName = null, double borderThickness = 0)
        {
            DisplayName = displayName;
            ForegroundDisplayName = foregroundDisplayName;
            Name = $"MyNet.Brushes.{displayName}";
            Brush = WpfHelper.GetResource<SolidColorBrush>(Name);
            BorderThickness = new Thickness(borderThickness);

            if (!string.IsNullOrEmpty(ForegroundDisplayName))
            {
                ForegroundName = $"MyNet.Brushes.{foregroundDisplayName}";
                ForegroundBrush = WpfHelper.GetResource<SolidColorBrush>(ForegroundName);
            }

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            Brush = WpfHelper.GetResource<SolidColorBrush>(Name);

            if (!string.IsNullOrEmpty(ForegroundName))
                ForegroundBrush = WpfHelper.GetResource<SolidColorBrush>(ForegroundName);
        }

        public string DisplayName { get; }

        public string Name { get; }

        public string? ForegroundDisplayName { get; }

        public string? ForegroundName { get; }

        public SolidColorBrush Brush { get; private set; }

        public SolidColorBrush? ForegroundBrush { get; private set; }

        public Thickness BorderThickness { get; }
    }

    internal class OpacityData : ObservableObject
    {
        private readonly string _brushName;

        public OpacityData(string displayName, string brushName)
        {
            DisplayName = displayName;
            Name = $"MyNet.Opacity.{displayName}";
            _brushName = brushName;
            Brush = WpfHelper.GetResource<SolidColorBrush>($"MyNet.Brushes.{brushName}");
            Opacity = WpfHelper.GetResource<double>(Name);

            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e) => Brush = WpfHelper.GetResource<SolidColorBrush>($"MyNet.Brushes.{_brushName}");

        public string DisplayName { get; }

        public string Name { get; }

        public double Opacity { get; }

        public SolidColorBrush Brush { get; private set; }
    }
}
