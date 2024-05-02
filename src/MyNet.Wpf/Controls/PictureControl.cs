// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyNet.Wpf.Controls
{
    public class PictureControl : ContentControl
    {
        static PictureControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PictureControl), new FrameworkPropertyMetadata(typeof(PictureControl)));

        #region Image

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                nameof(Image),
                typeof(ImageSource),
                typeof(PictureControl));

        public ImageSource? Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        #endregion Image

        #region Stretch

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(PictureControl),
                new PropertyMetadata(System.Windows.Media.Stretch.UniformToFill));

        public Stretch? Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        #endregion Image
    }
}
