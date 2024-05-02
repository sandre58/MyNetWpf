// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Toasting.Settings
{
    public sealed class ControlLayoutProvider : ILayoutProvider
    {
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly Corner _corner;
        private readonly FrameworkElement _element;
        private readonly Func<Window?> _getParentWindow;
        private Window? _parentWindow;

        public bool TopMost => true;

        public double Width { get; }

        public Window? ParentWindow
        {
            get
            {
                if (_parentWindow is null)
                {
                    _parentWindow = _getParentWindow();

                    if (_parentWindow is not null)
                    {
                        _parentWindow.SizeChanged += ParentWindowOnSizeChanged;
                        _parentWindow.LocationChanged += ParentWindowOnLocationChanged;
                    }
                }

                return _parentWindow;
            }
        }

        public EjectDirection EjectDirection { get; private set; }

        public ControlLayoutProvider(Func<Window?> parentWindow, FrameworkElement trackingElement, Corner corner, double offsetX, double offsetY, double width)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _element = trackingElement;
            Width = width;

            _getParentWindow = parentWindow;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            var source = PresentationSource.FromVisual(ParentWindow);
            if (source?.CompositionTarget == null)
                return new Point(0, 0);

            var transform = source.CompositionTarget.TransformFromDevice;
            var location = transform.Transform(_element.PointToScreen(new Point(0, 0)));

            return _corner switch
            {
                Corner.TopRight => GetPositionForTopRightCorner(location, actualPopupWidth),
                Corner.TopLeft => GetPositionForTopLeftCorner(location),
                Corner.BottomRight => GetPositionForBottomRightCorner(location, actualPopupWidth, actualPopupHeight),
                Corner.BottomCenter => GetPositionForBottomCenterCorner(location, actualPopupWidth, actualPopupHeight),
                Corner.BottomLeft => GetPositionForBottomLeftCorner(location, actualPopupHeight),
                _ => throw new ArgumentOutOfRangeException(nameof(actualPopupWidth)),
            };
        }

        public double GetHeight() => ParentWindow?.ActualHeight ?? double.NaN;

        private void SetEjectDirection(Corner corner) => EjectDirection = corner switch
        {
            Corner.TopRight or Corner.TopLeft => EjectDirection.ToBottom,
            Corner.BottomRight or Corner.BottomLeft or Corner.BottomCenter => EjectDirection.ToTop,
            _ => throw new ArgumentOutOfRangeException(nameof(corner), corner, null),
        };

        private Point GetPositionForBottomLeftCorner(Point location, double actualPopupHeight) => new(location.X + _offsetX, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);

        private Point GetPositionForBottomRightCorner(Point location, double actualPopupWidth, double actualPopupHeight) => new(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);

        private Point GetPositionForBottomCenterCorner(Point location, double actualPopupWidth, double actualPopupHeight) => new(location.X + (_element.ActualWidth - _offsetX - actualPopupWidth) / 2, location.Y + _element.ActualHeight - _offsetY - actualPopupHeight);


        private Point GetPositionForTopLeftCorner(Point location) => new(location.X + _offsetX, location.Y + _offsetY);

        private Point GetPositionForTopRightCorner(Point location, double actualPopupWidth) => new(location.X + _element.ActualWidth - _offsetX - actualPopupWidth, location.Y + _offsetY);

        public void Dispose()
        {
            if (_parentWindow is not null)
            {
                _parentWindow.LocationChanged -= ParentWindowOnLocationChanged;
                _parentWindow.SizeChanged -= ParentWindowOnSizeChanged;
            }
        }

        private void HandleUpdatePosition() => UpdatePositionRequested?.Invoke(this, EventArgs.Empty);

        private void ParentWindowOnLocationChanged(object? sender, EventArgs eventArgs) => HandleUpdatePosition();

        private void ParentWindowOnSizeChanged(object? sender, SizeChangedEventArgs sizeChangedEventArgs) => HandleUpdatePosition();

        public event EventHandler? UpdatePositionRequested;

#pragma warning disable 0067
        public event EventHandler? UpdateEjectDirectionRequested;

        public event EventHandler? UpdateHeightRequested;
#pragma warning restore 0067

    }
}
