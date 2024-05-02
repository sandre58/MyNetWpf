// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;

namespace MyNet.Wpf.Extensions
{
    internal static class PopupExtensions
    {
        public static void Show(this Popup popup)
        {
            popup.Visibility = Visibility.Visible;
            popup.IsOpen = true;
        }

        public static void Hide(this Popup popup)
        {
            popup.IsOpen = false;
            popup.Visibility = Visibility.Collapsed;
        }

        public static void Move(this Popup popup, Point point, Point offset)
        {
            var translationX = point.X + offset.X;
            var translationY = point.Y + offset.Y;

            popup.Placement = PlacementMode.Relative;
            popup.SetCurrentValue(Popup.HorizontalOffsetProperty, translationX);
            popup.SetCurrentValue(Popup.VerticalOffsetProperty, translationY);
        }
    }
}
