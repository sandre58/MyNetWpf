// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MyNet.Utilities;
using MyNet.Utilities.Collections;
using PropertyChanged;

namespace MyNet.Wpf.Controls.Calendars
{
    /// <summary>
    /// Initializes a new instance of the SchedulerBlackoutDatesCollection class.
    /// </summary>
    /// <param name="owner"></param>
    public class BlackoutDatesCollection(CalendarBase owner) : OptimizedObservableCollection<DateRange>
    {
        #region Data

        private readonly Thread _dispatcherThread = Thread.CurrentThread;
        private readonly CalendarBase _owner = owner;

        #endregion Data

        #region Public Methods

        /// <summary>
        /// Dates that are in the past are added to the BlackoutDates.
        /// </summary>
        public void AddDatesInPast() => Add(new DateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));

        /// <summary>
        /// Checks if a DateTime is in the Collection
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool Contains(DateTime date) => null != GetContainingDateRange(date);

        /// <summary>
        /// Checks if a Range is in the collection
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool Contains(DateTime start, DateTime end)
        {
            DateTime rangeStart, rangeEnd;
            var n = Count;

            if (DateTime.Compare(end, start) > -1)
            {
                rangeStart = start.DiscardTime();
                rangeEnd = end.DiscardTime();
            }
            else
            {
                rangeStart = end.DiscardTime();
                rangeEnd = start.DiscardTime();
            }

            for (var i = 0; i < n; i++)
            {
                if (DateTime.Compare(this[i].Start, rangeStart) == 0 && DateTime.Compare(this[i].End, rangeEnd) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if any day in the given DateTime range is contained in the BlackOutDays.
        /// </summary>
        /// <param name="range">SchedulerDateRange that is searched in BlackOutDays</param>
        /// <returns>true if at least one day in the range is included in the BlackOutDays</returns>
        public bool ContainsAny(DateRange range) => this.Any(x => x.ContainsAny(range));

        /// <summary>
        /// This finds the next date that is not blacked out in a certian direction.
        /// </summary>
        /// <param name="requestedDate"></param>
        /// <param name="dayInterval"></param>
        /// <returns></returns>
        internal DateTime? GetNonBlackoutDate(DateTime? requestedDate, int dayInterval)
        {
            Debug.Assert(dayInterval != 0);

            var currentDate = requestedDate;
            DateRange? range;

            if (requestedDate == null)
            {
                return null;
            }

            if ((range = GetContainingDateRange(currentDate)) == null)
            {
                return requestedDate;
            }

            do
            {
                if (dayInterval > 0)
                {
                    // Moving Forwards.
                    // The DateRanges require start <= end
                    currentDate = range.End.AddDays(dayInterval);
                }
                else
                {
                    //Moving backwards.
                    currentDate = range.Start.AddDays(dayInterval);
                }
            } while ((range = GetContainingDateRange((DateTime)currentDate)) != null);

            return currentDate;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// All the items in the collection are removed.
        /// </summary>
        protected override void ClearItems()
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            foreach (var item in Items)
            {
                UnRegisterItem(item);
            }

            base.ClearItems();
        }

        /// <summary>
        /// The item is inserted in the specified place in the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, DateRange item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (IsValid(item))
            {
                RegisterItem(item);
                base.InsertItem(index, item);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
        }

        /// <summary>
        /// The item in the specified index is removed from the collection.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (index >= 0 && index < Count)
            {
                UnRegisterItem(Items[index]);
            }

            base.RemoveItem(index);
        }

        /// <summary>
        /// The object in the specified index is replaced with the provided item.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, DateRange item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (IsValid(item))
            {
                DateRange? oldItem = null;
                if (index >= 0 && index < Count)
                {
                    oldItem = Items[index];
                }

                base.SetItem(index, item);

                UnRegisterItem(oldItem);
                RegisterItem(Items[index]);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Registers for change notification on date ranges
        /// </summary>
        /// <param name="item"></param>
        private void RegisterItem(DateRange? item)
        {
            if (item != null)
            {
                item.Changing += Item_Changing;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Un registers for change notification on date ranges
        /// </summary>
        private void UnRegisterItem(DateRange? item)
        {
            if (item != null)
            {
                item.Changing -= Item_Changing;
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Reject date range changes that would make the blackout dates collection invalid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_Changing(object? sender, DateRangeChangingEventArgs e) => OnItemChanging(sender as DateRange, e.Start, e.End);

        protected virtual void OnItemChanging(DateRange? item, DateTime start, DateTime end) { }

        /// <summary>
        /// Update the Scheduler view to reflect the new blackout dates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e) => OnItemPropertyChanged(sender as DateRange, e.PropertyName);

        [SuppressPropertyChangedWarnings]
        protected virtual void OnItemPropertyChanged(DateRange? item, string? propertyName) { }

        /// <summary>
        /// Tests to see if a date range is not already selected
        /// </summary>
        /// <param name="item">date range to test</param>
        /// <returns>True if no selected day falls in the given date range</returns>
        private bool IsValid(DateRange item) => IsValid(item.Start, item.End);

        /// <summary>
        /// Tests to see if a date range is not already selected
        /// </summary>
        /// <param name="start">First day of date range to test</param>
        /// <param name="end">Last day of date range to test</param>
        /// <returns>True if no selected day falls between start and end</returns>
        private bool IsValid(DateTime start, DateTime end) => _owner.SelectedDates.OfType<DateTime>().All(x => !x.InRange(start, end));

        private bool IsValidThread() => Thread.CurrentThread == _dispatcherThread;

        /// <summary>
        /// Gets the DateRange that contains the date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private DateRange? GetContainingDateRange(DateTime? date)
        {
            if (!date.HasValue) return null;

            for (var i = 0; i < Count; i++)
            {
                if (date.Value.InRange(this[i].Start, this[i].End))
                {
                    return this[i];
                }
            }
            return null;
        }

        #endregion Private Methods
    }
}
