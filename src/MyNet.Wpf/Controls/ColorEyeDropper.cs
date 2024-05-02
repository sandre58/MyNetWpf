// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
[DefaultProperty("Content")]
[ContentProperty("Content")]
public class ColorEyeDropper : ColorPickerBase, IAddChild
{
    private DispatcherOperation? _currentTask;
    internal ColorEyePreviewData previewData = new();
    private Popup? _previewPopup;

    static ColorEyeDropper() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorEyeDropper), new FrameworkPropertyMetadata(typeof(ColorEyeDropper)));

    /// <summary>
    ///     The DependencyProperty for the Content property.
    ///     Flags:              None
    ///     Default Value:      null
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                    "Content",
                    typeof(object),
                    typeof(ColorEyeDropper),
                    new FrameworkPropertyMetadata(
                            default,
                            new PropertyChangedCallback(OnContentChanged)));

    /// <summary>
    ///     Content is the data used to generate the child elements of this control.
    /// </summary>
    [Bindable(true)]
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    ///     Called when ContentProperty is invalidated on "d."
    /// </summary>
    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ColorEyeDropper)d;
        ctrl.SetValue(HasContentPropertyKey, e.NewValue != null);

        ctrl.OnContentChanged(e.OldValue, e.NewValue!);
    }

    /// <summary>
    ///     This method is invoked when the Content property changes.
    /// </summary>
    /// <param name="oldContent">The old value of the Content property.</param>
    /// <param name="newContent">The new value of the Content property.</param>
    protected virtual void OnContentChanged(object oldContent, object newContent)
    {
        // Remove the old content child
        RemoveLogicalChild(oldContent);
        // Add the new content child
        AddLogicalChild(newContent);
    }

    /// <summary>
    ///     The key needed set a read-only property.
    /// </summary>
    private static readonly DependencyPropertyKey HasContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                    "HasContent",
                    typeof(bool),
                    typeof(ColorEyeDropper),
                    new FrameworkPropertyMetadata(
                            false,
                            FrameworkPropertyMetadataOptions.None));

    /// <summary>
    ///     The DependencyProperty for the HasContent property.
    ///     Flags:              None
    ///     Other:              Read-Only
    ///     Default Value:      false
    /// </summary>
    public static readonly DependencyProperty HasContentProperty =
            HasContentPropertyKey.DependencyProperty;

    /// <summary>
    ///     True if Content is non-null, false otherwise.
    /// </summary>
    [Browsable(false), ReadOnly(true)]
    public bool HasContent => (bool)GetValue(HasContentProperty);

    /// <summary>Identifies the <see cref="PreviewImageOuterPixelCount"/> dependency property.</summary>
    public static readonly DependencyProperty PreviewImageOuterPixelCountProperty
        = DependencyProperty.Register(nameof(PreviewImageOuterPixelCount),
                                      typeof(int),
                                      typeof(ColorEyeDropper),
                                      new PropertyMetadata(2));

    /// <summary>
    /// Gets or sets the number of additional pixel in the preview image.
    /// </summary>
    public int PreviewImageOuterPixelCount
    {
        get => (int)GetValue(PreviewImageOuterPixelCountProperty);
        set => SetValue(PreviewImageOuterPixelCountProperty, value);
    }

    /// <summary>Identifies the <see cref="EyeDropperCursor"/> dependency property.</summary>
    public static readonly DependencyProperty EyeDropperCursorProperty
        = DependencyProperty.Register(nameof(EyeDropperCursor),
                                      typeof(Cursor),
                                      typeof(ColorEyeDropper),
                                      new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the Cursor for Selecting Color Mode
    /// </summary>
    public Cursor? EyeDropperCursor
    {
        get => (Cursor?)GetValue(EyeDropperCursorProperty);
        set => SetValue(EyeDropperCursorProperty, value);
    }

    /// <summary>Identifies the <see cref="PreviewContentTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty PreviewContentTemplateProperty
        = DependencyProperty.Register(nameof(PreviewContentTemplate),
                                      typeof(DataTemplate),
                                      typeof(ColorEyeDropper),
                                      new PropertyMetadata(default(DataTemplate)));

    /// <summary>
    /// Gets or sets the ContentControl.ContentTemplate for the preview.
    /// </summary>
    public DataTemplate? PreviewContentTemplate
    {
        get => (DataTemplate?)GetValue(PreviewContentTemplateProperty);
        set => SetValue(PreviewContentTemplateProperty, value);
    }

    private void SetPreview(Point mousePos)
    {
        _previewPopup?.Move(mousePos, new Point(16, 16));

        if (_currentTask?.Status == DispatcherOperationStatus.Executing || _currentTask?.Status == DispatcherOperationStatus.Pending)
        {
            _currentTask.Abort();
        }

        var action = new Action(() =>
        {
            mousePos = PointToScreen(mousePos);
            var outerPixelCount = PreviewImageOuterPixelCount;
            var posX = (int)Math.Round(mousePos.X - outerPixelCount);
            var posY = (int)Math.Round(mousePos.Y - outerPixelCount);
            var region = new Int32Rect(posX, posY, 2 * outerPixelCount + 1, 2 * outerPixelCount + 1);
            var previewImage = EyeDropperHelper.CaptureRegion(region);
            var previewBrush = new SolidColorBrush(EyeDropperHelper.GetPixelColor(mousePos));
            previewBrush.Freeze();

            previewData.SetValue(ColorEyePreviewData.PreviewImagePropertyKey, previewImage);
            previewData.SetValue(ColorEyePreviewData.PreviewBrushPropertyKey, previewBrush);
        });

        _currentTask = Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonDown(e);

        Mouse.Capture(this);

        _previewPopup ??= GetPreviewPopup();

        _previewPopup.Show();

        Cursor = EyeDropperCursor;

        SetPreview(e.GetPosition(this));
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseLeftButtonUp(e);

        Mouse.Capture(null);

        _previewPopup?.Hide();

        Cursor = Cursors.Arrow;

        SetCurrentValue(SelectedColorProperty, EyeDropperHelper.GetPixelColor(PointToScreen(e.GetPosition(this))));
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            SetPreview(e.GetPosition(this));
        }
    }

    private PopupEx GetPreviewPopup()
    {
        var content = new ContentControl() { Content = previewData };

        var toolTip = new PopupEx
        {
            PlacementTarget = this,
            Focusable = false,
            Placement = PlacementMode.Relative,
            StaysOpen = true,
            HorizontalOffset = -9999,
            VerticalOffset = -9999,
            IsHitTestVisible = false,
            AllowDrop = false,
            IsOpen = false,
            Visibility = Visibility.Collapsed,
            AllowsTransparency = true,
            DataContext = this,
            Child = content
        };
        BindingOperations.SetBinding(content, ContentControl.ContentTemplateProperty, new Binding { Path = new PropertyPath(PreviewContentTemplateProperty), Source = this });
        return toolTip;
    }

    public void AddChild(object value) => Content = Content == null || value == null ? value! : throw new InvalidOperationException();
    public void AddText(string text) => AddChild(text);
}

