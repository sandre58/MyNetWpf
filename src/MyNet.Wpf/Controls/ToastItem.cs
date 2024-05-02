// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyNet.UI.Toasting.Settings;
using MyNet.Wpf.Controls.Toasts;
using MyNet.Wpf.Helpers;
using MyNet.Wpf.Toasting;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
    public class ToastItem : UserControl
    {
        private const string CloseButtonName = "CloseButton";

        protected INotificationAnimator _animator;
        protected Button? _closeButton;

        public Toast? Toast => DataContext as Toast;

        static ToastItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ToastItem), new FrameworkPropertyMetadata(typeof(ToastItem)));

        public ToastItem()
        {
            _animator = new ToastAnimator(this, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(300));

            _animator.Setup();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (Toast is null) return;

            var options = Toast.Settings;
            if (options != null && options.FreezeOnMouseEnter)
            {
                if (!options.UnfreezeOnMouseEnter) // message stay freezed, show close button
                {
                    if (Toast.CanClose)
                    {
                        Toast.CanClose = false;
                        if (_closeButton != null)
                        {
                            _closeButton.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    Toast.CanClose = false;
                }
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (Toast is null) return;

            var opts = Toast.Settings;
            if (opts != null && opts.FreezeOnMouseEnter && opts.UnfreezeOnMouseEnter)
            {
                Toast.CanClose = true;

                if (_closeButton is not null)
                    _closeButton.Visibility = Toast?.Settings.ClosingStrategy is ToastClosingStrategy.Both or ToastClosingStrategy.CloseButton ? Visibility.Visible : Visibility.Collapsed;
            }
            base.OnMouseLeave(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) => _animator.PlayShowAnimation();

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_closeButton is not null)
            {
                _closeButton.Click -= OnCloseButtonClick;
            }
            if (Toast is not null)
            {
                Toast.CloseRequest -= Toast_CloseRequest;
            }
            Loaded -= OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _closeButton = (Button)GetTemplateChild(CloseButtonName);
            if (_closeButton is not null)
            {
                _closeButton.Click -= OnCloseButtonClick;
                _closeButton.Click += OnCloseButtonClick;

                _closeButton.Visibility = Toast?.Settings.ClosingStrategy is ToastClosingStrategy.Both or ToastClosingStrategy.CloseButton ? Visibility.Visible : Visibility.Collapsed;
            }

            if (Toast is not null)
            {
                Toast.CloseRequest += Toast_CloseRequest;

                foreach (var (dpKey, valueKey) in Toast.Settings.Style)
                {
                    SetValue(GetDependencyPropertyByName(this, $"{dpKey}Property"), WpfHelper.GetResource(valueKey));
                }
            }
        }

        private void Toast_CloseRequest(object? sender, EventArgs e) => _animator.PlayHideAnimation();

        private void OnCloseButtonClick(object sender, RoutedEventArgs e) => Toast?.Close();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Toast?.OnClick?.Invoke(Toast.Notification);
        }

        public static DependencyProperty? GetDependencyPropertyByName(DependencyObject dependencyObject, string dpName) => GetDependencyPropertyByName(dependencyObject.GetType(), dpName);

        public static DependencyProperty? GetDependencyPropertyByName(Type dependencyObjectType, string dpName)
        {
            DependencyProperty? dp = null;

            var fieldInfo = dependencyObjectType.GetField(dpName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null)
            {
                dp = fieldInfo.GetValue(null) as DependencyProperty;
            }

            return dp;
        }
    }
}
