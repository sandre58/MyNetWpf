// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls
{
    public class DataGridNumericColumn : DataGridTextColumn
    {
        static DataGridNumericColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridNumericColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridNumericColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #region Styles

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.NumericUpDown.Embedded.DataGrid.Edition");

        #endregion

        #region Element Generation

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new NumericUpDown();

            SyncProperties(control);
            WpfHelper.SyncProperty(this, control, NumericUpDown.TextAlignmentProperty, TextAlignmentProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.StringFormatProperty, StringFormatProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.IntervalProperty, IntervalProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.ButtonsAlignmentProperty, ButtonsAlignmentProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.HideUpDownButtonsProperty, HideUpDownButtonsProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.UpDownButtonsWidthProperty, UpDownButtonsWidthProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.UpDownButtonsFocusableProperty, UpDownButtonsFocusableProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.SwitchUpDownButtonsProperty, SwitchUpDownButtonsProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.MaximumProperty, MaximumProperty);
            WpfHelper.SyncProperty(this, control, NumericUpDown.MinimumProperty, MinimumProperty);

            ApplyStyle(true, control);

            ApplyBinding(control, NumericUpDown.ValueProperty);
            ApplyBinding(control, NumericUpDown.MaximumProperty, MaximumBinding);
            ApplyBinding(control, NumericUpDown.MinimumProperty, MinimumBinding);
            return control;
        }

        #endregion

        #region Editing

        /// <summary>
        ///     Called when a cell has just switched to edit mode.
        /// </summary>
        /// <param name="editingElement">A reference to element returned by GenerateEditingElement.</param>
        /// <param name="editingEventArgs">The event args of the input event that caused the cell to go into edit mode. May be null.</param>
        /// <returns>The unedited value of the cell.</returns>
        protected override object? PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if (editingElement is NumericUpDown control)
            {
                _ = control.Focus();
                control.SelectAll();

                return control.Value;
            }

            return null;
        }

        #endregion

        /// <summary>Identifies the <see cref="TextAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty TextAlignmentProperty = TextBox.TextAlignmentProperty.AddOwner(typeof(DataGridNumericColumn));

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

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty
            = DependencyProperty.Register(nameof(StringFormat),
                                          typeof(string),
                                          typeof(DataGridNumericColumn),
                                          new FrameworkPropertyMetadata(string.Empty));

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

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(nameof(Minimum),
                                          typeof(double),
                                          typeof(DataGridNumericColumn),
                                          new FrameworkPropertyMetadata(double.MinValue));

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

        public BindingBase? MinimumBinding { get; set; }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(nameof(Maximum),
                                          typeof(double),
                                          typeof(DataGridNumericColumn),
                                          new FrameworkPropertyMetadata(double.MaxValue));

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

        public BindingBase? MaximumBinding { get; set; }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(double),
                                          typeof(DataGridNumericColumn),
                                          new FrameworkPropertyMetadata(1d));

        /// <summary>
        /// Gets or sets the interval value for increasing/decreasing the <see cref="Value" /> .
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonsAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonsAlignmentProperty
            = DependencyProperty.Register(nameof(ButtonsAlignment),
                                          typeof(ButtonsAlignment),
                                          typeof(DataGridNumericColumn),
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
                                          typeof(DataGridNumericColumn),
                                          new PropertyMetadata(false));

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool HideUpDownButtons
        {
            get => (bool)GetValue(HideUpDownButtonsProperty);
            set => SetValue(HideUpDownButtonsProperty, value);
        }

        /// <summary>Identifies the <see cref="UpDownButtonsWidth"/> dependency property.</summary>
        public static readonly DependencyProperty UpDownButtonsWidthProperty
            = DependencyProperty.Register(nameof(UpDownButtonsWidth),
                                          typeof(double),
                                          typeof(DataGridNumericColumn),
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
                                          typeof(DataGridNumericColumn),
                                          new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether the up and down buttons will got the focus when using them.
        /// </summary>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool UpDownButtonsFocusable
        {
            get => (bool)GetValue(UpDownButtonsFocusableProperty);
            set => SetValue(UpDownButtonsFocusableProperty, value);
        }

        /// <summary>Identifies the <see cref="SwitchUpDownButtons"/> dependency property.</summary>
        public static readonly DependencyProperty SwitchUpDownButtonsProperty
            = DependencyProperty.Register(nameof(SwitchUpDownButtons),
                                          typeof(bool),
                                          typeof(DataGridNumericColumn),
                                          new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the up/down buttons will be switched.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool SwitchUpDownButtons
        {
            get => (bool)GetValue(SwitchUpDownButtonsProperty);
            set => SetValue(SwitchUpDownButtonsProperty, value);
        }
    }
}
