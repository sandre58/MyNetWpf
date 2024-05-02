// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DynamicData.Binding;
using MyNet.Wpf.Resources;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    public enum AddToRecentColorsTrigger
    {
        Never,
        ColorPickerClosed,
        SelectedColorChanged
    }

    [TemplatePart(Name = ButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = PopupPartName, Type = typeof(Popup))]
    [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
    [TemplatePart(Name = ColorCanvasPartName, Type = typeof(ColorCanvas))]
    public class ColorPicker : ColorPickerBase
    {
        public const string TextBoxPartName = "PART_TextBox";
        private const string ButtonPartName = "PART_Button";
        private const string PopupPartName = "PART_Popup";
        public const string ColorCanvasPartName = "PART_ColorCanvas";

        private TextBox? _textBox;
        private ButtonBase? _dropDownButton;
        private Popup? _popUp;
        private ColorCanvas? _colorCanvas;
        private bool _disablePopupReopen;

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
            EventManager.RegisterClassHandler(typeof(ColorPicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(false));
        }

        public ColorPicker() => Errors = [];

        #region Errors

        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(nameof(Errors), typeof(ObservableCollection<string>), typeof(ColorPicker), new PropertyMetadata(null));

        public ObservableCollection<string> Errors
        {
            get => (ObservableCollection<string>)GetValue(ErrorsProperty);
            private set => SetValue(ErrorsProperty, value);
        }

        #endregion

        #region HasErrors

        internal static readonly DependencyPropertyKey HasErrorsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasErrors),
                typeof(bool),
                typeof(ColorPicker),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasErrorsProperty = HasErrorsPropertyKey.DependencyProperty;

        public bool HasErrors => (bool)GetValue(HasErrorsProperty);

        #endregion HasErrors

        #region DropDownClosed

        /// <summary>Identifies the <see cref="DropDownClosed"/> routed event.</summary>
        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent(nameof(DropDownClosed), RoutingStrategy.Bubble, typeof(EventHandler<EventArgs>), typeof(ColorPicker));

        /// <summary>
        ///     Occurs when the DropDown is closed.
        /// </summary>
        public event EventHandler<EventArgs> DropDownClosed
        {
            add => AddHandler(DropDownClosedEvent, value);
            remove => RemoveHandler(DropDownClosedEvent, value);
        }

        #endregion

        #region DropDownOpened

        /// <summary>Identifies the <see cref="DropDownOpened"/> routed event.</summary>
        public static readonly RoutedEvent DropDownOpenedEvent
            = EventManager.RegisterRoutedEvent(nameof(DropDownOpened), RoutingStrategy.Bubble, typeof(EventHandler<EventArgs>), typeof(ColorPicker));

        /// <summary>
        ///     Occurs when the DropDown is opened.
        /// </summary>
        public event EventHandler<EventArgs> DropDownOpened
        {
            add => AddHandler(DropDownOpenedEvent, value);
            remove => RemoveHandler(DropDownOpenedEvent, value);
        }

        #endregion

        #region DropDownHeight

        /// <summary>Identifies the <see cref="DropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownHeightProperty
            = DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(ColorPicker), new PropertyMetadata(280d));

        /// <summary>
        /// Gets or sets the height of the DropDown.
        /// </summary>
        public double DropDownHeight
        {
            get => (double)GetValue(DropDownHeightProperty);
            set => SetValue(DropDownHeightProperty, value);
        }

        #endregion

        #region DropDownWidth

        /// <summary>Identifies the <see cref="DropDownWidth"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownWidthProperty
            = DependencyProperty.Register(nameof(DropDownWidth), typeof(double), typeof(ColorPicker), new PropertyMetadata(350d));

        /// <summary>
        /// Gets or sets the width of the DropDown.
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double DropDownWidth
        {
            get => (double)GetValue(DropDownWidthProperty);
            set => SetValue(DropDownWidthProperty, value);
        }

        #endregion

        #region IsDropDownOpen

        /// <summary>Identifies the <see cref="IsDropDownOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty
            = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

        /// <summary>
        /// Whether or not the "popup" for this control is currently open
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        #endregion

        #region SelectedColorTemplate

        /// <summary>Identifies the <see cref="SelectedColorTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorTemplateProperty
            = DependencyProperty.Register(nameof(SelectedColorTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the <see cref="ColorPickerBase.SelectedColor"/>
        /// </summary>
        public DataTemplate? SelectedColorTemplate
        {
            get => (DataTemplate?)GetValue(SelectedColorTemplateProperty);
            set => SetValue(SelectedColorTemplateProperty, value);
        }

        #endregion

        #region AddToRecentColorsTrigger

        /// <summary>Identifies the <see cref="AddToRecentColorsTrigger"/> dependency property.</summary>
        public static readonly DependencyProperty AddToRecentColorsTriggerProperty
            = DependencyProperty.Register(nameof(AddToRecentColorsTrigger), typeof(AddToRecentColorsTrigger), typeof(ColorPicker), new PropertyMetadata(AddToRecentColorsTrigger.ColorPickerClosed));

        /// <summary>
        /// Gets or sets when to add the <see cref="ColorPickerBase.SelectedColor"/> to the <see cref="RecentColorPaletteItemsSource"/>
        /// </summary>
        public AddToRecentColorsTrigger AddToRecentColorsTrigger
        {
            get => (AddToRecentColorsTrigger)GetValue(AddToRecentColorsTriggerProperty);
            set => SetValue(AddToRecentColorsTriggerProperty, value);
        }

        #endregion

        #region ShowCustomColors1

        /// <summary>Identifies the <see cref="ShowCustomColors1"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCustomColors1Property =
            DependencyProperty.Register(nameof(ShowCustomColors1), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the available <see cref="ColorPicker"/>.
        /// </summary>
        public bool ShowCustomColors1
        {
            get => (bool)GetValue(ShowCustomColors1Property);
            set => SetValue(ShowCustomColors1Property, value);
        }

        #endregion

        #region CustomColors1Header

        public static readonly DependencyProperty CustomColors1HeaderProperty =
            DependencyProperty.Register(nameof(CustomColors1Header), typeof(string), typeof(ColorPicker), new PropertyMetadata(null));

        public string CustomColors1Header
        {
            get => (string)GetValue(CustomColors1HeaderProperty);
            set => SetValue(CustomColors1HeaderProperty, value);
        }

        #endregion

        #region CustomColors1ItemsSource

        /// <summary>Identifies the <see cref="CustomColors1ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColors1ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColors1ItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the available <see cref="ColorPicker"/>.
        /// </summary>
        public IEnumerable? CustomColors1ItemsSource
        {
            get => (IEnumerable?)GetValue(CustomColors1ItemsSourceProperty);
            set => SetValue(CustomColors1ItemsSourceProperty, value);
        }

        #endregion

        #region ShowCustomColors2

        /// <summary>Identifies the <see cref="ShowCustomColors2"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCustomColors2Property =
            DependencyProperty.Register(nameof(ShowCustomColors2),
                                        typeof(bool),
                                        typeof(ColorPicker),
                                        new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the recent <see cref="ColorPicker"/>.
        /// </summary>
        public bool ShowCustomColors2
        {
            get => (bool)GetValue(ShowCustomColors2Property);
            set => SetValue(ShowCustomColors2Property, value);
        }

        #endregion

        #region CustomColors2Header

        public static readonly DependencyProperty CustomColors2HeaderProperty =
            DependencyProperty.Register(nameof(CustomColors2Header), typeof(string), typeof(ColorPicker), new PropertyMetadata(null));

        public string CustomColors2Header
        {
            get => (string)GetValue(CustomColors2HeaderProperty);
            set => SetValue(CustomColors2HeaderProperty, value);
        }

        #endregion

        #region CustomColors2ItemsSource

        /// <summary>Identifies the <see cref="CustomColors2ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColors2ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColors2ItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the recent <see cref="ColorPicker"/>.
        /// </summary>
        public IEnumerable? CustomColors2ItemsSource
        {
            get => (IEnumerable?)GetValue(CustomColors2ItemsSourceProperty);
            set => SetValue(CustomColors2ItemsSourceProperty, value);
        }

        #endregion

        #region ShowRecentColors

        /// <summary>Identifies the <see cref="ShowRecentColors"/> dependency property.</summary>
        public static readonly DependencyProperty ShowRecentColorsProperty =
            DependencyProperty.Register(nameof(ShowRecentColors), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the standard <see cref="ColorPicker"/>.
        /// </summary>
        public bool ShowRecentColors
        {
            get => (bool)GetValue(ShowRecentColorsProperty);
            set => SetValue(ShowRecentColorsProperty, value);
        }

        #endregion

        #region RecentColorsHeader

        public static readonly DependencyProperty RecentColorsHeaderProperty =
            DependencyProperty.Register(nameof(RecentColorsHeader), typeof(string), typeof(ColorPicker), new PropertyMetadata(null));

        public string RecentColorsHeader
        {
            get => (string)GetValue(RecentColorsHeaderProperty);
            set => SetValue(RecentColorsHeaderProperty, value);
        }

        #endregion

        #region RecentColorsItemsSource

        /// <summary>Identifies the <see cref="RecentColorsItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorsItemsSourceProperty =
            DependencyProperty.Register(nameof(RecentColorsItemsSource), typeof(IEnumerable), typeof(ColorPicker), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the standard <see cref="ColorPicker"/>.
        /// </summary>
        public IEnumerable? RecentColorsItemsSource
        {
            get => (IEnumerable?)GetValue(RecentColorsItemsSourceProperty);
            set => SetValue(RecentColorsItemsSourceProperty, value);
        }

        #endregion

        #region CanAddRecentColor

        /// <summary>Identifies the <see cref="CanAddRecentColor"/> dependency property.</summary>
        public static readonly DependencyProperty CanAddRecentColorProperty =
            DependencyProperty.Register(nameof(CanAddRecentColor), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the standard <see cref="ColorCanvas"/>.
        /// </summary>
        public bool CanAddRecentColor
        {
            get => (bool)GetValue(CanAddRecentColorProperty);
            set => SetValue(CanAddRecentColorProperty, value);
        }

        #endregion

        #region CloseOnSelectedColorChanged

        /// <summary>Identifies the <see cref="CloseOnSelectedColorChanged"/> dependency property.</summary>
        public static readonly DependencyProperty CloseOnSelectedColorChangedProperty =
            DependencyProperty.Register(nameof(CloseOnSelectedColorChanged), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the DropDown should close after a color was selected from a <see cref="ColorPalette"/>. The default is <see langword="false" />
        /// </summary>
        public bool CloseOnSelectedColorChanged
        {
            get => (bool)GetValue(CloseOnSelectedColorChangedProperty);
            set => SetValue(CloseOnSelectedColorChangedProperty, value);
        }

        #endregion

        #region ShowNumericValues

        public static readonly DependencyProperty ShowNumericValuesProperty = DependencyProperty.Register(nameof(ShowNumericValues), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        public bool ShowNumericValues
        {
            get => (bool)GetValue(ShowNumericValuesProperty);
            set => SetValue(ShowNumericValuesProperty, value);
        }

        #endregion

        #region ShowRGB

        public static readonly DependencyProperty ShowRGBProperty = DependencyProperty.Register(nameof(ShowRGB), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        public bool ShowRGB
        {
            get => (bool)GetValue(ShowRGBProperty);
            set => SetValue(ShowRGBProperty, value);
        }

        #endregion

        #region ShowHSV

        public static readonly DependencyProperty ShowHSVProperty = DependencyProperty.Register(nameof(ShowHSV), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        public bool ShowHSV
        {
            get => (bool)GetValue(ShowHSVProperty);
            set => SetValue(ShowHSVProperty, value);
        }

        #endregion

        #region ShowTransparency

        public static readonly DependencyProperty ShowTransparencyProperty = DependencyProperty.Register(nameof(ShowTransparency), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        public bool ShowTransparency
        {
            get => (bool)GetValue(ShowTransparencyProperty);
            set => SetValue(ShowTransparencyProperty, value);
        }

        #endregion

        #region ShowHexa

        public static readonly DependencyProperty ShowHexaProperty = DependencyProperty.Register(nameof(ShowHexa), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        public bool ShowHexa
        {
            get => (bool)GetValue(ShowHexaProperty);
            set => SetValue(ShowHexaProperty, value);
        }

        #endregion

        #region HexaIsReadOnly

        public static readonly DependencyProperty HexaIsReadOnlyProperty = DependencyProperty.Register(nameof(HexaIsReadOnly), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        public bool HexaIsReadOnly
        {
            get => (bool)GetValue(HexaIsReadOnlyProperty);
            set => SetValue(HexaIsReadOnlyProperty, value);
        }

        #endregion

        #region ShowEyeDropper

        public static readonly DependencyProperty ShowEyeDropperProperty = DependencyProperty.Register(nameof(ShowEyeDropper), typeof(bool), typeof(ColorPicker), new PropertyMetadata(true));

        public bool ShowEyeDropper
        {
            get => (bool)GetValue(ShowEyeDropperProperty);
            set => SetValue(ShowEyeDropperProperty, value);
        }

        #endregion

        #region ShowSVPicker

        public static readonly DependencyProperty ShowSVPickerProperty = DependencyProperty.Register(nameof(ShowSVPicker), typeof(bool), typeof(ColorPicker), new PropertyMetadata(false));

        public bool ShowSVPicker
        {
            get => (bool)GetValue(ShowSVPickerProperty);
            set => SetValue(ShowSVPickerProperty, value);
        }

        #endregion

        #region MaximumRecentColorsCount

        public static readonly DependencyProperty MaximumRecentColorsCountProperty = DependencyProperty.Register(nameof(MaximumRecentColorsCount), typeof(int), typeof(ColorPicker), new PropertyMetadata(18));

        public int MaximumRecentColorsCount
        {
            get => (int)GetValue(MaximumRecentColorsCountProperty);
            set => SetValue(MaximumRecentColorsCountProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_popUp != null)
            {
                _popUp.RemoveHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));
                _popUp.Opened -= PopUp_Opened;
                _popUp.Closed -= PopUp_Closed;
                _popUp.Child = null;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
                _dropDownButton.RemoveHandler(MouseLeaveEvent, new MouseEventHandler(DropDownButton_MouseLeave));
            }

            if (_colorCanvas is not null)
            {
                _colorCanvas.PreviewKeyDown -= ColorCanvas_PreviewKeyDown;
                _colorCanvas.RemoveHandler(ColorCanvas.SelectionChangedEvent, new RoutedEventHandler(ColorCanvas_SelectionChanged));
            }

            _textBox?.RemoveHandler(KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));

            base.OnApplyTemplate();

            _textBox = GetTemplateChild(TextBoxPartName) as TextBox;
            _popUp = GetTemplateChild(PopupPartName) as Popup;
            _dropDownButton = GetTemplateChild(ButtonPartName) as Button;
            _colorCanvas = Template.FindName(ColorCanvasPartName, this) as ColorCanvas;

            if (_popUp != null)
            {
                _popUp.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopUp_PreviewMouseLeftButtonDown));
                _popUp.Opened += PopUp_Opened;
                _popUp.Closed += PopUp_Closed;

                if (IsDropDownOpen)
                {
                    _popUp.IsOpen = true;
                }
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click += DropDownButton_Click;
                _dropDownButton.AddHandler(MouseLeaveEvent, new MouseEventHandler(DropDownButton_MouseLeave), true);
            }

            if (_colorCanvas is not null)
            {
                _colorCanvas.AddHandler(ColorCanvas.SelectionChangedEvent, new RoutedEventHandler(ColorCanvas_SelectionChanged), true);
                _colorCanvas.PreviewKeyDown += ColorCanvas_PreviewKeyDown;
            }

            _textBox?.AddHandler(KeyDownEvent, new KeyEventHandler(TextBox_KeyDown), true);
        }

        protected override void OnColorNameChanged(string? oldColorName, string? colorName)
        {
            Errors.Clear();
            SetValue(HasErrorsPropertyKey, false);
            base.OnColorNameChanged(oldColorName, colorName);
        }

        protected override void OnColorNameFailed(string? colorName)
        {
            Errors.Add(WpfResources.XIsNotAValidColorError.FormatWith(colorName));
            SetValue(HasErrorsPropertyKey, true);
        }

        private void ColorCanvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    TogglePopUp();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Datepicker gets focus move it to the TextBox
            var picker = (ColorPicker)sender;
            if ((!e.Handled) && (picker._textBox != null))
            {
                if (e.OriginalSource == picker)
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (e.OriginalSource == picker._textBox)
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private void TextBox_KeyDown(object? sender, KeyEventArgs e) => e.Handled = ProcessDatePickerKey(e) || e.Handled;

        private void PopUp_PreviewMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
        {
            if (sender is Popup popup && !popup.StaysOpen && _dropDownButton != null && _dropDownButton.InputHitTest(e.GetPosition(_dropDownButton)) != null)
            {
                // This popup is being closed by a mouse press on the drop down button
                // The following mouse release will cause the closed popup to immediately reopen.
                // Raise a flag to block reopeneing the popup
                _disablePopupReopen = true;
            }
        }

        private void PopUp_Opened(object? sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }
        }

        private void PopUp_Closed(object? sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }

        private void DropDownButton_Click(object? sender, RoutedEventArgs e) => TogglePopUp();

        private void DropDownButton_MouseLeave(object? sender, MouseEventArgs e) => _disablePopupReopen = false;

        private bool ProcessDatePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System:
                    {
                        switch (e.SystemKey)
                        {
                            case Key.Down:
                                {
                                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                                    {
                                        TogglePopUp();
                                        return true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }
            }

            return false;
        }

        private void TogglePopUp()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    SetCurrentValue(IsDropDownOpenProperty, true);
                }
            }
        }

        protected override void OnSelectedColorChanged(Color? oldValue, Color? newValue)
        {
            base.OnSelectedColorChanged(oldValue, newValue);

            if (AddToRecentColorsTrigger == AddToRecentColorsTrigger.SelectedColorChanged && newValue.HasValue)
            {
                _colorCanvas?.AddToRecentColors(newValue.Value);
            }
        }

        private void ColorCanvas_SelectionChanged(object? sender, RoutedEventArgs e)
        {
            if (CloseOnSelectedColorChanged)
                TogglePopUp();
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPicker colorPicker)
            {
                return;
            }

            var newValue = (bool)e.NewValue;

            if (colorPicker._popUp != null && colorPicker._popUp.IsOpen != newValue)
            {
                try
                {
                    colorPicker._popUp.IsOpen = newValue;
                }
                catch (Exception)
                {
                    // Ignore error
                }
            }

            if (newValue)
            {
                colorPicker.RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent));

                colorPicker.Dispatcher.BeginInvoke(DispatcherPriority.Send, () => colorPicker._colorCanvas?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)));
            }
            else
            {
                colorPicker.RaiseEvent(new RoutedEventArgs(DropDownClosedEvent));

                if (colorPicker.AddToRecentColorsTrigger == AddToRecentColorsTrigger.ColorPickerClosed && colorPicker.SelectedColor.HasValue)
                {
                    colorPicker._colorCanvas?.AddToRecentColors(colorPicker.SelectedColor.Value);
                }
            }
        }
    }
}
