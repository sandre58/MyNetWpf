// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using MyNet.Wpf.Animations;

namespace MyNet.Wpf.Controls
{
    /// <summary>
    /// Displays Page instances, supports navigation to new pages, and maintains a navigation
    /// history to support forward and backward navigation.
    /// </summary>
    public class NavigationFrame : Frame
    {
        static NavigationFrame()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(typeof(NavigationFrame)));
            NavigationUIVisibilityProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(NavigationUIVisibility.Hidden));
            IsTabStopProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(false));
            FocusableProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(false));
            FocusVisualStyleProperty.OverrideMetadata(typeof(NavigationFrame), new FrameworkPropertyMetadata(null));
        }

        /// <summary>
        /// Initialzies a new instance of the Frame class.
        /// </summary>
        public NavigationFrame() : base()
        {
            InheritanceBehavior = InheritanceBehavior.Default;
            JournalOwnership = JournalOwnership.OwnsJournal;

            Navigated += OnNavigated;
        }

        #region Transition

        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register(
                "Transition",
                typeof(Transition),
                typeof(NavigationFrame),
                new PropertyMetadata(null));

        public Transition Transition
        {
            get => (Transition)GetValue(TransitionProperty);
            set => SetValue(TransitionProperty, value);
        }

        #endregion NavigationTransitionInfo

        #region TransitionType

        public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register(
            nameof(TransitionType),
            typeof(TransitionType), typeof(NavigationFrame),
            new PropertyMetadata(TransitionType.FadeIn));

        public TransitionType TransitionType
        {
            get => (TransitionType)GetValue(TransitionTypeProperty);
            set => SetValue(TransitionTypeProperty, value);
        }

        #endregion

        #region TransitionDuration

        public static readonly DependencyProperty TransitionDurationProperty =
            DependencyProperty.Register(
                "TransitionDuration",
                typeof(int),
                typeof(NavigationFrame),
                new PropertyMetadata(500));

        public int TransitionDuration
        {
            get => (int)GetValue(TransitionDurationProperty);
            set => SetValue(TransitionDurationProperty, value);
        }

        #endregion Duration

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton is MouseButton.XButton1 or MouseButton.XButton2)
            {
                e.Handled = true;
            }

            base.OnMouseDown(e);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var transition = Transition;

            transition ??= TransitionType switch
            {
                TransitionType.FadeIn => new FadeInTransition() { Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                TransitionType.FadeInWithSlide => new FadeInWithSlideTransition() { Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                TransitionType.SlideBottom => new SlideTransition() { Direction = TransitionSlideDirection.Bottom, Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                TransitionType.SlideLeft => new SlideTransition() { Direction = TransitionSlideDirection.Left, Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                TransitionType.SlideRight => new SlideTransition() { Direction = TransitionSlideDirection.Right, Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                TransitionType.SlideTop => new SlideTransition() { Direction = TransitionSlideDirection.Top, Duration = TimeSpan.FromMilliseconds(TransitionDuration) },
                _ => null
            };

            if (transition is not null && e.Content is FrameworkElement frameworkElement)
            {
                transition.Begin(frameworkElement);
            }
        }
    }
}
