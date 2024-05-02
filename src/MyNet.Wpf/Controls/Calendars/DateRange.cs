// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace MyNet.Wpf.Controls.Calendars
{
    /// <summary>
    /// Specifies a DateTime range class which has a start and end.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the SchedulerDateRange class which accepts range start and end dates.
    /// </remarks>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public sealed class DateRange(DateTime start, DateTime end) : INotifyPropertyChanged
    {
        #region Data
        private DateTime _end = end;
        private DateTime _start = start;
        #endregion Data

        /// <summary>
        /// Initializes a new instance of the SchedulerDateRange class.
        /// </summary>
        public DateRange() :
            this(DateTime.MinValue, DateTime.MaxValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchedulerDateRange class which creates a range from a single DateTime value.
        /// </summary>
        /// <param name="day"></param>
        public DateRange(DateTime day) :
            this(day, day)
        {
        }

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Specifies the End date of the SchedulerDateRange.
        /// </summary>
        public DateTime End
        {
            get => CoerceEnd(_start, _end);

            set
            {
                var newEnd = CoerceEnd(_start, value);
                if (newEnd != End)
                {
                    OnChanging(new DateRangeChangingEventArgs(_start, newEnd));
                    _end = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(End)));
                }
            }
        }

        /// <summary>
        /// Specifies the Start date of the SchedulerDateRange.
        /// </summary>
        public DateTime Start
        {
            get => _start;

            set
            {
                if (_start != value)
                {
                    var oldEnd = End;
                    var newEnd = CoerceEnd(value, _end);

                    OnChanging(new DateRangeChangingEventArgs(value, newEnd));

                    _start = value;

                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Start)));

                    if (newEnd != oldEnd)
                    {
                        OnPropertyChanged(new PropertyChangedEventArgs(nameof(End)));
                    }
                }
            }
        }

        #endregion Public Properties

        #region Internal Events

        internal event EventHandler<DateRangeChangingEventArgs>? Changing;

        #endregion Internal Events

        #region Internal Methods

        /// <summary>
        /// Returns true if any day in the given DateTime range is contained in the current SchedulerDateRange.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        internal bool ContainsAny(DateRange range) => range.End >= Start && End >= range.Start;

        #endregion Internal Methods

        #region Private Methods

        private void OnChanging(DateRangeChangingEventArgs e) => Changing?.Invoke(this, e);

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);
        }

        /// <summary>
        /// Coerced the end parameter to satisfy the start &lt;= end constraint
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>If start &lt;= end the end parameter otherwise the start parameter</returns>
        private static DateTime CoerceEnd(DateTime start, DateTime end) => DateTime.Compare(start, end) <= 0 ? end : start;

        #endregion Private Methods
    }
}
