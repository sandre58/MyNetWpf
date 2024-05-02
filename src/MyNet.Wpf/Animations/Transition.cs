// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Animations
{
    /// <summary>
    /// Provides page navigation animations.
    /// </summary>
    public abstract class Transition : DependencyObject
    {
        protected static readonly double DecelerationRatio = 0.7;

        #region Duration

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(
                "Duration",
                typeof(Duration),
                typeof(Transition),
                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        public Duration Duration
        {
            get => (Duration)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        #endregion Duration

        public abstract void Begin(FrameworkElement frameworkElement);
    }
}
