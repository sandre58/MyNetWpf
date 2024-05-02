// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Toasting.Settings
{
    public static class PositionExtensions
    {
        public static Point GetActualPosition(this UIElement element)
        {
            var pt = element.PointToScreen(new Point(0, 0));
            var source = PresentationSource.FromVisual(element);
            return source.CompositionTarget.TransformFromDevice.Transform(pt);
        }
    }
}
