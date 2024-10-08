﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using DynamicData.Binding;
using MyNet.Observable.Collections.Sorting;
using MyNet.UI.ViewModels;
using MyNet.UI.ViewModels.Display;
using MyNet.UI.ViewModels.List.Grouping;
using MyNet.Utilities;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Parameters;
using MyNet.Wpf.Schedulers;

namespace MyNet.Wpf.Presentation.Parameters
{
    public static class ListAssist
    {
        #region SortingProperties

        public static readonly DependencyProperty SortingPropertiesProperty = DependencyProperty.RegisterAttached("SortingProperties", typeof(ICollection), typeof(ListAssist), new PropertyMetadata(OnSortingPropertiesCallback));

        public static ICollection GetSortingProperties(ListView obj) => (ICollection)obj.GetValue(SortingPropertiesProperty);

        public static void SetSortingProperties(ListView obj, ICollection value) => obj.SetValue(SortingPropertiesProperty, value);

        private static void OnSortingPropertiesCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            IDisposable? disposable = null;

            dependencyObject.OnLoading<ListView>(x =>
            {
                if (e.NewValue is not ICollection<SortingProperty> sortDescriptions) return;

                if (sortDescriptions is ObservableCollection<SortingProperty> sortingProperties)
                    disposable = sortingProperties.ToObservableChangeSet().ObserveOn(WpfScheduler.Current).Subscribe(_ => ListViewAssist.UpdateSortDirections(x, sortingProperties.ToDictionary(y => y.PropertyName, y => y.Direction)));
                ListViewAssist.UpdateSortDirections(x, sortDescriptions.ToDictionary(y => y.PropertyName, y => y.Direction));
            },
            x => disposable?.Dispose());
        }

        #endregion SortingProperties

        #region GroupingPrefix

        public static readonly DependencyProperty GroupingPrefixProperty = DependencyProperty.RegisterAttached("GroupingPrefix", typeof(string), typeof(ListAssist), new PropertyMetadata(OnGroupingPropertiesCallback));

        public static string GetGroupingPrefix(ItemsControl obj) => (string)obj.GetValue(GroupingPrefixProperty);

        public static void SetGroupingPrefix(ItemsControl obj, string value) => obj.SetValue(GroupingPrefixProperty, value);

        #endregion GroupingPrefix

        #region GroupingProperties

        public static readonly DependencyProperty GroupingPropertiesProperty = DependencyProperty.RegisterAttached("GroupingProperties", typeof(ICollection), typeof(ListAssist), new PropertyMetadata(OnGroupingPropertiesCallback));

        public static ICollection GetGroupingProperties(ItemsControl obj) => (ICollection)obj.GetValue(GroupingPropertiesProperty);

        public static void SetGroupingProperties(ItemsControl obj, ICollection value) => obj.SetValue(GroupingPropertiesProperty, value);

        private static void OnGroupingPropertiesCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            IDisposable? disposable = null;

            dependencyObject.OnLoading<ItemsControl>(x =>
            {
                if (e.NewValue is not ICollection<IGroupingPropertyViewModel> groupingCollection) return;

                if (groupingCollection is ObservableCollection<IGroupingPropertyViewModel> groupingProperties)
                    disposable = groupingProperties.ToObservableChangeSet().ObserveOn(WpfScheduler.Current).Subscribe(_ => UpdateGrouping(x, groupingCollection));
                UpdateGrouping(x, groupingCollection);
            },
            x => disposable?.Dispose());
        }

        public static void UpdateGrouping(ItemsControl itemsControl, IEnumerable<IGroupingPropertyViewModel> groups)
        {
            if (GetSynchronizedGrouping(itemsControl))
            {
                var groupStr = string.Join(";", groups.Where(x => x.IsEnabled).Select(x => $"{GetGroupingPrefix(itemsControl)}{x.PropertyName}").ToList());
                ItemsControlAssist.SetGroupingProperty(itemsControl, groupStr);
            }
        }

        #endregion GroupingProperties

        #region SynchronizedList

        public static readonly DependencyProperty SynchronizedListProperty = DependencyProperty.RegisterAttached("SynchronizedList", typeof(IListViewModel), typeof(ListAssist), new PropertyMetadata(null, OnSynchronizedListCallback));

        public static IListViewModel GetSynchronizedList(ItemsControl obj) => (IListViewModel)obj.GetValue(SynchronizedListProperty);

        public static void SetSynchronizedList(ItemsControl obj, IListViewModel value) => obj.SetValue(SynchronizedListProperty, value);

        private static void OnSynchronizedListCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not ItemsControl element || e.NewValue is not IListViewModel listViewModel) return;

            if (element is ListView listView)
            {
                ListViewAssist.SetCanSort(listView, listViewModel.CanSort);
                ListViewAssist.SetSortCommand(listView, listViewModel.Sorting.ApplyCommand);
                SetSortingProperties(listView, listViewModel.CurrentSorting);

                if (listViewModel.Display.AllowedModes.OfType<DisplayModeList>().FirstOrDefault() is DisplayModeList displayModeList)
                {
                    var currentColumns = ListViewAssist.GetColumnLayouts(listView);

                    if (currentColumns is not null)
                        displayModeList.ColumnLayouts.AddRange(currentColumns.Where(x => !displayModeList.ColumnLayouts.Select(y => y.Identifier).Contains(x.Identifier)));

                    ListViewAssist.SetColumnLayouts(listView, displayModeList.ColumnLayouts);
                }
            }

            if (listViewModel.CanGroup)
            {
                if (listViewModel is IWrapperListViewModel)
                    SetGroupingPrefix(element, $"{nameof(IWrapper<object>.Item)}.");

                if (GetSynchronizedGrouping(element))
                    SetGroupingProperties(element, listViewModel.CurrentGroups);
            }
        }

        #endregion SynchronizedList

        #region SynchronizedGrouping

        public static readonly DependencyProperty SynchronizedGroupingProperty = DependencyProperty.RegisterAttached("SynchronizedGrouping", typeof(bool), typeof(ListAssist), new PropertyMetadata(true, OnSynchronizedGroupingCallback));

        public static bool GetSynchronizedGrouping(ItemsControl obj) => (bool)obj.GetValue(SynchronizedGroupingProperty);

        public static void SetSynchronizedGrouping(ItemsControl obj, bool value) => obj.SetValue(SynchronizedGroupingProperty, value);

        private static void OnSynchronizedGroupingCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ItemsControl element || e.NewValue is not bool result) return;

            if (!result)
                SetGroupingProperties(element, Array.Empty<IGroupingPropertyViewModel>());
        }

        #endregion SynchronizedGrouping
    }
}
