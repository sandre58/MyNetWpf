// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using MyNet.Wpf.Helpers;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Controls
{
    public class DataGridTextColumn : System.Windows.Controls.DataGridTextColumn
    {
        static DataGridTextColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridTextColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridTextColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        public static new Style DefaultElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.ContentControl.Embedded.DataGrid");

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.TextBox.Embedded.DataGrid.Edition");

        public static readonly DependencyProperty HasClearButtonProperty
            = DependencyProperty.Register(nameof(HasClearButton), typeof(bool), typeof(DataGridTextColumn), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool HasClearButton
        {
            get => (bool)GetValue(HasClearButtonProperty);
            set => SetValue(HasClearButtonProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(DataGridTextColumn), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
                                                                            typeof(DataGridTextColumn),
                                                                            new FrameworkPropertyMetadata(null));

        public DataTemplateSelector CellTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(CellTemplateSelectorProperty);
            set => SetValue(CellTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(
                                                                                    "CellTemplateSelector",
                                                                                    typeof(DataTemplateSelector),
                                                                                    typeof(DataGridTextColumn),
                                                                                    new FrameworkPropertyMetadata(null));

        #region Element Generation

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
            var textBox = new TextBox();

            SyncProperties(textBox);

            ApplyStyle(true, textBox);
            ApplyBinding(textBox, TextBox.TextProperty);

            return textBox;
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
        protected void ApplyBinding(DependencyObject target, DependencyProperty property) => ApplyBinding(target, property, Binding);

        protected virtual void ApplyBinding(DependencyObject target, DependencyProperty property, BindingBase? bindingBase)
        {
            if (bindingBase != null)
                _ = BindingOperations.SetBinding(target, property, bindingBase);
            else
                BindingOperations.ClearBinding(target, property);
        }

        protected virtual void SyncProperties(FrameworkElement e)
        {
            WpfHelper.SyncProperty(this, e, TextElement.FontSizeProperty, FontSizeProperty);
            WpfHelper.SyncProperty(this, e, TextElement.FontStyleProperty, FontStyleProperty);
            WpfHelper.SyncProperty(this, e, TextElement.FontWeightProperty, FontWeightProperty);
            WpfHelper.SyncProperty(this, e, TextElement.ForegroundProperty, ForegroundProperty);
            WpfHelper.SyncProperty(this, e, TextElement.FontFamilyProperty, FontFamilyProperty);
            WpfHelper.SyncProperty(this, e, TextFieldAssist.HasClearButtonProperty, HasClearButtonProperty);
            WpfHelper.SyncProperty(this, e, IconAssist.IconProperty, IconProperty);
        }

        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            if (element is DataGridCell cell && cell.Content is FrameworkElement textElement)
            {
                switch (propertyName)
                {
                    case "HasClearButton":
                        textElement.SetValue(TextFieldAssist.HasClearButtonProperty, GetValue(HasClearButtonProperty));
                        break;

                    case "Icon":
                        textElement.SetValue(IconAssist.IconProperty, GetValue(IconProperty));
                        break;
                }
            }

            base.RefreshCellContent(element, propertyName);
        }

        #endregion
    }
}
