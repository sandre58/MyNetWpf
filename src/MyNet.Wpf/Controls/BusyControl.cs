// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MyNet.Wpf.Controls
{
    public class BusyControl : ContentControl
    {
        private const string DefaultAnimationDuration = "0:0:0.15";

        private readonly Storyboard? _showBusyAnimation;
        private readonly Storyboard? _hideBusyAnimation;
        private IInputElement? _restoreFocus;
        private bool _restoreIsEnabledAssociatedControl = false;

        static BusyControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyControl), new FrameworkPropertyMetadata(typeof(BusyControl)));

        public BusyControl()
        {
            _showBusyAnimation = CreateAnimationStoryboard(0, 1);
            _hideBusyAnimation = CreateAnimationStoryboard(1, 0);

            void handler(object? o, EventArgs e) => RaiseEvent(new RoutedEventArgs(BusyHiddenEvent));

            _hideBusyAnimation.Completed += handler;
        }

        #region AssociatedControl

        public UIElement AssociatedControl
        {
            get => (UIElement)GetValue(AssociatedControlProperty);
            set => SetValue(AssociatedControlProperty, value);
        }

        public static readonly DependencyProperty AssociatedControlProperty =
            DependencyProperty.Register(nameof(AssociatedControl), typeof(UIElement), typeof(BusyControl), new PropertyMetadata(null));

        #endregion AssociatedControl

        #region IsActive

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(BusyControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IActivePropertyChangedCallback));

        private static void IActivePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (BusyControl)dependencyObject;

            if (control.IsActive)
            {
                var window = Window.GetWindow(control);
                control._restoreFocus = window != null ? FocusManager.GetFocusedElement(window) : null;
                control._restoreIsEnabledAssociatedControl = control.AssociatedControl?.IsEnabled ?? true;

                if (control.AssociatedControl is not null)
                    control.AssociatedControl.IsEnabled = false;
                control._showBusyAnimation?.Begin();
            }
            else
            {
                control._hideBusyAnimation?.Begin();

                if (control.AssociatedControl is not null)
                    control.AssociatedControl.IsEnabled = control._restoreIsEnabledAssociatedControl;
                control.Dispatcher.InvokeAsync(() => control._restoreFocus?.Focus(), DispatcherPriority.Input);
            }

            control.RaiseEvent(new RoutedPropertyChangedEventArgs<bool>((bool)dependencyPropertyChangedEventArgs.OldValue, (bool)dependencyPropertyChangedEventArgs.OldValue, IsActiveChangedEvent));
        }

        #endregion IsActive

        #region Events

        /// <summary>Identifies the <see cref="IsActiveChanged"/> routed event.</summary>
        public static readonly RoutedEvent IsActiveChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(IsActiveChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<bool>),
                                               typeof(BusyControl));

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsActiveChanged
        {
            add => AddHandler(IsActiveChangedEvent, value);
            remove => RemoveHandler(IsActiveChangedEvent, value);
        }

        public static readonly RoutedEvent BusyHiddenEvent =
            EventManager.RegisterRoutedEvent(
                "BusyHidden",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(BusyControl));

        /// <summary>
        /// Raised when a dialog is closed.
        /// </summary>
        public event RoutedEventHandler BusyHidden
        {
            add => AddHandler(BusyHiddenEvent, value);
            remove => RemoveHandler(BusyHiddenEvent, value);
        }

        #endregion

        private Storyboard CreateAnimationStoryboard(int from, int to)
        {
            var storyboard = new Storyboard();

            var doubleAnimation = new DoubleAnimation(from, to, new Duration(TimeSpan.Parse(DefaultAnimationDuration, CultureInfo.InvariantCulture)));
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(doubleAnimation, this);

            storyboard.Children.Add(doubleAnimation);

            return storyboard;
        }
    }
}
