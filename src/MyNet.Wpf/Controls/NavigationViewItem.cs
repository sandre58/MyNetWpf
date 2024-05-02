// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyNet.UI.Locators;
using MyNet.UI.Navigation.Models;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = TemplateMainBorder, Type = typeof(Border))]
    public class NavigationViewItem : HeaderedItemsControl
    {
        private bool _userInitiatedPress;
        private const string TemplateMainBorder = "PART_MainBorder";

        private Border? _mainBorder;

        static NavigationViewItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationViewItem), new FrameworkPropertyMetadata(typeof(NavigationViewItem)));

        #region Static properties

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(NavigationViewItem), new PropertyMetadata(false, OnIsActiveChanged));
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(NavigationViewItem), new PropertyMetadata(false, OnIsExpandedChanged));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(NavigationViewItem), new PropertyMetadata(null));
        public static readonly DependencyProperty TargetPageProperty = DependencyProperty.Register(nameof(TargetPage), typeof(INavigationPage), typeof(NavigationViewItem), new PropertyMetadata(null));
        public static readonly DependencyProperty TargetPageTypeProperty = DependencyProperty.Register(nameof(TargetPageType), typeof(Type), typeof(NavigationViewItem), new PropertyMetadata(null, OnTargetPageTypeChangedCallback));
        public static readonly DependencyProperty TargetViewTypeProperty = DependencyProperty.Register(nameof(TargetViewType), typeof(Type), typeof(NavigationViewItem), new PropertyMetadata(null, OnTargetViewTypeChangedCallback));
        public static readonly DependencyProperty NavigationParametersProperty = DependencyProperty.Register(nameof(NavigationParameters), typeof(NavigationParameters), typeof(NavigationViewItem), new PropertyMetadata(null));
        public static readonly DependencyProperty IconForegroundProperty = DependencyProperty.Register(nameof(IconForeground), typeof(Brush), typeof(NavigationViewItem), new FrameworkPropertyMetadata(SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly RoutedEvent ActivatedEvent = EventManager.RegisterRoutedEvent(nameof(Activated), RoutingStrategy.Direct, typeof(TypedEventHandler<NavigationViewItem, RoutedEventArgs>), typeof(NavigationViewItem));
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(nameof(Expanded), RoutingStrategy.Direct, typeof(TypedEventHandler<NavigationViewItem, RoutedEventArgs>), typeof(NavigationViewItem));
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NavigationViewItem));

        #endregion

        #region Properties

        public NavigationView? NavigationView { get; private set; }

        [Browsable(false), ReadOnly(true)]
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        [Browsable(false), ReadOnly(true)]
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        [Bindable(true), Category("Appearance")]
        public object? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        [Bindable(true), Category("Appearance")]
        public Brush IconForeground
        {
            get => (Brush)GetValue(IconForegroundProperty);
            set => SetValue(IconForegroundProperty, value);
        }

        public INavigationPage? TargetPage
        {
            get => (INavigationPage)GetValue(TargetPageProperty);
            set => SetValue(TargetPageProperty, value);
        }

        public Type? TargetPageType
        {
            get => (Type?)GetValue(TargetPageTypeProperty);
            set => SetValue(TargetPageTypeProperty, value);
        }

        public Type? TargetViewType
        {
            get => (Type?)GetValue(TargetViewTypeProperty);
            set => SetValue(TargetViewTypeProperty, value);
        }

        public NavigationParameters? NavigationParameters
        {
            get => (NavigationParameters?)GetValue(NavigationParametersProperty);
            set => SetValue(NavigationParametersProperty, value);
        }

        public event TypedEventHandler<NavigationViewItem, RoutedEventArgs> Activated
        {
            add => AddHandler(ActivatedEvent, value);
            remove => RemoveHandler(ActivatedEvent, value);
        }

        public event TypedEventHandler<NavigationViewItem, RoutedEventArgs> Expanded
        {
            add => AddHandler(ExpandedEvent, value);
            remove => RemoveHandler(ExpandedEvent, value);
        }

        [Category("Behavior")]
        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        #endregion

        public IEnumerable<NavigationViewItem> GetNavigationViewItems() => Items.OfType<object>().Select(x => ItemContainerGenerator.ContainerFromItem(x)).OfType<NavigationViewItem>();

        internal void SetNavigationView(NavigationView parent) => NavigationView = parent;

        public virtual void Activate()
        {
            if (!IsActive)
                IsActive = true;
        }

        public virtual void Deactivate()
        {
            if (IsActive)
                IsActive = false;

            GetNavigationViewItems().ForEach(x => x.Deactivate());
        }

        public void Expand()
        {
            if (HasItems && !IsExpanded)
                IsExpanded = true;
        }

        public void Collapse()
        {
            if (IsExpanded)
                IsExpanded = false;

            GetNavigationViewItems().ForEach(x => x.Collapse());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mainBorder = GetTemplateChild(TemplateMainBorder) as Border;
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NavigationViewItem navigationViewItem) return;

            if ((bool)e.NewValue)
                navigationViewItem.RaiseEvent(new RoutedEventArgs(ActivatedEvent, navigationViewItem));
        }

        private void OnItemActivated(NavigationViewItem sender, RoutedEventArgs args) => Activate();

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NavigationViewItem navigationViewItem) return;

            if ((bool)e.NewValue)
                navigationViewItem.RaiseEvent(new RoutedEventArgs(ExpandedEvent, navigationViewItem));
        }

        private void OnItemExpanded(NavigationViewItem sender, RoutedEventArgs args) => Expand();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled) _userInitiatedPress = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || !_userInitiatedPress) return;

            if (_mainBorder is not null)
            {
                var pt = e.GetPosition(_mainBorder);
                if (pt.X < 0 || pt.Y < 0 || pt.X > _mainBorder.ActualWidth || pt.Y > _mainBorder.ActualHeight) return;
            }

            RaiseEvent(new RoutedEventArgs(ClickEvent, this));

            if (!e.Handled) _userInitiatedPress = false;
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers is not ModifierKeys.None)
            {
                // We handle Left/Up/Right/Down keys for keyboard navigation only,
                // so no modifiers are needed.
                return;
            }

            switch (e.Key)
            {
                // We use Direction Left/Up/Right/Down instead of Previous/Next to make sure
                // that the KeyboardNavigation.DirectionalNavigation property works correctly.
                case Key.Left:
                    moveFocus(this, FocusNavigationDirection.Left);
                    e.Handled = true;
                    break;

                case Key.Up:
                    moveFocus(this, FocusNavigationDirection.Up);
                    e.Handled = true;
                    break;

                case Key.Right:
                    moveFocus(this, FocusNavigationDirection.Right);
                    e.Handled = true;
                    break;

                case Key.Down:
                    moveFocus(this, FocusNavigationDirection.Down);
                    e.Handled = true;
                    break;

                case Key.Space:
                case Key.Enter:
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                    e.Handled = true;
                    break;
            }

            // If it is simply treated as a button, pass the information about the click on.
            if (!e.Handled)
                base.OnKeyDown(e);

            static void moveFocus(FrameworkElement element, FocusNavigationDirection direction)
            {
                var request = new TraversalRequest(direction);
                element.MoveFocus(request);
            }
        }

        private static void OnTargetPageTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NavigationViewItem navigationItem) return;

            navigationItem.OnTargetPageTypeChanged();
        }

        private static void OnTargetViewTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not NavigationViewItem navigationItem) return;

            navigationItem.OnTargetViewTypeChanged();
        }

        private void OnTargetPageTypeChanged()
        {
            if (TargetPageType != null)
            {
                var viewModel = ViewModelManager.Get(TargetPageType);
                TargetPage = viewModel is INavigationPage navigationPage ? navigationPage : null;
            }
        }

        private void OnTargetViewTypeChanged()
        {
            if (TargetViewType != null)
            {
                var view = ViewManager.Get(TargetViewType);
                switch (view)
                {
                    case FrameworkElement frameworkElement when frameworkElement.DataContext is INavigationPage navigationPage:
                        TargetPage = navigationPage;
                        break;
                    case INavigationPage navigationView:
                        TargetPage = navigationView;
                        break;
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride() => new NavigationViewItem();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is not NavigationViewItem navigationViewItem) return;

            navigationViewItem.Activated -= OnItemActivated;
            navigationViewItem.Activated += OnItemActivated;

            navigationViewItem.Expanded -= OnItemExpanded;
            navigationViewItem.Expanded += OnItemExpanded;

            navigationViewItem.Click -= OnItemClicked;
            navigationViewItem.Click += OnItemClicked;

            if (NavigationView is not null)
                navigationViewItem.SetNavigationView(NavigationView);
        }

        private void OnItemClicked(object sender, RoutedEventArgs e) => NavigationView?.OnNavigationViewItemClick((sender as NavigationViewItem)!);
    }
}
