// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyNet.UI.Navigation;
using MyNet.Wpf.Animations;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    public partial class NavigationView
    {
        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty FooterMenuItemsProperty = DependencyProperty.Register(nameof(FooterMenuItemsProperty), typeof(ObservableCollection<object>), typeof(NavigationView), new FrameworkPropertyMetadata(new ObservableCollection<object>()));
        public static readonly DependencyProperty ShowNavigationButtonsProperty = DependencyProperty.Register(nameof(ShowNavigationButtons), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsBackEnabledProperty = DependencyProperty.Register(nameof(IsBackEnabled), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsForwardEnabledProperty = DependencyProperty.Register(nameof(IsForwardEnabled), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty ShowAutoSuggestBoxProperty = DependencyProperty.Register(nameof(ShowAutoSuggestBox), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsPaneToggleVisibleProperty = DependencyProperty.Register(nameof(IsPaneToggleVisible), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(nameof(IsPaneOpen), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true, IsPaneOpenChangedCallback));
        public static readonly DependencyProperty IsPaneVisibleProperty = DependencyProperty.Register(nameof(IsPaneVisible), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty OpenPaneLengthProperty = DependencyProperty.Register(nameof(OpenPaneLength), typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(0D));
        public static readonly DependencyProperty CompactPaneLengthProperty = DependencyProperty.Register(nameof(CompactPaneLength), typeof(double), typeof(NavigationView), new FrameworkPropertyMetadata(0D));
        public static readonly DependencyProperty PaneHeaderProperty = DependencyProperty.Register(nameof(PaneHeader), typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty PaneFooterProperty = DependencyProperty.Register(nameof(PaneFooter), typeof(object), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ExpandOnlyActiveItemProperty = DependencyProperty.Register(nameof(ExpandOnlyActiveItem), typeof(bool), typeof(NavigationView), new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register(nameof(TransitionDuration), typeof(int), typeof(NavigationView), new FrameworkPropertyMetadata(500));
        public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(nameof(Transition), typeof(Transition), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty FrameMarginProperty = DependencyProperty.Register(nameof(FrameMargin), typeof(Thickness), typeof(NavigationView), new FrameworkPropertyMetadata(new Thickness()));
        public static readonly DependencyProperty FrameCornerRadiusProperty = DependencyProperty.Register(nameof(FrameCornerRadius), typeof(CornerRadius), typeof(NavigationView), new FrameworkPropertyMetadata(new CornerRadius(0)));
        public static readonly DependencyProperty FrameBackgroundProperty = DependencyProperty.Register(nameof(FrameBackground), typeof(Brush), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty FrameBorderThicknessProperty = DependencyProperty.Register(nameof(FrameBorderThickness), typeof(Thickness), typeof(NavigationView), new FrameworkPropertyMetadata(new Thickness(0)));
        public static readonly DependencyProperty FrameBorderBrushProperty = DependencyProperty.Register(nameof(FrameBorderBrush), typeof(Brush), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register(nameof(HeaderForeground), typeof(Brush), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(NavigationView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NavigationServiceProperty = DependencyProperty.Register(nameof(NavigationService), typeof(INavigationService), typeof(NavigationView), new FrameworkPropertyMetadata(new NavigationService(), OnNavigationServiceChangedCallback));

        public INavigationService NavigationService
        {
            get => (INavigationService)GetValue(NavigationServiceProperty);
            set => SetValue(NavigationServiceProperty, value);
        }

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        public bool ExpandOnlyActiveItem
        {
            get => (bool)GetValue(ExpandOnlyActiveItemProperty);
            set => SetValue(ExpandOnlyActiveItemProperty, value);

        }

        public ObservableCollection<object> FooterMenuItems
        {
            get => (ObservableCollection<object>)GetValue(FooterMenuItemsProperty);
            set => SetValue(FooterMenuItemsProperty, value);
        }

        public bool ShowNavigationButtons
        {
            get => (bool)GetValue(ShowNavigationButtonsProperty);
            set => SetValue(ShowNavigationButtonsProperty, value);
        }

        public bool ShowAutoSuggestBox
        {
            get => (bool)GetValue(ShowAutoSuggestBoxProperty);
            set => SetValue(ShowAutoSuggestBoxProperty, value);
        }

        public bool IsPaneToggleVisible
        {
            get => (bool)GetValue(IsPaneToggleVisibleProperty);
            set => SetValue(IsPaneToggleVisibleProperty, value);
        }

        public bool IsPaneOpen
        {
            get => (bool)GetValue(IsPaneOpenProperty);
            set => SetValue(IsPaneOpenProperty, value);
        }

        public bool IsBackEnabled
        {
            get => (bool)GetValue(IsBackEnabledProperty);
            private set => SetValue(IsBackEnabledProperty, value);
        }

        public bool IsForwardEnabled
        {
            get => (bool)GetValue(IsForwardEnabledProperty);
            private set => SetValue(IsForwardEnabledProperty, value);
        }

        public bool IsPaneVisible
        {
            get => (bool)GetValue(IsPaneVisibleProperty);
            set => SetValue(IsPaneVisibleProperty, value);
        }

        public double OpenPaneLength
        {
            get => (double)GetValue(OpenPaneLengthProperty);
            set => SetValue(OpenPaneLengthProperty, value);
        }

        public double CompactPaneLength
        {
            get => (double)GetValue(CompactPaneLengthProperty);
            set => SetValue(CompactPaneLengthProperty, value);
        }

        public object? PaneHeader
        {
            get => GetValue(PaneHeaderProperty);
            set => SetValue(PaneHeaderProperty, value);
        }

        public object? PaneFooter
        {
            get => GetValue(PaneFooterProperty);
            set => SetValue(PaneFooterProperty, value);
        }

        [Bindable(true), Category("Appearance")]
        public int TransitionDuration
        {
            get => (int)GetValue(TransitionDurationProperty);
            set => SetValue(TransitionDurationProperty, value);
        }

        public Transition Transition
        {
            get => (Transition)GetValue(TransitionProperty);
            set => SetValue(TransitionProperty, value);
        }

        public Thickness FrameMargin
        {
            get => (Thickness)GetValue(FrameMarginProperty);
            set => SetValue(FrameMarginProperty, value);
        }

        public CornerRadius FrameCornerRadius
        {
            get => (CornerRadius)GetValue(FrameCornerRadiusProperty);
            set => SetValue(FrameCornerRadiusProperty, value);
        }

        public Brush FrameBackground
        {
            get => (Brush)GetValue(FrameBackgroundProperty);
            set => SetValue(FrameBackgroundProperty, value);
        }

        public Brush FrameBorderBrush
        {
            get => (Brush)GetValue(FrameBorderBrushProperty);
            set => SetValue(FrameBorderBrushProperty, value);
        }

        public Thickness FrameBorderThickness
        {
            get => (Thickness)GetValue(FrameBorderThicknessProperty);
            set => SetValue(FrameBorderThicknessProperty, value);
        }
        public Brush HeaderForeground
        {
            get => (Brush)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }

        public DataTemplateSelector? ContentTemplateSelector
        {
            get => (DataTemplateSelector?)GetValue(ContentTemplateSelectorProperty);
            set => SetValue(ContentTemplateSelectorProperty, value);
        }

        public NavigationViewItem? SelectedItem { get; protected set; }

        private static void OnNavigationServiceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NavigationView navigationView)
                return;

            navigationView.OnNavigationServiceChanged(e.OldValue as INavigationService, e.NewValue as INavigationService);
        }

        private static void IsPaneOpenChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (d is not NavigationView navigationView)
                return;

            if ((bool)e.NewValue == (bool)e.OldValue)
                return;

            if (navigationView.IsPaneOpen)
                navigationView.OnPaneOpened();
            else
            {
                navigationView.GetAllNavigationViewItems().ForEach(x => x.Collapse());
                navigationView.OnPaneClosed();
            }

            VisualStateManager.GoToState(
                navigationView,
                navigationView.IsPaneOpen ? "PaneOpen" : "PaneCompact",
                true
            );
        }

        private void OnNavigationServiceChanged(INavigationService? oldItem, INavigationService? newItem)
        {
            if (oldItem is not null)
                oldItem.Navigated -= OnNavigatedCallback;

            if (newItem is not null)
                newItem.Navigated += OnNavigatedCallback;
        }
    }
}
