// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Controls.Dialogs
{
    public class DialogClosingEventArgs(RoutedEvent routedEvent, bool? dialogResult) : RoutedEventArgs(routedEvent)
    {

        /// <summary>
        /// Cancel the close.
        /// </summary>
        public void Cancel() => IsCancelled = true;

        /// <summary>
        /// Indicates if the close has already been cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        public bool? DialogResult { get; } = dialogResult;
    }

    public delegate void DialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs);
}
