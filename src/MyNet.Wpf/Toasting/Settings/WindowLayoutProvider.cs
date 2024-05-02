// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Toasting.Settings
{
    public sealed class WindowLayoutProvider : ILayoutProvider
    {
        private readonly Corner _corner;
        private readonly double _offsetX;
        private readonly double _offsetY;
        private readonly Func<Window?> _getParentWindow;
        private Window? _parentWindow;

        public bool TopMost { get; }

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
                        _parentWindow.StateChanged += ParentWindowOnStateChanged;
                        _parentWindow.Activated += ParentWindowOnActivated;
                    }
                }

                return _parentWindow;
            }
        }

        public EjectDirection EjectDirection { get; private set; }

        public WindowLayoutProvider(Func<Window?> parentWindow, Corner corner, double offsetX, double offsetY, double width)
        {
            _corner = corner;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _getParentWindow = parentWindow;
            Width = width;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight)
        {
            var parentPosition = (ParentWindow?.Content as FrameworkElement)?.GetActualPosition() ?? ParentWindow?.GetActualPosition() ?? default;

            return _corner switch
            {
                Corner.TopCenter => GetPositionForTopCenter(parentPosition, actualPopupWidth),
                Corner.TopRight => GetPositionForTopRightCorner(parentPosition, actualPopupWidth),
                Corner.TopLeft => GetPositionForTopLeftCorner(parentPosition),
                Corner.BottomRight => GetPositionForBottomRightCorner(parentPosition, actualPopupWidth, actualPopupHeight),
                Corner.BottomLeft => GetPositionForBottomLeftCorner(parentPosition, actualPopupHeight),
                Corner.BottomCenter => GetPositionForBottomCenter(parentPosition, actualPopupWidth, actualPopupHeight),
                _ => throw new ArgumentOutOfRangeException(nameof(actualPopupWidth)),
            };
        }

        public double GetHeight()
        {
            var actualHeight = (ParentWindow?.Content as FrameworkElement)?.ActualHeight ?? ParentWindow?.ActualHeight;

            return actualHeight ?? double.NaN;
        }

        private double GetWindowWidth()
        {
            var actualWidth = (ParentWindow?.Content as FrameworkElement)?.ActualWidth ?? ParentWindow?.ActualWidth;

            return actualWidth ?? double.NaN;
        }

        private void SetEjectDirection(Corner corner) => EjectDirection = corner switch
        {
            Corner.TopRight or Corner.TopLeft or Corner.TopCenter => EjectDirection.ToBottom,
            Corner.BottomRight or Corner.BottomLeft or Corner.BottomCenter => EjectDirection.ToTop,
            _ => throw new ArgumentOutOfRangeException(nameof(corner), corner, null),
        };

        private Point GetPositionForBottomLeftCorner(Point parentPosition, double actualPopupHeight) => new(parentPosition.X + _offsetX, parentPosition.Y + GetHeight() - actualPopupHeight - _offsetY);

        private Point GetPositionForBottomRightCorner(Point parentPosition, double actualPopupWidth, double actualPopupHeight) => new(parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth, parentPosition.Y + GetHeight() - actualPopupHeight - _offsetY);

        private Point GetPositionForBottomCenter(Point parentPosition, double actualPopupWidth, double actualPopupHeight) => new(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y + GetHeight() - actualPopupHeight - _offsetY);

        private Point GetPositionForTopCenter(Point parentPosition, double actualPopupWidth) => new(parentPosition.X + (GetWindowWidth() - actualPopupWidth) / 2, parentPosition.Y + _offsetY);
        private Point GetPositionForTopLeftCorner(Point parentPosition) => new(parentPosition.X + _offsetX, parentPosition.Y + _offsetY);

        private Point GetPositionForTopRightCorner(Point parentPosition, double actualPopupWidth) => new(parentPosition.X + GetWindowWidth() - _offsetX - actualPopupWidth, parentPosition.Y + _offsetY);

        public void Dispose()
        {
            if (_parentWindow is not null)
            {
                _parentWindow.LocationChanged -= ParentWindowOnLocationChanged;
                _parentWindow.SizeChanged -= ParentWindowOnSizeChanged;
                _parentWindow.StateChanged -= ParentWindowOnStateChanged;
                _parentWindow.Activated -= ParentWindowOnActivated;
            }
        }

        private void RequestUpdatePosition()
        {
            UpdateHeightRequested?.Invoke(this, EventArgs.Empty);
            UpdateEjectDirectionRequested?.Invoke(this, EventArgs.Empty);
            UpdatePositionRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ParentWindowOnLocationChanged(object? sender, EventArgs eventArgs) => RequestUpdatePosition();

        private void ParentWindowOnSizeChanged(object? sender, SizeChangedEventArgs sizeChangedEventArgs) => RequestUpdatePosition();

        private void ParentWindowOnStateChanged(object? sender, EventArgs eventArgs) => RequestUpdatePosition();

        private void ParentWindowOnActivated(object? sender, EventArgs eventArgs) => RequestUpdatePosition();

        public event EventHandler? UpdatePositionRequested;

        public event EventHandler? UpdateEjectDirectionRequested;

        public event EventHandler? UpdateHeightRequested;
    }
}
