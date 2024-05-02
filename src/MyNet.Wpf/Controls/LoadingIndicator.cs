// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Controls.VisualStates;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Controls
{
    /// <inheritdoc />
    /// <summary>
    ///     A control featuring a range of loading indicating animations.
    /// </summary>
    [TemplatePart(Name = TemplateRootName, Type = typeof(Border))]
    public class LoadingIndicator : Control
    {
        internal const string TemplateRootName = "TemplateRoot";

        /// <summary>
        ///     Identifies the <see cref="LoadingIndicators.WPF.LoadingIndicator.SpeedRatio" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(LoadingIndicator), new PropertyMetadata(0.8D,
                OnSpeedRatioChanged));

        /// <summary>
        ///     Identifies the <see cref="LoadingIndicators.WPF.LoadingIndicator.IsActive" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(LoadingIndicator), new PropertyMetadata(true,
                OnIsActiveChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(LoadingIndicatorMode), typeof(LoadingIndicator),
            new PropertyMetadata(LoadingIndicatorMode.Arcs));

        private FrameworkElement? _templateRoot;

        static LoadingIndicator() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingIndicator),
                new FrameworkPropertyMetadata(typeof(LoadingIndicator)));

        public LoadingIndicatorMode Mode
        {
            get => (LoadingIndicatorMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        ///     Get/set the speed ratio of the animation.
        /// </summary>
        public double SpeedRatio
        {
            get => (double)GetValue(SpeedRatioProperty);
            set => SetValue(SpeedRatioProperty, value);
        }

        /// <summary>
        ///     Get/set whether the loading indicator is active.
        /// </summary>
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        private static void OnSpeedRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var li = (LoadingIndicator)o;

            if (li._templateRoot == null || !li.IsActive)
            {
                return;
            }

            SetStoryBoardSpeedRatio(li._templateRoot, (double)e.NewValue);
        }

        private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var li = (LoadingIndicator)o;

            if (li._templateRoot == null)
            {
                return;
            }

            if (!(bool)e.NewValue)
            {
                _ = VisualStateManager.GoToElementState(li._templateRoot, IndicatorVisualStateNames.InactiveState.Name,
                    false);
                li._templateRoot.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
            else
            {
                _ = VisualStateManager.GoToElementState(li._templateRoot, IndicatorVisualStateNames.ActiveState.Name, false);

                li._templateRoot.SetCurrentValue(VisibilityProperty, Visibility.Visible);

                SetStoryBoardSpeedRatio(li._templateRoot, li.SpeedRatio);
            }
        }

        private static void SetStoryBoardSpeedRatio(FrameworkElement element, double speedRatio)
        {
            var activeStates = element.GetActiveVisualStates();
            foreach (var activeState in activeStates)
            {
                activeState.Storyboard.SetSpeedRatio(element, speedRatio);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code
        ///     or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _templateRoot = (FrameworkElement)GetTemplateChild(TemplateRootName);

            if (_templateRoot == null)
            {
                return;
            }

            _ = VisualStateManager.GoToElementState(_templateRoot,
                IsActive
                    ? IndicatorVisualStateNames.ActiveState.Name
                    : IndicatorVisualStateNames.InactiveState.Name, false);

            SetStoryBoardSpeedRatio(_templateRoot, SpeedRatio);

            _templateRoot.SetCurrentValue(VisibilityProperty, IsActive ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}
