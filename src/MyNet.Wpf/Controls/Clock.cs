// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
public class Clock : MaterialDesignThemes.Wpf.Clock
{
    static Clock() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));

    #region ClockBackground

    public static readonly DependencyProperty ClockBackgroundProperty = DependencyProperty.Register("ClockBackground", typeof(Brush), typeof(Clock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush ClockBackground
    {
        get => (Brush)GetValue(ClockBackgroundProperty);
        set => SetValue(ClockBackgroundProperty, value);
    }

    #endregion

    #region HeaderBackground

    public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(Clock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush HeaderBackground
    {
        get => (Brush)GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    #endregion

    #region HeaderForeground

    public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(Clock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush HeaderForeground
    {
        get => (Brush)GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    #endregion

    #region LineBackground

    public static readonly DependencyProperty LineBackgroundProperty = DependencyProperty.Register("LineBackground", typeof(Brush), typeof(Clock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush LineBackground
    {
        get => (Brush)GetValue(LineBackgroundProperty);
        set => SetValue(LineBackgroundProperty, value);
    }

    #endregion

    #region SelectedForeground

    public static readonly DependencyProperty SelectedForegroundProperty = DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(Clock), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush SelectedForeground
    {
        get => (Brush)GetValue(SelectedForegroundProperty);
        set => SetValue(SelectedForegroundProperty, value);
    }

    #endregion
}
