// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MyNet.UI.Resources;
using MyNet.Utilities;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Controls.HintProxy;
using MyNet.Wpf.Converters;
using MyNet.Wpf.Parameters;
using MyNet.Wpf.Presentation.Controls.HintProxy;
using MyNet.Wpf.Presentation.Models;

namespace MyNet.Wpf.Presentation.Parameters
{
    public static class EditionAssist
    {
        static EditionAssist()
        {
            HintProxyFabric.RegisterBuilder(x => x is TextBox && GetMultipleEditableValue(x) != null, x => new MultipleTextBoxHintProxy((TextBox)x));
            HintProxyFabric.RegisterBuilder(x => x is TimePicker && GetMultipleEditableValue(x) != null, x => new MultipleTimePickerHintProxy((TimePicker)x));
            HintProxyFabric.RegisterBuilder(x => x is DatePicker && GetMultipleEditableValue(x) != null, x => new MultipleDatePickerHintProxy((DatePicker)x));
            HintProxyFabric.RegisterBuilder(x => x is ComboBox && GetMultipleEditableValue(x) != null, x => new MultipleComboBoxHintProxy((ComboBox)x));
            HintProxyFabric.RegisterBuilder(x => x is NumericUpDown && GetMultipleEditableValue(x) != null, x => new MultipleNumericUpDownHintProxy((NumericUpDown)x));
        }

        #region MultipleEditableValue

        public static readonly DependencyProperty MultipleEditableValueProperty = DependencyProperty.RegisterAttached("MultipleEditableValue", typeof(IMultipleEditableValue), typeof(EditionAssist), new PropertyMetadata(OnMultipleEditableValueCallback));

        public static IMultipleEditableValue GetMultipleEditableValue(DependencyObject obj) => (IMultipleEditableValue)obj.GetValue(MultipleEditableValueProperty);

        public static void SetMultipleEditableValue(DependencyObject obj, IMultipleEditableValue value) => obj.SetValue(MultipleEditableValueProperty, value);

        private static void OnMultipleEditableValueCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not FrameworkElement element || e.NewValue is not IMultipleEditableValue multipleWrapper) return;

            // Active checkbox
            var checkBox = new CheckBox()
            {
                ToolTip = UiResources.Edit
            };
            checkBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding(nameof(IMultipleEditableValue.IsActive)) { Source = multipleWrapper });
            element.SetValue(IconAssist.IconProperty, checkBox);
            element.SetValue(IconAssist.OpacityProperty, 1.0d);

            // Helper Text
            HintAssist.SetHelperTextFontSize(element, TextElement.GetFontSize(element));
            UpdateHelperText(element, multipleWrapper);

            // Main element
            element.SetBinding(TextFieldAssist.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { Source = checkBox, Converter = NotBooleanConverter.Default });

            if (element is TextBox textBox)
                textBox.SetBinding(System.Windows.Controls.Primitives.TextBoxBase.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { Source = checkBox, Converter = NotBooleanConverter.Default });

            if (element is ComboBox comboBox)
                comboBox.SetBinding(ComboBox.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { Source = checkBox, Converter = NotBooleanConverter.Default });

            if (element is NumericUpDown numericUpDown)
                numericUpDown.SetBinding(NumericUpDown.IsReadOnlyProperty, new Binding(nameof(CheckBox.IsChecked)) { Source = checkBox, Converter = NotBooleanConverter.Default });

            multipleWrapper.PropertyChanged += (sener, e) => UpdateHelperText(dependencyObject, multipleWrapper);
            element.GotKeyboardFocus += (sender, e) =>
            {
                if (checkBox.IsChecked.IsFalse() && e.OriginalSource != checkBox)
                    checkBox.IsChecked = true;
            };
            checkBox.Checked += (sender, e) =>
            {
                if (checkBox.IsChecked.IsTrue() && !element.IsFocused)
                    element.Focus();
            };
        }

        private static void UpdateHelperText(DependencyObject dependencyObject, IMultipleEditableValue multipleEditableValue)
        {
            if (multipleEditableValue.IsActive)
            {
                if (dependencyObject is UIElement control)
                    control.Opacity = 1.0d;

                if (multipleEditableValue.IsEmpty())
                {
                    HintAssist.SetHelperTextOpacity(dependencyObject, 0.56d);
                    HintAssist.SetHelperTextFontStyle(dependencyObject, FontStyles.Normal);
                    HintAssist.SetHelperText(dependencyObject, GetEmptyText(dependencyObject));
                }
                else
                {
                    HintAssist.SetHelperTextOpacity(dependencyObject, 0.0d);
                }
            }
            else
            {
                if (dependencyObject is UIElement control)
                    control.Opacity = 0.56d;

                if (multipleEditableValue.IsEmpty())
                {
                    HintAssist.SetHelperText(dependencyObject, GetInactiveText(dependencyObject));
                    HintAssist.SetHelperTextFontStyle(dependencyObject, FontStyles.Italic);
                    HintAssist.SetHelperTextOpacity(dependencyObject, 1d);
                }
                else
                {
                    HintAssist.SetHelperTextOpacity(dependencyObject, 0.0d);
                }
            }
        }

        #endregion MultipleEditableValue

        #region MultipleText

        public static readonly DependencyProperty MultipleTextProperty = DependencyProperty.RegisterAttached("MultipleText", typeof(string), typeof(EditionAssist), new PropertyMetadata(UiResources.MultiplePlaceHolder));

        public static string GetMultipleText(DependencyObject obj) => (string)obj.GetValue(MultipleTextProperty);

        public static void SetMultipleText(DependencyObject obj, string value) => obj.SetValue(MultipleTextProperty, value);

        #endregion

        #region EmptyText

        public static readonly DependencyProperty EmptyTextProperty = DependencyProperty.RegisterAttached("EmptyText", typeof(string), typeof(EditionAssist), new PropertyMetadata(UiResources.Clear));

        public static string GetEmptyText(DependencyObject obj) => (string)obj.GetValue(EmptyTextProperty);

        public static void SetEmptyText(DependencyObject obj, string value) => obj.SetValue(EmptyTextProperty, value);

        #endregion

        #region InactiveText

        public static readonly DependencyProperty InactiveTextProperty = DependencyProperty.RegisterAttached("InactiveText", typeof(string), typeof(EditionAssist), new PropertyMetadata(UiResources.Ignore));

        public static string GetInactiveText(DependencyObject obj) => (string)obj.GetValue(InactiveTextProperty);

        public static void SetInactiveText(DependencyObject obj, string value) => obj.SetValue(InactiveTextProperty, value);

        #endregion
    }
}
