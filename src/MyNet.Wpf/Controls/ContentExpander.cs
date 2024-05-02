// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ToggleButtonName, Type = typeof(ToggleButton))]
    public class ContentExpander : Expander
    {
        private const string ToggleButtonName = "PART_ToggleButton";

        private ToggleButton? _toggleButton;
        private Storyboard? _openMenu;
        private Storyboard? _closeMenu;

        static ContentExpander() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentExpander), new FrameworkPropertyMetadata(typeof(ContentExpander)));

        protected override void OnCollapsed()
        {
            base.OnCollapsed();

            var size = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right ? ActualWidth : ActualHeight;
            if (size > CollapsedSize)
                _closeMenu?.Begin();
        }

        protected override void OnExpanded()
        {
            base.OnExpanded();

            var actualSize = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right ? ActualWidth : ActualHeight;
            if (actualSize > 0 && actualSize < ExpandedSize)
                _openMenu?.Begin();
        }

        #region DurationAnimation

        public static readonly DependencyProperty DurationAnimationProperty
    = DependencyProperty.RegisterAttached(nameof(DurationAnimation), typeof(KeyTime), typeof(ContentExpander), new PropertyMetadata(KeyTime.FromTimeSpan(new System.TimeSpan(0, 0, 0, 1)), OnStoryboardChanged));

        /// <summary>
        /// Gets the property IsOpen.
        /// </summary>
        public KeyTime DurationAnimation
        {
            get => (KeyTime)GetValue(DurationAnimationProperty);
            set => SetValue(DurationAnimationProperty, value);
        }

        #endregion

        #region CollapsedSize

        public static readonly DependencyProperty CollapsedSizeProperty
    = DependencyProperty.RegisterAttached(nameof(CollapsedSize), typeof(double), typeof(ContentExpander), new PropertyMetadata(60.0, OnStoryboardChanged));

        /// <summary>
        /// Gets the property IsOpen.
        /// </summary>
        public double CollapsedSize
        {
            get => (double)GetValue(CollapsedSizeProperty);
            set => SetValue(CollapsedSizeProperty, value);
        }

        #endregion

        #region ExpandedSize

        public static readonly DependencyProperty ExpandedSizeProperty
    = DependencyProperty.RegisterAttached(nameof(ExpandedSize), typeof(double), typeof(ContentExpander), new PropertyMetadata(250.0, OnStoryboardChanged));

        /// <summary>
        /// Gets the property IsOpen.
        /// </summary>
        public double ExpandedSize
        {
            get => (double)GetValue(ExpandedSizeProperty);
            set => SetValue(ExpandedSizeProperty, value);
        }

        #endregion

        #region ToggleButtontyle

        public static readonly DependencyProperty ToggleButtontyleProperty
    = DependencyProperty.RegisterAttached(nameof(ToggleButtontyle), typeof(Style), typeof(ContentExpander), new PropertyMetadata());

        public Style ToggleButtontyle
        {
            get => (Style)GetValue(ToggleButtontyleProperty);
            set => SetValue(ToggleButtontyleProperty, value);
        }

        #endregion

        #region Methods

        private static void OnStoryboardChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is not ContentExpander menu)
            {
                return;
            }

            menu.UpdateStoryboards();
        }

        private void UpdateStoryboards()
        {
            _openMenu = GetAnimationStoryboard(
                CollapsedSize,
                ExpandedSize,
                KeyTime.FromTimeSpan(new System.TimeSpan(0)),
                DurationAnimation
                );

            _closeMenu = GetAnimationStoryboard(
                ExpandedSize,
                CollapsedSize,
                KeyTime.FromTimeSpan(new System.TimeSpan(0)),
                DurationAnimation
                );
        }

        private Storyboard GetAnimationStoryboard(double startWidth, double endWidth, KeyTime startTime, KeyTime endTime)
        {
            var storyboard = new Storyboard();

            var startEasingDoubleKeyFrame = new EasingDoubleKeyFrame(startWidth, startTime);
            var endEasingDoubleKeyFrame = new EasingDoubleKeyFrame(endWidth, endTime);
            var property = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right ? WidthProperty : HeightProperty;

            var doubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(doubleAnimationUsingKeyFrames, this);
            Storyboard.SetTargetProperty(doubleAnimationUsingKeyFrames, new PropertyPath(property));
            _ = doubleAnimationUsingKeyFrames.KeyFrames.Add(startEasingDoubleKeyFrame);
            _ = doubleAnimationUsingKeyFrames.KeyFrames.Add(endEasingDoubleKeyFrame);

            storyboard.Children.Add(doubleAnimationUsingKeyFrames);

            storyboard.Completed += (sender, e) => SetCurrentValue(property, endWidth);

            return storyboard;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _toggleButton = GetTemplateChild(ToggleButtonName) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Click -= OnToggleButtonClick;
                _toggleButton.Click += OnToggleButtonClick;
            }

            UpdateStoryboards();

            var property = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right ? WidthProperty : HeightProperty;
            var size = IsExpanded ? ExpandedSize : CollapsedSize;
            SetCurrentValue(property, size);
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e) => IsExpanded = _toggleButton != null && _toggleButton.IsChecked.HasValue && _toggleButton.IsChecked.Value;

        #endregion
    }
}
