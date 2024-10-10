// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static void OnLoading<T>(this DependencyObject? dependencyObject, Action<T> onLoadAction, Action<T>? onUnloadAction = null) where T : FrameworkElement
        {
            if (dependencyObject is not T element) return;

            if (element.IsLoaded)
            {
                onLoadAction(element);
                element.Unloaded -= onUnloaded;
                element.Unloaded += onUnloaded;
            }
            else
            {
                element.Loaded -= onLoaded;
                element.Loaded += onLoaded;
            }

            void onLoaded(object sender, RoutedEventArgs _)
            {
                onLoadAction(element);
                element.Loaded -= onLoaded;
                element.Unloaded -= onUnloaded;
                element.Unloaded += onUnloaded;
            }

            void onUnloaded(object sender, RoutedEventArgs _)
            {
                onUnloadAction?.Invoke(element);
                element.Unloaded -= onUnloaded;
                element.Loaded -= onLoaded;
                element.Loaded += onLoaded;
            }
        }
    }
}
