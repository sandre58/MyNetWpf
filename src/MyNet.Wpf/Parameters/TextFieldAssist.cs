// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Parameters
{
    public enum MaskType
    {
        Any,
        Integer,
        PositiveInteger,
        Decimal,
        PositiveDecimal,
        Time
    }

    /// <summary>
    /// Helper properties for working with text fields.
    /// </summary>
    public static class TextFieldAssist
    {
        #region TextBoxViewMargin

        public static readonly DependencyProperty TextBoxViewMarginProperty = DependencyProperty.RegisterAttached(
            "TextBoxViewMargin",
            typeof(Thickness),
            typeof(TextFieldAssist),
            new FrameworkPropertyMetadata(new Thickness(double.NegativeInfinity), FrameworkPropertyMetadataOptions.Inherits, TextBoxViewMarginPropertyChangedCallback));

        public static void SetTextBoxViewMargin(DependencyObject element, Thickness value) => element.SetValue(TextBoxViewMarginProperty, value);

        public static Thickness GetTextBoxViewMargin(DependencyObject element) => (Thickness)element.GetValue(TextBoxViewMarginProperty);

        #endregion

        #region DecorationVisibility

        public static readonly DependencyProperty DecorationVisibilityProperty = DependencyProperty.RegisterAttached(
            "DecorationVisibility", typeof(Visibility), typeof(TextFieldAssist), new PropertyMetadata(default(Visibility)));

        public static void SetDecorationVisibility(DependencyObject element, Visibility value) => element.SetValue(DecorationVisibilityProperty, value);

        public static Visibility GetDecorationVisibility(DependencyObject element) => (Visibility)element.GetValue(DecorationVisibilityProperty);

        #endregion

        #region UnderlineBrush

        public static readonly DependencyProperty UnderlineBrushProperty = DependencyProperty.RegisterAttached(
            "UnderlineBrush", typeof(Brush), typeof(TextFieldAssist), new PropertyMetadata(Brushes.Transparent));

        public static void SetUnderlineBrush(DependencyObject element, Brush value) => element.SetValue(UnderlineBrushProperty, value);

        public static Brush GetUnderlineBrush(DependencyObject element) => (Brush)element.GetValue(UnderlineBrushProperty);

        #endregion

        #region RippleOnFocusEnabled

        public static readonly DependencyProperty RippleOnFocusEnabledProperty = DependencyProperty.RegisterAttached(
            "RippleOnFocusEnabled", typeof(bool), typeof(TextFieldAssist), new PropertyMetadata(false));

        public static void SetRippleOnFocusEnabled(DependencyObject element, bool value) => element.SetValue(RippleOnFocusEnabledProperty, value);

        public static bool GetRippleOnFocusEnabled(DependencyObject element) => (bool)element.GetValue(RippleOnFocusEnabledProperty);

        #endregion

        #region SuffixText

        public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.RegisterAttached(
            "SuffixText", typeof(string), typeof(TextFieldAssist), new PropertyMetadata(default(string?)));

        public static void SetSuffixText(DependencyObject element, string? value)
            => element.SetValue(SuffixTextProperty, value);

        public static string? GetSuffixText(DependencyObject element)
            => (string?)element.GetValue(SuffixTextProperty);

        #endregion

        #region PrefixText

        public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.RegisterAttached(
            "PrefixText", typeof(string), typeof(TextFieldAssist), new PropertyMetadata(default(string?)));

        public static void SetPrefixText(DependencyObject element, string? value)
            => element.SetValue(PrefixTextProperty, value);

        public static string? GetPrefixText(DependencyObject element)
            => (string?)element.GetValue(PrefixTextProperty);

        #endregion

        #region HasClearButton

        public static readonly DependencyProperty HasClearButtonProperty = DependencyProperty.RegisterAttached(
            "HasClearButton", typeof(bool), typeof(TextFieldAssist), new PropertyMetadata(false));

        public static void SetHasClearButton(DependencyObject element, bool value)
            => element.SetValue(HasClearButtonProperty, value);

        public static bool GetHasClearButton(DependencyObject element)
            => (bool)element.GetValue(HasClearButtonProperty);

        #endregion

        #region CharacterCounterStyle

        public static readonly DependencyProperty CharacterCounterStyleProperty =
            DependencyProperty.RegisterAttached("CharacterCounterStyle", typeof(Style), typeof(TextFieldAssist), new PropertyMetadata(null));

        public static Style GetCharacterCounterStyle(DependencyObject obj) => (Style)obj.GetValue(CharacterCounterStyleProperty);

        public static void SetCharacterCounterStyle(DependencyObject obj, Style value) => obj.SetValue(CharacterCounterStyleProperty, value);

        #endregion

        #region CharacterCounterIsVisible

        public static readonly DependencyProperty CharacterCounterIsVisibleProperty =
            DependencyProperty.RegisterAttached("CharacterCounterIsVisible", typeof(bool), typeof(TextFieldAssist),
                new PropertyMetadata(false, CharacterCounterVisibilityChanged));

        public static bool GetCharacterCounterIsVisible(DependencyObject obj)
            => (bool)obj.GetValue(CharacterCounterIsVisibleProperty);

        public static void SetCharacterCounterIsVisible(DependencyObject obj, bool value)
            => obj.SetValue(CharacterCounterIsVisibleProperty, value);


        private static void CharacterCounterVisibilityChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBoxOnPasswordChanged;
                if (Equals(true, e.NewValue))
                {
                    SetPasswordBoxCharacterCount(passwordBox, passwordBox.SecurePassword.Length);
                    passwordBox.PasswordChanged += PasswordBoxOnPasswordChanged;
                }
            }
        }

        #endregion

        #region PasswordBoxCharacterCount

        internal static readonly DependencyProperty PasswordBoxCharacterCountProperty = DependencyProperty.RegisterAttached(
            "PasswordBoxCharacterCount", typeof(int), typeof(TextFieldAssist), new PropertyMetadata(default(int)));

        private static void PasswordBoxOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            SetPasswordBoxCharacterCount(passwordBox, passwordBox.SecurePassword.Length);
        }

        internal static void SetPasswordBoxCharacterCount(DependencyObject element, int value) => element.SetValue(PasswordBoxCharacterCountProperty, value);
        internal static int GetPasswordBoxCharacterCount(DependencyObject element) => (int)element.GetValue(PasswordBoxCharacterCountProperty);

        #endregion

        #region ClearTextCommand

        public static readonly RoutedCommand ClearCommand = new();

        public static bool GetHandlesClearCommand(DependencyObject obj)
            => (bool)obj.GetValue(HandlesClearCommandProperty);

        public static void SetHandlesClearCommand(DependencyObject obj, bool value)
            => obj.SetValue(HandlesClearCommandProperty, value);

        public static readonly DependencyProperty HandlesClearCommandProperty =
            DependencyProperty.RegisterAttached("HandlesClearCommand", typeof(bool), typeof(TextFieldAssist), new PropertyMetadata(false, OnHandlesClearCommandChanged));

        private static void OnHandlesClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.CommandBindings.Add(new CommandBinding(ClearCommand, onClearCommand));
                }
                else
                {
                    for (var i = element.CommandBindings.Count - 1; i >= 0; i--)
                    {
                        if (element.CommandBindings[i].Command == ClearCommand)
                        {
                            element.CommandBindings.RemoveAt(i);
                        }
                    }
                }
            }

            static void onClearCommand(object sender, ExecutedRoutedEventArgs e)
            {
                switch (sender ?? e.Source)
                {
                    case DatePicker datePicker:
                        datePicker.SetCurrentValue(DatePicker.SelectedDateProperty, null);
                        break;
                    case AutoSuggestBox autoSuggestBox:
                        autoSuggestBox.SetCurrentValue(TextBox.TextProperty, null);
                        break;
                    case TextBox textBox:
                        textBox.SetCurrentValue(TextBox.TextProperty, null);
                        break;
                    case ComboBox comboBox:
                        comboBox.SetCurrentValue(ComboBox.TextProperty, null);
                        comboBox.SetCurrentValue(Selector.SelectedItemProperty, null);
                        break;
                    case PasswordBox passwordBox:
                        passwordBox.Password = null;
                        break;
                    case NumericUpDown numericUpDown:
                        numericUpDown.SetCurrentValue(NumericUpDown.ValueProperty, null);
                        break;
                    case TimePicker timePicker:
                        timePicker.SetCurrentValue(TimePicker.SelectedTimeProperty, null);
                        break;
                    case ColorPickerBase colorPicker:
                        colorPicker.SetCurrentValue(ColorPickerBase.ColorNameProperty, null);
                        break;
                    case ImagePicker imagePicker:
                        imagePicker.SetCurrentValue(ImagePicker.ImagePathProperty, null);
                        imagePicker.SetCurrentValue(ImagePicker.SelectedImageProperty, null);
                        break;
                }
                e.Handled = true;
            }
        }

        #endregion

        #region IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached(
            "IsReadOnly",
            typeof(bool),
            typeof(TextFieldAssist),
            new PropertyMetadata(false));

        public static bool GetIsReadOnly(DependencyObject item) => (bool)item.GetValue(IsReadOnlyProperty);

        public static void SetIsReadOnly(DependencyObject item, bool value) => item.SetValue(IsReadOnlyProperty, value);

        #endregion IsReadOnly

        #region Mask

        public static readonly DependencyProperty MaskProperty =
    DependencyProperty.RegisterAttached(
        "Mask",
        typeof(MaskType),
        typeof(TextFieldAssist),
        new FrameworkPropertyMetadata(MaskChangedCallback)
        );

        public static MaskType GetMask(DependencyObject obj) => (MaskType)obj.GetValue(MaskProperty);

        public static void SetMask(DependencyObject obj, MaskType value) => obj.SetValue(MaskProperty, value);

        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox oldTextbox)
            {
                oldTextbox.PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler(oldTextbox, TextBoxPastingEventHandler);
            }

            if (d is not TextBox textbox)
            {
                return;
            }

            if ((MaskType)e.NewValue != MaskType.Any)
            {
                textbox.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(textbox, TextBoxPastingEventHandler);
            }

            ValidateTextBox(textbox);
        }

        private static void ValidateTextBox(TextBox textbox)
        {
            if (GetMask(textbox) != MaskType.Any)
            {
                textbox.Text = ValidateValue(GetMask(textbox), textbox.Text);
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            var textbox = (TextBox)sender;
            var clipboard = e.DataObject.GetData(typeof(string)) as string;
            clipboard = ValidateValue(GetMask(textbox), clipboard);
            if (!string.IsNullOrEmpty(clipboard))
            {
                textbox.Text = clipboard;
            }

            e.CancelCommand();
            e.Handled = true;
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textbox = (TextBox)sender;
            var isValid = IsSymbolValid(GetMask(textbox), e.Text);
            e.Handled = !isValid;
        }

        private static string ValidateValue(MaskType mask, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = value!.Trim();
            return mask switch
            {
                MaskType.Integer or MaskType.PositiveInteger or MaskType.Time => int.TryParse(value, out _) ? value : string.Empty,
                MaskType.Decimal or MaskType.PositiveDecimal => double.TryParse(value, out _) ? value : string.Empty,
                _ => value,
            };
        }

        private static bool IsSymbolValid(MaskType mask, string str)
        {
            switch (mask)
            {
                case MaskType.Any:
                    return true;

                case MaskType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        return true;
                    }

                    break;

                case MaskType.Decimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                    {
                        return true;
                    }

                    break;

                case MaskType.PositiveDecimal:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        return true;
                    }

                    break;

                case MaskType.Time:
                    if (str == DateTimeFormatInfo.CurrentInfo.TimeSeparator)
                    {
                        return true;
                    }

                    break;
            }

            return (mask.Equals(MaskType.Integer) || mask.Equals(MaskType.Decimal) || mask.Equals(MaskType.PositiveInteger) || mask.Equals(MaskType.PositiveDecimal) || mask.Equals(MaskType.Time))
