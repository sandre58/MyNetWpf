// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    public class DataGridTemplateColumn : System.Windows.Controls.DataGridTextColumn
    {
        /// <summary>
        ///     A template describing how to display data for a cell in this column.
        /// </summary>
        public DataTemplate CellTemplate
        {
            get => (DataTemplate)GetValue(CellTemplateProperty);
            set => SetValue(CellTemplateProperty, value);
        }

        /// <summary>
        ///     The DependencyProperty representing the CellTemplate property.
        /// </summary>
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register(
                                                                            "CellTemplate",
                                                                            typeof(DataTemplate),
                                                                            typeof(DataGridTemplateColumn),
                                                                            new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     A template selector describing how to display data for a cell in this column.
        /// </summary>
        public DataTemplateSelector CellTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(CellTemplateSelectorProperty);
            set => SetValue(CellTemplateSelectorProperty, value);
        }

        /// <summary>
        ///     The DependencyProperty representing the CellTemplateSelector property.
        /// </summary>
        public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.Register(
                                                                                    "CellTemplateSelector",
                                                                                    typeof(DataTemplateSelector),
                                                                                    typeof(DataGridTemplateColumn),
                                                                                    new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     A template describing how to display data for a cell 
        ///     that is being edited in this column.
        /// </summary>
        public DataTemplate CellEditingTemplate
        {
            get => (DataTemplate)GetValue(CellEditingTemplateProperty);
            set => SetValue(CellEditingTemplateProperty, value);
        }

        /// <summary>
        ///     The DependencyProperty representing the CellEditingTemplate
        /// </summary>
        public static readonly DependencyProperty CellEditingTemplateProperty = DependencyProperty.Register(
                                                                                    "CellEditingTemplate",
                                                                                    typeof(DataTemplate),
                                                                                    typeof(DataGridTemplateColumn),
                                                                                    new FrameworkPropertyMetadata(null));

        /// <summary>
        ///     A template selector describing how to display data for a cell 
        ///     that is being edited in this column.
        /// </summary>
        public DataTemplateSelector CellEditingTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty);
            set => SetValue(CellEditingTemplateSelectorProperty, value);
        }

        /// <summary>
        ///     The DependencyProperty representing the CellEditingTemplateSelector
        /// </summary>
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty = DependencyProperty.Register(
                                                                                            "CellEditingTemplateSelector",
                                                                                            typeof(DataTemplateSelector),
                                                                                            typeof(DataGridTemplateColumn),
                                                                                            new FrameworkPropertyMetadata(null));

        private void ChooseCellTemplateAndSelector(bool isEditing, out DataTemplate? template, out DataTemplateSelector? templateSelector)
        {
            template = null;
            templateSelector = null;

            if (isEditing)
            {
                template = CellEditingTemplate;
                templateSelector = CellEditingTemplateSelector;
            }

            if (template == null && templateSelector == null)
            {
                template = CellTemplate;
                templateSelector = CellTemplateSelector;
            }
        }

        /// <summary>
        ///     Creates the visual tree that will become the content of a cell.
        /// </summary>
        /// <param name="isEditing">Whether the editing version is being requested.</param>
        private ContentPresenter? LoadTemplateContent(bool isEditing)
        {
            ChooseCellTemplateAndSelector(isEditing, out var template, out var templateSelector);
            if (template != null || templateSelector != null)
            {
                var contentPresenter = new ContentPresenter();
                Validation.SetErrorTemplate(contentPresenter, null);
                BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, new Binding());
                contentPresenter.ContentTemplate = template;
                contentPresenter.ContentTemplateSelector = templateSelector;
                return contentPresenter;
            }

            return null;
        }

        protected override FrameworkElement? GenerateElement(DataGridCell cell, object dataItem) => LoadTemplateContent(false);

        protected override FrameworkElement? GenerateEditingElement(DataGridCell cell, object dataItem) => LoadTemplateContent(true);

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            editingElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }
    }
}
