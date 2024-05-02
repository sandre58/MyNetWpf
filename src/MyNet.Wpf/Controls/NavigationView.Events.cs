// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MyNet.UI.Navigation.Models;

namespace MyNet.Wpf.Controls
{
    public partial class NavigationView
    {
        public static readonly RoutedEvent PaneOpenedEvent = EventManager.RegisterRoutedEvent(nameof(PaneOpened), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, RoutedEventArgs>), typeof(NavigationView));
        public static readonly RoutedEvent PaneClosedEvent = EventManager.RegisterRoutedEvent(nameof(PaneClosed), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, RoutedEventArgs>), typeof(NavigationView));
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, RoutedEventArgs>), typeof(NavigationView));
        public static readonly RoutedEvent ItemInvokedEvent = EventManager.RegisterRoutedEvent(nameof(ItemInvoked), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, RoutedEventArgs>), typeof(NavigationView));
        public static readonly RoutedEvent NavigatingEvent = EventManager.RegisterRoutedEvent(nameof(Navigating), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, NavigatingCancelEventArgs>), typeof(NavigationView));
        public static readonly RoutedEvent NavigatedEvent = EventManager.RegisterRoutedEvent(nameof(Navigated), RoutingStrategy.Bubble, typeof(TypedEventHandler<NavigationView, NavigatedEventArgs>), typeof(NavigationView));

        public event TypedEventHandler<NavigationView, RoutedEventArgs> PaneOpened
        {
            add => AddHandler(PaneOpenedEvent, value);
            remove => RemoveHandler(PaneOpenedEvent, value);
        }

        public event TypedEventHandler<NavigationView, RoutedEventArgs> PaneClosed
        {
            add => AddHandler(PaneClosedEvent, value);
            remove => RemoveHandler(PaneClosedEvent, value);
        }

        public event TypedEventHandler<NavigationView, RoutedEventArgs> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public event TypedEventHandler<NavigationView, RoutedEventArgs> ItemInvoked
        {
            add => AddHandler(ItemInvokedEvent, value);
            remove => RemoveHandler(ItemInvokedEvent, value);
        }

        public event TypedEventHandler<NavigationView, NavigatingCancelEventArgs> Navigating
        {
            add => AddHandler(NavigatingEvent, value);
            remove => RemoveHandler(NavigatingEvent, value);
        }

        public event TypedEventHandler<NavigationView, NavigatedEventArgs> Navigated
        {
            add => AddHandler(NavigatedEvent, value);
            remove => RemoveHandler(NavigatedEvent, value);
        }

        protected virtual void OnPaneOpened() => RaiseEvent(new RoutedEventArgs(PaneOpenedEvent, this));

        protected virtual void OnPaneClosed() => RaiseEvent(new RoutedEventArgs(PaneClosedEvent, this));

        protected virtual void OnSelectionChanged() => RaiseEvent(new RoutedEventArgs(SelectionChangedEvent, this));

        protected virtual void OnItemInvoked() => RaiseEvent(new RoutedEventArgs(ItemInvokedEvent, this));

        protected virtual bool OnNavigating(INavigationPage page)
        {
            var eventArgs = new NavigatingCancelEventArgs(NavigatingEvent, this) { Page = page };

            RaiseEvent(eventArgs);

            return !eventArgs.Cancel;
        }

        protected virtual void OnNavigated(INavigationPage page)
        {
            var eventArgs = new NavigatedEventArgs(NavigatedEvent, this) { Page = page };

            RaiseEvent(eventArgs);
        }
    }
}
