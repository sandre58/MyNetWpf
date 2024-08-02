// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MyNet.Observable;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class CalendarMonthsByYear : CalendarBase
    {
        static CalendarMonthsByYear() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarMonthsByYear), new FrameworkPropertyMetadata(typeof(CalendarMonthsByYear)));

        public override TimeUnit IntervalUnit => TimeUnit.Month;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => !oldDate.SameYear(newDate);

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            var start = DisplayDateInternal.BeginningOfYear();
            var end = DisplayDateInternal.EndOfYear();

            return DateTimeHelper.Range(start, end, 1, TimeUnit.Month).Select(date => (date.BeginningOfMonth(), (date.Month - 1) / 4, (date.Month - 1) % 4)).Where(x => x.Item1.IsBetween(MinimumDateInternal, MaximumDateInternal)).ToList();
        }

        protected internal override bool IsInactive(DateTime date) => !date.SameYear(DisplayDateInternal);

        protected override IEnumerable<(int row, int column, int rowSpan, int columnSpan)> GetDisplayedAppointments(IAppointment appointment, IEnumerable<(ImmutablePeriod period, int row, int column)> allDisplayedDates)
            => allDisplayedDates.GroupBy(x => x.row).Select(x => (x.Key, x.Min(y => y.column), 1, x.Count()));

        public override DateTime GetNextDate(DateTime date) => date.AddYears(1);

        public override DateTime GetPreviousDate(DateTime date) => date.AddYears(-1);

        protected override bool ProcessDownKey(bool shift)
        {
            ProcessSelection(4, TimeUnit.Month, shift);
            return true;
        }

        protected override bool ProcessUpKey(bool shift)
        {
            ProcessSelection(-4, TimeUnit.Month, shift);
            return true;
        }

        protected override bool ProcessLeftKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Month, shift);
            return true;
        }

        protected override bool ProcessRightKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Month, shift);
            return true;
        }

        protected override bool ProcessEndKey(bool shift)
        {
            ProcessSelection(CurrentDate.EndOfYear().Date, shift);
            return true;
        }

        protected override bool ProcessHomeKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfYear().Date, shift);
            return true;
        }

        protected override bool ProcessPageDownKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessPageUpKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Year, shift);
            return true;
        }
    }
}
