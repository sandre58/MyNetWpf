// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls.Primitives;

namespace MyNet.Wpf.Controls
{
    public class DropDownButton : MaterialDesignThemes.Wpf.PopupBox
    {
        static DropDownButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));

        #region ButtonStyle

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(DropDownButton), new UIPropertyMetadata(null));

        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        #endregion ButtonStyle

        #region ShowDropDownButton

        public static readonly DependencyProperty ShowDropDownButtonProperty = DependencyProperty.Register(nameof(ShowDropDownButton), typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        public bool ShowDropDownButton
        {
            get => (bool)GetValue(ShowDropDownButtonProperty);
            set => SetValue(ShowDropDownButtonProperty, value);
        }

        #endregion DropDownButtonPosition

        #region DropDownButtonPosition

        public static readonly DependencyProperty DropDownButtonPositionProperty = DependencyProperty.Register(nameof(DropDownButtonPosition), typeof(DropDownButtonPosition), typeof(DropDownButton), new UIPropertyMetadata(DropDownButtonPosition.Right));

        public DropDownButtonPosition DropDownButtonPosition
        {
            get => (DropDownButtonPosition)GetValue(DropDownButtonPositionProperty);
            set => SetValue(DropDownButtonPositionProperty, value);
        }

        #endregion DropDownButtonPosition

        #region DropDownButtonOrientation

        public static readonly DependencyProperty DropDownButtonOrientationProperty = DependencyProperty.Register(nameof(DropDownButtonOrientation), typeof(DropDownButtonOrientation), typeof(DropDownButton), new UIPropertyMetadata(DropDownButtonOrientation.Down));

        public DropDownButtonOrientation DropDownButtonOrientation
        {
            get => (DropDownButtonOrientation)GetValue(DropDownButtonOrientationProperty);
            set => SetValue(DropDownButtonOrientationProperty, value);
        }

        #endregion DropDownButtonPosition

        #region PopupPadding

        public static readonly DependencyProperty PopupPaddingProperty = DependencyProperty.Register(nameof(PopupPadding), typeof(Thickness), typeof(DropDownButton), new UIPropertyMetadata(new Thickness(10)));

        public Thickness PopupPadding
        {
            get => (Thickness)GetValue(PopupPaddingProperty);
            set => SetValue(PopupPaddingProperty, value);
        }

        #endregion ButtonStyle

        #region PopupAnimation

        public static readonly DependencyProperty PopupAnimationProperty = DependencyProperty.Register(nameof(PopupAnimation), typeof(PopupAnimation), typeof(DropDownButton), new UIPropertyMetadata(PopupAnimation.Slide));

        public PopupAnimation PopupAnimation
        {
            get => (PopupAnimation)GetValue(PopupAnimationProperty);
            set => SetValue(PopupAnimationProperty, value);
        }

        #endregion PopupAnimation
    }
}
