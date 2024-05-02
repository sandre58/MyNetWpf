// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media.Animation;

namespace MyNet.Wpf.Animations
{
    public class FadeInTransition : Transition
    {
        public override void Begin(FrameworkElement frameworkElement)
        {
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
