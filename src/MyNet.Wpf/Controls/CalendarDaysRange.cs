// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Helpers;
using MyNet.Observable;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class CalendarDaysRange : CalendarBase
    {
        static CalendarDaysRange()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaysRange), new FrameworkPropertyMetadata(typeof(CalendarDaysRange)));
            ItemsOrientationProperty.OverrideMetadata(typeof(CalendarDaysRange), new FrameworkPropertyMetadata(Orientation.Horizontal));
        }

        #region DisplayDaysCount

        public int DisplayDaysCount
        {
            get => (int)GetValue(DisplayDaysCountProperty);
            set => SetValue(DisplayDaysCountProperty, value);
        }

        public static readonly DependencyProperty DisplayDaysCountProperty = DependencyProperty.Register(nameof(DisplayDaysCount), typeof(int), typeof(CalendarDaysRange), new FrameworkPropertyMetadata(DateTimeHelper.NumberOfDaysInWeek(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDaysCountChanged));

        private static void OnDisplayDaysCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarDaysRange;
            Debug.Assert(c != null);

            c.Build();
        }

        #endregion DisplayDaysCount

        public override TimeUnit IntervalUnit => TimeUnit.Day;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => !oldDate.SameDay(newDate);

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            if (DisplayDateInternal == DateTime.MinValue) return [];

            var start = DisplayDateInternal.AddDays((int)(-DisplayDaysCount / 2.0));

            return Enumerable.Range(0, DisplayDaysCount).Select(x => (start.AddDays(x), 0, x)).ToList();
        }

        protected override IEnumerable<(int row, int column, int rowSpan, int columnSpan)> GetDisplayedAppointments(IAppointment appointment, IEnumerable<(ImmutablePeriod period, int row, int column)> allDisplayedDates)
            => allDisplayedDates.GroupBy(x => x.row).Select(x => (x.Key, x.Min(y => y.column), 1, x.Count()));

        public override DateTime GetNextDate(DateTime date) => date.AddDays(1);

        public override DateTime GetPreviousDate(DateTime date) => date.AddDays(-1);

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
    }
}
