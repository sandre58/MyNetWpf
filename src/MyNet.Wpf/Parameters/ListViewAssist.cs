// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DynamicData.Binding;
using Microsoft.Xaml.Behaviors;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.Layout;
using MyNet.Utilities;
using MyNet.Wpf.Behaviors;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Schedulers;

namespace MyNet.Wpf.Parameters
{
    public static class ListViewAssist
    {
        #region ItemContextMenu

        public static readonly DependencyProperty ItemContextMenuProperty = DependencyProperty.RegisterAttached(
            "ItemContextMenu",
            typeof(ContextMenu),
            typeof(ListViewAssist),
            new PropertyMetadata(null));

        public static ContextMenu GetItemContextMenu(ListView item) => (ContextMenu)item.GetValue(ItemContextMenuProperty);

        public static void SetItemContextMenu(ListView item, ContextMenu value) => item.SetValue(ItemContextMenuProperty, value);

        #endregion ItemContextMenu

        #region ShowHeader

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.RegisterAttached(
            "ShowHeader",
            typeof(bool),
            typeof(ListViewAssist),
            new PropertyMetadata(true));

        public static bool GetShowHeader(ListView item) => (bool)item.GetValue(ShowHeaderProperty);

        public static void SetShowHeader(ListView item, bool value) => item.SetValue(ShowHeaderProperty, value);

        #endregion ShowHeader

        #region HeaderHeight

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.RegisterAttached(
            "HeaderHeight",
            typeof(double),
            typeof(ListViewAssist),
            new PropertyMetadata(45.0));

        public static double GetHeaderHeight(ListView item) => (double)item.GetValue(HeaderHeightProperty);

        public static void SetHeaderHeight(ListView item, double value) => item.SetValue(HeaderHeightProperty, value);

        #endregion HeaderHeight

        #region ItemMinHeight

        public static readonly DependencyProperty ItemMinHeightProperty = DependencyProperty.RegisterAttached(
            "ItemMinHeight",
            typeof(double),
            typeof(ListViewAssist),
            new PropertyMetadata(30.0));

        public static double GetItemMinHeight(ListView item) => (double)item.GetValue(ItemMinHeightProperty);

        public static void SetItemMinHeight(ListView item, double value) => item.SetValue(ItemMinHeightProperty, value);

        #endregion ItemMinHeight

        #region LayoutBehavior

        #region LayoutBehavior

        private static readonly DependencyProperty LayoutBehaviorProperty = DependencyProperty.RegisterAttached("LayoutBehavior", typeof(ListViewLayoutBehavior), typeof(ListViewAssist));

        private static ListViewLayoutBehavior GetLayoutBehavior(ListView obj) => (ListViewLayoutBehavior)obj.GetValue(LayoutBehaviorProperty);

        private static void SetLayoutBehavior(ListView obj, ListViewLayoutBehavior value) => obj.SetValue(LayoutBehaviorProperty, value);

        #endregion

        #region ColumnLayouts

        public static readonly DependencyProperty ColumnLayoutsProperty = DependencyProperty.RegisterAttached("ColumnLayouts", typeof(ColumnLayoutCollection), typeof(ListViewAssist));

        public static ColumnLayoutCollection GetColumnLayouts(ListView obj) => (ColumnLayoutCollection)obj.GetValue(ColumnLayoutsProperty);

        public static void SetColumnLayouts(ListView obj, ColumnLayoutCollection value) => obj.SetValue(ColumnLayoutsProperty, value);

        #endregion

        #region AutoResizeIsEnabled

        public static readonly DependencyProperty AutoResizeIsEnabledProperty = DependencyProperty.RegisterAttached("AutoResizeIsEnabled", typeof(bool), typeof(ListViewAssist), new PropertyMetadata(false, OnAutoResizeIsEnabledChanged));

        public static bool GetAutoResizeIsEnabled(ListView obj) => (bool)obj.GetValue(AutoResizeIsEnabledProperty);

        public static void SetAutoResizeIsEnabled(ListView obj, bool value) => obj.SetValue(AutoResizeIsEnabledProperty, value);

        private static void OnAutoResizeIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ListView listView) return;

            var itemBehaviors = Interaction.GetBehaviors(listView);
            var behavior = GetLayoutBehavior(listView);

            if ((bool)args.NewValue && !itemBehaviors.Any(x => x is ListViewLayoutBehavior))
            {
                if (behavior is null)
                {
                    behavior = new ListViewLayoutBehavior();
                    SetLayoutBehavior(listView, behavior);
                }
                itemBehaviors.Add(behavior);
            }

