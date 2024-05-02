// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls
{
    public class DataGridColorColumn : DataGridTextColumn
    {
        static DataGridColorColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridColorColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridColorColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #region Styles

        public static new Style DefaultElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.ContentControl.Embedded.DataGrid.Color");

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.ColorPicker.Embedded.DataGrid.Edition");

        #endregion

        #region Element Generation

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var colorPicker = new ColorPicker();

            SyncProperties(colorPicker);

            ApplyStyle(true, colorPicker);
            ApplyBinding(colorPicker, ColorPickerBase.SelectedColorProperty);

            return colorPicker;
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
            if (editingElement is ColorPicker colorPicker)
            {
                _ = colorPicker.Focus();

                return colorPicker.SelectedColor;
            }

            return null;
        }

        #endregion
    }
}
