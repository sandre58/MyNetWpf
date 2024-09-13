// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Helpers
{
    public static class DpiHelper
    {
        private static readonly int DpiX;
        private static readonly int DpiY;

        private const double StandardDpiX = 96.0;
        private const double StandardDpiY = 96.0;

        static DpiHelper()
        {
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Could not find DpiX property on {nameof(SystemParameters)}");
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Could not find Dpi property on {nameof(SystemParameters)}");


            DpiX = (int)dpiXProperty.GetValue(null, null)!;
            DpiY = (int)dpiYProperty.GetValue(null, null)!;
        }

        public static double TransformToDeviceY(Visual visual, double y)
        {
            var source = PresentationSource.FromVisual(visual);
            return source?.CompositionTarget != null ? y * source.CompositionTarget.TransformToDevice.M22 : TransformToDeviceY(y);
        }

        public static double TransformFromDeviceY(Visual visual, double y)
        {
            var source = PresentationSource.FromVisual(visual);
            return source?.CompositionTarget != null ? y / source.CompositionTarget.TransformToDevice.M22 : TransformFromDeviceY(y);
        }

        public static double TransformToDeviceX(Visual visual, double x)
        {
            var source = PresentationSource.FromVisual(visual);
            return source?.CompositionTarget != null ? x * source.CompositionTarget.TransformToDevice.M11 : TransformToDeviceX(x);
        }

        public static double TransformFromDeviceX(Visual visual, double x)
        {
            var source = PresentationSource.FromVisual(visual);
            return source?.CompositionTarget != null ? x / source.CompositionTarget.TransformToDevice.M11 : TransformFromDeviceX(x);
        }

        public static double TransformToDeviceY(double y) => y * DpiY / StandardDpiY;

        public static double TransformFromDeviceY(double y) => y / DpiY * StandardDpiY;

        public static double TransformToDeviceX(double x) => x * DpiX / StandardDpiX;

        public static double TransformFromDeviceX(double x) => x / DpiX * StandardDpiX;
    }
}
