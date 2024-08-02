// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Automation
{
    /// <summary>
    /// AutomationPeer for Scheduler Control
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the SchedulerAutomationPeer class.
    /// </remarks>
    /// <param name="owner">Owning Scheduler</param>
    public sealed class CalendarAutomationPeer(CalendarBase owner) : FrameworkElementAutomationPeer(owner), ISelectionProvider, ITableProvider, IItemContainerProvider
    {

        #region Private Properties

        private CalendarBase? OwningScheduler => Owner as CalendarBase;

        private CalendarItemsControl? OwningItemsControl => OwningScheduler?.DatesItemsControl;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Grid:
                case PatternInterface.Table:
                case PatternInterface.MultipleView:
                case PatternInterface.Selection:
                case PatternInterface.ItemContainer:
                    {
                        if (OwningItemsControl != null)
                        {
                            return this;
                        }

                        break;
                    }
            }

            return base.GetPattern(patternInterface);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Calendar;

        protected override List<AutomationPeer> GetChildrenCore()
        {
            if (OwningItemsControl == null || OwningScheduler?.DatesItemsControl == null)
            {
                return [];
            }

            var peers = new List<AutomationPeer>();

            // Step 1: Add previous, header and next buttons
            if (OwningScheduler.PreviousButton is not null)
            {
                var buttonPeer = CreatePeerForElement(OwningScheduler.PreviousButton);
                if (buttonPeer != null)
                {
                    peers.Add(buttonPeer);
                }
            }

            if (OwningScheduler.PreviousButton is not null)
            {
                var buttonPeer = CreatePeerForElement(OwningScheduler.NextButton);
                if (buttonPeer != null)
                {
                    peers.Add(buttonPeer);
                }
            }

            if (OwningScheduler.TodayButton is not null)
            {
                var buttonPeer = CreatePeerForElement(OwningScheduler.TodayButton);
                if (buttonPeer != null)
                {
                    peers.Add(buttonPeer);
                }
            }

            // Step 2: Add Scheduler Buttons depending on the Scheduler.DisplayMode
            CalendarItemAutomationPeer peer;
            foreach (CalendarItem child in OwningItemsControl.Items)
            {
                peer = GetOrCreateDateTimeAutomationPeer(child.Date);
                peers.Add(peer);
            }

            return peers;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType,
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore() => Owner.GetType().Name;

        protected override void SetFocusCore()
        {
            if (OwningScheduler == null) return;

            if (OwningScheduler.Focusable)
            {
                if (!OwningScheduler.Focus())
                {
                    var focusedDate = OwningScheduler.CurrentDate;

                    var focusedItem = GetOrCreateDateTimeAutomationPeer(focusedDate);
                    FrameworkElement? focusedButton = focusedItem.OwningButton;

                    if (focusedButton == null || !focusedButton.IsKeyboardFocused)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        #endregion Protected Methods

        #region InternalMethods

        ///<SecurityNote>
        /// Security Critical - Calls a Security Critical operation AddParentInfo which adds parent peer and provides
        ///                     security critical Hwnd value for this peer created asynchronously.
        /// SecurityTreatAsSafe - It's being called from this object which is real parent for the item peer.
        /// </SecurityNote>
        [SecurityCritical, SecuritySafeCritical]
        private CalendarItemAutomationPeer GetOrCreateDateTimeAutomationPeer(DateTime? date) => new(date, OwningScheduler);

        internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
        {
            var numSelected = OwningScheduler?.SelectedDatesInternal.Count;
            var numAdded = e.AddedItems.Count;

            if (ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && numSelected == 1 && numAdded == 1)
            {
                var peer = GetOrCreateDateTimeAutomationPeer((DateTime?)e.AddedItems[0]);
                peer?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
            }
            else
            {
                if (ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                {
                    foreach (DateTime date in e.AddedItems)
                    {
                        var peer = GetOrCreateDateTimeAutomationPeer(date);
                        peer?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                    }
                }
            }

            if (ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                foreach (DateTime date in e.RemovedItems)
                {
                    var peer = GetOrCreateDateTimeAutomationPeer(date);
                    peer?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                }
            }
        }

        #endregion InternalMethods

        #region IGridProvider

        int IGridProvider.ColumnCount => OwningItemsControl != null ? OwningItemsControl.ColumnsCount : 0;

        int IGridProvider.RowCount => OwningItemsControl != null ? OwningItemsControl.RowsCount : 0;

        IRawElementProviderSimple? IGridProvider.GetItem(int row, int column)
        {
            if (OwningItemsControl != null && row * column < OwningItemsControl.Items.Count)
            {
                var children = OwningItemsControl.Items[row * column];
                var dataContext = (children as FrameworkElement)?.DataContext;
                if (dataContext is DateTime date)
                {
                    AutomationPeer peer = GetOrCreateDateTimeAutomationPeer(date);
                    return ProviderFromPeer(peer);
                }
            }

            return null;
        }

        #endregion IGridProvider

        #region ISelectionProvider

        bool ISelectionProvider.CanSelectMultiple => OwningScheduler?.DatesSelectionMode == CalendarSelectionMode.SingleRange || OwningScheduler?.DatesSelectionMode == CalendarSelectionMode.MultipleRange;

        bool ISelectionProvider.IsSelectionRequired => false;

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            var providers = new List<IRawElementProviderSimple>();

            if (OwningScheduler == null) return [];

            foreach (var date in OwningScheduler.SelectedDatesInternal)
            {
                AutomationPeer peer = GetOrCreateDateTimeAutomationPeer(date);
                providers.Add(ProviderFromPeer(peer));
            }

            return providers.Count > 0 ? [.. providers] : [];
        }

        #endregion ISelectionProvider

        #region IItemContainerProvider

        IRawElementProviderSimple? IItemContainerProvider.FindItemByProperty(IRawElementProviderSimple startAfter, int propertyId, object value)
        {
            if (OwningScheduler == null) return null;

            CalendarItemAutomationPeer? startAfterDatePeer = null;

            if (startAfter != null)
            {
                startAfterDatePeer = PeerFromProvider(startAfter) as CalendarItemAutomationPeer;
                // if provider is not null, peer must exist
                if (startAfterDatePeer == null)
                {
                    throw new InvalidOperationException();
                }
            }

            DateTime? nextDate = null;

            if (propertyId == SelectionItemPatternIdentifiers.IsSelectedProperty.Id)
            {
                nextDate = GetNextSelectedDate(startAfterDatePeer, (bool)value);
            }
            else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
            {
                // finds the button for the given DateTime
                var format = CultureInfo.CurrentCulture.DateTimeFormat;
                if (DateTime.TryParse(value as string, format, DateTimeStyles.None, out var parsedDate))
                {
                    nextDate = parsedDate;
                }

                if (!nextDate.HasValue || startAfterDatePeer != null && nextDate <= startAfterDatePeer.Date)
                {
                    throw new InvalidOperationException();
                }
            }
            else if (propertyId == 0 || propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
            {
                // propertyId = 0 returns the button next to the startAfter or the DisplayDate if startAfter is null
                // All items here are buttons, so same behaviour as propertyId = 0
                if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id && (int)value != ControlType.Button.Id)
                {
                    return null;
                }
                nextDate = GetNextDate(startAfterDatePeer);
            }
            else
            {
                return null;
            }

            if (nextDate.HasValue)
            {
                AutomationPeer nextPeer = GetOrCreateDateTimeAutomationPeer(nextDate.Value);
                if (nextPeer != null)
                {
                    return ProviderFromPeer(nextPeer);
                }
            }
            return null;
        }

        private DateTime? GetNextDate(CalendarItemAutomationPeer? currentDatePeer)
        {
            if (OwningScheduler == null) return null;

            var startDate = currentDatePeer != null ? currentDatePeer.Date : OwningScheduler.DisplayDateInternal;

            return OwningScheduler.GetNextDate(startDate);
        }

        private DateTime? GetNextSelectedDate(CalendarItemAutomationPeer? currentDatePeer, bool isSelected)
        {
            if (OwningScheduler == null) return null;

            var startDate = currentDatePeer != null ? currentDatePeer.Date : OwningScheduler.DisplayDateInternal;

            if (isSelected)
            {
                // If SelectedDatesInternal is empty or startDate is beyond last SelectedDate
                if (OwningScheduler.SelectedDatesInternal.MaximumDate == DateTime.MinValue || OwningScheduler.SelectedDatesInternal.MaximumDate <= startDate)
                {
                    return null;
                }
                // startDate is before first SelectedDate
                if (OwningScheduler.SelectedDatesInternal.MinimumDate != DateTime.MinValue && startDate < OwningScheduler.SelectedDatesInternal.MinimumDate)
                {
                    return OwningScheduler.SelectedDatesInternal.MinimumDate;
                }
            }
            while (true)
            {
                startDate = startDate.AddDays(1);
                if (OwningScheduler?.SelectedDatesInternal.Contains(startDate) == isSelected)
                {
                    break;
                }
            }

            return startDate;
        }

        #endregion IItemContainerProvider

        #region ITableProvider

        RowOrColumnMajor ITableProvider.RowOrColumnMajor => RowOrColumnMajor.RowMajor;

        IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
        {
            if (OwningScheduler == null || OwningItemsControl == null) return [];

            var providers = new List<IRawElementProviderSimple>();

            foreach (UIElement child in OwningItemsControl.Items)
            {
                var childRow = (int)child.GetValue(Grid.RowProperty);

                if (childRow == 0)
                {
                    var peer = CreatePeerForElement(child);

                    if (peer != null)
                    {
                        providers.Add(ProviderFromPeer(peer));
                    }
                }
            }

            return providers.Count switch
            {
                > 0 => [.. providers],
                _ => [],
            };
        }

        // If WeekNumber functionality is supported by Scheduler in the future,
        // this method should return weeknumbers
        IRawElementProviderSimple[] ITableProvider.GetRowHeaders() => [];

        #endregion ITableProvider
    }
}
