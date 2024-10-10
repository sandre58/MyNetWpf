// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using MyNet.Utilities.Helpers;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Behaviors
{
    public sealed class ListViewSortBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.OnLoading<ListView>(_ => AddBehavior());
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            RemoveBehavior();
        }

        private void AddBehavior()
        {
            if (AssociatedObject.View is GridView gridView)
            {
                gridView.Columns.ToList().ForEach(InitializeNewColumn);
                gridView.Columns.CollectionChanged += Columns_CollectionChanged;
            }
        }

        private void RemoveBehavior()
        {
            if (AssociatedObject.View is GridView gridView)
            {
                gridView.Columns.CollectionChanged -= Columns_CollectionChanged;
                gridView.Columns.ToList().ForEach(DisposeColumn);
            }
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked)
            {
                var propertyName = GridViewColumnAssist.GetPropertyName(headerClicked.Column);
                var canSort = GridViewColumnAssist.GetCanSort(headerClicked.Column);
                if (!string.IsNullOrEmpty(propertyName) && canSort)
                {
                    ApplySort(headerClicked, propertyName);
                }
            }
        }

        private void InitializeNewColumn(GridViewColumn column)
        {
            var header = ListViewAssist.FindColumnHeader(AssociatedObject, column);

            if (header != null)
            {
                header.Click += ColumnHeader_Click;
                GridViewColumnHeaderAssist.SetCanSort(header, GridViewColumnAssist.GetCanSort(column));
            }
        }

        private void DisposeColumn(GridViewColumn column)
        {
            var header = ListViewAssist.FindColumnHeader(AssociatedObject, column);

            if (header != null)
            {
                GridViewColumnHeaderAssist.SetSortDirection(header, null);
                header.Click -= ColumnHeader_Click;
            }
        }

        private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            => CollectionHelper.OnCollectionChanged<GridViewColumn>(e.OldItems, e.NewItems, DisposeColumn, InitializeNewColumn);

        private void ApplySort(GridViewColumnHeader clickedColumnHeader, string propertyName)
        {
            var direction = GetSortDirectionInternal(clickedColumnHeader);

            if (ListViewAssist.GetSortCommand(AssociatedObject) != null)
            {
                ListViewAssist.GetSortCommand(AssociatedObject).Execute(new List<(string, ListSortDirection)> { (propertyName, direction) });
            }
            else
            {
                using (AssociatedObject.Items.DeferRefresh())
                {
                    AssociatedObject.Items.SortDescriptions.Clear();
                    AssociatedObject.Items.SortDescriptions.Add(new SortDescription(propertyName, direction));
                }

                (AssociatedObject.View as GridView)?.Columns.ToList().ForEach(x =>
                {
                    var columnHeader = ListViewAssist.FindColumnHeader(AssociatedObject, x);
                    if (columnHeader is null) return;
                    GridViewColumnHeaderAssist.SetSortDirection(columnHeader, null);
                });
                GridViewColumnHeaderAssist.SetSortDirection(clickedColumnHeader, direction);
            }
        }

        private static ListSortDirection GetSortDirectionInternal(GridViewColumnHeader clickedColumnHeader)
        {
            var oldDirection = GridViewColumnHeaderAssist.GetSortDirection(clickedColumnHeader);
            return !oldDirection.HasValue
                ? ListSortDirection.Ascending
                : oldDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }
    }
}
