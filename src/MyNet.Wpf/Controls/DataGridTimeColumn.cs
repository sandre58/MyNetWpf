// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls
{
    public class DataGridTimeColumn : DataGridTextColumn
    {
        static DataGridTimeColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridTimeColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridTimeColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        #region Styles

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.TimePicker.Embedded.DataGrid.Edition");

        #endregion

        #region Element Generation

        /// <summary>
        ///     Creates the visual tree for text based cells.
        /// </summary>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var timePicker = new TimePicker();

            SyncProperties(timePicker);

            ApplyStyle(true, timePicker);
            ApplyBinding(timePicker, TimePicker.SelectedTimeProperty);

            return timePicker;
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
            if (editingElement is TimePicker timePicker)
            {
                _ = timePicker.Focus();

                return timePicker.SelectedTime;
            }

            return null;
        }

        #endregion
    }
}
