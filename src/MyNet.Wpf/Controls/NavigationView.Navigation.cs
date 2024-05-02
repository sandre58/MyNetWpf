// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyNet.UI.Locators;
using MyNet.UI.Navigation;
using MyNet.UI.Navigation.Models;

namespace MyNet.Wpf.Controls
{
    public partial class NavigationView
    {
        public virtual void GoForward() => NavigationService?.GoForward();

        public virtual void GoBack() => NavigationService?.GoBack();

        private bool Navigate(NavigationViewItem navigationViewItem)
            => navigationViewItem.TargetPage is not null
            && OnNavigating(navigationViewItem.TargetPage)
            && (NavigationService?.NavigateTo(navigationViewItem.TargetPage, navigationViewItem.NavigationParameters) ?? false);

        private void OnNavigatedCallback(object? sender, NavigationEventArgs e)
        {
            var view = GetViewFromNavigationPage(e.NewPage);

            if (view is null) return;

            if (!Equals(_navigationViewContentPresenter?.Content, view))
            {
                UpdateContent(view, e.NewPage);
            }

            UpdateNavigationControls();

            OnNavigated(e.NewPage);
        }

        private void UpdateNavigationControls()
        {
            UpdateItems();

            IsBackEnabled = NavigationService?.CanGoBack() ?? false;
            IsForwardEnabled = NavigationService?.CanGoForward() ?? false;
            Header = SelectedItem?.Header;
        }

        private void UpdateItems() => SetActiveItems(GetAllNavigationViewItems());

        private void UpdateContent(object view, object? dataContext = null)
        {
            if (dataContext is not null && view is FrameworkElement frameworkViewContent)
                frameworkViewContent.DataContext = dataContext;

            _navigationViewContentPresenter?.Navigate(view);
        }

        private void SetActiveItems(IEnumerable<NavigationViewItem> items)
        {
            foreach (var item in items)
            {
                var canBeActivatedByNavigation = NavigationService.CurrentContext is not null && item.TargetPage is not null;
                var parentItem = canBeActivatedByNavigation && Equals(item.TargetPage?.GetType(), NavigationService.CurrentContext?.Page.GetParentPageType());
                var currentItem = canBeActivatedByNavigation
                   && Equals(item.TargetPage, NavigationService.CurrentContext?.Page)
                   && (item.NavigationParameters is null || item.NavigationParameters.IsSimilar(NavigationService.CurrentContext?.Parameters));
                if (parentItem || currentItem)
                {
                    item.Activate();
                    ExpandItem(item, ExpandOnlyActiveItem);
                    SelectedItem = item;
                }
                else
                {
                    item.Deactivate();
                }

                SetActiveItems(item.GetNavigationViewItems().ToList());
            }
        }

        public IEnumerable<NavigationViewItem> GetAllNavigationViewItems()
            => Items.OfType<object>().Select(x => ItemContainerGenerator.ContainerFromItem(x)).OfType<NavigationViewItem>().Concat(FooterMenuItems.OfType<NavigationViewItem>());

        private static FrameworkElement? GetViewFromNavigationPage(INavigationPage page)
            => page is FrameworkElement fe ? fe : ViewManager.GetNullableView(page.GetType()) is FrameworkElement frameworkElement ? frameworkElement : null;
    }
}
