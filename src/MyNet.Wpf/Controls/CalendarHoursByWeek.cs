﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MyNet.Utilities;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class CalendarHoursByWeek : CalendarBase
    {
        static CalendarHoursByWeek() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarHoursByWeek), new FrameworkPropertyMetadata(typeof(CalendarHoursByWeek)));

        #region DisplayTimeEnd

        public TimeSpan DisplayTimeEnd
        {
            get => (TimeSpan)GetValue(DisplayTimeEndProperty);
            set => SetValue(DisplayTimeEndProperty, value);
        }

        public static readonly DependencyProperty DisplayTimeEndProperty = DependencyProperty.Register(nameof(DisplayTimeEnd), typeof(TimeSpan), typeof(CalendarHoursByWeek), new FrameworkPropertyMetadata(23.Hours().TimeSpan, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayTimeEndChanged, CoerceDisplayTimeEnd));

        /// <summary>
        /// DisplayTimeEndProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayTimeEnd.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayTimeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarHoursByWeek;
            Debug.Assert(c != null);

            c.Rebuild();
        }

        private static object CoerceDisplayTimeEnd(DependencyObject d, object value)
        {
            var c = d as CalendarHoursByWeek;

            var date = (TimeSpan)value;

            if (c?.DisplayTimeStart != null && date < c.DisplayTimeStart)
            {
                value = c.DisplayTimeStart;
            }

            return value;
        }

        #endregion DisplayTimeEnd

        #region DisplayTimeStart

        public TimeSpan DisplayTimeStart
        {
            get => (TimeSpan)GetValue(DisplayTimeStartProperty);
            set => SetValue(DisplayTimeStartProperty, value);
        }

        public static readonly DependencyProperty DisplayTimeStartProperty = DependencyProperty.Register(nameof(DisplayTimeStart), typeof(TimeSpan), typeof(CalendarHoursByWeek), new FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayTimeStartChanged));

        private static void OnDisplayTimeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarHoursByWeek;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayTimeEndProperty);
            c.Rebuild();
        }

        #endregion DisplayTimeStart

        public override TimeUnit IntervalUnit => TimeUnit.Hour;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => oldDate == DateTime.MinValue || !oldDate.SameWeek(newDate);

        protected override IEnumerable<object> GetColumnHeaders()
        {
            var start = DisplayDateInternal.BeginningOfWeek(FirstDayOfWeek);
            var end = DisplayDateInternal.EndOfWeek(FirstDayOfWeek).DiscardTime();

            return DateTimeHelper.Range(start, end, 1, TimeUnit.Day).OfType<object>().ToList();
        }

        protected override IEnumerable<object> GetRowHeaders()
            => DateTimeHelper.Range(DateTime.Today.BeginningOfDay(), DateTime.Today.EndOfDay(), 1, TimeUnit.Hour).Where(x => x.IsBetween(DateTime.Today.At(DisplayTimeStart), DateTime.Today.At(DisplayTimeEnd))).Select(x => x.TimeOfDay).OfType<object>().ToList();

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            var start = DisplayDateInternal.BeginningOfWeek(FirstDayOfWeek);
            var end = DisplayDateInternal.EndOfWeek(FirstDayOfWeek).DiscardTime();

            var columnHeaders = DateTimeHelper.Range(start, end, 1, TimeUnit.Day).ToList();
            var rowHeaders = DateTimeHelper.Range(DateTime.Today.BeginningOfDay(), DateTime.Today.EndOfDay(), 1, TimeUnit.Hour).Where(x => x.IsBetween(DateTime.Today.At(DisplayTimeStart), DateTime.Today.At(DisplayTimeEnd))).Select(x => x.TimeOfDay).ToList();

            return columnHeaders.SelectMany((x, columnIndex) => rowHeaders.Select((y, rowIndex) => (x.At(y), rowIndex, columnIndex)))
                                .Where(x => x.Item1.IsBetween(MinimumDateInternal, MaximumDateInternal))
                                .ToList();
        }

        public override DateTime GetNextDate(DateTime date) => date.AddDays(DateTimeHelper.NumberOfDaysInWeek());

        public override DateTime GetPreviousDate(DateTime date) => date.AddDays(-DateTimeHelper.NumberOfDaysInWeek());

        protected override bool ProcessDownKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Hour, shift);
            return true;
        }

        protected override bool ProcessUpKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Hour, shift);
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
            ProcessSelection(CurrentDate.EndOfWeek().BeginningOfHour(), shift);
            return true;
        }

        protected override bool ProcessHomeKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfWeek(), shift);
            return true;
        }

        protected override bool ProcessPageDownKey(bool shift)
        {
            ProcessSelection(CurrentDate.EndOfDay().BeginningOfHour(), shift);
            return true;
        }

        protected override bool ProcessPageUpKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfDay(), shift);
            return true;
        }
    }
}
