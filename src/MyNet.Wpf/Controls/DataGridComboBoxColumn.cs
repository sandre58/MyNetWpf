// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MyNet.Wpf.Helpers;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Controls
{
    public class DataGridComboBoxColumn : System.Windows.Controls.DataGridComboBoxColumn
    {
        public BindingBase? ItemsSourceBinding { get; set; }

        static DataGridComboBoxColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #region Styles

        public static new Style DefaultElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.ContentControl.Embedded.DataGrid");

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.ComboBox.Embedded.DataGrid.Edition");

        #endregion

        public static readonly DependencyProperty HasClearButtonProperty
    = DependencyProperty.Register(nameof(HasClearButton), typeof(bool), typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool HasClearButton
        {
            get => (bool)GetValue(HasClearButtonProperty);
            set => SetValue(HasClearButtonProperty, value);
        }

        public static readonly DependencyProperty IsEditableProperty
            = DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public DataTemplate CellTemplate
        {
            get => (DataTemplate)GetValue(CellTemplateProperty);
            set => SetValue(CellTemplateProperty, value);
        }

        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register(
                                                                            "CellTemplate",
                                                                            typeof(DataTemplate),
                                                                            typeof(DataGridComboBoxColumn),
                                                                            new FrameworkPropertyMetadata(null));

        public DataTemplateSelector CellTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(CellTemplateSelectorProperty);
            set => SetValue(CellTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(
                                                                                    "CellTemplateSelector",
                                                                                    typeof(DataTemplateSelector),
                                                                                    typeof(DataGridComboBoxColumn),
                                                                                    new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var content = new ContentControl();

            SyncProperties(content);

            if (CellTemplate is not null)
            {
                content.ContentTemplate = CellTemplate;
                content.ContentTemplateSelector = CellTemplateSelector;
            }

            ApplyStyle(false, content);
            ApplyBinding(content, ContentControl.ContentProperty);

            return content;
        }

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var comboBox = (ComboBox)base.GenerateEditingElement(cell, dataItem);

            if (ItemsSourceBinding != null)
                comboBox.SetBinding(ItemsControl.ItemsSourceProperty, ItemsSourceBinding);

            SyncProperties(comboBox);

            return comboBox;
        }

        protected virtual void SyncProperties(FrameworkElement e)
        {
            WpfHelper.SyncProperty(this, e, TextFieldAssist.HasClearButtonProperty, HasClearButtonProperty);
            WpfHelper.SyncProperty(this, e, IconAssist.IconProperty, IconProperty);
            WpfHelper.SyncProperty(this, e, ComboBox.IsEditableProperty, IsEditableProperty);
        }

        /// <summary>
        ///     Assigns the ElementStyle to the desired property on the given element.
        /// </summary>
        protected void ApplyStyle(bool isEditing, FrameworkElement element)
        {
            var style = isEditing ? EditingElementStyle : ElementStyle;
            if (style != null && style.TargetType == element.GetType())
                element.Style = style;
        }

        /// <summary>
        ///     Assigns the Binding to the desired property on the target object.
        /// </summary>
        protected void ApplyBinding(DependencyObject target, DependencyProperty property) => ApplyBinding(target, property, SelectedValueBinding ?? SelectedItemBinding);

        protected virtual void ApplyBinding(DependencyObject target, DependencyProperty property, BindingBase? bindingBase)
        {
            if (bindingBase != null)
                _ = BindingOperations.SetBinding(target, property, bindingBase);
            else
                BindingOperations.ClearBinding(target, property);
        }

        protected override void CancelCellEdit(FrameworkElement? editingElement, object? uneditedValue)
        {
            (editingElement as ComboBox)?.SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);
            base.CancelCellEdit(editingElement, uneditedValue);
        }

        protected override bool CommitCellEdit(FrameworkElement? editingElement)
        {
            (editingElement as ComboBox)?.SetCurrentValue(ComboBox.IsDropDownOpenProperty, false);
            return base.CommitCellEdit(editingElement);
        }
    }
}
