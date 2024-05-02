// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class GridAssist
    {
        #region RowCount Property

        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(GridAssist),
                new PropertyMetadata(-1, RowCountChanged));

        public static int GetRowCount(DependencyObject obj) => (int)obj.GetValue(RowCountProperty);

        public static void SetRowCount(DependencyObject obj, int value) => obj.SetValue(RowCountProperty, value);

        public static void RowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is not Grid || (int)e.NewValue < 0)
            {
                return;
            }

            var grid = (Grid)obj;
            grid.RowDefinitions.Clear();

            for (var i = 0; i < (int)e.NewValue; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }
        }

        #endregion

        #region ColumnCount Property

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(GridAssist),
                new PropertyMetadata(-1, ColumnCountChanged));

        public static int GetColumnCount(DependencyObject obj) => (int)obj.GetValue(ColumnCountProperty);

        public static void SetColumnCount(DependencyObject obj, int value) => obj.SetValue(ColumnCountProperty, value);

        public static void ColumnCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is not Grid || (int)e.NewValue < 0)
            {
                return;
            }

            var grid = (Grid)obj;
            grid.ColumnDefinitions.Clear();

            for (var i = 0; i < (int)e.NewValue; i++)
            {
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }
        }

        #endregion
    }
}
