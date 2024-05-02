// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using MyNet.UI.Commands;
using MyNet.UI.Navigation;

namespace MyNet.Wpf.Commands
{
    public static class NavigationCommands
    {
        public static ICommand GoBackCommand { get; } = CommandsManager.Create(GoBack, CanGoBack);

        public static ICommand GoForwardCommand { get; } = CommandsManager.Create(GoForward, CanGoForward);

        public static ICommand NavigateCommand => CommandsManager.CreateNotNull<Type>(x => NavigationManager.NavigateTo(x));

        private static void GoBack() => NavigationManager.GoBack();

        private static bool CanGoBack() => NavigationManager.CanGoBack();

        private static void GoForward() => NavigationManager.GoForward();

        private static bool CanGoForward() => NavigationManager.CanGoForward();
    }
}