public class ColorEyePreviewData : DependencyObject
{
    /// <summary>Identifies the <see cref="PreviewImageProperty"/> dependency property.</summary>
    internal static readonly DependencyPropertyKey PreviewImagePropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(PreviewImage),
                                              typeof(ImageSource),
                                              typeof(ColorEyePreviewData),
                                              new PropertyMetadata(default(ImageSource)));

    /// <summary>Identifies the <see cref="PreviewImage"/> dependency property.</summary>
    public static readonly DependencyProperty PreviewImageProperty = PreviewImagePropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the preview image while the cursor is moving
    /// </summary>
    public ImageSource? PreviewImage => (ImageSource?)GetValue(PreviewImageProperty);

    /// <summary>Identifies the <see cref="PreviewBrushProperty"/> dependency property.</summary>
    internal static readonly DependencyPropertyKey PreviewBrushPropertyKey
        = DependencyProperty.RegisterReadOnly(nameof(PreviewBrush),
                                              typeof(Brush),
                                              typeof(ColorEyePreviewData),
                                              new PropertyMetadata(Brushes.Transparent));

    /// <summary>Identifies the <see cref="PreviewBrush"/> dependency property.</summary>
    public static readonly DependencyProperty PreviewBrushProperty = PreviewBrushPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets the preview brush while the cursor is moving
    /// </summary>
    public Brush PreviewBrush => (Brush)GetValue(PreviewBrushProperty);
}
