// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Automation
{
    /// <summary>
    /// AutomationPeer for SchedulerDay and SchedulerButton
    /// </summary>
    public sealed class CalendarItemAutomationPeer : AutomationPeer, ISelectionItemProvider, ITableItemProvider, IInvokeProvider, IVirtualizedItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the DateTimeAutomationPeer class.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="owningCalendar"></param>
        internal CalendarItemAutomationPeer(DateTime? date, CalendarBase? owningCalendar)
        {
            Date = date ?? throw new ArgumentNullException(nameof(date));
            Owner = owningCalendar ?? throw new ArgumentNullException(nameof(owningCalendar));
        }

        #region Private Properties

        private CalendarBase Owner
        {
            get;
            set;
        }

        internal DateTime Date
        {
            get;
            private set;
        }

        private IRawElementProviderSimple? OwningSchedulerProvider
        {
            get
            {
                if (Owner != null)
                {
                    var peer = UIElementAutomationPeer.CreatePeerForElement(Owner);

                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }

                return null;
            }
        }

        internal CalendarItem? OwningButton => Owner.GetCalendarItemFromDate(Date);

        internal FrameworkElementAutomationPeer? WrapperPeer
        {
            get
            {
                var owningButton = OwningButton;
                return owningButton != null ? UIElementAutomationPeer.CreatePeerForElement(owningButton) as FrameworkElementAutomationPeer : null;
            }
        }

        #endregion Private Properties

        #region AutomationPeer override Methods

        protected override string GetAcceleratorKeyCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetAcceleratorKey();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return string.Empty;
        }

        protected override string GetAccessKeyCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetAccessKey();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return string.Empty;
        }

        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Button;

        protected override string GetAutomationIdCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetAutomationId();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return string.Empty;
        }

        protected override Rect GetBoundingRectangleCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetBoundingRectangle();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return new Rect();
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetChildren();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return [];
        }

        protected override string GetClassNameCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            return wrapperPeer != null ? wrapperPeer.GetClassName() : "CalendarItem";
        }

        protected override Point GetClickablePointCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetClickablePoint();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return new Point(double.NaN, double.NaN);
        }

        protected override string GetHelpTextCore()
        {
            var dateString = Date.ToLongDateString();
            return Owner.BlackoutDates.Contains(Date)
                ? string.Format(CultureInfo.CurrentCulture.DateTimeFormat, dateString)
                : dateString;
        }

        protected override string GetItemStatusCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetItemStatus();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return string.Empty;
        }

        protected override string GetItemTypeCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetItemType();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return string.Empty;
        }

        protected override AutomationPeer? GetLabeledByCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetLabeledBy();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return null;
        }

        protected override AutomationLiveSetting GetLiveSettingCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetLiveSetting();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return AutomationLiveSetting.Off;
        }

        protected override string GetLocalizedControlTypeCore() => string.Empty;

        protected override string GetNameCore() => Date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentCulture.DateTimeFormat);

        protected override AutomationOrientation GetOrientationCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetOrientation();
            }
            ThrowElementNotAvailableException();

            return AutomationOrientation.None;
        }

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object? GetPattern(PatternInterface patternInterface)
        {
            object? result = null;
            var owningButton = OwningButton;

            switch (patternInterface)
            {
                case PatternInterface.Invoke:
                case PatternInterface.GridItem:
                    {
                        if (owningButton != null)
                        {
                            result = this;
                        }
                        break;
                    }
                case PatternInterface.TableItem:
                    {
                        if (owningButton != null)
                        {
                            result = this;
                        }
                        break;
                    }
                case PatternInterface.SelectionItem:
                    {
                        result = this;
                        break;
                    }
                case PatternInterface.VirtualizedItem:
                    if (VirtualizedItemPatternIdentifiers.Pattern != null)
                    {
                        if (owningButton == null)
                        {
                            result = this;
                        }
                        else
                        {
                            return this;
                        }
                    }
                    break;
            }

            return result;
        }

        protected override bool HasKeyboardFocusCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.HasKeyboardFocus();
            }
            ThrowElementNotAvailableException();

            return false;
        }

        protected override bool IsContentElementCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsContentElement();
            }
            ThrowElementNotAvailableException();

            return true;
        }

        protected override bool IsControlElementCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsControlElement();
            }
            ThrowElementNotAvailableException();

            return true;
        }

        protected override bool IsEnabledCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsEnabled();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return false;
        }

        protected override bool IsKeyboardFocusableCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsKeyboardFocusable();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return false;
        }

        protected override bool IsOffscreenCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsOffscreen();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return true;
        }

        protected override bool IsPasswordCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsPassword();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return false;
        }

        protected override bool IsRequiredForFormCore()
        {
            AutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                return wrapperPeer.IsRequiredForForm();
            }
            else
            {
                ThrowElementNotAvailableException();
            }

            return false;
        }

        protected override void SetFocusCore()
        {
            UIElementAutomationPeer? wrapperPeer = WrapperPeer;
            if (wrapperPeer != null)
            {
                wrapperPeer.SetFocus();
            }
            else
            {
                ThrowElementNotAvailableException();
            }
        }

        #endregion AutomationPeer override Methods

        #region IGridItemProvider

        /// <summary>
        /// Grid item column.
        /// </summary>
        int IGridItemProvider.Column
        {
            get
            {
                var owningButton = OwningButton;
                return owningButton != null ? (int)owningButton.GetValue(Grid.ColumnProperty) - 1 : 0;
            }
        }

        /// <summary>
        /// Grid item column span.
        /// </summary>
        int IGridItemProvider.ColumnSpan
        {
            get
            {
                var owningButton = OwningButton;
                return owningButton != null ? (int)owningButton.GetValue(Grid.ColumnSpanProperty) : 0;
            }
        }

        /// <summary>
        /// Grid item's containing grid.
        /// </summary>
        IRawElementProviderSimple? IGridItemProvider.ContainingGrid => OwningSchedulerProvider;

        /// <summary>
        /// Grid item row.
        /// </summary>
        int IGridItemProvider.Row
        {
            get
            {
                var owningButton = OwningButton;
                if (owningButton != null)
                {
                    Debug.Assert((int)owningButton.GetValue(Grid.RowProperty) > 0);

                    // we decrement the Row value by one since the first row is composed of DayTitles
                    return (int)owningButton.GetValue(Grid.RowProperty) - 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Grid item row span.
        /// </summary>
        int IGridItemProvider.RowSpan
        {
            get
            {
                var owningButton = OwningButton;
                return owningButton != null ? (int)owningButton.GetValue(Grid.RowSpanProperty) : 0;
            }
        }

        #endregion IGridItemProvider

        #region ISelectionItemProvider

        /// <summary>
        /// True if the owning SchedulerDay is selected.
        /// </summary>
        bool ISelectionItemProvider.IsSelected => Owner.SelectedDatesInternal.Contains(Date);

        /// <summary>
        /// Selection items selection container.
        /// </summary>
        IRawElementProviderSimple? ISelectionItemProvider.SelectionContainer => OwningSchedulerProvider;

        /// <summary>
        /// Adds selection item to selection.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            // Return if the day is already selected or if it is a BlackoutDate
            if (((ISelectionItemProvider)this).IsSelected)
            {
                return;
            }

            if (EnsureSelection())
            {
                if (Owner.DatesSelectionMode == CalendarSelectionMode.SingleDate)
                {
                    Owner.SelectedDateInternal = Date;
                }
                else
                {
                    Owner.SelectedDatesInternal.Add(Date);
                }
            }
        }

        /// <summary>
        /// Removes selection item from selection.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            // Return if the item is not already selected.
            if (!((ISelectionItemProvider)this).IsSelected)
            {
                return;
            }

            _ = Owner.SelectedDatesInternal.Remove(Date);
        }

        /// <summary>
        /// Selects this item.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (EnsureSelection() && Owner.DatesSelectionMode == CalendarSelectionMode.SingleDate)
            {
                Owner.SelectedDateInternal = Date;
            }
        }

        #endregion ISelectionItemProvider

        #region ITableItemProvider

        /// <summary>
        /// Gets the table item's column headers.
        /// </summary>
        /// <returns>The table item's column headers</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            if (OwningButton != null && Owner != null && OwningSchedulerProvider != null)
            {
                var headers = ((ITableProvider)UIElementAutomationPeer.CreatePeerForElement(Owner)).GetColumnHeaders();

                if (headers != null)
                {
                    var column = ((IGridItemProvider)this).Column;
                    return [headers[column]];
                }
            }
            return [];
        }

        /// <summary>
        /// Get's the table item's row headers.
        /// </summary>
        /// <returns>The table item's row headers</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems() => [];

        #endregion ITableItemProvider

        #region IInvokeProvider

        void IInvokeProvider.Invoke()
        {
            var owningButton = OwningButton;
            if (owningButton == null || !IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        #endregion IInvokeProvider

        #region IVirtualizedItemProvider

        void IVirtualizedItemProvider.Realize() => Owner.DisplayDate = Date;

        #endregion IVirtualizedItemProvider

        #region Private Methods

        private bool EnsureSelection() =>
            // If the day is a blackout day or the SelectionMode is None, selection is not allowed
            !Owner.BlackoutDates.Contains(Date) &&
                Owner.DatesSelectionMode != CalendarSelectionMode.None;

        private static void ThrowElementNotAvailableException()
        {
            // To avoid the situation on legacy systems which may not have new unmanaged core. this check with old unmanaged core
            // avoids throwing exception and provide older behavior returning default values for items which are virtualized rather than throwing exception.
            if (VirtualizedItemPatternIdentifiers.Pattern != null)
            {
                // Do nothing
            }
        }

        #endregion Private Methods
    }
}
