// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyNet.Wpf.Animations
{
    public class SlideTransition : Transition
    {
        #region Direction

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                "Direction",
                typeof(TransitionSlideDirection),
                typeof(SlideTransition),
                new PropertyMetadata(null));

        public TransitionSlideDirection Direction
        {
            get => (TransitionSlideDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        #endregion NavigationTransitionInfo
        public override void Begin(FrameworkElement frameworkElement)
        {
            var translateDoubleAnimation = new DoubleAnimation
            {
                Duration = Duration,
                DecelerationRatio = DecelerationRatio,
                From = Direction == TransitionSlideDirection.Left || Direction == TransitionSlideDirection.Bottom ? -50
                : 30,
                To = 0,
            };

            if (frameworkElement.RenderTransform is not TranslateTransform)
                frameworkElement!.RenderTransform = new TranslateTransform(0, 0);

            if (!frameworkElement.RenderTransformOrigin.Equals(new Point(0.5, 0.5)))
                frameworkElement!.RenderTransformOrigin = new Point(0.5, 0.5);

            frameworkElement.RenderTransform.BeginAnimation(Direction == TransitionSlideDirection.Top || Direction == TransitionSlideDirection.Bottom ? TranslateTransform.YProperty : TranslateTransform.XProperty, translateDoubleAnimation);
        }
    }
}
