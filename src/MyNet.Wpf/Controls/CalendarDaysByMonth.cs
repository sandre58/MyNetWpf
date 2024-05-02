// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using MyNet.Observable;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class CalendarDaysByMonth : CalendarBase
    {
        private readonly Calendar _calendar = new GregorianCalendar();

        static CalendarDaysByMonth() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaysByMonth), new FrameworkPropertyMetadata(typeof(CalendarDaysByMonth)));

        public override TimeUnit IntervalUnit => TimeUnit.Day;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => !oldDate.SameMonth(newDate);

        protected override IEnumerable<object> GetColumnHeaders()
        {
            return EnumerableHelper.Range(0, DateTimeHelper.NumberOfDaysInWeek() - 1, 1).Select(getDayOfWeek).OfType<object>().ToList();

            DayOfWeek getDayOfWeek(int index) => (DayOfWeek)((index + (int)FirstDayOfWeek) % DateTimeHelper.NumberOfDaysInWeek());
        }

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            var columnHeaders = EnumerableHelper.Range(0, DateTimeHelper.NumberOfDaysInWeek() - 1, 1).Select(getDayOfWeek).OfType<object>().ToList();
            var firstDayOfMonth = DisplayDateInternal.BeginningOfMonth();
            var lastDayOfMonth = DisplayDateInternal.EndOfMonth().DiscardTime();
            var start = firstDayOfMonth.AddDays(-CountWeekDaysBefore(firstDayOfMonth));
            var end = lastDayOfMonth.AddDays(CountWeekDaysAfter(lastDayOfMonth));

            var rowIndex = -1;
            return DateTimeHelper.Range(start, end, 1, TimeUnit.Day).Select(date =>
            {
                var column = columnHeaders.IndexOf(date.DayOfWeek);
                if (column == 0) rowIndex++;
                return (date, rowIndex, column);
            }).Where(x => x.date.IsBetween(MinimumDateInternal, MaximumDateInternal))
              .ToList();

            DayOfWeek getDayOfWeek(int index) => (DayOfWeek)((index + (int)FirstDayOfWeek) % DateTimeHelper.NumberOfDaysInWeek());
        }

        protected internal override bool IsInactive(DateTime date) => !date.SameMonth(DisplayDateInternal);

        protected override IEnumerable<(int row, int column, int rowSpan, int columnSpan)> GetDisplayedAppointments(IAppointment appointment, IEnumerable<(ImmutablePeriod period, int row, int column)> allDisplayedDates)
            => allDisplayedDates.GroupBy(x => x.row).Select(x => (x.Key, x.Min(y => y.column), 1, x.Count()));

        public override DateTime GetNextDate(DateTime date) => date.AddMonths(1);

        public override DateTime GetPreviousDate(DateTime date) => date.AddMonths(-1);

        protected override bool ProcessDownKey(bool shift)
        {
            ProcessSelection(DateTimeHelper.NumberOfDaysInWeek(), TimeUnit.Day, shift);
            return true;
        }

        protected override bool ProcessUpKey(bool shift)
        {
            ProcessSelection(-DateTimeHelper.NumberOfDaysInWeek(), TimeUnit.Day, shift);
            return true;
        }

        protected override bool ProcessLeftKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Day, shift);
            return true;
        }

        protected override bool ProcessRightKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Day, shift);
            return true;
        }

        protected override bool ProcessEndKey(bool shift)
        {
            ProcessSelection(CurrentDate.EndOfMonth().Date, shift);
            return true;
        }

        protected override bool ProcessHomeKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfMonth().Date, shift);
            return true;
        }

        protected override bool ProcessPageDownKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Month, shift);
            return true;
        }

        protected override bool ProcessPageUpKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Month, shift);
            return true;
        }

        private int CountWeekDaysBefore(DateTime date)
        {
            var day = _calendar.GetDayOfWeek(date) - FirstDayOfWeek;
            var numberOfDaysInWeek = DateTimeHelper.NumberOfDaysInWeek();
            var i = (day + numberOfDaysInWeek) % numberOfDaysInWeek;
            return i;
        }

        private int CountWeekDaysAfter(DateTime date)
        {
            var day = _calendar.GetDayOfWeek(date) - FirstDayOfWeek;
            var numberOfDaysInWeek = DateTimeHelper.NumberOfDaysInWeek();
            var i = (day + numberOfDaysInWeek) % numberOfDaysInWeek;
            return numberOfDaysInWeek - i - 1;
        }
    }
}
