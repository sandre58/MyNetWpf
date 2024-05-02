// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Media;
using MyNet.Wpf.Resources;
using MyNet.Utilities;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.Controls
{
    public enum ColorDisplayNameMode
    {
        Name,

        Hexa,

        NameAndHexa
    }

    [DefaultEvent("SelectedColorChanged")]
    public class ColorPickerBase : Control
    {
        protected bool _colorIsUpdating;
        protected bool _updateHsvValues = true;

        public ColorPickerBase() => CultureInfoService.Current.CultureChanged += (sender, e) => UpdateColorName(SelectedColor);

        #region A

        public static readonly DependencyProperty DisplayNameModeProperty
            = DependencyProperty.Register(nameof(DisplayNameMode),
                                          typeof(ColorDisplayNameMode),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(ColorDisplayNameMode.Hexa));

        public ColorDisplayNameMode DisplayNameMode
        {
            get => (ColorDisplayNameMode)GetValue(DisplayNameModeProperty);
            set => SetValue(DisplayNameModeProperty, value);
        }

        #endregion

        #region SelectedColor

        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty
            = DependencyProperty.Register(nameof(SelectedColor),
                                          typeof(Color?),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged, CoerceSelectedColorProperty));

        /// <summary>
        /// Gets or sets the selected <see cref="Color"/>.
        /// </summary>
        [TypeConverter(typeof(ColorConverter))]
        public Color? SelectedColor
        {
            get => (Color?)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && e.OldValue != e.NewValue && !colorPicker._colorIsUpdating)
            {
                colorPicker._colorIsUpdating = true;
                try
                {
                    colorPicker.OnSelectedColorChanged(e.OldValue as Color?, e.NewValue as Color? ?? colorPicker.DefaultColor);
                }
                finally
                {
                    colorPicker._colorIsUpdating = false;
                }
            }
        }

        private static object? CoerceSelectedColorProperty(DependencyObject dependencyObject, object? basevalue)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                basevalue ??= colorPicker.DefaultColor;
            }

            return basevalue;
        }

        /// <summary>Identifies the <see cref="SelectedColorChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectedColorChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(SelectedColorChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<Color?>),
                                               typeof(ColorPickerBase));

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, value);
            remove => RemoveHandler(SelectedColorChangedEvent, value);
        }

        #endregion

        #region DefaultColor

        /// <summary>Identifies the <see cref="DefaultColor"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultColorProperty
            = DependencyProperty.Register(nameof(DefaultColor),
                                          typeof(Color?),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(null, OnDefaultColorPropertyChanged));

        private static void OnDefaultColorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && e.OldValue != e.NewValue)
            {
                colorPicker.SetCurrentValue(SelectedColorProperty, e.NewValue ?? colorPicker.SelectedColor);
            }
        }

        /// <summary>
        /// Gets or sets a default selected <see cref="Color"/>
        /// </summary>
        public Color? DefaultColor
        {
            get => (Color?)GetValue(DefaultColorProperty);
            set => SetValue(DefaultColorProperty, value);
        }

        #endregion

        #region SelectedHSVColor

        private static readonly DependencyPropertyKey SelectedHSVColorPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SelectedHSVColor),
                                                  typeof(HSVColor),
                                                  typeof(ColorPickerBase),
                                                  new PropertyMetadata(new HSVColor(Colors.Black)));

        /// <summary>Identifies the <see cref="SelectedHSVColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedHSVColorProperty = SelectedHSVColorPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="SelectedColor"/> as <see cref="HSVColor"/>. This property is read only.
        /// </summary>
        public HSVColor SelectedHSVColor => (HSVColor)GetValue(SelectedHSVColorProperty);

        #endregion

        #region A

        /// <summary>Identifies the <see cref="A"/> dependency property.</summary>
        public static readonly DependencyProperty AProperty
            = DependencyProperty.Register(nameof(A),
                                          typeof(byte),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata((byte)255, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRGBAColorChanged));

        /// <summary>
        /// Gets or sets the Alpha-Channel
        /// </summary>
        public byte A
        {
            get => (byte)GetValue(AProperty);
            set => SetValue(AProperty, value);
        }

        #endregion

        #region R

        /// <summary>Identifies the <see cref="R"/> dependency property.</summary>
        public static readonly DependencyProperty RProperty
            = DependencyProperty.Register(nameof(R),
                                          typeof(byte),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRGBAColorChanged));

        /// <summary>
        /// Gets or sets the Red-Channel
        /// </summary>
        public byte R
        {
            get => (byte)GetValue(RProperty);
            set => SetValue(RProperty, value);
        }

        protected static void OnRGBAColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker._colorIsUpdating)
            {
                colorPicker.SetCurrentValue(SelectedColorProperty, Color.FromArgb(colorPicker.A, colorPicker.R, colorPicker.G, colorPicker.B));
            }
        }

        #endregion

        #region G

        /// <summary>Identifies the <see cref="G"/> dependency property.</summary>
        public static readonly DependencyProperty GProperty
            = DependencyProperty.Register(nameof(G),
                                          typeof(byte),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRGBAColorChanged));

        /// <summary>
        /// Gets or sets the Green-Channel
        /// </summary>
        public byte G
        {
            get => (byte)GetValue(GProperty);
            set => SetValue(GProperty, value);
        }

        #endregion

        #region B

        /// <summary>Identifies the <see cref="B"/> dependency property.</summary>
        public static readonly DependencyProperty BProperty
            = DependencyProperty.Register(nameof(B),
                                          typeof(byte),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata((byte)0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRGBAColorChanged));

        /// <summary>
        /// Gets or sets the Blue-Channel
        /// </summary>
        public byte B
        {
            get => (byte)GetValue(BProperty);
            set => SetValue(BProperty, value);
        }

        #endregion

        #region H

        /// <summary>Identifies the <see cref="Hue"/> dependency property.</summary>
        public static readonly DependencyProperty HueProperty
            = DependencyProperty.Register(nameof(Hue),
                                          typeof(double),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHSVColorChanged));

        /// <summary>
        /// Gets or sets the Hue-Channel
        /// </summary>
        public double Hue
        {
            get => (double)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }

        private static void OnHSVColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker && !colorPicker._colorIsUpdating)
            {
                var hsv = new HSVColor(colorPicker.A / 255d, colorPicker.Hue, colorPicker.Saturation, colorPicker.Value);

                colorPicker._updateHsvValues = false;
                try
                {
                    colorPicker.SetCurrentValue(SelectedColorProperty, hsv.ToColor());
                }
                finally
                {
                    colorPicker._updateHsvValues = true;
                }
            }
        }

        #endregion

        #region S

        /// <summary>Identifies the <see cref="Saturation"/> dependency property.</summary>
        public static readonly DependencyProperty SaturationProperty
            = DependencyProperty.Register(nameof(Saturation),
                                          typeof(double),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHSVColorChanged));

        /// <summary>
        /// Gets or sets the Saturation-Channel
        /// </summary>
        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }

        #endregion

        #region V

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(double),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHSVColorChanged));

        /// <summary>
        /// Gets or sets the Value-Channel
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region Hexa

        private static readonly DependencyPropertyKey HexaPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Hexa),
                                                  typeof(string),
                                                  typeof(ColorPickerBase),
                                                  new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="SelectedHSVColor"/> dependency property.</summary>
        public static readonly DependencyProperty HexaProperty = HexaPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="SelectedColor"/> as string. This property is read only.
        /// </summary>
        public string Hexa => (string)GetValue(HexaProperty);

        #endregion

        #region ColorName

        /// <summary>Identifies the <see cref="ColorName"/> dependency property.</summary>
        public static readonly DependencyProperty ColorNameProperty
            = DependencyProperty.Register(nameof(ColorName),
                                          typeof(string),
                                          typeof(ColorPickerBase),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnColorNamePropertyChanged));

        /// <summary>
        /// Gets or sets the name of the <see cref="SelectedColor"/>.
        /// </summary>
        public string? ColorName
        {
            get => (string?)GetValue(ColorNameProperty);
            set => SetValue(ColorNameProperty, value);
        }

        private static void OnColorNamePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorPickerBase colorPicker)
            {
                colorPicker.OnColorNameChanged(e.OldValue?.ToString(), e.NewValue?.ToString());
            }
        }

        protected virtual void OnColorNameChanged(string? oldColorName, string? colorName)
        {
            if (_colorIsUpdating) return;

            if (string.IsNullOrEmpty(colorName))
            {
                SetCurrentValue(SelectedColorProperty, null);
            }
            else if (colorName.ToColor() is Color color)
            {
                if (SelectedColor != color)
                {
                    SetCurrentValue(SelectedColorProperty, color);
                }
                else // if the color stayed the same we still have to update the displayed name
                {
                    _colorIsUpdating = true;
                    try
                    {
                        UpdateColorName(color);
                        SetValue(HexaPropertyKey, color.ToHex());
                    }
                    finally
                    {
                        _colorIsUpdating = false;
                    }
                }
            }
            else
            {
                OnColorNameFailed(colorName);
            }

            RaiseEvent(new RoutedPropertyChangedEventArgs<string?>(oldColorName, colorName, ColorNameChangedEvent));
        }

        protected virtual void OnColorNameFailed(string? colorName) => throw new FormatException(WpfResources.XIsNotAValidColorError.FormatWith(colorName));

        public static readonly RoutedEvent ColorNameChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(ColorNameChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<string?>),
                                               typeof(ColorPickerBase));

        public event RoutedPropertyChangedEventHandler<string?> ColorNameChanged
        {
            add => AddHandler(ColorNameChangedEvent, value);
            remove => RemoveHandler(ColorNameChangedEvent, value);
        }

        #endregion

        protected virtual void OnSelectedColorChanged(Color? oldValue, Color? newValue)
        {
            UpdateColorName(newValue);
            SetValue(HexaPropertyKey, newValue?.ToHex());

            // We just update the following lines if we have a Color.
            if (newValue != null)
            {
                var color = (Color)newValue;

                if (_updateHsvValues)
                {
                    var hsv = new HSVColor(color);
                    SetCurrentValue(HueProperty, hsv.Hue);
                    SetCurrentValue(SaturationProperty, hsv.Saturation);
                    SetCurrentValue(ValueProperty, hsv.Value);
                }

                SetValue(SelectedHSVColorPropertyKey, new HSVColor(A / 255d, Hue, Saturation, Value));

                SetCurrentValue(AProperty, color.A);
                SetCurrentValue(RProperty, color.R);
                SetCurrentValue(GProperty, color.G);
                SetCurrentValue(BProperty, color.B);
            }

            RaiseEvent(new RoutedPropertyChangedEventArgs<Color?>(oldValue, newValue, SelectedColorChangedEvent));
        }

        protected virtual void UpdateColorName(Color? color)
        {
            if (DisplayNameMode == ColorDisplayNameMode.Hexa)
                SetCurrentValue(ColorNameProperty, color?.ToHex());
            else
            {
                var name = color?.ToName();

                if (DisplayNameMode == ColorDisplayNameMode.Name)
                    SetCurrentValue(ColorNameProperty, name);
                else
                {
                    var hexa = color?.ToHex();
                    SetCurrentValue(ColorNameProperty, name == hexa ? hexa : $"{name} ({hexa})");
                }
            }
        }
    }
}
