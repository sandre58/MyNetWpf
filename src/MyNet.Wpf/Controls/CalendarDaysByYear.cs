// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MyNet.Utilities;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class CalendarDaysByYear : CalendarBase
    {
        static CalendarDaysByYear() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDaysByYear), new FrameworkPropertyMetadata(typeof(CalendarDaysByYear)));

        #region DisplayStartMonth

        public int DisplayStartMonth
        {
            get => (int)GetValue(DisplayStartMonthProperty);
            set => SetValue(DisplayStartMonthProperty, value);
        }

        public static readonly DependencyProperty DisplayStartMonthProperty = DependencyProperty.Register(nameof(DisplayStartMonth), typeof(int), typeof(CalendarDaysByYear), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayBoundChanged));

        #endregion DisplayStartMonth

        #region DisplayEndMonth

        public int DisplayEndMonth
        {
            get => (int)GetValue(DisplayEndMonthProperty);
            set => SetValue(DisplayEndMonthProperty, value);
        }

        public static readonly DependencyProperty DisplayEndMonthProperty = DependencyProperty.Register(nameof(DisplayEndMonth), typeof(int), typeof(CalendarDaysByYear), new FrameworkPropertyMetadata(12, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayBoundChanged));

        private static void OnDisplayBoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarDaysByYear;
            Debug.Assert(c != null);

            c.Rebuild();
        }

        #endregion DisplayEndMonth

        #region ShowWeeks

        public static readonly DependencyProperty ShowWeeksProperty = DependencyProperty.Register(nameof(ShowWeeks), typeof(bool), typeof(CalendarDaysByYear), new PropertyMetadata(true));

        public bool ShowWeeks
        {
            get => (bool)GetValue(ShowWeeksProperty);
            set => SetValue(ShowWeeksProperty, value);
        }

        #endregion ShowWeeks

        #region WeekStyle

        public static readonly DependencyProperty WeekStyleProperty = DependencyProperty.Register(nameof(WeekStyle), typeof(Style), typeof(CalendarDaysByYear));

        public Style WeekStyle
        {
            get => (Style)GetValue(WeekStyleProperty);
            set => SetValue(WeekStyleProperty, value);
        }

        #endregion WeekStyle

        public override TimeUnit IntervalUnit => TimeUnit.Day;

        protected override bool MustBeRebuild(DateTime oldDate, DateTime newDate) => !oldDate.SameYear(newDate);

        protected override IEnumerable<object> GetColumnHeaders()
        {
            var start = DisplayDateInternal.Previous(1, DisplayStartMonth).DiscardTime();
            var end = DisplayDateInternal.Next(28, DisplayEndMonth).EndOfMonth().DiscardTime();

            return DateTimeHelper.Range(start, end, 1, TimeUnit.Month).OfType<object>().ToList();
        }

        protected override IEnumerable<(DateTime date, int row, int column)> GetDisplayDates()
        {
            var start = DisplayDateInternal.Previous(1, DisplayStartMonth).DiscardTime();
            var end = DisplayDateInternal.Next(28, DisplayEndMonth).EndOfMonth().DiscardTime();

            var columnHeaders = DateTimeHelper.Range(start, end, 1, TimeUnit.Month).ToList();
            var rowHeaders = EnumerableHelper.Range(1, 31, 1).ToList();

            return columnHeaders.SelectMany((x, columnIndex) => rowHeaders.Where(y => y <= x.EndOfMonth().Day)
                                                                               .Select((y, rowIndex) => (new DateTime(x.Year, x.Month, y, 0, 0, 0, x.Kind), rowIndex, columnIndex)))
                                     .Where(x => x.Item1.IsBetween(MinimumDateInternal, MaximumDateInternal))
                                     .ToList();
        }

        protected override void BuildCore(CancellationToken cancellationToken)
        {
            base.BuildCore(cancellationToken);

            if (Grid is null) return;
            Grid.Children.RemoveRange(2, Grid.Children.Count - 2);

            foreach (var calendarItem in GetCalendarItems().ToList())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var date = calendarItem.Date;
                if (date.Date == date.LastDayOfWeek(FirstDayOfWeek).Date || date.Date == date.LastDayOfMonth().Date)
                {
                    var columnIndex = Grid.GetColumn(calendarItem);
                    var firstDay = date.FirstDayOfWeek(FirstDayOfWeek);
                    var row = firstDay.Month == date.Month ? firstDay.Day - 1 : 0;
                    var rowSpan = date.Day - row;
                    var week = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, FirstDayOfWeek);

                    _ = Grid.Children.Add(CreateWeekContent(week, row, columnIndex, rowSpan));
                }
            }
        }

        private ContentControl CreateWeekContent(int week, int row, int col, int rowSpan)
        {
            var weekCell = new ContentControl()
            {
                Content = week
            };
            _ = weekCell.SetBinding(StyleProperty, new Binding(nameof(WeekStyle)) { Source = this });
            _ = weekCell.SetBinding(VisibilityProperty, new Binding(nameof(ShowWeeks)) { Source = this, Converter = Converters.BooleanToVisibilityConverter.CollapsedIfFalse });
            weekCell.SetValue(Grid.RowProperty, row);
            weekCell.SetValue(Grid.ColumnProperty, col);
            weekCell.SetValue(Grid.RowSpanProperty, rowSpan);

            return weekCell;
        }

        public override DateTime GetNextDate(DateTime date) => date.AddYears(1);

        public override DateTime GetPreviousDate(DateTime date) => date.AddYears(-1);

        protected override bool ProcessDownKey(bool shift)
        {
            ProcessSelection(1, TimeUnit.Day, shift);
            return true;
        }

        protected override bool ProcessUpKey(bool shift)
        {
            ProcessSelection(-1, TimeUnit.Day, shift);
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
            ProcessSelection(CurrentDate.EndOfYear().BeginningOfDay(), shift);
            return true;
        }

        protected override bool ProcessHomeKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfYear(), shift);
            return true;
        }

        protected override bool ProcessPageDownKey(bool shift)
        {
            ProcessSelection(CurrentDate.EndOfMonth().BeginningOfDay(), shift);
            return true;
        }

        protected override bool ProcessPageUpKey(bool shift)
        {
            ProcessSelection(CurrentDate.BeginningOfMonth(), shift);
            return true;
        }
    }
}
