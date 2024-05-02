// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls.Calendars
{
    /// <summary>
    /// Workaround for Dev10 Bug 527138 UIElement.RaiseEvent(e) throws InvalidCastException when 
    ///     e is of type SelectionChangedEventArgs 
    ///     e.RoutedEvent was registered with a handler not of type System.Windows.Controls.SelectionChangedEventHandler
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="eventId">Routed Event</param>
    /// <param name="removedItems">Items removed from selection</param>
    /// <param name="addedItems">Items added to selection</param>
    public class DatesSelectionChangedEventArgs(RoutedEvent eventId, IList removedItems, IList addedItems) : SelectionChangedEventArgs(eventId, removedItems, addedItems)
    {
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            if (genericHandler is EventHandler<SelectionChangedEventArgs> handler)
            {
                handler(genericTarget, this);
            }
            else
            {
                base.InvokeEventHandler(genericHandler, genericTarget);
            }
        }
    }
}
