// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = SaturationBrightnessPickerPartName, Type = typeof(Canvas))]
    [TemplatePart(Name = SaturationBrightnessPickerThumbPartName, Type = typeof(Thumb))]
    public class ColorHSVPicker : ColorPickerBase
    {
        public const string SaturationBrightnessPickerPartName = "PART_SaturationBrightnessPicker";
        public const string SaturationBrightnessPickerThumbPartName = "PART_SaturationBrightnessPickerThumb";

        private Thumb? _saturationBrightnessThumb;
        private Canvas? _saturationBrightnessCanvas;

        static ColorHSVPicker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorHSVPicker), new FrameworkPropertyMetadata(typeof(ColorHSVPicker)));

        public override void OnApplyTemplate()
        {
            if (_saturationBrightnessCanvas != null)
            {
                _saturationBrightnessCanvas.MouseDown -= SaturationBrightnessCanvasMouseDown;
                _saturationBrightnessCanvas.MouseUp -= SaturationBrightnessCanvasMouseUp;
            }
            _saturationBrightnessCanvas = GetTemplateChild(SaturationBrightnessPickerPartName) as Canvas;
            if (_saturationBrightnessCanvas != null)
            {
                _saturationBrightnessCanvas.MouseDown += SaturationBrightnessCanvasMouseDown;
                _saturationBrightnessCanvas.MouseUp += SaturationBrightnessCanvasMouseUp;
            }

            if (_saturationBrightnessThumb != null) _saturationBrightnessThumb.DragDelta -= SaturationBrightnessThumbDragDelta;
            _saturationBrightnessThumb = GetTemplateChild(SaturationBrightnessPickerThumbPartName) as Thumb;
            if (_saturationBrightnessThumb != null) _saturationBrightnessThumb.DragDelta += SaturationBrightnessThumbDragDelta;

            base.OnApplyTemplate();
        }
        private void SaturationBrightnessCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(_saturationBrightnessCanvas);
            _saturationBrightnessCanvas!.MouseMove += SaturationBrightnessCanvasMouseMove;

            var position = e.GetPosition(_saturationBrightnessCanvas);
            ApplyThumbPosition(position.X, position.Y);
        }

        private void SaturationBrightnessCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(_saturationBrightnessCanvas);
                ApplyThumbPosition(position.X, position.Y);
            }
        }

        private void SaturationBrightnessCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            _saturationBrightnessCanvas!.ReleaseMouseCapture();
            _saturationBrightnessCanvas.MouseMove -= SaturationBrightnessCanvasMouseMove;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);
            SetThumbLeft();
            SetThumbTop();
            return result;
        }

        private void SaturationBrightnessThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var position = Mouse.GetPosition(_saturationBrightnessCanvas);
            ApplyThumbPosition(position.X, position.Y);
        }

        protected override void OnSelectedColorChanged(Color? oldValue, Color? newValue)
        {
            base.OnSelectedColorChanged(oldValue, newValue);

            SetThumbLeft();
            SetThumbTop();
        }

        private void ApplyThumbPosition(double left, double top)
        {

            if (_saturationBrightnessCanvas!.ActualWidth < 1 || _saturationBrightnessCanvas.ActualHeight < 1)
            {
                return;
            }

            var s = left / _saturationBrightnessCanvas.ActualWidth;
            var v = 1 - (top / _saturationBrightnessCanvas.ActualHeight);

            if (s > 1)
            {
                s = 1;
            }

            if (v > 1)
            {
                v = 1;
            }

            if (s < 0)
            {
                s = 0;
            }

            if (v < 0)
            {
                v = 0;
            }

            var hsv = new HSVColor(A / 255d, Hue, s, v);
            SetCurrentValue(SelectedColorProperty, hsv.ToColor());
        }

        private void SetThumbLeft()
        {
            if (_saturationBrightnessCanvas is null) return;
            var left = (_saturationBrightnessCanvas.ActualWidth) / (1 / SelectedHSVColor.Saturation);
            Canvas.SetLeft(_saturationBrightnessThumb, left);
        }

        private void SetThumbTop()
        {
            if (_saturationBrightnessCanvas is null) return;
            var top = ((1 - SelectedHSVColor.Value) * _saturationBrightnessCanvas.ActualHeight);
            Canvas.SetTop(_saturationBrightnessThumb, top);
        }
    }
}
