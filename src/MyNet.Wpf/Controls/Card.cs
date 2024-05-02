// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ClipBorderPartName, Type = typeof(Border))]
    public class Card : ContentControl
    {
        private Border? _clipBorder;

        private const string ClipBorderPartName = "PART_ClipBorder";

        static Card() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));

        #region UniformCornerRadius

        public static readonly DependencyProperty UniformCornerRadiusProperty = DependencyProperty.Register("UniformCornerRadius", typeof(double), typeof(Card), new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double UniformCornerRadius
        {
            get => (double)GetValue(UniformCornerRadiusProperty);
            set => SetValue(UniformCornerRadiusProperty, value);
        }

        #endregion

        #region ContentClip

        private static readonly DependencyPropertyKey ContentClipPropertyKey = DependencyProperty.RegisterReadOnly("ContentClip", typeof(Geometry), typeof(Card), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentClipProperty = ContentClipPropertyKey.DependencyProperty;

        public Geometry? ContentClip
        {
            get => (Geometry)GetValue(ContentClipProperty);
            private set => SetValue(ContentClipPropertyKey, value);
        }

        public static readonly DependencyProperty ClipContentProperty = DependencyProperty.Register("ClipContent", typeof(bool), typeof(Card), new PropertyMetadata(false));

        public bool ClipContent
        {
            get => (bool)GetValue(ClipContentProperty);
            set => SetValue(ClipContentProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _clipBorder = Template.FindName(ClipBorderPartName, this) as Border;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (_clipBorder != null)
            {
                ContentClip = new RectangleGeometry(new Rect(point2: new Point(Math.Max(0.0, _clipBorder!.ActualWidth), Math.Max(0.0, _clipBorder!.ActualHeight)), point1: new Point(0.0, 0.0)), UniformCornerRadius, UniformCornerRadius);
            }
        }
    }
}
