// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Controls
{
    public class BottomDashedLineAdorner(UIElement adornedElement) : Adorner(adornedElement)
    {
        private static readonly Thickness DefaultThickness = new(1);
        private const double DefaultThicknessScale = 1.33;
        private const double DefaultOpacity = 1;

        #region AttachedProperty : IsActiveProperty
        public static readonly DependencyProperty IsActiveProperty
            = DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(BottomDashedLineAdorner), new PropertyMetadata(true));

        public static bool GetIsActive(DependencyObject element)
            => (bool)element.GetValue(IsActiveProperty);
        public static void SetIsActive(DependencyObject element, bool value)
            => element.SetValue(IsActiveProperty, value);
        #endregion

        #region AttachedProperty : IsAttachedProperty
        public static readonly DependencyProperty IsAttachedProperty
            = DependencyProperty.RegisterAttached("IsAttached", typeof(bool), typeof(BottomDashedLineAdorner), new PropertyMetadata(default(bool), OnIsAttachedChanged));

        public static bool GetIsAttached(DependencyObject element)
            => (bool)element.GetValue(IsAttachedProperty);
        public static void SetIsAttached(DependencyObject element, bool value)
            => element.SetValue(IsAttachedProperty, value);
        #endregion

        #region AttachedProperty : BrushProperty
        public static readonly DependencyProperty BrushProperty
            = DependencyProperty.RegisterAttached("Brush", typeof(Brush), typeof(BottomDashedLineAdorner), new PropertyMetadata(default(Brush)));

        public static Brush? GetBrush(DependencyObject element)
            => (Brush)element.GetValue(BrushProperty);
        public static void SetBrush(DependencyObject element, Brush? value)
            => element.SetValue(BrushProperty, value);
        #endregion

        #region AttachedProperty : ThicknessProperty
        public static readonly DependencyProperty ThicknessProperty
            = DependencyProperty.RegisterAttached("Thickness", typeof(Thickness), typeof(BottomDashedLineAdorner), new PropertyMetadata(DefaultThickness));

        public static Thickness GetThickness(DependencyObject element)
            => (Thickness)element.GetValue(ThicknessProperty);
        public static void SetThickness(DependencyObject element, Thickness value)
            => element.SetValue(ThicknessProperty, value);
        #endregion

        #region AttachedProperty : ThicknessScaleProperty
        public static readonly DependencyProperty ThicknessScaleProperty
            = DependencyProperty.RegisterAttached("ThicknessScale", typeof(double), typeof(BottomDashedLineAdorner), new PropertyMetadata(DefaultThicknessScale));

        public static double GetThicknessScale(DependencyObject element)
            => (double)element.GetValue(ThicknessScaleProperty);
        public static void SetThicknessScale(DependencyObject element, double value)
            => element.SetValue(ThicknessScaleProperty, value);
        #endregion

        #region AttachedProperty : BrushOpacityProperty
        public static readonly DependencyProperty BrushOpacityProperty
            = DependencyProperty.RegisterAttached("BrushOpacity", typeof(double), typeof(BottomDashedLineAdorner), new PropertyMetadata(DefaultOpacity));

        public static double GetBrushOpacity(DependencyObject element)
            => (double)element.GetValue(BrushOpacityProperty);
        public static void SetBrushOpacity(DependencyObject element, double value)
            => element.SetValue(BrushOpacityProperty, value);

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            var targetElementBox = new Rect(AdornedElement.RenderSize);
            var lineThickness = GetThickness(AdornedElement).Bottom * GetThicknessScale(AdornedElement);
            var lineOpacity = GetBrushOpacity(AdornedElement);
            var lineBrush = GetBrush(AdornedElement);

            var pen = new Pen(lineBrush, lineThickness)
            {
                DashStyle = DashStyles.Dash,
                DashCap = PenLineCap.Round
            };

            drawingContext.PushOpacity(lineOpacity);
            drawingContext.DrawLine(pen, targetElementBox.BottomLeft, targetElementBox.BottomRight);
            drawingContext.Pop();
        }

        private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;
            var adornerIsAttached = (bool)e.NewValue;

            if (adornerIsAttached && GetIsActive(d))
            {
                element.AddAdorner(new BottomDashedLineAdorner(element));
            }
            else
            {
                element.RemoveAdorners<BottomDashedLineAdorner>();
            }
        }
    }
}
