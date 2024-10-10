// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using MyNet.Utilities.Collections;

namespace MyNet.Wpf.Controls.Calendars
{
    /// <summary>
    /// Represents the collection of SelectedDatesInternal for the Scheduler Control.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the SchedulerSelectedDatesCollection class.
    /// </remarks>
    /// <param name="owner"></param>
    public sealed class SelectedDatesCollection(CalendarBase owner) : OptimizedObservableCollection<DateTime>
    {
        #region Data

        private readonly Collection<DateTime> _addedItems = [];
        private readonly Collection<DateTime> _removedItems = [];
        private readonly Thread _dispatcherThread = Thread.CurrentThread;
        private bool _isAddingRange;
        private readonly CalendarBase _owner = owner;

        #endregion Data

        #region Properties

        public DateTime? MaximumDate => Count > 0 ? this.Max() : null;

        public DateTime? MinimumDate => Count > 0 ? this.Min() : null;

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Adds a range of dates to the Scheduler SelectedDatesInternal.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void AddRange(DateTime start, DateTime end)
        {
            BeginAddRange();

            // If SchedulerSelectionMode.SingleRange and a user programmatically tries to add multiple ranges, we will throw away the old range and replace it with the new one.
            if (_owner.DatesSelectionMode == CalendarSelectionMode.SingleRange && Count > 0)
                ClearInternal();

            foreach (var current in _owner.GetValidDates(start, end))
                Add(current);

            EndAddRange();
        }

        #endregion Public methods

        #region Protected methods

        /// <summary>
        /// Clears all the items of the SelectedDatesInternal.
        /// </summary>
        protected override void ClearItems()
        {
            ClearInternal();

            RaiseSelectionChanged(_removedItems, new List<object>());
            _removedItems.Clear();
        }

        /// <summary>
        /// Clears all the items of the SelectedDatesInternal.
        /// </summary>
        internal void ClearInternal()
        {
            if (Count > 0)
            {
                foreach (var item in this)
                    _removedItems.Add(item);

                base.ClearItems();
            }

        }

        internal void ClearRangeInternal(DateTime start, DateTime end)
        {
            foreach (var current in _owner.GetValidDates(start, end))
            {
                _removedItems.Add(current);
                Remove(current);
            }
        }

        /// <summary>
        /// Inserts the item in the specified position of the SelectedDatesInternal collection.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, DateTime item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (!Contains(item))
            {
                var addedItems = new Collection<DateTime>();

                var isCleared = CheckSelectionMode();

                if (_owner.IsValidDateSelection(item))
                {
                    // If the Collection is cleared since it is SingleRange and it had another range
                    // set the index to 0
                    if (isCleared)
                    {
                        index = 0;
                    }

                    base.InsertItem(index, item);

                    if (!_isAddingRange)
                    {
                        addedItems.Add(item);

                        RaiseSelectionChanged(_removedItems, addedItems);
                        _removedItems.Clear();
                    }
                    else
                    {
                        _addedItems.Add(item);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(item));
                }
            }
        }

        /// <summary>
        /// Removes the item at the specified position.
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (index >= Count)
            {
                base.RemoveItem(index);
            }
            else
            {
                var addedItems = new Collection<DateTime>();
                var removedItems = new Collection<DateTime>
                {
                    this[index]
                };
                base.RemoveItem(index);

                RaiseSelectionChanged(removedItems, addedItems);
            }
        }

        /// <summary>
        /// The object in the specified index is replaced with the provided item.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, DateTime item)
        {
            if (!IsValidThread())
            {
                throw new NotSupportedException();
            }

            if (!Contains(item))
            {
                var addedItems = new Collection<DateTime>();
                var removedItems = new Collection<DateTime>();

                if (index >= Count)
                {
                    base.SetItem(index, item);
                }
                else
                {
                    if (DateTime.Compare(this[index], item) != 0 && _owner.IsValidDateSelection(item))
                    {
                        removedItems.Add(this[index]);
                        base.SetItem(index, item);

                        addedItems.Add(item);

                        RaiseSelectionChanged(removedItems, addedItems);
                    }
                }
            }
        }

        #endregion Protected methods

        #region Private Methods

        private void RaiseSelectionChanged(IList removedItems, IList addedItems) => _owner.OnSelectedDatesCollectionChanged(new DatesSelectionChangedEventArgs(CalendarBase.SelectedDatesChangedEvent, removedItems, addedItems));

        private void BeginAddRange()
        {
            Debug.Assert(!_isAddingRange);
            _isAddingRange = true;
        }

        private void EndAddRange()
        {
            Debug.Assert(_isAddingRange);

            _isAddingRange = false;
            RaiseSelectionChanged(_removedItems, _addedItems);
            _removedItems.Clear();
            _addedItems.Clear();
        }

        private bool CheckSelectionMode()
        {
            if (_owner.DatesSelectionMode == CalendarSelectionMode.None)
                return true;

            if (_owner.DatesSelectionMode == CalendarSelectionMode.SingleDate && Count > 0)
            {
                throw new InvalidOperationException();
            }

            // if user tries to add an item into the SelectedDatesInternal in SingleRange mode, we throw away the old range and replace it with the new one
            // in order to provide the removed items without an additional event, we are calling ClearInternal
            if (_owner.DatesSelectionMode == CalendarSelectionMode.SingleRange && !_isAddingRange && Count > 0)
            {
                ClearInternal();
                return true;
            }

            return false;
        }

        private bool IsValidThread() => Thread.CurrentThread == _dispatcherThread;

        #endregion Private Methods
    }
}
