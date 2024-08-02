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
    public class CalendarYearsByDecade : CalendarBase
    {
        static CalendarYearsByDecade() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarYearsByDecade), new FrameworkPropertyMetadata(typeof(CalendarYearsByDecade)));

        public override TimeUnit IntervalUnit => TimeUnit.Year;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => oldDate == DateTime.MinValue || !oldDate.SameDecade(newDate);

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            var start = DisplayDateInternal.BeginningOfDecade();
            var end = DisplayDateInternal.EndOfDecade();

            return DateTimeHelper.Range(start, end, 1, TimeUnit.Year).Select(date => (date.BeginningOfYear(), date.Year % 10 / 4, date.Year % 10 % 4)).Where(x => x.Item1.IsBetween(MinimumDateInternal, MaximumDateInternal)).ToList();
        }

        protected internal override bool IsInactive(DateTime date) => !date.SameDecade(DisplayDateInternal);

        protected override IEnumerable<(int row, int column, int rowSpan, int columnSpan)> GetDisplayedAppointments(IAppointment appointment, IEnumerable<(ImmutablePeriod period, int row, int column)> allDisplayedDates)
            => allDisplayedDates.GroupBy(x => x.row).Select(x => (x.Key, x.Min(y => y.column), 1, x.Count()));

        public override DateTime GetNextDate(DateTime date) => date.AddYears(10);

        public override DateTime GetPreviousDate(DateTime date) => date.AddYears(-10);

        protected override bool ProcessDownKey(bool shift)
        {
            ProcessSelection(4, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessUpKey(bool shift)
        {
            ProcessSelection(-4, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessLeftKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessRightKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessEndKey(bool shift)
        {
            ProcessSelection(CurrentDate.EndOfDecade().Date, shift);
            return true;
        }

        protected override bool ProcessHomeKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfDecade().Date, shift);
            return true;
        }

        protected override bool ProcessPageDownKey(bool shift)
        {
            ProcessSelection(10, TimeUnit.Year, shift);
            return true;
        }

        protected override bool ProcessPageUpKey(bool shift)
        {
            ProcessSelection(-10, TimeUnit.Year, shift);
            return true;
        }
    }
}
