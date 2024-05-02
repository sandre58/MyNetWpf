// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyNet.Wpf.Controls
{
    public class ApplicationIcon : Control
    {
        static ApplicationIcon() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationIcon), new FrameworkPropertyMetadata(typeof(ApplicationIcon)));

        #region UniformCornerRadius


        public static readonly DependencyProperty UniformCornerRadiusProperty
            = DependencyProperty.RegisterAttached(nameof(UniformCornerRadius), typeof(double), typeof(ApplicationIcon), new FrameworkPropertyMetadata(20.0D, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public double UniformCornerRadius
        {
            get => (double)GetValue(UniformCornerRadiusProperty);
            set => SetValue(UniformCornerRadiusProperty, value);
        }

        #endregion

        #region Data


        public static readonly DependencyProperty DataProperty
            = DependencyProperty.RegisterAttached(nameof(Data), typeof(Geometry), typeof(ApplicationIcon), new PropertyMetadata(null));

        public Geometry Data
        {
            get => (Geometry)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        #endregion

        #region IconSize


        public static readonly DependencyProperty IconSizeProperty
            = DependencyProperty.RegisterAttached(nameof(IconSize), typeof(double), typeof(ApplicationIcon), new FrameworkPropertyMetadata(100.0D));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        #endregion

        #region Text


        public static readonly DependencyProperty TextProperty
            = DependencyProperty.RegisterAttached(nameof(Text), typeof(string), typeof(ApplicationIcon), new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region TextBackground


        public static readonly DependencyProperty TextBackgroundProperty
            = DependencyProperty.RegisterAttached(nameof(TextBackground), typeof(Brush), typeof(ApplicationIcon), new PropertyMetadata(null));

        public Brush TextBackground
        {
            get => (Brush)GetValue(TextBackgroundProperty);
            set => SetValue(TextBackgroundProperty, value);
        }

        #endregion

        #region TextForeground


        public static readonly DependencyProperty TextForegroundProperty
            = DependencyProperty.RegisterAttached(nameof(TextForeground), typeof(Brush), typeof(ApplicationIcon), new PropertyMetadata(null));

        public Brush TextForeground
        {
            get => (Brush)GetValue(TextForegroundProperty);
            set => SetValue(TextForegroundProperty, value);
        }

        #endregion

        #region ShowText


        public static readonly DependencyProperty ShowTextProperty
            = DependencyProperty.RegisterAttached(nameof(ShowText), typeof(bool), typeof(ApplicationIcon), new PropertyMetadata(true));

        public bool ShowText
        {
            get => (bool)GetValue(ShowTextProperty);
            set => SetValue(ShowTextProperty, value);
        }

        #endregion

        #region OffsetX


        public static readonly DependencyProperty OffsetXProperty
            = DependencyProperty.RegisterAttached(nameof(OffsetX), typeof(double), typeof(ApplicationIcon), new FrameworkPropertyMetadata(15.0D, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        #endregion

        #region OffsetY


        public static readonly DependencyProperty OffsetYProperty
            = DependencyProperty.RegisterAttached(nameof(OffsetY), typeof(double), typeof(ApplicationIcon), new FrameworkPropertyMetadata(15.0D, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        #endregion

        #region TextSize


        public static readonly DependencyProperty TextSizeProperty
            = DependencyProperty.RegisterAttached(nameof(TextSize), typeof(double), typeof(ApplicationIcon), new FrameworkPropertyMetadata(100.0D));

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        #endregion
    }
}
