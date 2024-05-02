// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Parameters
{
    public static class ItemsControlAssist
    {
        #region EmptyTemplate

        public static readonly DependencyProperty EmptyTemplateProperty = DependencyProperty.RegisterAttached(
            "EmptyTemplate",
            typeof(ControlTemplate),
            typeof(ItemsControlAssist));

        public static ControlTemplate GetEmptyTemplate(ItemsControl item) => (ControlTemplate)item.GetValue(EmptyTemplateProperty);

        public static void SetEmptyTemplate(ItemsControl item, ControlTemplate value) => item.SetValue(EmptyTemplateProperty, value);

        #endregion EmptyTemplate

        #region ManyTemplate

        public static readonly DependencyProperty ManyTemplateProperty = DependencyProperty.RegisterAttached(
            "ManyTemplate",
            typeof(ControlTemplate),
            typeof(ItemsControlAssist));

        public static ControlTemplate GetManyTemplate(ItemsControl item) => (ControlTemplate)item.GetValue(ManyTemplateProperty);

        public static void SetManyTemplate(ItemsControl item, ControlTemplate value) => item.SetValue(ManyTemplateProperty, value);

        #endregion ManyTemplate

        #region SingleTemplate

        public static readonly DependencyProperty SingleTemplateProperty = DependencyProperty.RegisterAttached(
            "SingleTemplate",
            typeof(ControlTemplate),
            typeof(ItemsControlAssist));

        public static ControlTemplate GetSingleTemplate(ItemsControl item) => (ControlTemplate)item.GetValue(SingleTemplateProperty);

        public static void SetSingleTemplate(ItemsControl item, ControlTemplate value) => item.SetValue(SingleTemplateProperty, value);

        #endregion SingleTemplate

        #region Sorting

        #region SortingProperty

        public static readonly DependencyProperty SortingPropertyProperty = DependencyProperty.RegisterAttached(
            "SortingProperty",
            typeof(string),
            typeof(ItemsControlAssist),
            new PropertyMetadata(string.Empty, OnSortingChangedChanged));

        public static string GetSortingProperty(ItemsControl item) => (string)item.GetValue(SortingPropertyProperty);

        public static void SetSortingProperty(ItemsControl item, string value) => item.SetValue(SortingPropertyProperty, value);

        #endregion SortingProperty

        #region SortDirection

        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached(
            "SortDirection",
            typeof(ListSortDirection),
            typeof(ItemsControlAssist),
            new PropertyMetadata(ListSortDirection.Ascending, OnSortingChangedChanged));

        public static ListSortDirection GetSortDirection(ItemsControl item) => (ListSortDirection)item.GetValue(SortDirectionProperty);

        public static void SetSortDirection(ItemsControl item, ListSortDirection value) => item.SetValue(SortDirectionProperty, value);

        #endregion SortDirection

        private static void OnSortingChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ItemsControl element || string.IsNullOrEmpty(GetSortingProperty(element))) return;

            if (element.IsLoaded)
            {
                var properties = GetSortingProperty(element).Split(";");
                var sortDirection = GetSortDirection(element);
                UpdateSort(element, properties.Select(x => (x, sortDirection)));
            }
            else
            {
                element.Loaded += listView_Loaded;
            }

            void listView_Loaded(object sender, RoutedEventArgs e)
            {
                var properties = GetSortingProperty(element).Split(";");
                var sortDirection = GetSortDirection(element);
                UpdateSort(element, properties.Select(x => (x, sortDirection)));
                element.Loaded -= listView_Loaded;
            }
        }

        private static void UpdateSort(ItemsControl control, IEnumerable<(string property, ListSortDirection direction)> sorts)
        {
            using (control.Items.DeferRefresh())
            {
                control.Items.SortDescriptions.Clear();
                control.Items.SortDescriptions.AddRange(sorts.Select(x => new SortDescription(x.property, x.direction)));
            }
        }

        #endregion

        #region GroupingProperty

        public static readonly DependencyProperty GroupingPropertyProperty = DependencyProperty.RegisterAttached(
            "GroupingProperty",
            typeof(string),
            typeof(ItemsControlAssist),
            new PropertyMetadata(string.Empty, OnGroupChanged));

        public static string GetGroupingProperty(ItemsControl item) => (string)item.GetValue(GroupingPropertyProperty);

        public static void SetGroupingProperty(ItemsControl item, string value) => item.SetValue(GroupingPropertyProperty, value);

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ItemsControl element) return;

            if (element.IsLoaded)
            {
                var groups = GetGroupingProperty(element)?.Split(";") ?? [];
                UpdateGroups(element, groups);
            }
            else
            {
                element.Loaded += listView_Loaded;
            }

            void listView_Loaded(object sender, RoutedEventArgs e)
            {
                var groups = GetGroupingProperty(element)?.Split(";") ?? [];
                UpdateGroups(element, groups);
                element.Loaded -= listView_Loaded;
            }
        }

        private static void UpdateGroups(ItemsControl control, IEnumerable<string> groups)
        {
            using (control.Items.DeferRefresh())
            {
                control.Items.GroupDescriptions.Clear();
                control.Items.GroupDescriptions.AddRange(groups.Where(x => !string.IsNullOrEmpty(x)).Select(x => new PropertyGroupDescription(x)));
            }
        }
        #endregion GroupingProperty
    }
}
