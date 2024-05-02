// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls
{
    public class DataGridCheckBoxColumn : System.Windows.Controls.DataGridCheckBoxColumn
    {
        static DataGridCheckBoxColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridCheckBoxColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridCheckBoxColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        public static readonly DependencyProperty IsEnabledProperty
            = UIElement.IsEnabledProperty.AddOwner(
                        typeof(DataGridCheckBoxColumn),
                        new FrameworkPropertyMetadata(true));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        #region Styles

        public static new Style DefaultElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.CheckBox.Embedded.DataGrid");

        public static new Style DefaultEditingElementStyle => (Style)WpfHelper.GetResource("MyNet.Styles.CheckBox.Embedded.DataGrid.Edition");

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var checkbox = base.GenerateEditingElement(cell, dataItem);
            checkbox.IsEnabled = IsEnabled;

            return checkbox;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var checkbox = base.GenerateElement(cell, dataItem);
            checkbox.IsEnabled = IsEnabled;

            return checkbox;
        }

        #endregion
    }
}
