// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MyNet.Wpf.Controls.Toasts
{
    public class ToastAnimator(ToastItem toast, TimeSpan showAnimationTime, TimeSpan hideAnimationTime) : INotificationAnimator
    {
        private readonly ToastItem _toast = toast;
        private readonly TimeSpan _showAnimationTime = showAnimationTime;
        private readonly TimeSpan _hideAnimationTime = hideAnimationTime;

        public void Setup()
        {
            var scale = new ScaleTransform(1, 0);
            _toast.RenderTransform = scale;
        }

        public void PlayShowAnimation()
        {
            var scale = (ScaleTransform)_toast.RenderTransform;
            scale.CenterY = _toast.ActualHeight / 2;
            scale.CenterX = _toast.ActualWidth / 2;

            var storyboard = new Storyboard();

            SetGrowYAnimation(storyboard);
            SetGrowXAnimation(storyboard);
            SetFadeInAnimation(storyboard);

            storyboard.Begin();
        }

        private void SetFadeInAnimation(Storyboard storyboard)
        {
            var fadeInAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(fadeInAnimation);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(path: "Opacity"));
            Storyboard.SetTarget(fadeInAnimation, _toast);
        }

        private void SetGrowXAnimation(Storyboard storyboard)
        {
            var growXAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growXAnimation);
            Storyboard.SetTargetProperty(growXAnimation, new PropertyPath(path: "RenderTransform.ScaleX"));
            Storyboard.SetTarget(growXAnimation, _toast);
        }

        private void SetGrowYAnimation(Storyboard storyboard)
        {
            var growYAnimation = new DoubleAnimation
            {
                Duration = _showAnimationTime,
                From = 0,
                To = 1
            };
            storyboard.Children.Add(growYAnimation);
            Storyboard.SetTargetProperty(growYAnimation, new PropertyPath(path: "RenderTransform.ScaleY"));
            Storyboard.SetTarget(growYAnimation, _toast);
        }

        public void PlayHideAnimation()
        {
            _toast.MinHeight = 0;
            var scale = (ScaleTransform)_toast.RenderTransform;
            scale.CenterY = _toast.ActualHeight / 2;
            scale.CenterX = _toast.ActualWidth / 2;

            var storyboard = new Storyboard();

            SetShrinkYAnimation(storyboard);
            SetShrinkXAnimation(storyboard);
            SetFadeoutAnimation(storyboard);

            storyboard.Begin();
        }

        private void SetFadeoutAnimation(Storyboard storyboard)
        {
            var fadeOutAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(path: "Opacity"));
            Storyboard.SetTarget(fadeOutAnimation, _toast);
        }

        private void SetShrinkXAnimation(Storyboard storyboard)
        {
            var shrinkXAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = 1,
                To = 0
            };

            storyboard.Children.Add(shrinkXAnimation);

            Storyboard.SetTargetProperty(shrinkXAnimation, new PropertyPath(path: "RenderTransform.ScaleX"));
            Storyboard.SetTarget(shrinkXAnimation, _toast);
        }

        private void SetShrinkYAnimation(Storyboard storyboard)
        {
            var shrinkYAnimation = new DoubleAnimation
            {
                Duration = _hideAnimationTime,
                From = _toast.ActualHeight,
                To = 0
            };

            storyboard.Children.Add(shrinkYAnimation);

            Storyboard.SetTargetProperty(shrinkYAnimation, new PropertyPath(path: "Height"));
            Storyboard.SetTarget(shrinkYAnimation, _toast);
        }
    }
}
