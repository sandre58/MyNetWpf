﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace MyNet.Wpf.Parameters
{
    [Flags]
    public enum ShadowEdges
    {
        None = 0,
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = Left | Top | Right | Bottom
    }

    public static class ShadowAssist
    {
        #region AttachedProperty : LocalInfoPropertyKey

        private sealed class ShadowLocalInfo(double standardOpacity)
        {
            public double StandardOpacity { get; } = standardOpacity;
        }

        private static readonly DependencyPropertyKey LocalInfoPropertyKey = DependencyProperty.RegisterAttachedReadOnly(
            "LocalInfo", typeof(ShadowLocalInfo), typeof(ShadowAssist), new PropertyMetadata(default(ShadowLocalInfo)));

        private static void SetLocalInfo(DependencyObject element, ShadowLocalInfo? value)
            => element.SetValue(LocalInfoPropertyKey, value);

        private static ShadowLocalInfo? GetLocalInfo(DependencyObject element)
            => (ShadowLocalInfo?)element.GetValue(LocalInfoPropertyKey.DependencyProperty);

        #endregion

        #region AttachedProperty : DarkenProperty
        public static readonly DependencyProperty DarkenProperty = DependencyProperty.RegisterAttached(
            "Darken", typeof(bool), typeof(ShadowAssist), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender, DarkenPropertyChangedCallback));

        private static void DarkenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var uiElement = dependencyObject as UIElement;

            if (uiElement?.Effect is not DropShadowEffect dropShadowEffect) return;

            if ((bool)dependencyPropertyChangedEventArgs.NewValue)
            {
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, null);

                SetLocalInfo(dependencyObject, new ShadowLocalInfo(dropShadowEffect.Opacity));

                var time = GetShadowAnimationDuration(dependencyObject);

                var doubleAnimation = new DoubleAnimation()
                {
                    To = 1,
                    Duration = new Duration(time),
                    FillBehavior = FillBehavior.HoldEnd,
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.4,
                    DecelerationRatio = 0.2
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
            }
            else
            {
                var shadowLocalInfo = GetLocalInfo(dependencyObject);
                if (shadowLocalInfo is null) return;

                var time = GetShadowAnimationDuration(dependencyObject);

                var doubleAnimation = new DoubleAnimation()
                {
                    To = shadowLocalInfo.StandardOpacity,
                    Duration = new Duration(time),
                    FillBehavior = FillBehavior.HoldEnd,
                    EasingFunction = new CubicEase(),
                    AccelerationRatio = 0.4,
                    DecelerationRatio = 0.2
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, doubleAnimation);
            }
        }

        public static void SetDarken(DependencyObject element, bool value) => element.SetValue(DarkenProperty, value);

        public static bool GetDarken(DependencyObject element) => (bool)element.GetValue(DarkenProperty);
        #endregion

        #region AttachedProperty : CacheModeProperty
        public static readonly DependencyProperty CacheModeProperty = DependencyProperty.RegisterAttached(
            "CacheMode", typeof(CacheMode), typeof(ShadowAssist), new FrameworkPropertyMetadata(new BitmapCache { EnableClearType = true, SnapsToDevicePixels = true }, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetCacheMode(DependencyObject element, CacheMode value) => element.SetValue(CacheModeProperty, value);

        public static CacheMode GetCacheMode(DependencyObject element) => (CacheMode)element.GetValue(CacheModeProperty);
        #endregion

        #region AttachedProperty : ShadowEdgesProperty
        public static readonly DependencyProperty ShadowEdgesProperty = DependencyProperty.RegisterAttached(
            "ShadowEdges", typeof(ShadowEdges), typeof(ShadowAssist), new PropertyMetadata(ShadowEdges.All));

        public static void SetShadowEdges(DependencyObject element, ShadowEdges value) => element.SetValue(ShadowEdgesProperty, value);

        public static ShadowEdges GetShadowEdges(DependencyObject element) => (ShadowEdges)element.GetValue(ShadowEdgesProperty);
        #endregion

        #region AttachedProperty : ShadowAnimationDurationProperty
        public static readonly DependencyProperty ShadowAnimationDurationProperty =
           DependencyProperty.RegisterAttached(
               name: "ShadowAnimationDuration",
               propertyType: typeof(TimeSpan),
               ownerType: typeof(ShadowAssist),
               defaultMetadata: new FrameworkPropertyMetadata(
                   defaultValue: new TimeSpan(0, 0, 0, 0, 180),
                   flags: FrameworkPropertyMetadataOptions.Inherits)
               );

        public static TimeSpan GetShadowAnimationDuration(DependencyObject element) => (TimeSpan)element.GetValue(ShadowAnimationDurationProperty);
        public static void SetShadowAnimationDuration(DependencyObject element, TimeSpan value) => element.SetValue(ShadowAnimationDurationProperty, value);
        #endregion
    }
}
