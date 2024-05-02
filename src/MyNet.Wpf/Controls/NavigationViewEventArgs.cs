// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MyNet.UI.Navigation.Models;

namespace MyNet.Wpf.Controls
{
    public class NavigatingCancelEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
    {
        public INavigationPage? Page { get; set; }

        public bool Cancel { get; set; }
    }

    public class NavigatedEventArgs(RoutedEvent routedEvent, object source) : RoutedEventArgs(routedEvent, source)
    {
        public INavigationPage? Page { get; set; }
    }
}
