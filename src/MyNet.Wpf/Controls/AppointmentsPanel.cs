// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyNet.Observable;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Sequences;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    public class AppointmentsPanel : SimpleStackPanel
    {
        #region Public Properties

        public CalendarBase Owner
        {
            get => (CalendarBase)GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        public static readonly DependencyProperty OwnerProperty =
                DependencyProperty.Register(
                        "Owner",
                        typeof(CalendarBase),
                        typeof(AppointmentsPanel),
                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty IsStretchProperty
            = DependencyProperty.RegisterAttached("IsStretch", typeof(bool), typeof(AppointmentsPanel),
                new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetIsStretch(DependencyObject element) => (bool)element.GetValue(IsStretchProperty);

        public static void SetIsStretch(DependencyObject element, bool value) => element.SetValue(IsStretchProperty, value);

        #endregion Public Properties

        #region Protected Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Compute((x, y) => x.Measure(y.Size));

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Compute((x, y) => x.Arrange(y));
            return finalSize;
        }

        private void Compute(Action<CalendarAppointment, Rect> action)
        {
            if (Owner is null) return;

            var items = InternalChildren.OfType<CalendarAppointment>().Select(x => new CalendarAppointmentInfo(
                                                                                    new Interval<int>(Grid.GetRow(x), Grid.GetRow(x) + Grid.GetRowSpan(x) - 1),
                                                                                    new Interval<int>(Grid.GetColumn(x), Grid.GetColumn(x) + Grid.GetColumnSpan(x) - 1),
                                                                                    new Period(((IAppointment)x.DataContext).StartDate, ((IAppointment)x.DataContext).EndDate),
                                                                                    x
            )).OrderBy(x => x.Date).ToList();

            foreach (var item in items)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    var itemBoundsInSamePeriod = items.Where(x => x.ComputedBounds.HasValue && x.Period.Intersect(item.Period)).OrderBy(x => x.ComputedBounds!.Value.Y).ToList();
                    ComputeHorizontal(item, itemBoundsInSamePeriod);
                }
                else
                {
                    var itemBoundsInSamePeriod = items.Where(x => x.ComputedBounds.HasValue && x.Period.Intersect(item.Period)).OrderBy(x => x.ComputedBounds!.Value.X).ToList();
                    ComputeVertical(item, itemBoundsInSamePeriod);
                }
            }

            items.Where(item => item.ComputedBounds.HasValue).ForEach(x => action(x.Item, x.ComputedBounds!.Value));
        }

        private void ComputeVertical(CalendarAppointmentInfo item, List<CalendarAppointmentInfo> itemBoundsInSamePeriod)
        {
            var calendarItemSize = new Size(Owner.CalendarItemBounds?.Width ?? double.PositiveInfinity, Owner.CalendarItemBounds?.Height ?? double.PositiveInfinity);
            var isStretch = GetIsStretch(item.Item);
            var offsetFromStartDate = ApplyCoef(double.IsInfinity(calendarItemSize.Height) ? 0 : calendarItemSize.Height, item.Date);
            var offsetFromEndDate = ApplyCoef(double.IsInfinity(calendarItemSize.Height) ? 0 : calendarItemSize.Height, item.EndDate, true);

            var y = calendarItemSize.Height * item.Row + Owner.AppointmentsMargin.Top + offsetFromStartDate;
            var height = Math.Max(1, calendarItemSize.Height * item.RowsCount - Owner.AppointmentsMargin.Top - Owner.AppointmentsMargin.Bottom) - offsetFromStartDate - offsetFromEndDate;

            var x = calendarItemSize.Width * item.Column + Owner.AppointmentsMargin.Left;
            var width = Math.Max(1, calendarItemSize.Width - Owner.AppointmentsMargin.Right - Owner.AppointmentsMargin.Left);

            // 1 => Not stretch
            if (!isStretch)
            {
                var found = false;

                // 2 => Other items in same periods
                if (itemBoundsInSamePeriod.Count > 0)
                {
                    // Search an available space which width is upper or equals than calendarItemSize.Width / (itemsInSamePeriod + 1)
                    var minBounds = new Rect(x, y, width / (itemBoundsInSamePeriod.Count + 1), height);

                    // 2.1 => Search space before items in same Period
                    foreach (var itemBound in itemBoundsInSamePeriod.Select(x => x.ComputedBounds!.Value))
                    {
                        var itemBoundAndSpacing = new Rect(itemBound.X - Spacing, itemBound.Y, itemBound.Width + Spacing, itemBound.Height);
                        if (!minBounds.IntersectsWith(itemBoundAndSpacing))
                        {
                            x = minBounds.X;
                            width = itemBoundAndSpacing.Left - minBounds.X;
                            found = true;
                            break;
                        }
                        else
                        {
                            minBounds.X = itemBoundAndSpacing.Right;
                        }
                    }

                    // 2.2 => Search after last item in same period 
                    if (!found)
                    {
                        if (minBounds.Right <= x + width)
                        {
                            width = x + width - minBounds.Left;
                            x = minBounds.X;
                            found = true;
                        }

                        // 3 => No space found, we must resize other items
                        if (!found)
                        {
                            var widthToRemove = minBounds.Width / itemBoundsInSamePeriod.Count;

                            for (var i = 0; i < itemBoundsInSamePeriod.Count; i++)
                            {
                                var itemBound = itemBoundsInSamePeriod[i];
                                itemBound.ComputedBounds = new Rect(itemBound.ComputedBounds!.Value.X - (widthToRemove * i), itemBound.ComputedBounds!.Value.Y, Math.Max(1, itemBound.ComputedBounds!.Value.Width - widthToRemove), Math.Max(1, itemBound.ComputedBounds!.Value.Height));
                            }

                            minBounds.X = itemBoundsInSamePeriod[^1].ComputedBounds!.Value.Right;
                            width = x + width - minBounds.Left;
                            x = minBounds.X;
                        }
                    }
                }
            }

            // Affect bounds to item
            item.ComputedBounds = new Rect(x, y, Math.Max(1, width), Math.Max(1, height));
        }

        private void ComputeHorizontal(CalendarAppointmentInfo item, List<CalendarAppointmentInfo> itemBoundsInSamePeriod)
        {
            var calendarItemSize = new Size(Owner.CalendarItemBounds?.Width ?? double.PositiveInfinity, Owner.CalendarItemBounds?.Height ?? double.PositiveInfinity);
            var isStretch = GetIsStretch(item.Item);
            var offsetFromStartDate = ApplyCoef(double.IsInfinity(calendarItemSize.Width) ? 0 : calendarItemSize.Width, item.Date);
            var offsetFromEndDate = ApplyCoef(double.IsInfinity(calendarItemSize.Width) ? 0 : calendarItemSize.Width, item.EndDate, true);

            var x = calendarItemSize.Width * item.Column + Owner.AppointmentsMargin.Left + offsetFromStartDate;
            var width = Math.Max(1, calendarItemSize.Width * item.ColumnsCount - Owner.AppointmentsMargin.Left - Owner.AppointmentsMargin.Right) - offsetFromStartDate - offsetFromEndDate;

            var y = calendarItemSize.Width * item.Column + Owner.AppointmentsMargin.Top;
            var height = Math.Max(1, calendarItemSize.Height - Owner.AppointmentsMargin.Bottom - Owner.AppointmentsMargin.Top);

            // 1 => Not stretch
            if (!isStretch)
            {
                var found = false;

                // 2 => Other items in same periods
                if (itemBoundsInSamePeriod.Count > 0)
                {
                    // Search an available space which width is upper or equals than calendarItemSize.Width / (itemsInSamePeriod + 1)
                    var minBounds = new Rect(x, y, width, height / (itemBoundsInSamePeriod.Count + 1));

                    // 2.1 => Search space before items in same Period
                    foreach (var itemBound in itemBoundsInSamePeriod.Select(x => x.ComputedBounds!.Value))
                    {
                        var itemBoundAndSpacing = new Rect(itemBound.X, itemBound.Y - Spacing, itemBound.Width, itemBound.Height + Spacing);
                        if (!minBounds.IntersectsWith(itemBoundAndSpacing))
                        {
                            y = minBounds.Y;
                            height = itemBoundAndSpacing.Top - minBounds.Y;
                            found = true;
                            break;
                        }
                        else
                        {
                            minBounds.Y = itemBoundAndSpacing.Bottom;
                        }
                    }

                    // 2.2 => Search after last item in same period 
                    if (!found)
                    {
                        if (minBounds.Bottom <= y + height)
                        {
                            height = y + height - minBounds.Top;
                            y = minBounds.Y;
                            found = true;
                        }

                        // 3 => No space found, we must resize other items
                        if (!found)
                        {
                            var heightToRemove = minBounds.Height / itemBoundsInSamePeriod.Count;

                            for (var i = 0; i < itemBoundsInSamePeriod.Count; i++)
                            {
                                var itemBound = itemBoundsInSamePeriod[i];
                                itemBound.ComputedBounds = new Rect(itemBound.ComputedBounds!.Value.X, itemBound.ComputedBounds!.Value.Y - (heightToRemove * i), itemBound.ComputedBounds!.Value.Width, itemBound.ComputedBounds!.Value.Height - heightToRemove);
                            }

                            minBounds.Y = itemBoundsInSamePeriod[^1].ComputedBounds!.Value.Bottom;
                            height = y + height - minBounds.Top;
                            y = minBounds.Y;
                        }
                    }
                }
            }

            // Affect bounds to item
            item.ComputedBounds = new Rect(x, y, width, height);
        }

        private double ApplyCoef(double value, DateTime date, bool inverse = false) => Owner.UseAccurateItemPosition ? value * (inverse ? 1 - GetCoef(date) : GetCoef(date)) : 0;

        private double GetCoef(DateTime date)
            => Owner.IntervalUnit switch
            {
                TimeUnit.Hour => date.Minute / 60.0,
                TimeUnit.Day => date.TimeOfDay.TotalMinutes / 1440,
                _ => 0,
            };

        #endregion Protected Methods
    }

    internal class CalendarAppointmentInfo
    {
        public Interval<int> Rows { get; }

        public Interval<int> Columns { get; }

        public Period Period { get; }

        public CalendarAppointment Item { get; }

        public Rect? ComputedBounds { get; set; }

        public int Row => Rows.Start;

        public int Column => Columns.Start;

        public DateTime Date => Period.Start;

        public DateTime EndDate => Period.End;

        public int RowsCount => Rows.End - Rows.Start + 1;

        public int ColumnsCount => Columns.End - Columns.Start + 1;

        public CalendarAppointmentInfo(Interval<int> rows, Interval<int> columns, Period period, CalendarAppointment item)
        {
            Rows = rows;
            Columns = columns;
            Period = period;
            Item = item;
        }
    }
}
