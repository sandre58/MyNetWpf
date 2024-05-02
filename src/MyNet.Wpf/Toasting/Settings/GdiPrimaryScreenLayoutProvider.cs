// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Forms;
using MyNet.Utilities;

namespace MyNet.Wpf.Toasting.Settings
{
    public sealed class GdiPrimaryScreenLayoutProvider : ILayoutProvider
    {
        private readonly Corner _corner;
        private readonly float _dpiRatioX;
        private readonly float _dpiRatioY;
        private readonly double _offsetX;
        private readonly double _offsetY;

        private double ScreenHeight => (Screen.PrimaryScreen?.Bounds.Height ?? 0) / _dpiRatioY;
        private double ScreenWidth => (Screen.PrimaryScreen?.Bounds.Width ?? 0) / _dpiRatioX;

        private double WorkAreaHeight => (Screen.PrimaryScreen?.WorkingArea.Height ?? 0) / _dpiRatioY;
        private double WorkAreaWidth => (Screen.PrimaryScreen?.WorkingArea.Width ?? 0) / _dpiRatioX;

        public bool TopMost => true;

        public double Width { get; }

        public Window? ParentWindow { get; }

        public EjectDirection EjectDirection { get; private set; }

        public GdiPrimaryScreenLayoutProvider(
            Corner corner,
            double offsetX,
            double offsetY, double width)
        {
            _corner = corner;
            using (var gfx = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                _dpiRatioX = gfx.DpiX / 96F;
                _dpiRatioY = gfx.DpiY / 96F;
            }
            _offsetX = offsetX;
            _offsetY = offsetY;
            Width = width;

            ParentWindow = null;

            SetEjectDirection(corner);
        }

        public Point GetPosition(double actualPopupWidth, double actualPopupHeight) => _corner switch
        {
            Corner.TopRight => GetPositionForTopRightCorner(actualPopupWidth),
            Corner.TopLeft => GetPositionForTopLeftCorner(),
            Corner.BottomRight => GetPositionForBottomRightCorner(actualPopupWidth, actualPopupHeight),
            Corner.BottomLeft => GetPositionForBottomLeftCorner(actualPopupHeight),
            Corner.BottomCenter => GetPositionForBottomCenterCorner(actualPopupWidth, actualPopupHeight),
            _ => throw new ArgumentOutOfRangeException(nameof(actualPopupWidth)),
        };

        public double GetHeight() => ScreenHeight;

        private void SetEjectDirection(Corner corner) => EjectDirection = corner switch
        {
            Corner.TopRight or Corner.TopLeft => EjectDirection.ToBottom,
            Corner.BottomRight or Corner.BottomLeft or Corner.BottomCenter => EjectDirection.ToTop,
            _ => throw new ArgumentOutOfRangeException(nameof(corner), corner, null),
        };

        private Point GetPositionForBottomLeftCorner(double actualPopupHeight)
        {
            var pointX = _offsetX;
            var pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - WorkAreaWidth + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForBottomCenterCorner(double actualPopupWidth, double actualPopupHeight)
        {
            var pointX = (WorkAreaWidth - _offsetX - actualPopupWidth) / 2;
            var pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = (ScreenWidth - _offsetX - actualPopupWidth) / 2;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }


        private Point GetPositionForBottomRightCorner(double actualPopupWidth, double actualPopupHeight)
        {
            var pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            var pointY = WorkAreaHeight - _offsetY - actualPopupHeight;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - _offsetX - actualPopupWidth;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - _offsetY - actualPopupHeight;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForTopLeftCorner()
        {
            var pointX = _offsetX;
            var pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - WorkAreaWidth + _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }

        private Point GetPositionForTopRightCorner(double actualPopupWidth)
        {
            var pointX = WorkAreaWidth - _offsetX - actualPopupWidth;
            var pointY = _offsetY;

            switch (GetTaskBarLocation())
            {
                case WindowsTaskBarLocation.Left:
                    pointX = ScreenWidth - actualPopupWidth - _offsetX;
                    break;

                case WindowsTaskBarLocation.Top:
                    pointY = ScreenHeight - WorkAreaHeight + _offsetY;
                    break;
            }

            return new Point(pointX, pointY);
        }


        private static WindowsTaskBarLocation GetTaskBarLocation() => SystemParameters.WorkArea.Left > 0
                ? WindowsTaskBarLocation.Left
                : SystemParameters.WorkArea.Top > 0
                ? WindowsTaskBarLocation.Top
                : SystemParameters.WorkArea.Left.NearlyEqual(0) &&
                SystemParameters.WorkArea.Width < SystemParameters.PrimaryScreenWidth
                ? WindowsTaskBarLocation.Right
                : WindowsTaskBarLocation.Bottom;


        public void Dispose()
        {
            // Method intentionally left empty.
        }

#pragma warning disable 0067
        public event EventHandler? UpdatePositionRequested;

        public event EventHandler? UpdateEjectDirectionRequested;

        public event EventHandler? UpdateHeightRequested;
#pragma warning restore 0067
    }
}
