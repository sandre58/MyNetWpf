// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MyNet.UI;
using MyNet.Wpf.Controls.Dialogs;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ContentPartName, Type = typeof(ContentControl))]
    [TemplatePart(Name = ContentCoverGridName, Type = typeof(Grid))]
    [TemplateVisualState(GroupName = "PopupStates", Name = OpenStateName)]
    [TemplateVisualState(GroupName = "PopupStates", Name = ClosedStateName)]
    public class OverlayDialogControl : HeaderedContentControl
    {
        public const string ContentPartName = "PART_ContentElement";
        public const string ContentCoverGridName = "PART_ContentCoverGrid";
        public const string OpenStateName = "Open";
        public const string ClosedStateName = "Closed";

        private bool? _dialogResult = null;
        private ContentControl? _contentControl;
        private Grid? _contentCoverGrid;
        private IInputElement? _restoreFocus;
        private bool _restoreIsEnabledAssociatedControl = false;
        private TaskCompletionSource<bool?>? _dialogTaskCompletionSource;

        /// <summary>
        /// Routed command to be used inside dialog content to close a dialog. Use a <see cref="Button.CommandParameter"/> to indicate the result of the parameter.
        /// </summary>
        public static readonly RoutedCommand CloseDialogCommand = new();

        static OverlayDialogControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayDialogControl), new FrameworkPropertyMetadata(typeof(OverlayDialogControl)));

        public OverlayDialogControl()
        {
            CommandBindings.Add(new CommandBinding(CloseDialogCommand, async (sender, e) => await CloseDialogHandlerAsync(e).ConfigureAwait(false)));
            InputBindings.Add(new KeyBinding(CloseDialogCommand, Key.Escape, ModifierKeys.None));
        }

        public void Show()
        {
            if (IsOpen)
                throw new InvalidOperationException("Dialog is already open.");

            SetCurrentValue(IsOpenProperty, true);
        }

        public async Task<bool?> ShowDialogAsync()
        {
            if (IsOpen)
                throw new InvalidOperationException("Dialog is already open.");

            _dialogTaskCompletionSource = new TaskCompletionSource<bool?>();

            SetCurrentValue(IsOpenProperty, true);

            var result = await _dialogTaskCompletionSource.Task;

            return result;
        }

        public async Task CloseAsync(bool? dialogResult = null) => await InternalCloseAsync(dialogResult).ConfigureAwait(false);

        #region AssociatedControl

        public UIElement? AssociatedControl
        {
            get => (UIElement?)GetValue(AssociatedControlProperty);
            set => SetValue(AssociatedControlProperty, value);
        }

        public static readonly DependencyProperty AssociatedControlProperty =
            DependencyProperty.Register(nameof(AssociatedControl), typeof(UIElement), typeof(OverlayDialogControl), new PropertyMetadata(null));

        #endregion AssociatedControl

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(OverlayDialogControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenPropertyChangedCallback));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void IsOpenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var overlayDialogControl = (OverlayDialogControl)dependencyObject;

            if (overlayDialogControl._contentControl != null)
                ValidationAssist.SetSuppress(overlayDialogControl._contentControl, !overlayDialogControl.IsOpen);
            VisualStateManager.GoToState(overlayDialogControl, overlayDialogControl.GetStateName(), !TransitionAssist.GetDisableTransitions(overlayDialogControl));

            if (overlayDialogControl.IsOpen)
            {
                overlayDialogControl._restoreIsEnabledAssociatedControl = overlayDialogControl.AssociatedControl?.IsEnabled ?? true;

                if (overlayDialogControl.AssociatedControl is not null)
                    overlayDialogControl.AssociatedControl.IsEnabled = false;
            }
            else
            {
                overlayDialogControl._dialogTaskCompletionSource?.TrySetResult(overlayDialogControl._dialogResult);

                var dialogClosedEventArgs = new RoutedEventArgs(ClosedEvent);
                overlayDialogControl.OnClosed(dialogClosedEventArgs);

                if (overlayDialogControl.AssociatedControl is not null)
                    overlayDialogControl.AssociatedControl.IsEnabled = overlayDialogControl._restoreIsEnabledAssociatedControl;

                // Don't attempt to Invoke if _restoreFocusDialogClose hasn't been assigned yet. Can occur
                // if the MainWindow has started up minimized. Even when Show() has been called, this doesn't
                // seem to have been set.
                overlayDialogControl.Dispatcher.InvokeAsync(() => overlayDialogControl._restoreFocus?.Focus(), DispatcherPriority.Input);

                return;
            }

            var window = Window.GetWindow(overlayDialogControl);
            overlayDialogControl._restoreFocus = window != null ? FocusManager.GetFocusedElement(window) : null;

            //multiple ways of calling back that the dialog has opened:
            // * routed event
            // * the attached property (which should be applied to the button which opened the dialog
            // * straight forward dependency property 
            // * handler provided to the async show method
            var dialogOpenedEventArgs = new RoutedEventArgs(OpenedEvent);
            overlayDialogControl.OnDialogOpened(dialogOpenedEventArgs);

            overlayDialogControl.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CommandManager.InvalidateRequerySuggested();

                if (overlayDialogControl.FocusOnShow)
                {
                    var child = overlayDialogControl.FocusContent();

                    if (child != null)
                    {
                        //https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/issues/187
                        //totally not happy about this, but on immediate validation we can get some weird looking stuff...give WPF a kick to refresh...
                        Task.Delay(300).ContinueWith(t => child.Dispatcher.BeginInvoke(new Action(() => child.InvalidateVisual())));
                    }
                }
            }));
        }

        #endregion

        #region CloseOnClickAway

        public static readonly DependencyProperty CloseOnClickAwayProperty = DependencyProperty.Register(
            "CloseOnClickAway", typeof(bool), typeof(OverlayDialogControl), new PropertyMetadata(false));

        /// <summary>
        /// Indicates whether the dialog will close if the user clicks off the dialog, on the obscured background.
        /// </summary>
        public bool CloseOnClickAway
        {
            get => (bool)GetValue(CloseOnClickAwayProperty);
            set => SetValue(CloseOnClickAwayProperty, value);
        }

        #endregion

        #region FocusOnShow

        public static readonly DependencyProperty FocusOnShowProperty = DependencyProperty.Register(
            "FocusOnShow", typeof(bool), typeof(OverlayDialogControl), new PropertyMetadata(true));

        /// <summary>
        /// Indicates whether the dialog will close if the user clicks off the dialog, on the obscured background.
        /// </summary>
        public bool FocusOnShow
        {
            get => (bool)GetValue(FocusOnShowProperty);
            set => SetValue(FocusOnShowProperty, value);
        }

        #endregion

        #region OverlayBackground

        public static readonly DependencyProperty OverlayBackgroundProperty = DependencyProperty.Register(
            nameof(OverlayBackground), typeof(Brush), typeof(OverlayDialogControl), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Represents the overlay brush that is used to dim the background behind the dialog
        /// </summary>
        public Brush? OverlayBackground
        {
            get => (Brush?)GetValue(OverlayBackgroundProperty);
            set => SetValue(OverlayBackgroundProperty, value);
        }

        #endregion

        #region open dialog events/callbacks

        public static readonly RoutedEvent OpenedEvent =
            EventManager.RegisterRoutedEvent(
                "Opened",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(OverlayDialogControl));

        /// <summary>
        /// Raised when a dialog is opened.
        /// </summary>
        public event RoutedEventHandler Opened
        {
            add => AddHandler(OpenedEvent, value);
            remove => RemoveHandler(OpenedEvent, value);
        }

        protected void OnDialogOpened(RoutedEventArgs eventArgs)
            => RaiseEvent(eventArgs);

        #endregion

        #region close dialog events/callbacks

        public static readonly RoutedEvent ClosingEvent =
            EventManager.RegisterRoutedEvent(
                "Closing",
                RoutingStrategy.Direct,
                typeof(DialogClosingEventHandler),
                typeof(OverlayDialogControl));

        /// <summary>
        /// Raised just before a dialog is closed.
        /// </summary>
        public event DialogClosingEventHandler Closing
        {
            add => AddHandler(ClosingEvent, value);
            remove => RemoveHandler(ClosingEvent, value);
        }

        protected void OnClosing(DialogClosingEventArgs eventArgs) => RaiseEvent(eventArgs);

        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent(
                "DialogClosed",
                RoutingStrategy.Direct,
                typeof(RoutedEventHandler),
                typeof(OverlayDialogControl));

        /// <summary>
        /// Raised when a dialog is closed.
        /// </summary>
        public event RoutedEventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        protected void OnClosed(RoutedEventArgs eventArgs)
            => RaiseEvent(eventArgs);

        #endregion

        public override void OnApplyTemplate()
        {
            if (_contentCoverGrid != null)
                _contentCoverGrid.MouseLeftButtonUp -= ContentCoverGridOnMouseLeftButtonUpAsync;

            _contentControl = GetTemplateChild(ContentPartName) as ContentControl;
            _contentCoverGrid = GetTemplateChild(ContentCoverGridName) as Grid;

            if (_contentCoverGrid != null)
                _contentCoverGrid.MouseLeftButtonUp += ContentCoverGridOnMouseLeftButtonUpAsync;

            VisualStateManager.GoToState(this, GetStateName(), true);

            base.OnApplyTemplate();
        }

        internal async Task InternalCloseAsync(bool? dialogResult = false)
        {
            var canClose = true;

            if (DataContext is IClosable closable)
                canClose = await closable.CanCloseAsync().ConfigureAwait(false);

            var dialogClosingEventArgs = new DialogClosingEventArgs(ClosingEvent, dialogResult);
            if (!canClose)
                dialogClosingEventArgs.Cancel();

            await Dispatcher.InvokeAsync(() =>
            {
                //multiple ways of calling back that the dialog is closing:
                // * routed event
                // * the attached property (which should be applied to the button which opened the dialog
                // * straight forward IsOpen dependency property 
                // * handler provided to the async show method
                OnClosing(dialogClosingEventArgs);

                if (!dialogClosingEventArgs.IsCancelled)
                {
                    _dialogResult = dialogClosingEventArgs.DialogResult;
                    SetCurrentValue(IsOpenProperty, false);
                }
            });
        }

        private string GetStateName() => IsOpen ? OpenStateName : ClosedStateName;

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is { } window && !window.IsActive)
            {
                window.Activate();
            }
            base.OnPreviewMouseDown(e);
        }

        private async void ContentCoverGridOnMouseLeftButtonUpAsync(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (CloseOnClickAway)
                await InternalCloseAsync().ConfigureAwait(false);
        }

        internal UIElement? FocusContent()
        {
            var child = _contentControl;
            if (child is null) return null;

            CommandManager.InvalidateRequerySuggested();
            var focusable = child.VisualDepthFirstTraversal().OfType<FrameworkElement>().FirstOrDefault(FocusAssist.GetIsDefault)
                            ?? child.VisualDepthFirstTraversal().OfType<UIElement>().FirstOrDefault(ui => ui.Focusable && ui.IsVisible);
            focusable?.Dispatcher.InvokeAsync(() =>
            {
                if (!focusable.Focus()) return;
                focusable.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }, DispatcherPriority.Background);

            return child;
        }

        private async Task CloseDialogHandlerAsync(ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Handled) return;

            await InternalCloseAsync().ConfigureAwait(false);

            executedRoutedEventArgs.Handled = true;
        }
    }
}
