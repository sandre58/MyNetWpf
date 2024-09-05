// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyNet.Observable;
using MyNet.UI.Threading;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Units;

namespace MyNet.Wpf.Controls
{
    /// <summary>
    /// Represents a button control used in Scheduler Control, which reacts to the Click event.
    /// </summary>
    [TemplatePart(Name = PartAddButton, Type = typeof(Button))]
    public class CalendarItem : Control
    {
        private const string PartAddButton = "PART_AddButton";

        private readonly ReadOnlyObservableCollection<IAppointment> _appointments;
        private readonly IDisposable _disposable;

        public DateTime Date { get; internal set; }

        public ImmutablePeriod Period => Unit switch
        {
            TimeUnit.Millisecond => new ImmutablePeriod(Date, Date.AddMilliseconds(1).AddMilliseconds(-1)),
            TimeUnit.Second => new ImmutablePeriod(Date, Date.AddSeconds(1).AddMilliseconds(-1)),
            TimeUnit.Minute => new ImmutablePeriod(Date, Date.AddMinutes(1).AddMilliseconds(-1)),
            TimeUnit.Hour => new ImmutablePeriod(Date, Date.AddHours(1).AddMilliseconds(-1)),
            TimeUnit.Day => new ImmutablePeriod(Date, Date.AddDays(1).AddMilliseconds(-1)),
            TimeUnit.Week => new ImmutablePeriod(Date, Date.AddDays(7).AddMilliseconds(-1)),
            TimeUnit.Month => new ImmutablePeriod(Date, Date.AddMonths(1).AddMilliseconds(-1)),
            TimeUnit.Year => new ImmutablePeriod(Date, Date.AddYears(1).AddMilliseconds(-1)),
            _ => throw new InvalidOperationException(),
        };

        public CalendarBase Owner { get; }