            if (!(bool)args.NewValue && itemBehaviors.Any(x => x is ListViewLayoutBehavior) && behavior is not null)
            {
                _ = itemBehaviors.Remove(behavior);
            }
        }

        #endregion

        #endregion

        #region SortBehavior

        private static readonly DependencyProperty SortBehaviorProperty = DependencyProperty.RegisterAttached("SortBehavior", typeof(ListViewSortBehavior), typeof(ListViewAssist));

        private static ListViewSortBehavior GetSortBehavior(ListView obj) => (ListViewSortBehavior)obj.GetValue(SortBehaviorProperty);

        private static void SetSortBehavior(ListView obj, ListViewSortBehavior value) => obj.SetValue(SortBehaviorProperty, value);

        #region CanSort

        public static readonly DependencyProperty CanSortProperty
            = DependencyProperty.RegisterAttached(@"CanSort", typeof(bool), typeof(ListViewAssist), new FrameworkPropertyMetadata(false, OnCanSortChanged));

        public static bool GetCanSort(DependencyObject uie) => (bool)uie.GetValue(CanSortProperty);

        public static void SetCanSort(DependencyObject uie, bool value) => uie.SetValue(CanSortProperty, value);

        private static void OnCanSortChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not ListView listView) return;

            var itemBehaviors = Interaction.GetBehaviors(listView);
            var behavior = GetSortBehavior(listView);

            if ((bool)args.NewValue && !itemBehaviors.Any(x => x is ListViewSortBehavior))
            {
                if (behavior is null)
                {
                    behavior = new ListViewSortBehavior();
                    SetSortBehavior(listView, behavior);
                }
                itemBehaviors.Add(behavior);
            }

            if (!(bool)args.NewValue && itemBehaviors.Any(x => x is ListViewSortBehavior) && behavior is not null)
            {
                _ = itemBehaviors.Remove(behavior);
            }
        }

        #endregion CanSort

        #region SortCommand

        public static readonly DependencyProperty SortCommandProperty = DependencyProperty.RegisterAttached(
            "SortCommand",
            typeof(ICommand),
            typeof(ListViewAssist),
            new PropertyMetadata(null));

        public static ICommand GetSortCommand(ListView item) => (ICommand)item.GetValue(SortCommandProperty);

        public static void SetSortCommand(ListView item, ICommand value) => item.SetValue(SortCommandProperty, value);

        #endregion SortCommand

        #region SortDescriptions

        public static readonly DependencyProperty SortDescriptionsProperty = DependencyProperty.RegisterAttached("SortDescriptions", typeof(ICollection<SortDescription>), typeof(ListViewAssist), new PropertyMetadata(OnSortDescriptionsCallback));

        public static ICollection<SortDescription> GetSortDescriptions(ListView obj) => (ICollection<SortDescription>)obj.GetValue(SortDescriptionsProperty);

        public static void SetSortDescriptions(ListView obj, ICollection<SortDescription> value) => obj.SetValue(SortDescriptionsProperty, value);

        private static void OnSortDescriptionsCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            IDisposable? disposable = null;

            dependencyObject.OnLoading<ListView>(x =>
            {
                if (e.NewValue is not ICollection<SortDescription> sortDescriptions) return;

                if (sortDescriptions is ObservableCollection<SortDescription> sortingProperties)
                    disposable = sortingProperties.ToObservableChangeSet().ObserveOn(WpfScheduler.Current).Subscribe(_ => UpdateSortDirections(x, sortingProperties.ToDictionary(y => y.PropertyName, y => y.Direction)));
                UpdateSortDirections(x, sortDescriptions.ToDictionary(y => y.PropertyName, y => y.Direction));
            },
            x => disposable?.Dispose());
        }

        public static void UpdateSortDirections(ListView listView, IReadOnlyDictionary<string, ListSortDirection> sortDescriptions)
        {
            if (listView.View is GridView gv)
            {
                foreach (var column in gv.Columns)
                {
                    var header = FindColumnHeader(listView, column);

                    if (header != null)
                    {
                        var propertyName = GridViewColumnAssist.GetPropertyName(column).OrEmpty();
                        var found = sortDescriptions.ContainsKey(propertyName);

                        if (!found)
                        {
                            GridViewColumnHeaderAssist.SetSortDirection(header, null);
                        }
                        else
                        {
                            GridViewColumnHeaderAssist.SetSortDirection(header, sortDescriptions[propertyName]);
                        }
                    }
                }
            }
        }

        internal static GridViewColumnHeader? FindColumnHeader(DependencyObject listView, GridViewColumn column)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(listView); i++)
            {
                var childVisual = (Visual)VisualTreeHelper.GetChild(listView, i);
                if (childVisual is GridViewColumnHeader gridViewHeader && gridViewHeader.Column == column)
                {
                    return gridViewHeader;
                }
                var childGridViewHeader = FindColumnHeader(childVisual, column);
                if (childGridViewHeader != null)
                {
                    return childGridViewHeader;
                }
            }
            return null;
        }

        internal static GridViewColumn? FindColumn(GridView gridView, string propertyName) => gridView.Columns.FirstOrDefault(column => GridViewColumnAssist.GetPropertyName(column) == propertyName);

        #endregion SortDescriptions

        #endregion

        #region ListViewItemPadding

        public static readonly DependencyProperty ListViewItemPaddingProperty = DependencyProperty.RegisterAttached(
            "ListViewItemPadding",
            typeof(Thickness),
            typeof(ListViewAssist),
            new FrameworkPropertyMetadata(new Thickness(8, 8, 8, 8), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetListViewItemPadding(DependencyObject element, Thickness value) => element.SetValue(ListViewItemPaddingProperty, value);

        public static Thickness GetListViewItemPadding(DependencyObject element) => (Thickness)element.GetValue(ListViewItemPaddingProperty);

        #endregion
    }
}
