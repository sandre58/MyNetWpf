// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.TestApp.Views
{
    public partial class InputsFieldsView
    {
        private IEnumerable<Control> Controls => FieldsLineUpGrid.Children.OfType<Control>();

        private sealed class ValidationErrorRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
                => value is string errorMessage && !string.IsNullOrWhiteSpace(errorMessage)
                    ? new ValidationResult(false, errorMessage)
                    : ValidationResult.ValidResult;
        }

        private static readonly ValidationErrorRule ValidationRule = new();

        public InputsFieldsView()
        {
            InitializeComponent();
            HorizontalPaddingSlider.ValueChanged += delegate { UpdateThickness(HorizontalPaddingSlider, Control.PaddingProperty, true); };
            VerticalPaddingSlider.ValueChanged += delegate { UpdateThickness(VerticalPaddingSlider, Control.PaddingProperty, false); };
            HorizontalTextBoxViewMarginSlider.ValueChanged += delegate { UpdateThickness(HorizontalTextBoxViewMarginSlider, TextFieldAssist.TextBoxViewMarginProperty, true); };
            VerticalTextBoxViewMarginSlider.ValueChanged += delegate { UpdateThickness(VerticalTextBoxViewMarginSlider, TextFieldAssist.TextBoxViewMarginProperty, false); };

            ValidationErrorTextBox.TextChanged += delegate
            {
                foreach (var control in Controls)
                    control.GetBindingExpression(TagProperty)!.UpdateSource();
            };

            foreach (var control in Controls)
            {
                _ = control.SetBinding(HintAssist.HintProperty, new Binding(nameof(TextBox.Text)) { ElementName = nameof(HintTextBox) });
                _ = control.SetBinding(HintAssist.HelperTextProperty, new Binding(nameof(TextBox.Text)) { ElementName = nameof(HelperTextBox) });
                _ = control.SetBinding(TextFieldAssist.HasClearButtonProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(HasClearButtonCheckBox) });
                _ = control.SetBinding(TextFieldAssist.PrefixTextProperty, new Binding(nameof(TextBox.Text)) { ElementName = nameof(PrefixTextBox) });
                _ = control.SetBinding(TextFieldAssist.SuffixTextProperty, new Binding(nameof(TextBox.Text)) { ElementName = nameof(SuffixTextBox) });
                _ = control.SetBinding(TextFieldAssist.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                _ = control.SetBinding(IconAssist.AlignmentProperty, new Binding(nameof(ComboBox.SelectedValue)) { ElementName = nameof(IconAlignment) });
                _ = control.SetBinding(TagProperty, new Binding(nameof(TextBox.Text))
                {
                    Mode = BindingMode.TwoWay,
                    ElementName = nameof(ValidationErrorTextBox),
                    ValidationRules = { ValidationRule },
                    ValidatesOnDataErrors = true
                });

                if (control is RevealPasswordBox revealPasswordBox)
                {
                    _ = revealPasswordBox.SetBinding(TextBoxBase.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                    _ = revealPasswordBox.SetBinding(TextFieldAssist.CharacterCounterIsVisibleProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(CharacterCounterIsVisible) });
                    _ = revealPasswordBox.SetBinding(TextBox.MaxLengthProperty, new Binding(nameof(Slider.Value)) { ElementName = nameof(MaxLength) });
                    IconAssist.SetIcon(revealPasswordBox, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Password });
                }

                else if (control is TextBox textbox)
                {
                    _ = textbox.SetBinding(SpellCheck.IsEnabledProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(SpellChecking) });
                    _ = textbox.SetBinding(TextBoxBase.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                    _ = textbox.SetBinding(TextFieldAssist.CharacterCounterIsVisibleProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(CharacterCounterIsVisible) });
                    _ = textbox.SetBinding(TextBox.MaxLengthProperty, new Binding(nameof(Slider.Value)) { ElementName = nameof(MaxLength) });
                    IconAssist.SetIcon(textbox, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Search });
                }

                else if (control is PasswordBox passwordBox)
                {
                    _ = passwordBox.SetBinding(TextFieldAssist.CharacterCounterIsVisibleProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(CharacterCounterIsVisible) });
                    _ = passwordBox.SetBinding(PasswordBox.MaxLengthProperty, new Binding(nameof(Slider.Value)) { ElementName = nameof(MaxLength) });
                    IconAssist.SetIcon(passwordBox, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Password });
                }

                else if (control is ComboBox comboBox)
                {
                    _ = comboBox.SetBinding(ComboBox.IsEditableProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsEditableCheckBox) });
                    _ = comboBox.SetBinding(ComboBoxAssist.ShowCaretProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowCaret) });
                    _ = comboBox.SetBinding(ComboBoxAssist.ShowDropDownPopupProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowDropDownPopup) });
                    _ = comboBox.SetBinding(ComboBoxAssist.ShowUpDownButtonProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowUpDownButton) });
                    _ = comboBox.SetBinding(ComboBox.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                    IconAssist.SetIcon(comboBox, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Search });
                }

                else if (control is MultiComboBox multiComboBox)
                {
                    _ = multiComboBox.SetBinding(MultiComboBox.IsEditableProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsEditableCheckBox) });
                    _ = multiComboBox.SetBinding(ComboBoxAssist.ShowCaretProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowCaret) });
                    _ = multiComboBox.SetBinding(ComboBoxAssist.ShowDropDownPopupProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowDropDownPopup) });
                    _ = multiComboBox.SetBinding(MultiComboBox.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                    _ = multiComboBox.SetBinding(MultiComboBox.ShowRemoveItemButtonProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowRemoveButton) });
                    multiComboBox.MaxWidth = 200;
                    IconAssist.SetIcon(multiComboBox, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Search });
                }

                else if (control is NumericUpDown num)
                {
                    _ = num.SetBinding(NumericUpDown.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(IsReadOnly) });
                    _ = num.SetBinding(NumericUpDown.HideUpDownButtonsProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(HideUpDownButton) });
                    _ = num.SetBinding(NumericUpDown.SwitchUpDownButtonsProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(SwitchButtons) });
                    _ = num.SetBinding(NumericUpDown.ButtonsAlignmentProperty, new Binding(nameof(Selector.SelectedValue)) { ElementName = nameof(NumericButtonsAlignment) });
                    IconAssist.SetIcon(num, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Numeric });
                }

                else if (control is DatePicker datePicker)
                {
                    IconAssist.SetIcon(datePicker, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.CalendarAdd });
                }

                else if (control is TimePicker tp)
                {
                    _ = tp.SetBinding(TimePicker.Is24HoursProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(Is24Hours) });
                    _ = tp.SetBinding(TimePicker.WithSecondsProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(WithSeconds) });
                    IconAssist.SetIcon(tp, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.ClockCheck });
                }

                else if (control is ColorPicker cp)
                {
                    _ = cp.SetBinding(ColorPicker.DropDownHeightProperty, new Binding(nameof(Slider.Value)) { ElementName = nameof(ColorDropDownHeight) });
                    _ = cp.SetBinding(ColorPicker.DropDownWidthProperty, new Binding(nameof(Slider.Value)) { ElementName = nameof(ColorDropDownWidth) });
                    _ = cp.SetBinding(ColorPicker.CanAddRecentColorProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(CanAddRecentColor) });
                    _ = cp.SetBinding(ColorPicker.CloseOnSelectedColorChangedProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(CloseOnSelectedColorChanged) });
                    _ = cp.SetBinding(ColorPicker.ShowCustomColors1Property, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowCustomColors1) });
                    _ = cp.SetBinding(ColorPicker.ShowCustomColors2Property, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowCustomColors2) });
                    _ = cp.SetBinding(ColorPicker.ShowRecentColorsProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowRecentColors) });
                    _ = cp.SetBinding(ColorPicker.ShowNumericValuesProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowNumericValues) });
                    _ = cp.SetBinding(ColorPicker.ShowEyeDropperProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowEyeDropper) });
                    _ = cp.SetBinding(ColorPicker.ShowHexaProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowHexa) });
                    _ = cp.SetBinding(ColorPicker.ShowHSVProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowHSV) });
                    _ = cp.SetBinding(ColorPicker.ShowRGBProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowRGB) });
                    _ = cp.SetBinding(ColorPicker.ShowSVPickerProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowSVPicker) });
                    _ = cp.SetBinding(ColorPicker.ShowTransparencyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(ShowTransparency) });
                    _ = cp.SetBinding(ColorPicker.HexaIsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { ElementName = nameof(HexaIsReadOnly) });
                    _ = cp.SetBinding(ColorPickerBase.DisplayNameModeProperty, new Binding(nameof(ComboBox.SelectedValue)) { ElementName = nameof(DisplayNameMode) });
                    _ = cp.SetBinding(ColorPicker.AddToRecentColorsTriggerProperty, new Binding(nameof(ComboBox.SelectedValue)) { ElementName = nameof(AddToRecentColorsTrigger) });
                    IconAssist.SetIcon(cp, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Paint });
                }

                else if (control is ImagePicker ip)
                {
                    IconAssist.SetIcon(ip, new PackIcon { Kind = MaterialDesignThemes.Wpf.PackIconKind.Image });
                }

                control.VerticalAlignment = VerticalAlignment.Top;
                control.Margin = new Thickness(2, 7, 2, 7);
                control.HorizontalAlignment = HorizontalAlignment.Stretch;
                SetValue(control);
                UpdateThickness(HorizontalPaddingSlider, Control.PaddingProperty, true);
                UpdateThickness(VerticalPaddingSlider, Control.PaddingProperty, false);
                UpdateThickness(HorizontalTextBoxViewMarginSlider, TextFieldAssist.TextBoxViewMarginProperty, true);
            }
        }

        private void UpdateThickness(RangeBase slider, DependencyProperty property, bool horizontal)
        {
            var newValue = slider.Value;
            foreach (var element in Controls)
            {
                var current = (Thickness)element.GetValue(property);
                var updated = horizontal
                    ? new Thickness(newValue, current.Top, newValue, current.Bottom)
                    : new Thickness(current.Left, newValue, current.Right, newValue);

                if (!double.IsInfinity(current.Top) && !double.IsInfinity(current.Left) && !double.IsInfinity(current.Right) && !double.IsInfinity(current.Bottom))
                    element.SetValue(property, updated);
            }
        }

        private static void SetValue(Control control)
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.Text = nameof(TextBox.Text);
                    break;
                case PasswordBox passwordBox:
                    passwordBox.Password = nameof(PasswordBox.Password);
                    break;
                case ComboBox comboBox:
                    comboBox.ItemsSource = Enumerable.Range(1, 15).Select(x => nameof(ComboBox.Text) + x);
                    comboBox.SelectedIndex = 0;
                    break;
                case MultiComboBox cb:
                    cb.ItemsSource = Enumerable.Range(1, 30).Select(x => nameof(ComboBox.Text) + x);
                    cb.SelectedIndex = 0;
                    break;
                case DatePicker datePicker:
                    datePicker.SelectedDate = DateTime.Now;
                    break;
                case NumericUpDown num:
                    num.Value = 5;
                    break;
                case TimePicker timePicker:
                    timePicker.SelectedTime = DateTime.Now;
                    break;
                case ColorPicker cp:
                    cp.SelectedColor = Color.FromRgb(255, 0, 0);
                    break;
                case ImagePicker ip:
                    ip.SelectedImage = null;
                    break;
                default:
                    throw new NotSupportedException(control.GetType().FullName);
            }
        }
    }
}