&& str.All(char.IsDigit);
        }

        #endregion Mask

        #region Methods

        /// <summary>
        /// Applies the text box view margin.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="margin">The margin.</param>
        private static void ApplyTextBoxViewMargin(Control textBox, Thickness margin)
        {
            if (margin.Equals(new Thickness(double.NegativeInfinity))
                || textBox.Template is null)
            {
                return;
            }

            if (textBox is ComboBox
                && textBox.Template.FindName("PART_EditableTextBox", textBox) is TextBox editableTextBox)
            {
                textBox = editableTextBox;
                if (textBox.Template is null) return;
                textBox.ApplyTemplate();
            }

            if (textBox.Template.FindName("PART_ContentHost", textBox) is ScrollViewer scrollViewer
                && scrollViewer.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.Margin = margin;
            }
        }

        /// <summary>
        /// The text box view margin property changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The dependency property changed event args.</param>
        private static void TextBoxViewMarginPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is not Control box)
            {
                return;
            }

            if (box.IsLoaded)
            {
                ApplyTextBoxViewMargin(box, (Thickness)dependencyPropertyChangedEventArgs.NewValue);
            }

            box.Loaded += (sender, args) =>
            {
                var textBox = (Control)sender;
                ApplyTextBoxViewMargin(textBox, GetTextBoxViewMargin(textBox));
            };
        }

        #endregion
    }
}
