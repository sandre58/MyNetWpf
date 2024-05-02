// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNet.Wpf.Controls;

/// <summary>
/// A custom control implementing a rating bar.
/// The icon aka content may be set as a DataTemplate via the ButtonContentTemplate property.
/// </summary>
public class RatingBar : Control
{
    public static readonly RoutedCommand SelectRatingCommand = new();

    static RatingBar() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingBar), new FrameworkPropertyMetadata(typeof(RatingBar)));

    private readonly ObservableCollection<RatingBarButton> _ratingButtonsInternal = [];
    private readonly ReadOnlyObservableCollection<RatingBarButton> _ratingButtons;

    public RatingBar()
    {
        CommandBindings.Add(new CommandBinding(SelectRatingCommand, SelectItemHandler));
        _ratingButtons = new ReadOnlyObservableCollection<RatingBarButton>(_ratingButtonsInternal);
        MouseLeave += RatingBar_MouseLeave;
    }

    private void SelectItemHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
    {
        if (executedRoutedEventArgs.Parameter is int parameter && !IsReadOnly)
        {
            if (!IsFractionalValueEnabled)
            {
                Value = parameter;
                return;
            }
            Value = GetValueAtMousePosition((RatingBarButton)executedRoutedEventArgs.OriginalSource);
        }
    }

    private double GetValueAtMousePosition(RatingBarButton ratingBarButton)
    {
        // Get mouse offset inside source
        var p = Mouse.GetPosition(ratingBarButton);
        var percentSelected = Orientation == Orientation.Horizontal ? p.X / ratingBarButton.ActualWidth : p.Y / ratingBarButton.ActualHeight;
        return ratingBarButton.Value - 1 + percentSelected;
    }

    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
        nameof(Min), typeof(int), typeof(RatingBar), new PropertyMetadata(1, MinMaxPropertyChangedCallback, MinPropertyCoerceValueCallback));

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
        nameof(Max), typeof(int), typeof(RatingBar), new PropertyMetadata(5, MinMaxPropertyChangedCallback, MaxPropertyCoerceValueCallback));

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    private static readonly DependencyPropertyKey IsFractionalValueEnabledPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsFractionalValueEnabled), typeof(bool), typeof(RatingBar), new PropertyMetadata(false));

    internal static readonly DependencyProperty IsFractionalValueEnabledProperty =
        IsFractionalValueEnabledPropertyKey.DependencyProperty;

    internal bool IsFractionalValueEnabled
    {
        get => (bool)GetValue(IsFractionalValueEnabledProperty);
        private set => SetValue(IsFractionalValueEnabledPropertyKey, value);
    }

    public static readonly DependencyProperty ValueIncrementsProperty = DependencyProperty.Register(
        nameof(ValueIncrements), typeof(double), typeof(RatingBar), new PropertyMetadata(1.0, ValueIncrementsPropertyChangedCallback, ValueIncrementsCoerceValueCallback));

    private static void ValueIncrementsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ratingBar = (RatingBar)d;
        ratingBar.IsFractionalValueEnabled = Math.Abs(ratingBar.ValueIncrements - 1.0) > 1e-10;
        ratingBar.RebuildButtons();
    }

    private static object ValueIncrementsCoerceValueCallback(DependencyObject d, object baseValue)
        => Math.Max(double.Epsilon, Math.Min(1.0, (double)baseValue));

    /// <summary>
    /// Gets or sets the value increments. Set to a value between 0.0 and 1.0 (both exclusive) to enable fractional values. Default value is 1.0 (i.e. fractional values disabled)
    /// </summary>
    public double ValueIncrements
    {
        get => (double)GetValue(ValueIncrementsProperty);
        set => SetValue(ValueIncrementsProperty, value);
    }

    public static readonly DependencyProperty IsPreviewValueEnabledProperty = DependencyProperty.Register(
        nameof(IsPreviewValueEnabled), typeof(bool), typeof(RatingBar), new PropertyMetadata(false));

    public bool IsPreviewValueEnabled
    {
        get => (bool)GetValue(IsPreviewValueEnabledProperty);
        set => SetValue(IsPreviewValueEnabledProperty, value);
    }

    private static readonly DependencyPropertyKey PreviewValuePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(PreviewValue), typeof(double?), typeof(RatingBar), new PropertyMetadata(null, null, PreviewValuePropertyCoerceValueCallback));

    private static object? PreviewValuePropertyCoerceValueCallback(DependencyObject d, object? baseValue)
    {
        if (baseValue == null)
            return null;

        var ratingBar = (RatingBar)d;
        if (baseValue is double value)
        {
            if (!ratingBar.IsFractionalValueEnabled)
                value = Math.Ceiling(value);
            return ratingBar.CoerceToValidIncrement(value);
        }
        return (double)ratingBar.Min;
    }

    internal static readonly DependencyProperty PreviewValueProperty =
        PreviewValuePropertyKey.DependencyProperty;

    internal double? PreviewValue
    {
        get => (double?)GetValue(PreviewValueProperty);
        private set => SetValue(PreviewValuePropertyKey, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(RatingBar), new PropertyMetadata(0.0, ValuePropertyChangedCallback, ValuePropertyCoerceValueCallback));

    private static void ValuePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var ratingBar = (RatingBar)dependencyObject;

        OnValueChanged(ratingBar, dependencyPropertyChangedEventArgs);
    }

    private static object ValuePropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var ratingBar = (RatingBar)d;

        // If factional values are disabled we don't do any coercion. This maintains back-compat where coercion was not applied and Value could be outside of Min/Max range. 
        return !ratingBar.IsFractionalValueEnabled
            ? baseValue
            : baseValue is double value ? ratingBar.CoerceToValidIncrement(value) : (object)(double)ratingBar.Min;
    }

    private double CoerceToValidIncrement(double value)
    {
        // Coerce the value into a multiple of ValueIncrements and within the bounds.
        var valueInCorrectMultiple = Math.Round(value / ValueIncrements, MidpointRounding.AwayFromZero) * ValueIncrements;
        return Math.Min(Max, Math.Max(Min, valueInCorrectMultiple));
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(Value),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(RatingBar));

    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    private static void OnValueChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (RatingBar)d;
        var args = new RoutedPropertyChangedEventArgs<double>(
                (double)e.OldValue,
                (double)e.NewValue)
        { RoutedEvent = ValueChangedEvent };
        instance.RaiseEvent(args);
    }

    public ReadOnlyObservableCollection<RatingBarButton> RatingButtons => _ratingButtons;

    public static readonly DependencyProperty ValueItemContainerButtonStyleProperty = DependencyProperty.Register(
        nameof(ValueItemContainerButtonStyle), typeof(Style), typeof(RatingBar), new PropertyMetadata(default(Style)));

    public Style? ValueItemContainerButtonStyle
    {
        get => (Style?)GetValue(ValueItemContainerButtonStyleProperty);
        set => SetValue(ValueItemContainerButtonStyleProperty, value);
    }

    public static readonly DependencyProperty ValueItemTemplateProperty = DependencyProperty.Register(
        nameof(ValueItemTemplate), typeof(DataTemplate), typeof(RatingBar), new PropertyMetadata(default(DataTemplate)));

    public DataTemplate? ValueItemTemplate
    {
        get => (DataTemplate?)GetValue(ValueItemTemplateProperty);
        set => SetValue(ValueItemTemplateProperty, value);
    }

    public static readonly DependencyProperty ValueItemTemplateSelectorProperty = DependencyProperty.Register(
        nameof(ValueItemTemplateSelector), typeof(DataTemplateSelector), typeof(RatingBar), new PropertyMetadata(default(DataTemplateSelector)));

    public DataTemplateSelector? ValueItemTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(ValueItemTemplateSelectorProperty);
        set => SetValue(ValueItemTemplateSelectorProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(Orientation), typeof(RatingBar), new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        nameof(IsReadOnly), typeof(bool), typeof(RatingBar), new PropertyMetadata(default(bool)));

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    private static object MinPropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var ratingBar = (RatingBar)d;
        return Math.Min((int)baseValue, ratingBar.Max);
    }

    private static void MinMaxPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var ratingBar = (RatingBar)dependencyObject;
        ratingBar.CoerceValue(ValueProperty);
        ratingBar.RebuildButtons();
    }

    private static object MaxPropertyCoerceValueCallback(DependencyObject d, object baseValue)
    {
        var ratingBar = (RatingBar)d;
        return Math.Max((int)baseValue, ratingBar.Min);
    }

    private void RebuildButtons()
    {
        foreach (var ratingBarButton in _ratingButtonsInternal)
        {
            ratingBarButton.MouseMove -= RatingBarButton_MouseMove;
        }
        _ratingButtonsInternal.Clear();

        var start = IsFractionalValueEnabled ? Min + 1 : Min;
        for (var i = start; i <= Max; i++)
        {
            var ratingBarButton = new RatingBarButton
            {
                Content = i,
                ContentTemplate = ValueItemTemplate,
                ContentTemplateSelector = ValueItemTemplateSelector,
                Style = ValueItemContainerButtonStyle,
                Value = i,
            };
            ratingBarButton.MouseMove += RatingBarButton_MouseMove;
            _ratingButtonsInternal.Add(ratingBarButton);
        }
    }

    private void RatingBar_MouseLeave(object sender, MouseEventArgs e) => PreviewValue = null;

    private void RatingBarButton_MouseMove(object sender, MouseEventArgs e)
    {
        if (!IsPreviewValueEnabled)
            return;

        var ratingBarButton = (RatingBarButton)sender;
        PreviewValue = GetValueAtMousePosition(ratingBarButton);
    }

    public override void OnApplyTemplate()
    {
        RebuildButtons();

        base.OnApplyTemplate();
    }
}
