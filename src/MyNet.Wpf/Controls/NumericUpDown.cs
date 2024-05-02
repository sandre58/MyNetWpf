﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.ValueBoxes;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = PART_NumericUp, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_NumericDown, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [StyleTypedProperty(Property = nameof(SpinButtonStyle), StyleTargetType = typeof(ButtonBase))]
    public partial class NumericUpDown : Control
    {
        private const string PART_NumericDown = "PART_NumericDown";
        private const string PART_NumericUp = "PART_NumericUp";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ContentHost = "PART_ContentHost";
        private const double DefaultInterval = 1d;
        private const int DefaultDelay = 500;

        [GeneratedRegex(@"^(?<complexHEX>.*{\d\s*:[Xx]\d*}.*)?(?<simpleHEX>[Xx]\d*)?$")]
        private static partial Regex StringFormatHexadecimalRegex();
        [GeneratedRegex(@"^([a-fA-F0-9]{1,2}\s?)+$")]
        private static partial Regex HexadecimalRegex();
        [GeneratedRegex(@"\{0\s*(:(?<format>.*))?\}")]
        private static partial Regex StringFormatRegex();

        private const string RawRegexNumberString = @"[-+]?(?<![0-9][<DecimalSeparator><GroupSeparator>])[<DecimalSeparator><GroupSeparator>]?[0-9]+(?:[<DecimalSeparator><GroupSeparator>\s][0-9]+)*[<DecimalSeparator><GroupSeparator>]?[0-9]?(?:[eE][-+]?[0-9]+)?(?!\.[0-9])";
        private Regex? _regexNumber = null;

        private Lazy<PropertyInfo?> _handlesMouseWheelScrolling = new();
        private double _internalIntervalMultiplierForCalculation = DefaultInterval;
        private double _internalLargeChange = DefaultInterval * 100;
        private double _intervalValueSinceReset;
        private bool _manualChange;
        private RepeatButton? _repeatDown;
        private RepeatButton? _repeatUp;
        private TextBox? _valueTextBox;
        private ScrollViewer? _scrollViewer;

        /// <summary>Identifies the <see cref="ValueIncremented"/> routed event.</summary>
        public static readonly RoutedEvent ValueIncrementedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueIncremented),
                                               RoutingStrategy.Bubble,
                                               typeof(NumericUpDownChangedRoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueIncrementedEvent handler
        /// Event which will be fired from this NumericUpDown when its value was incremented.
        /// </summary>
        public event NumericUpDownChangedRoutedEventHandler ValueIncremented
        {
            add => AddHandler(ValueIncrementedEvent, value);
            remove => RemoveHandler(ValueIncrementedEvent, value);
        }

        /// <summary>Identifies the <see cref="ValueDecremented"/> routed event.</summary>
        public static readonly RoutedEvent ValueDecrementedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueDecremented),
                                               RoutingStrategy.Bubble,
                                               typeof(NumericUpDownChangedRoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueDecrementedEvent handler
        /// Event which will be fired from this NumericUpDown when its value was decremented.
        /// </summary>
        public event NumericUpDownChangedRoutedEventHandler ValueDecremented
        {
            add => AddHandler(ValueDecrementedEvent, value);
            remove => RemoveHandler(ValueDecrementedEvent, value);
        }

        /// <summary>Identifies the <see cref="DelayChanged"/> routed event.</summary>
        public static readonly RoutedEvent DelayChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(DelayChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove DelayChangedEvent handler
        /// Event which will be fired from this NumericUpDown when its delay value has been changed.
        /// </summary>
        public event RoutedEventHandler DelayChanged
        {
            add => AddHandler(DelayChangedEvent, value);
            remove => RemoveHandler(DelayChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="MaximumReached"/> routed event.</summary>
        public static readonly RoutedEvent MaximumReachedEvent
            = EventManager.RegisterRoutedEvent(nameof(MaximumReached),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove MaximumReachedEvent handler
        /// Event fired from this NumericUpDown when its value has reached the maximum value.
        /// </summary>
        public event RoutedEventHandler MaximumReached
        {
            add => AddHandler(MaximumReachedEvent, value);
            remove => RemoveHandler(MaximumReachedEvent, value);
        }

        /// <summary>Identifies the <see cref="MinimumReached"/> routed event.</summary>
        public static readonly RoutedEvent MinimumReachedEvent
            = EventManager.RegisterRoutedEvent(nameof(MinimumReached),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove MinimumReachedEvent handler
        /// Event fired from this NumericUpDown when its value has reached the minimum value.
        /// </summary>
        public event RoutedEventHandler MinimumReached
        {
            add => AddHandler(MinimumReachedEvent, value);
            remove => RemoveHandler(MinimumReachedEvent, value);
        }

        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ValueChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<double?>),
                                               typeof(NumericUpDown));

        /// <summary>
        /// Add / Remove ValueChangedEvent handler
        /// Event which will be fired from this NumericUpDown when its value has been changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double?> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        /// <summary>Identifies the <see cref="Delay"/> dependency property.</summary>
        public static readonly DependencyProperty DelayProperty
            = DependencyProperty.Register(nameof(Delay),
                                          typeof(int),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(DefaultDelay, OnDelayPropertyChanged),
                                          value => Convert.ToInt32(value) >= 0);

        private static void OnDelayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.OldValue is int oldDelay && e.NewValue is int newDelay && d is NumericUpDown numericUpDown)
            {
                numericUpDown.RaiseChangeDelay();
                numericUpDown.OnDelayChanged(oldDelay, newDelay);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the NumericUpDown waits while the up/down button is pressed
        /// before it starts increasing/decreasing the <see cref="Value" /> for the specified <see cref="Interval" /> .
        /// The value must be non-negative.
        /// </summary>
        [Bindable(true)]
        [DefaultValue(DefaultDelay)]
        [Category("Behavior")]
        public int Delay
        {
            get => (int)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        /// <summary>Identifies the <see cref="TextAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(NumericUpDown));

        /// <summary>
        /// Gets or sets the horizontal alignment of the contents inside the text box.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(TextAlignment.Right)]
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="Speedup"/> dependency property.</summary>
        public static readonly DependencyProperty SpeedupProperty
            = DependencyProperty.Register(nameof(Speedup),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, OnSpeedupPropertyChanged));

        private static void OnSpeedupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (d as NumericUpDown)?.OnSpeedupChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value to be added to or subtracted from <see cref="Value" /> remains
        /// always <see cref="Interval" /> or if it will increase faster after pressing the up/down button/arrow some time.
        /// </summary>
        [Category("Common")]
        [DefaultValue(true)]
        public bool Speedup
        {
            get => (bool)GetValue(SpeedupProperty);
            set => SetValue(SpeedupProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsReadOnly"/> dependency property.</summary>
        public static readonly DependencyProperty IsReadOnlyProperty
            = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(NumericUpDown),
                                                      new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnIsReadOnlyPropertyChanged));

        private static void OnIsReadOnlyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool isReadOnly && dependencyObject is NumericUpDown numericUpDown)
            {
                numericUpDown.ToggleReadOnlyMode(isReadOnly);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the text can be changed by the use of the up or down buttons only.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty
            = DependencyProperty.Register(nameof(StringFormat),
                                          typeof(string),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(string.Empty, OnStringFormatPropertyChanged, CoerceStringFormat));

        private static void OnStringFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && d is NumericUpDown numericUpDown)
            {
                if (numericUpDown._valueTextBox != null && numericUpDown.Value.HasValue)
                {
                    numericUpDown.InternalSetText(numericUpDown.Value);
                }

                var isMatch = e.NewValue is string format && !string.IsNullOrEmpty(format) && StringFormatHexadecimalRegex().IsMatch(format);

                if (isMatch)
                {
                    numericUpDown.SetCurrentValue(ParsingNumberStyleProperty, NumberStyles.HexNumber);
                    numericUpDown.SetCurrentValue(NumericInputModeProperty, numericUpDown.NumericInputMode | NumericInput.Decimal);
                }
            }
        }

        private static object CoerceStringFormat(DependencyObject d, object? baseValue) => baseValue ?? string.Empty;

        /// <summary>
        /// Gets or sets the formatting for the displaying <see cref="Value" />
        /// </summary>
        /// <remarks>
        /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings"></see>
        /// </remarks>
        [Category("Common")]
        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="InterceptArrowKeys"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptArrowKeysProperty
            = DependencyProperty.Register(nameof(InterceptArrowKeys),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the arrow keys <see cref="Key.Up"/> and <see cref="Key.Down"/> to change the value. 
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptArrowKeys
        {
            get => (bool)GetValue(InterceptArrowKeysProperty);
            set => SetValue(InterceptArrowKeysProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="InterceptMouseWheel"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptMouseWheelProperty
            = DependencyProperty.Register(nameof(InterceptMouseWheel),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets a value indicating whether the user can use the mouse wheel to change the value.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptMouseWheel
        {
            get => (bool)GetValue(InterceptMouseWheelProperty);
            set => SetValue(InterceptMouseWheelProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="InterceptManualEnter"/> dependency property.</summary>
        public static readonly DependencyProperty InterceptManualEnterProperty
            = DependencyProperty.Register(nameof(InterceptManualEnter),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.TrueBox, OnInterceptManualEnterPropertyChanged));

        private static void OnInterceptManualEnterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && dependencyObject is NumericUpDown numericUpDown)
            {
                numericUpDown.ToggleReadOnlyMode(numericUpDown.IsReadOnly);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can enter text in the control.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool InterceptManualEnter
        {
            get => (bool)GetValue(InterceptManualEnterProperty);
            set => SetValue(InterceptManualEnterProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(double?),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(default(double?),
                                                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                        OnValuePropertyChanged,
                                                                        (o, value) => CoerceValue(o, value).Item1));

        private static void OnValuePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                (dependencyObject as NumericUpDown)?.OnValueChanged((double?)e.OldValue, (double?)e.NewValue);
            }
        }

        private static Tuple<double?, bool> CoerceValue(DependencyObject d, object? baseValue)
        {
            var numericUpDown = (NumericUpDown)d;
            if (baseValue is null)
            {
                return new Tuple<double?, bool>(numericUpDown.DefaultValue, false);
            }

            var value = ((double?)baseValue).Value;

            if (!numericUpDown.NumericInputMode.HasFlag(NumericInput.Decimal))
            {
                value = Math.Truncate(value);
            }

            return new Tuple<double?, bool>(value, false);
        }

        /// <summary>
        /// Gets or sets the value of the NumericUpDown.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? Value
        {
            get => (double?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>Identifies the <see cref="DefaultValue"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultValueProperty
            = DependencyProperty.Register(nameof(DefaultValue),
                                          typeof(double?),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(null, OnDefaultValuePropertyChanged, CoerceDefaultValue));

        private static void OnDefaultValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            if (!numericUpDown.Value.HasValue && numericUpDown.DefaultValue.HasValue)
            {
                numericUpDown.SetValueTo(numericUpDown.DefaultValue.Value);
            }
        }

        private static object? CoerceDefaultValue(DependencyObject d, object? baseValue)
        {
            if (baseValue is double val)
            {
                var minimum = ((NumericUpDown)d).Minimum;
                var maximum = ((NumericUpDown)d).Maximum;

                if (val < minimum)
                {
                    return minimum;
                }
                else if (val > maximum)
                {
                    return maximum;
                }
            }

            return baseValue;
        }

        /// <summary>
        /// Gets or sets the default value of the NumericUpDown which will be used if the <see cref="Value"/> is <see langword="null"/>.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(null)]
        public double? DefaultValue
        {
            get => (double?)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(nameof(Minimum),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(double.MinValue, OnMinimumPropertyChanged));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(MaximumProperty);
            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.CoerceValue(DefaultValueProperty);
            numericUpDown.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        /// <summary>
        /// Minimum restricts the minimum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MinValue)]
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(nameof(Maximum),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(double.MaxValue, OnMaximumPropertyChanged, CoerceMaximum));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown)d;

            numericUpDown.CoerceValue(ValueProperty);
            numericUpDown.CoerceValue(DefaultValueProperty);
            numericUpDown.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
            numericUpDown.EnableDisableUpDown();
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            var minimum = ((NumericUpDown)d).Minimum;
            var val = (double)value;
            return val < minimum ? minimum : val;
        }

        /// <summary>
        /// Maximum restricts the maximum value of the Value property.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(double.MaxValue)]
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(DefaultInterval, OnIntervalPropertyChanged));

        private static void OnIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as NumericUpDown)?.ResetInternal();

        /// <summary>
        /// Gets or sets the interval value for increasing/decreasing the <see cref="Value" /> .
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(DefaultInterval)]
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="TrackMouseWheelWhenMouseOver"/> dependency property.</summary>
        public static readonly DependencyProperty TrackMouseWheelWhenMouseOverProperty
            = DependencyProperty.Register(nameof(TrackMouseWheelWhenMouseOver),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the control must have the focus in order to change values using the mouse wheel.
        /// </summary>
        /// <remarks>
        /// If the value is true then the value changes when the mouse wheel is over the control. <br/>
        /// If the value is false then the value changes only if the control has the focus. <br/>
        /// If <see cref="InterceptMouseWheel"/> is set to "false" then this property has no effect.
        /// </remarks>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool TrackMouseWheelWhenMouseOver
        {
            get => (bool)GetValue(TrackMouseWheelWhenMouseOverProperty);
            set => SetValue(TrackMouseWheelWhenMouseOverProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="SpinButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SpinButtonStyleProperty
            = DependencyProperty.Register(nameof(SpinButtonStyle),
                                          typeof(Style),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="FrameworkElement.Style"/> for the spin buttons.
        /// </summary>
        public Style? SpinButtonStyle
        {
            get => (Style?)GetValue(SpinButtonStyleProperty);
            set => SetValue(SpinButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty
            = DependencyProperty.Register(nameof(ButtonsAlignment),
                                          typeof(ButtonsAlignment),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The ButtonsAlignment property specifies horizontal alignment of the up/down buttons.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(ButtonsAlignment.Right)]
        public ButtonsAlignment ButtonsAlignment
        {
            get => (ButtonsAlignment)GetValue(ButtonsAlignmentProperty);
            set => SetValue(ButtonsAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="HideUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty HideUpDownButtonsProperty
            = DependencyProperty.Register(nameof(HideUpDownButtons),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down button of the control are visible.
        /// </summary>
        /// <remarks>
        /// If the value is false then the <see cref="Value" /> of the control can be changed only if one of the following cases is satisfied:
        /// <list type="bullet">
        ///     <item>
        ///         <description><see cref="InterceptArrowKeys" /> is true.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="InterceptMouseWheel" /> is true.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="InterceptManualEnter" /> is true.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool HideUpDownButtons
        {
            get => (bool)GetValue(HideUpDownButtonsProperty);
            set => SetValue(HideUpDownButtonsProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty
            = DependencyProperty.Register(nameof(UpDownButtonsWidth),
                                          typeof(double),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(20d));

        /// <summary>
        /// Gets or sets the width of the up/down buttons.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(20d)]
        public double UpDownButtonsWidth
        {
            get => (double)GetValue(UpDownButtonsWidthProperty);
            set => SetValue(UpDownButtonsWidthProperty, value);
        }

        /// <summary>Identifies the <see cref="UpDownButtonsFocusable"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsFocusableProperty
            = DependencyProperty.Register(nameof(UpDownButtonsFocusable),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the up and down buttons will got the focus when using them.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool UpDownButtonsFocusable
        {
            get => (bool)GetValue(UpDownButtonsFocusableProperty);
            set => SetValue(UpDownButtonsFocusableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="SwitchUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty SwitchUpDownButtonsProperty
            = DependencyProperty.Register(nameof(SwitchUpDownButtons),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down buttons will be switched.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool SwitchUpDownButtons
        {
            get => (bool)GetValue(SwitchUpDownButtonsProperty);
            set => SetValue(SwitchUpDownButtonsProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ButtonUpContent"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentProperty
            = DependencyProperty.Register(nameof(ButtonUpContent),
                                          typeof(object),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Provides the object content that should be displayed on the Up Button.
        /// </summary>
        public object? ButtonUpContent
        {
            get => (object?)GetValue(ButtonUpContentProperty);
            set => SetValue(ButtonUpContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonUpContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentTemplateProperty
            = DependencyProperty.Register(nameof(ButtonUpContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(NumericUpDown));

        /// <summary>
        /// Gets or sets the DataTemplate used to display the Up button's content.
        /// </summary>
        public DataTemplate? ButtonUpContentTemplate
        {
            get => (DataTemplate?)GetValue(ButtonUpContentTemplateProperty);
            set => SetValue(ButtonUpContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonUpContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonUpContentStringFormatProperty
            = DependencyProperty.Register(nameof(ButtonUpContentStringFormat),
                                          typeof(string),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the ButtonUpContent property if it is displayed as a string.
        /// </summary>
        /// <remarks> 
        /// This property is ignored if <seealso cref="ButtonUpContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        public string? ButtonUpContentStringFormat
        {
            get => (string?)GetValue(ButtonUpContentStringFormatProperty);
            set => SetValue(ButtonUpContentStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContent"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentProperty
            = DependencyProperty.Register(nameof(ButtonDownContent),
                                          typeof(object),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Provides the object content that should be displayed on the Down Button.
        /// </summary>
        public object? ButtonDownContent
        {
            get => (object?)GetValue(ButtonDownContentProperty);
            set => SetValue(ButtonDownContentProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentTemplateProperty
            = DependencyProperty.Register(nameof(ButtonDownContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(NumericUpDown));

        /// <summary>
        /// Gets or sets the DataTemplate used to display the Down button's content.
        /// </summary>
        public DataTemplate? ButtonDownContentTemplate
        {
            get => (DataTemplate?)GetValue(ButtonDownContentTemplateProperty);
            set => SetValue(ButtonDownContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonDownContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonDownContentStringFormatProperty
            = DependencyProperty.Register(nameof(ButtonDownContentStringFormat),
                                          typeof(string),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the ButtonDownContent property if it is displayed as a string.
        /// </summary>
        /// <remarks> 
        /// This property is ignored if <seealso cref="ButtonDownContentTemplate"/> is set.
        /// </remarks>
        [Bindable(true)]
        public string? ButtonDownContentStringFormat
        {
            get => (string?)GetValue(ButtonDownContentStringFormatProperty);
            set => SetValue(ButtonDownContentStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="Culture"/> dependency property.</summary>
        public static readonly DependencyProperty CultureProperty
            = DependencyProperty.Register(nameof(Culture),
                                          typeof(CultureInfo),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(null, OnCulturePropertyChanged));

        private static void OnCulturePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && d is NumericUpDown numericUpDown)
            {
                numericUpDown._regexNumber = null;
                numericUpDown.OnValueChanged(numericUpDown.Value, numericUpDown.Value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the culture to be used in string formatting and converting operations.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(null)]
        public CultureInfo? Culture
        {
            get => (CultureInfo?)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

        /// <summary>Identifies the <see cref="NumericInputMode"/> dependency property.</summary>
        public static readonly DependencyProperty NumericInputModeProperty
            = DependencyProperty.Register(nameof(NumericInputMode),
                                          typeof(NumericInput),
                                          typeof(NumericUpDown),
                                          new FrameworkPropertyMetadata(NumericInput.All, OnNumericInputModePropertyChanged));

        private static void OnNumericInputModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is NumericInput numericInput && d is NumericUpDown numericUpDown && numericUpDown.Value != null && !numericInput.HasFlag(NumericInput.Decimal))
            {
                numericUpDown.Value = Math.Truncate(numericUpDown.Value.GetValueOrDefault());
            }
        }

        /// <summary>
        /// Gets or sets which numeric input for this NumericUpDown is allowed.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumericInput.All)]
        public NumericInput NumericInputMode
        {
            get => (NumericInput)GetValue(NumericInputModeProperty);
            set => SetValue(NumericInputModeProperty, value);
        }

        /// <summary>Identifies the <see cref="DecimalPointCorrection"/> dependency property.</summary>
        public static readonly DependencyProperty DecimalPointCorrectionProperty
            = DependencyProperty.Register(nameof(DecimalPointCorrection),
                                          typeof(DecimalPointCorrectionMode),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(default(DecimalPointCorrectionMode)));

        /// <summary>
        /// Gets or sets the decimal-point correction mode. The default is <see cref="DecimalPointCorrectionMode.Inherits"/>
        /// </summary>
        public DecimalPointCorrectionMode DecimalPointCorrection
        {
            get => (DecimalPointCorrectionMode)GetValue(DecimalPointCorrectionProperty);
            set => SetValue(DecimalPointCorrectionProperty, value);
        }

        /// <summary>Identifies the <see cref="SnapToMultipleOfInterval"/> dependency property.</summary>
        public static readonly DependencyProperty SnapToMultipleOfIntervalProperty
            = DependencyProperty.Register(nameof(SnapToMultipleOfInterval),
                                          typeof(bool),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(BooleanBoxes.FalseBox, OnSnapToMultipleOfIntervalPropertyChanged));

        private static void OnSnapToMultipleOfIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && e.NewValue is bool snap && d is NumericUpDown numericUpDown)
            {
                if (!snap)
                {
                    return;
                }

                if (Math.Abs(numericUpDown.Interval) > 0)
                {
                    var value = numericUpDown.Value.GetValueOrDefault();
                    numericUpDown.Value = Math.Round(value / numericUpDown.Interval) * numericUpDown.Interval;
                }
            }
        }

        /// <summary>
        /// Indicates if the NumericUpDown should round the value to the nearest possible interval when the focus moves to another element.
        /// </summary>
        [Bindable(true)]
        [Category("Common")]
        [DefaultValue(false)]
        public bool SnapToMultipleOfInterval
        {
            get => (bool)GetValue(SnapToMultipleOfIntervalProperty);
            set => SetValue(SnapToMultipleOfIntervalProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="ParsingNumberStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ParsingNumberStyleProperty
            = DependencyProperty.Register(nameof(ParsingNumberStyle),
                                          typeof(NumberStyles),
                                          typeof(NumericUpDown),
                                          new PropertyMetadata(NumberStyles.Any));

        /// <summary>
        /// Gets or sets the parsing number style for the value from text to numeric value.
        /// </summary>
        [Category("Common")]
        [DefaultValue(NumberStyles.Any)]
        public NumberStyles ParsingNumberStyle
        {
            get => (NumberStyles)GetValue(ParsingNumberStyleProperty);
            set => SetValue(ParsingNumberStyleProperty, value);
        }

        private CultureInfo SpecificCultureInfo => Culture ?? Language.GetSpecificCulture();

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));

            VerticalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(VerticalAlignment.Center));
            HorizontalContentAlignmentProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(HorizontalAlignment.Right));

            EventManager.RegisterClassHandler(typeof(NumericUpDown), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        /// <summary> 
        ///     Called when this element or any below gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When NumericUpDown gets logical focus, select the text inside us.
            // If we're an editable NumericUpDown, forward focus to the TextBox element
            if (!e.Handled)
            {
                var numericUpDown = (NumericUpDown)sender;
                if ((numericUpDown.InterceptManualEnter || numericUpDown.IsReadOnly) && numericUpDown.Focusable && e.OriginalSource == numericUpDown)
                {
                    // MoveFocus takes a TraversalRequest as its argument.
                    var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
                    // Gets the element with keyboard focus.
                    // And change the keyboard focus.
                    if (Keyboard.FocusedElement is UIElement elementWithFocus)
                    {
                        elementWithFocus.MoveFocus(request);
                    }
                    else
                    {
                        numericUpDown.Focus();
                    }

                    e.Handled = true;
                }
            }
        }

        /// <summary>
        ///     When overridden in a derived class, is invoked whenever application code or internal processes call
        ///     <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _repeatUp = GetTemplateChild(PART_NumericUp) as RepeatButton;
            _repeatDown = GetTemplateChild(PART_NumericDown) as RepeatButton;

            _valueTextBox = GetTemplateChild(PART_TextBox) as TextBox;

            if (_repeatUp is null || _repeatDown is null || _valueTextBox is null)
            {
                throw new InvalidOperationException($"You have missed to specify {PART_NumericUp}, {PART_NumericDown} or {PART_TextBox} in your template!");
            }

            ToggleReadOnlyMode(IsReadOnly);

            _repeatUp.Click += (_, _) => ChangeValueWithSpeedUp(true);
            _repeatDown.Click += (_, _) => ChangeValueWithSpeedUp(false);

            _repeatUp.PreviewMouseUp += (_, _) => ResetInternal();
            _repeatDown.PreviewMouseUp += (_, _) => ResetInternal();

            OnValueChanged(Value, Value);

            _scrollViewer = null;
        }

        private void ToggleReadOnlyMode(bool isReadOnly)
        {
            if (_repeatUp is null || _repeatDown is null || _valueTextBox is null)
            {
                return;
            }

            if (isReadOnly)
            {
                _valueTextBox.LostFocus -= OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput -= OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown -= OnTextBoxKeyDown;
                _valueTextBox.TextChanged -= OnTextChanged;
                DataObject.RemovePastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
            else
            {
                _valueTextBox.LostFocus += OnTextBoxLostFocus;
                _valueTextBox.PreviewTextInput += OnPreviewTextInput;
                _valueTextBox.PreviewKeyDown += OnTextBoxKeyDown;
                _valueTextBox.TextChanged += OnTextChanged;
                DataObject.AddPastingHandler(_valueTextBox, OnValueTextBoxPaste);
            }
        }

        public void SelectAll() => _valueTextBox?.SelectAll();

        private void RaiseChangeDelay() => RaiseEvent(new RoutedEventArgs(DelayChangedEvent));

        /// <summary>
        /// This method is invoked when the Delay property changes.
        /// </summary>
        /// <param name="oldDelay">The old value of the Delay property.</param>
        /// <param name="newDelay">The new value of the Delay property.</param>
        protected virtual void OnDelayChanged(int oldDelay, int newDelay)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Speedup property changes.
        /// </summary>
        /// <param name="oldSpeedup">The old value of the Speedup property.</param>
        /// <param name="newSpeedup">The new value of the Speedup property.</param>
        protected virtual void OnSpeedupChanged(bool oldSpeedup, bool newSpeedup)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Maximum property changes.
        /// </summary>
        /// <param name="oldMaximum">The old value of the Maximum property.</param>
        /// <param name="newMaximum">The new value of the Maximum property.</param>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            // nothing here
        }

        /// <summary>
        /// This method is invoked when the Minimum property changes.
        /// </summary>
        /// <param name="oldMinimum">The old value of the Minimum property.</param>
        /// <param name="newMinimum">The new value of the Minimum property.</param>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            // nothing here
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!InterceptArrowKeys)
            {
                return;
            }

            if (e.Key == Key.Up)
            {
                ChangeValueWithSpeedUp(true);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                ChangeValueWithSpeedUp(false);
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            if (e.Key == Key.Down ||
                e.Key == Key.Up)
            {
                ResetInternal();
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (InterceptMouseWheel && (IsFocused || _valueTextBox?.IsFocused == true || TrackMouseWheelWhenMouseOver))
            {
                var increment = e.Delta > 0;
                ChangeValueWithSpeedUp(increment);
            }

            var sv = TryFindScrollViewer();

            if (sv != null && _handlesMouseWheelScrolling.Value is not null)
            {
                if (TrackMouseWheelWhenMouseOver)
                {
                    _handlesMouseWheelScrolling.Value.SetValue(sv, true, null);
                }
                else if (InterceptMouseWheel)
                {
                    _handlesMouseWheelScrolling.Value.SetValue(sv, _valueTextBox?.IsFocused == true, null);
                }
                else
                {
                    _handlesMouseWheelScrolling.Value.SetValue(sv, true, null);
                }
            }
        }

        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            var fullText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength).Insert(textBox.CaretIndex, e.Text);
            var textIsValid = ValidateText(fullText, out var convertedValue);
            // Value must be valid and not coerced
            e.Handled = !textIsValid || CoerceValue(this, convertedValue as double?).Item2;
            _manualChange = !e.Handled;
        }

        /// <summary>
        ///     Raises the <see cref="ValueChanged" /> routed event.
        /// </summary>
        /// <param name="oldValue">
        ///     Old value of the <see cref="Value" /> property
        /// </param>
        /// <param name="newValue">
        ///     New value of the <see cref="Value" /> property
        /// </param>
        protected virtual void OnValueChanged(double? oldValue, double? newValue)
        {
            if (!_manualChange)
            {
                if (!newValue.HasValue)
                {
                    if (_valueTextBox != null)
                    {
                        _valueTextBox.Text = null;
                    }

                    if (oldValue != newValue)
                    {
                        RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
                    }

                    return;
                }

                if (_repeatUp != null && !_repeatUp.IsEnabled)
                {
                    _repeatUp.IsEnabled = true;
                }

                if (_repeatDown != null && !_repeatDown.IsEnabled)
                {
                    _repeatDown.IsEnabled = true;
                }

                if (newValue <= Minimum)
                {
                    if (_repeatDown != null)
                    {
                        _repeatDown.IsEnabled = false;
                    }

                    ResetInternal();

                    if (IsLoaded)
                    {
                        RaiseEvent(new RoutedEventArgs(MinimumReachedEvent));
                    }
                }

                if (newValue >= Maximum)
                {
                    if (_repeatUp != null)
                    {
                        _repeatUp.IsEnabled = false;
                    }

                    ResetInternal();
                    if (IsLoaded)
                    {
                        RaiseEvent(new RoutedEventArgs(MaximumReachedEvent));
                    }
                }

                if (_valueTextBox != null)
                {
                    InternalSetText(newValue);
                }
            }

            if (oldValue != newValue)
            {
                RaiseEvent(new RoutedPropertyChangedEventArgs<double?>(oldValue, newValue, ValueChangedEvent));
            }
        }

        private void InternalSetText(double? newValue)
        {
            if (!newValue.HasValue)
            {
                if (_valueTextBox is not null)
                {
                    _valueTextBox.Text = null;
                }

                return;
            }

            if (_valueTextBox is not null)
            {
                _valueTextBox.Text = FormattedValueString(newValue.Value, StringFormat, SpecificCultureInfo);
            }
        }

        private static string? FormattedValueString(double newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format))
            {
                if (TryFormatHexadecimal(newValue, format, culture, out var hexValue))
                {
                    return hexValue;
                }
                else
                {
                    var match = StringFormatRegex().Match(format);
                    if (match.Success)
                    {
                        // we have a format template such as "{0:N0}"
                        return string.Format(culture, format, newValue);
                    }

                    // we have a format such as "N0"
                    return newValue.ToString(format, culture);
                }
            }

            return newValue.ToString(culture);
        }

        private static double FormattedValue(double newValue, string format, CultureInfo culture)
        {
            format = format.Replace("{}", string.Empty);
            if (!string.IsNullOrWhiteSpace(format) && !TryFormatHexadecimal(newValue, format, culture, out _))
            {
                var match = StringFormatRegex().Match(format);
                if (match.Success)
                {
                    // we have a format template such as "{0:N0}"
                    return ConvertStringFormatValue(newValue, match.Groups["format"].Value);
                }

                // we have a format such as "N0"
                return ConvertStringFormatValue(newValue, format);
            }

            return newValue;
        }

        private static double ConvertStringFormatValue(double value, string format)
        {
            if (format.ToUpperInvariant().Contains('P') || format.Contains('%'))
            {
                value /= 100d;
            }
            else if (format.Contains('‰'))
            {
                value /= 1000d;
            }

            return value;
        }

        private static bool TryFormatHexadecimal(double newValue, string format, CultureInfo culture, [NotNullWhen(true)] out string? output)
        {
            var match = StringFormatHexadecimalRegex().Match(format);
            if (match.Success)
            {
                if (match.Groups["simpleHEX"].Success)
                {
                    // HEX DOES SUPPORT INT ONLY.
                    output = ((int)newValue).ToString(match.Groups["simpleHEX"].Value, culture);
                    return true;
                }

                if (match.Groups["complexHEX"].Success)
                {
                    output = string.Format(culture, match.Groups["complexHEX"].Value, (int)newValue);
                    return true;
                }
            }

            output = null;
            return false;
        }

        [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Assumed")]
        private ScrollViewer? TryFindScrollViewer()
        {
            if (_scrollViewer != null)
            {
                return _scrollViewer;
            }

            _valueTextBox?.ApplyTemplate();

            _scrollViewer = _valueTextBox?.Template.FindName(PART_ContentHost, _valueTextBox) as ScrollViewer;
            if (_scrollViewer != null)
            {
                _handlesMouseWheelScrolling = new Lazy<PropertyInfo?>(() => _scrollViewer.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).SingleOrDefault(i => i.Name == "HandlesMouseWheelScrolling"));
            }

            return _scrollViewer;
        }

        private void ChangeValueWithSpeedUp(bool toPositive)
        {
            if (IsReadOnly)
            {
                return;
            }

            double direction = toPositive ? 1 : -1;

            if (Speedup)
            {
                var d = Interval * _internalLargeChange;
                if ((_intervalValueSinceReset += Interval * _internalIntervalMultiplierForCalculation) > d)
                {
                    _internalLargeChange *= 10;
                    _internalIntervalMultiplierForCalculation *= 10;
                }

                ChangeValueInternal(direction * _internalIntervalMultiplierForCalculation);
            }
            else
            {
                ChangeValueInternal(direction * Interval);
            }
        }

        private void ChangeValueInternal(double interval)
        {
            if (IsReadOnly)
            {
                return;
            }

            _manualChange = false;

            var routedEvent = interval > 0 ? new NumericUpDownChangedRoutedEventArgs(ValueIncrementedEvent, interval) : new NumericUpDownChangedRoutedEventArgs(ValueDecrementedEvent, interval);

            RaiseEvent(routedEvent);

            if (!routedEvent.Handled)
            {
                ChangeValueBy(routedEvent.Interval);

                InternalSetText(Value);

                if (_valueTextBox is not null)
                {
                    _valueTextBox.CaretIndex = _valueTextBox.Text.Length;
                }
            }
        }

        private void ChangeValueBy(double difference)
        {
            var newValue = Value.GetValueOrDefault() + difference;
            SetValueTo(newValue);
        }

        private void SetValueTo(double newValue)
        {
            var value = newValue;

            if (SnapToMultipleOfInterval && Math.Abs(Interval) > 0)
            {
                value = Math.Round(newValue / Interval) * Interval;
            }

            if (value > Maximum)
            {
                value = Maximum;
            }
            else if (value < Minimum)
            {
                value = Minimum;
            }

            SetCurrentValue(ValueProperty, CoerceValue(this, value).Item1);
        }

        private void EnableDisableDown()
        {
            if (_repeatDown != null)
            {
                _repeatDown.IsEnabled = Value is null || Value > Minimum;
            }
        }

        private void EnableDisableUp()
        {
            if (_repeatUp != null)
            {
                _repeatUp.IsEnabled = Value is null || Value < Maximum;
            }
        }

        private void EnableDisableUpDown()
        {
            EnableDisableUp();
            EnableDisableDown();
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _manualChange = _manualChange
                                || e.Key == Key.Back
                                || e.Key == Key.Delete
                                || e.Key == Key.Decimal
                                || e.Key == Key.OemComma
                                || e.Key == Key.OemPeriod;

            // Filter the Numpad's decimal-point key only
            if (e.Key == Key.Decimal && DecimalPointCorrection != DecimalPointCorrectionMode.Inherits)
            {
                // Mark the event as handled, so no further action will take place
                e.Handled = true;

                // Grab the originating TextBox control...
                var textBox = (TextBoxBase)sender;

                // The current correction mode...
                var correctionMode = DecimalPointCorrection;

                // And the culture of the NUD
                var culture = SpecificCultureInfo;

                // Surrogate the blocked key pressed
                SimulateDecimalPointKeyPressAsync(textBox, correctionMode, culture);
            }
        }

        /// <summary>
        /// Insertion of the proper decimal-point as part of the TextBox content
        /// </summary>
        /// <param name="textBox">The TextBox which will be used for the correction</param>
        /// <param name="mode">The decimal correction mode.</param>
        /// <param name="culture">The culture with the decimal-point information.</param>
        /// <remarks>
        /// Typical "async-void" pattern as "fire-and-forget" behavior.
        /// </remarks>
        private static void SimulateDecimalPointKeyPressAsync(TextBoxBase textBox, DecimalPointCorrectionMode mode, CultureInfo culture)
        {
            // Select the proper decimal-point string upon the context
            var replace = mode switch
            {
                DecimalPointCorrectionMode.Number => culture.NumberFormat.NumberDecimalSeparator,
                DecimalPointCorrectionMode.Currency => culture.NumberFormat.CurrencyDecimalSeparator,
                DecimalPointCorrectionMode.Percent => culture.NumberFormat.PercentDecimalSeparator,
                _ => null,
            };
            if (!string.IsNullOrEmpty(replace))
            {
                // Insert the desired string
                var tc = new TextComposition(InputManager.Current, textBox, replace);

                TextCompositionManager.StartComposition(tc);
            }
        }

        private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (_valueTextBox is not null)
            {
                _manualChange = false;
                _valueTextBox.TextChanged -= OnTextChanged;
                InternalSetText(Value);
                _valueTextBox.TextChanged += OnTextChanged;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            ChangeValueFromTextInput(text);
        }

        private void ChangeValueFromTextInput(string text)
        {
            if (!InterceptManualEnter)
            {
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                if (DefaultValue.HasValue)
                {
                    SetValueTo(DefaultValue.Value);
                    if (!_manualChange)
                    {
                        InternalSetText(DefaultValue.Value);
                    }
                }
                else
                {
                    SetCurrentValue(ValueProperty, null);
                }
            }
            else if (_manualChange)
            {
                if (ValidateText(text, out var convertedValue))
                {
                    convertedValue = FormattedValue(convertedValue, StringFormat, SpecificCultureInfo);
                    SetValueTo(convertedValue);
                }
                else if (DefaultValue.HasValue)
                {
                    SetValueTo(DefaultValue.Value);
                    InternalSetText(Value);
                }
                else
                {
                    SetCurrentValue(ValueProperty, null);
                }
            }

            OnValueChanged(Value, Value);

            _manualChange = false;
        }

        private void OnValueTextBoxPaste(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (TextBox)sender;
            var textPresent = textBox.Text;

            var isText = e.SourceDataObject.GetDataPresent(DataFormats.Text, true);
            if (!isText)
            {
                e.CancelCommand();
                return;
            }

            var text = e.SourceDataObject.GetData(DataFormats.Text) as string;

            var newText = string.Concat(textPresent.Substring(0, textBox.SelectionStart), text, textPresent.Substring(textBox.SelectionStart + textBox.SelectionLength));
            if (!ValidateText(newText, out _))
            {
                e.CancelCommand();
            }
            else
            {
                _manualChange = true;
            }
        }

        private void ResetInternal()
        {
            if (IsReadOnly)
            {
                return;
            }

            _internalLargeChange = 100 * Interval;
            _internalIntervalMultiplierForCalculation = Interval;
            _intervalValueSinceReset = 0;
        }

        private bool ValidateText(string text, out double convertedValue)
        {
            convertedValue = 0d;

            if (text == SpecificCultureInfo.NumberFormat.PositiveSign
                || text == SpecificCultureInfo.NumberFormat.NegativeSign)
            {
                return true;
            }

            if (text.Count(c => c == SpecificCultureInfo.NumberFormat.PositiveSign[0]) > 2
                || text.Count(c => c == SpecificCultureInfo.NumberFormat.NegativeSign[0]) > 2
               // || text.Count(c => c == this.SpecificCultureInfo.NumberFormat.NumberGroupSeparator[0]) > 1
               )
            {
                return false;
            }

            var isNumeric = NumericInputMode == NumericInput.Numbers
                            || ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier)
                            || ParsingNumberStyle == NumberStyles.HexNumber
                            || ParsingNumberStyle == NumberStyles.Integer
                            || ParsingNumberStyle == NumberStyles.Number;

            var isHex = NumericInputMode == NumericInput.Numbers
                        || ParsingNumberStyle.HasFlag(NumberStyles.AllowHexSpecifier)
                        || ParsingNumberStyle == NumberStyles.HexNumber;

            var number = TryGetNumberFromText(text, isHex);

            // If we are only accepting numbers then attempt to parse as an integer.
            return isNumeric
                ? ConvertNumber(number, out convertedValue)
                : number == SpecificCultureInfo.NumberFormat.NumberDecimalSeparator
                || number == SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator
                || number == SpecificCultureInfo.NumberFormat.PercentDecimalSeparator
                || number == SpecificCultureInfo.NumberFormat.NegativeSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator
                || number == SpecificCultureInfo.NumberFormat.PositiveSign + SpecificCultureInfo.NumberFormat.NumberDecimalSeparator
                || double.TryParse(number, ParsingNumberStyle, SpecificCultureInfo, out convertedValue);
        }

        private bool ConvertNumber(string text, out double convertedValue)
        {
            if (text.Any(c => c == SpecificCultureInfo.NumberFormat.NumberDecimalSeparator[0]
                              || c == SpecificCultureInfo.NumberFormat.PercentDecimalSeparator[0]
                              || c == SpecificCultureInfo.NumberFormat.CurrencyDecimalSeparator[0]))
            {
                convertedValue = 0d;
                return false;
            }

            if (!long.TryParse(text, ParsingNumberStyle, SpecificCultureInfo, out var convertedInt))
            {
                convertedValue = convertedInt;
                return false;
            }

            convertedValue = convertedInt;
            return true;
        }

        private string TryGetNumberFromText(string text, bool isHex)
        {
            if (isHex)
            {
                var hexMatches = HexadecimalRegex().Matches(text);
                return hexMatches.Count > 0 ? hexMatches[0].Value : text;
            }

            _regexNumber ??= new Regex(RawRegexNumberString.Replace("<DecimalSeparator>", SpecificCultureInfo.NumberFormat.NumberDecimalSeparator)
                                                                 .Replace("<GroupSeparator>", SpecificCultureInfo.NumberFormat.NumberGroupSeparator),
                                             RegexOptions.Compiled);

            var matches = _regexNumber.Matches(text);
            return matches.Count > 0 ? matches[0].Value : text;
        }
    }
}
