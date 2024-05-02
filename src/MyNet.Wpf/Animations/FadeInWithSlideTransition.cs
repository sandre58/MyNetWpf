// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyNet.Wpf.Animations
{
    public class FadeInWithSlideTransition : Transition
    {
        public override void Begin(FrameworkElement frameworkElement)
        {
            var translateDoubleAnimation = new DoubleAnimation
            {
                Duration = Duration,
                DecelerationRatio = DecelerationRatio,
                From = 30,
                To = 0,
            };

            if (frameworkElement.RenderTransform is not TranslateTransform)
                frameworkElement!.RenderTransform = new TranslateTransform(0, 0);

            if (!frameworkElement.RenderTransformOrigin.Equals(new Point(0.5, 0.5)))
                frameworkElement!.RenderTransformOrigin = new Point(0.5, 0.5);

            frameworkElement.RenderTransform.BeginAnimation(TranslateTransform.YProperty, translateDoubleAnimation);

            var opacityDoubleAnimation = new DoubleAnimation
            {
                Duration = Duration,
                DecelerationRatio = DecelerationRatio,
                From = 0.0,
                To = 1.0,
            };
            frameworkElement.BeginAnimation(UIElement.OpacityProperty, opacityDoubleAnimation);
        }
    }
}
