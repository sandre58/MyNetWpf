// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MyNet.UI.Navigation.Models;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    [ToolboxItem(true)]
    public partial class NavigationView : HeaderedItemsControl, ISuggestionProvider
    {
        private sealed class AutoSuggestItem(INavigationPage targetPage, object header, NavigationParameters? parameters = null)
        {
            public INavigationPage TargetPage { get; } = targetPage;

            public NavigationParameters? Parameters { get; } = parameters;

            public object Header { get; } = header;

            public override string? ToString() => Header?.ToString();
        }

        static NavigationView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationView), new FrameworkPropertyMetadata(typeof(NavigationView)));

        public NavigationView()
        {
            FooterMenuItems = [];

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            InvalidateArrange();
            InvalidateVisual();
            UpdateLayout();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService is not null)
                NavigationService.Navigated += OnNavigatedCallback;

            if (NavigationService?.CurrentContext?.Page is not null)
            {
                var view = GetViewFromNavigationPage(NavigationService.CurrentContext.Page);

                if (view is null) return;

                if (!Equals(_navigationViewContentPresenter?.Content, view))
                    UpdateContent(view, NavigationService.CurrentContext.Page);
            }
            UpdateNavigationControls();
        }

        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (NavigationService is not null)
                NavigationService.Navigated -= OnNavigatedCallback;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //Back button
            if (e.ChangedButton is MouseButton.XButton1)
            {
                GoBack();
                e.Handled = true;
            }
            if (e.ChangedButton is MouseButton.XButton2)
            {
                GoForward();
                e.Handled = true;
            }

            base.OnMouseDown(e);
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e) => IsPaneOpen = !IsPaneOpen;

        private void OnPreviousButtonClick(object sender, RoutedEventArgs e) => NavigationService?.GoBack();

        private void OnNextButtonClick(object sender, RoutedEventArgs e) => NavigationService?.GoForward();

        protected virtual void OnAutoSuggestButtonClick(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = true;
            _autoSuggestBox?.Focus();
        }

        private void NavigationViewItem_Click(object sender, RoutedEventArgs e) => OnNavigationViewItemClick((sender as NavigationViewItem)!);

        internal void OnNavigationViewItemClick(NavigationViewItem navigationViewItem)
        {
            OnItemInvoked();

            var hasNavigated = Navigate(navigationViewItem);

            if (!hasNavigated && navigationViewItem.HasItems)
            {
                if (navigationViewItem.IsExpanded)
                    navigationViewItem.Collapse();
                else
                    ExpandItem(navigationViewItem, false);
            }
        }

        private void ExpandItem(NavigationViewItem navigationViewItem, bool collapseOthers)
        {
            var allTopLevelItems = GetAllNavigationViewItems().ToList();
            var firstParentWithItems = navigationViewItem.HasItems ? navigationViewItem : GetParentOfChild(allTopLevelItems, navigationViewItem);

            if (collapseOthers)
            {
                var itemsToCollapse = GetAllNavigationViewItems().Except(new[] { firstParentWithItems }).ToList();
                itemsToCollapse.ForEach(x => x?.Collapse());
            }

            if (firstParentWithItems is not null)
            {
                firstParentWithItems.Expand();
                IsPaneOpen = true;
            }
        }

        private static NavigationViewItem? GetParentOfChild(List<NavigationViewItem> items, NavigationViewItem child)
            => items.Contains(child) ? null : items.Find(x => x.GetNavigationViewItems().Contains(child)) is NavigationViewItem navigationViewItem ? navigationViewItem : GetParentOfChild(items.SelectMany(x => x.GetNavigationViewItems()).ToList(), child);

        protected override DependencyObject GetContainerForItemOverride() => new NavigationViewItem();

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is not NavigationViewItem navigationViewItem) return;

            navigationViewItem.SetNavigationView(this);

            navigationViewItem.Click -= NavigationViewItem_Click;
            navigationViewItem.Click += NavigationViewItem_Click;
        }

        private void AutoSuggestBoxCommitSelectedItem(object? sender, EventArgs args)
        {
            var element = _autoSuggestBox?.SelectedItem;

            if (element is AutoSuggestItem autoSuggestItem)
                NavigationService?.NavigateTo(autoSuggestItem.TargetPage, autoSuggestItem.Parameters);
        }

        private static List<AutoSuggestItem> GetAllTargetableItems(IEnumerable<NavigationViewItem> items)
        {
            var result = new List<AutoSuggestItem>();

            foreach (var item in items)
            {
                if (item.TargetPage is not null)
                {
                    result.Add(new AutoSuggestItem(item.TargetPage, item.Header));
                }

                result.AddRange(GetAllTargetableItems(item.GetNavigationViewItems()));
            }
            return result;
        }

        public IEnumerable GetSuggestions(string? filter) => Dispatcher.Invoke(() => GetAllTargetableItems(GetAllNavigationViewItems().ToList()).Where(x => x.Header?.ToString()?.Contains(filter.OrEmpty(), StringComparison.OrdinalIgnoreCase) ?? false));
    }
}