        static CalendarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarItem), new FrameworkPropertyMetadata(typeof(CalendarItem)));
            EventManager.RegisterClassHandler(typeof(CalendarItem), MouseLeaveEvent, new RoutedEventHandler(OnVisualStateChanged));
        }

        public CalendarItem(CalendarBase owner, DateTime date, TimeUnit unit, ObservableCollection<CalendarAppointment> appointments)
        {
            Owner = owner;
            Unit = unit;
            var d = DependencyPropertyDescriptor.FromProperty(IsKeyboardFocusedProperty, typeof(CalendarItem));
            d.AddValueChanged(this, OnVisualStatePropertyChanged);

            var d1 = DependencyPropertyDescriptor.FromProperty(IsMouseOverProperty, typeof(CalendarItem));
            d1.AddValueChanged(this, OnVisualStatePropertyChanged);

            var d2 = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(CalendarItem));
            d2.AddValueChanged(this, OnVisualStatePropertyChanged);

            SetDate(date);

            _disposable = appointments.ToObservableChangeSet()
                                      .Transform(x => (IAppointment)x.DataContext)
                                      .ObserveOn(Scheduler.UI)
                                      .AutoRefresh(x => x.StartDate)
                                      .AutoRefresh(x => Date)
                                      .Filter(x => Dispatcher.Invoke(() => Owner.AppointmentsDisplayMode == AppointmentsDisplayMode.Cell) && IsMatch(x))
                                      .Sort(SortExpressionComparer<IAppointment>.Ascending(x => x.StartDate))
                                      .Bind(out _appointments)
                                      .Subscribe();
            SetValue(AppointmentsPropertyKey, _appointments);
        }

        ~CalendarItem() => _disposable.Dispose();

        public void SetDate(DateTime date)
        {
            Date = date;
            Content = date;
            DataContext = date;

            if (Date.IsBefore(Owner.MinimumDateInternal) || Date.IsAfter(Owner.MaximumDateInternal))
                IsEnabled = false;
            else
            {
                IsEnabled = true;
                SetValue(IsBlackedOutPropertyKey, Owner.BlackoutDates.Contains(Date));
                SetValue(IsInactivePropertyKey, Owner.IsInactive(Date));
                SetValue(IsSelectedPropertyKey, Owner.SelectedDatesInternal.Contains(Date));
                SetValue(IsTodayPropertyKey, Date.IsToday());
                SetValue(IsNowPropertyKey, Period.IsCurrent());
                SetValue(IsLastOfWeekPropertyKey, Date.IsLastDayOfWeek(Owner.FirstDayOfWeek));
                SetValue(IsFirstOfWeekPropertyKey, Date.IsFirstDayOfWeek(Owner.FirstDayOfWeek));
                SetValue(IsWeekendPropertyKey, Date.IsWeekend());
            }
        }

        #region AddCommand

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty AddCommandProperty =
                DependencyProperty.Register(
                        "AddCommand",
                        typeof(ICommand),
                        typeof(CalendarItem),
                        new FrameworkPropertyMetadata(null));

        #endregion

        #region Unit

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            nameof(Unit),
            typeof(TimeUnit),
            typeof(CalendarItem),
            new FrameworkPropertyMetadata(TimeUnit.Day));

        public TimeUnit Unit
        {
            get => (TimeUnit)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        #endregion TimeUnit

        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(CalendarItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// True if the SchedulerDay represents today
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion Content

        #region ContentTemplate

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
            nameof(ContentTemplate),
            typeof(DataTemplate),
            typeof(CalendarItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// True if the SchedulerDay represents today
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        #endregion ContentTemplate

        #region IsNow

        internal static readonly DependencyPropertyKey IsNowPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsNow),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Dependency property field for IsToday property
        /// </summary>
        public static readonly DependencyProperty IsNowProperty = IsNowPropertyKey.DependencyProperty;

        /// <summary>
        /// True if the SchedulerDay represents today
        /// </summary>
        public bool IsNow => (bool)GetValue(IsNowProperty);

        #endregion IsNow

        #region IsToday

        internal static readonly DependencyPropertyKey IsTodayPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsToday),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Dependency property field for IsToday property
        /// </summary>
        public static readonly DependencyProperty IsTodayProperty = IsTodayPropertyKey.DependencyProperty;

        /// <summary>
        /// True if the SchedulerDay represents today
        /// </summary>
        public bool IsToday => (bool)GetValue(IsTodayProperty);

        #endregion IsNow

        #region IsWeekend

        internal static readonly DependencyPropertyKey IsWeekendPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsWeekend),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsWeekendProperty = IsWeekendPropertyKey.DependencyProperty;

        public bool IsWeekend => (bool)GetValue(IsWeekendProperty);

        #endregion IsWeekend

        #region IsLastOfWeek

        internal static readonly DependencyPropertyKey IsLastOfWeekPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsLastOfWeek),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsLastOfWeekProperty = IsLastOfWeekPropertyKey.DependencyProperty;

        public bool IsLastOfWeek => (bool)GetValue(IsLastOfWeekProperty);

        #endregion IsLastOfWeek

        #region IsFirstOfWeek

        internal static readonly DependencyPropertyKey IsFirstOfWeekPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(IsFirstOfWeek),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsFirstOfWeekProperty = IsFirstOfWeekPropertyKey.DependencyProperty;

        public bool IsFirstOfWeek => (bool)GetValue(IsFirstOfWeekProperty);

        #endregion IsFirstOfWeek

        #region IsInactive

        internal static readonly DependencyPropertyKey IsInactivePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsInactive),
            typeof(bool),
            typeof(CalendarItem),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Dependency property field for IsInactive property
        /// </summary>
        public static readonly DependencyProperty IsInactiveProperty = IsInactivePropertyKey.DependencyProperty;

        /// <summary>
        /// True if the SchedulerButton represents
        ///     a month that falls outside the current year
        ///     or
        ///     a year that falls outside the current decade
        /// </summary>
        public bool IsInactive
        {
            get => (bool)GetValue(IsInactiveProperty);
            internal set => SetValue(IsInactivePropertyKey, value);
        }

        #endregion IsInactive

        #region IsSelected

        internal static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsSelected),
            typeof(bool),
            typeof(CalendarItem),
            new FrameworkPropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Dependency property field for IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        /// <summary>
        /// True if the CalendarItem is selected
        /// </summary>
        public bool IsSelected => (bool)GetValue(IsSelectedProperty);

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listItem = d as CalendarItem;

            listItem?.UpdateVisualState(true);
        }

        #endregion IsSelected

        #region IsBlackedOut

        internal static readonly DependencyPropertyKey IsBlackedOutPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsBlackedOut),
                typeof(bool),
                typeof(CalendarItem),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Dependency property field for IsBlackedOut property
        /// </summary>
        public static readonly DependencyProperty IsBlackedOutProperty = IsBlackedOutPropertyKey.DependencyProperty;

        /// <summary>
        /// True if the CalendarItem represents a blackout date
        /// </summary>
        public bool IsBlackedOut => (bool)GetValue(IsBlackedOutProperty);

        #endregion IsBlackedOut

        #region Appointments

        internal static readonly DependencyPropertyKey AppointmentsPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Appointments),
        typeof(ReadOnlyObservableCollection<IAppointment>),
        typeof(CalendarItem),
        new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty AppointmentsProperty = AppointmentsPropertyKey.DependencyProperty;

        public ReadOnlyObservableCollection<IAppointment> Appointments => (ReadOnlyObservableCollection<IAppointment>)GetValue(AppointmentsProperty);

        #endregion

        private static void OnVisualStateChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not CalendarItem calendarItem) return;

            calendarItem.UpdateVisualState(true);
        }

        public override void OnApplyTemplate()
        {
            var addButton = GetTemplateChild(PartAddButton) as Button;

            if (addButton is not null)
            {
                addButton.Click -= AddButton_Click;
                addButton.Click += AddButton_Click;
            }
            base.OnApplyTemplate();
            UpdateVisualState(false);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) => Owner?.AddCommand?.Execute(Date);

        private void OnVisualStatePropertyChanged(object? sender, EventArgs e)
        {
            if (sender is CalendarItem control)
            {
                control.UpdateVisualState(true);
            }
        }

        protected virtual void UpdateVisualState(bool useTransitions)
        {
            _ = !IsEnabled
                ? VisualStateManager.GoToState(this, Content is Control ? "Normal" : "Disabled", useTransitions)
                : IsMouseOver && Owner?.DatesSelectionMode != CalendarSelectionMode.None
                    ? VisualStateManager.GoToState(this, "MouseOver", useTransitions)
                    : VisualStateManager.GoToState(this, "Normal", useTransitions);

            _ = IsKeyboardFocused
                ? VisualStateManager.GoToState(this, "Focused", useTransitions)
                : VisualStateManager.GoToState(this, "Unfocused", useTransitions);

            _ = IsSelected
                ? VisualStateManager.GoToState(this, "Selected", useTransitions)
                : VisualStateManager.GoToState(this, "Unselected", useTransitions);
        }

        protected bool IsMatch(IAppointment appointment) => Period.Start < appointment.StartDate && Period.End > appointment.EndDate
                                                    || appointment.StartDate < Period.Start && appointment.EndDate > Period.End
                                                    || Period.Contains(appointment.StartDate)
                                                    || Period.Contains(appointment.EndDate);

        public DateTime GetAccurateDate(Point itemMousePosition)
            => ActualHeight.NearlyEqual(0)
                ? Date
                : Unit switch
                {
                    TimeUnit.Hour => Date.AddMinutes((int)(itemMousePosition.Y * 60 / ActualHeight)),
                    TimeUnit.Day => Date.AddMinutes((int)(itemMousePosition.Y * 1440 / ActualHeight)),
                    _ => Date,
                };

        public Point GetRelativePositionFromDate(DateTime date)
        {
            var height = ActualHeight > 0 ? ActualHeight : !double.IsNaN(Height) ? Height : 0;

            return height.NearlyEqual(0)
                ? new Point(0, 0)
                : Unit switch
                {
                    TimeUnit.Hour => new(0.0, (date - Date).TotalMinutes * height / 60),
                    TimeUnit.Day => new(0.0, (date - Date).TotalMinutes * height / 1440),
                    _ => new Point(0, 0),
                };
        }
    }
}
