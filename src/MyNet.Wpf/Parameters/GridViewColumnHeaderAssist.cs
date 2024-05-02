// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class GridViewColumnHeaderAssist
    {
        #region CanSort

        public static readonly DependencyProperty CanSortProperty = DependencyProperty.RegisterAttached(
            "CanSort",
            typeof(bool),
            typeof(GridViewColumnHeaderAssist),
            new PropertyMetadata(true));


        public static bool GetCanSort(GridViewColumnHeader item) => (bool)item.GetValue(CanSortProperty);

        public static void SetCanSort(GridViewColumnHeader item, bool value) => item.SetValue(CanSortProperty, value);

        #endregion

        #region SortDirection

        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached(
            "SortDirection",
            typeof(ListSortDirection?),
            typeof(GridViewColumnHeaderAssist),
            new PropertyMetadata(null));

        public static ListSortDirection? GetSortDirection(GridViewColumnHeader item) => (ListSortDirection?)item.GetValue(SortDirectionProperty);

        internal static void SetSortDirection(GridViewColumnHeader item, ListSortDirection? value) => item.SetValue(SortDirectionProperty, value);

        #endregion
    }
}
