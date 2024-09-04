// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DynamicData;
using MyNet.DynamicData.Extensions;
using MyNet.Observable;
using MyNet.UI.Busy;
using MyNet.UI.Busy.Models;
using MyNet.UI.Collections;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Localization;
using MyNet.Utilities.Logging;
using MyNet.Utilities.Threading;
using MyNet.Utilities.Units;
using MyNet.Wpf.Busy;
using MyNet.Wpf.Controls.Calendars;
using MyNet.Wpf.Converters;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ElementPreviousButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementNextButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementTodayButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementDatesItemsControl, Type = typeof(CalendarItemsControl))]
    [TemplatePart(Name = ElementRowHeaders, Type = typeof(ItemsControl))]
    [TemplatePart(Name = ElementColumnHeaders, Type = typeof(ItemsControl))]
    [TemplatePart(Name = ElementGrid, Type = typeof(Grid))]
    [TemplatePart(Name = ElementAccurateDate, Type = typeof(ContentControl))]
    [TemplatePart(Name = ElementAccurateDatePreview, Type = typeof(ContentControl))]
    [ContentProperty("Items")]
    public abstract class CalendarBase : ListBox
    {
        private const string ElementPreviousButton = "PART_PreviousButton";
        private const string ElementNextButton = "PART_NextButton";
        private const string ElementTodayButton = "PART_TodayButton";
        private const string ElementDatesItemsControl = "PART_DatesItemsControl";
        private const string ElementRowHeaders = "PART_RowHeaders";
        private const string ElementColumnHeaders = "PART_ColumnHeaders";
        private const string ElementGrid = "PART_Grid";
        private const string ElementAccurateDate = "PART_AccurateDate";
        private const string ElementAccurateDatePreview = "PART_AccurateDatePreview";

        private DateTime? _hoverStart;
        private DateTime? _hoverEnd;
        private bool _isItemPressed;
        private DateTime? _currentDate;
        private ContentControl? _accurateDateControl;
        private ContentControl? _accurateDatePreviewControl;
        private readonly UiObservableCollection<object> _columnHeaders = [];
        private readonly UiObservableCollection<object> _rowHeaders = [];
        private readonly UiObservableCollection<CalendarItem> _displayDates = [];
        private readonly UiObservableCollection<CalendarAppointment> _appointments = [];
        private readonly SingleTaskRunner _updateAppointments;
        private readonly SingleTaskRunner _build;

        protected Grid? Grid { get; private set; }

        internal CalendarItemsControl? DatesItemsControl { get; private set; }

        internal Button? NextButton { get; private set; }

        internal Button? TodayButton { get; private set; }

        internal Button? PreviousButton { get; private set; }

        internal Size? CalendarItemBounds => DatesItemsControl is not null ? new Size(DatesItemsControl.ActualWidth / ColumnsCount, DatesItemsControl.ActualHeight / RowsCount) : null;

        public static readonly RoutedEvent SelectedDatesChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedDatesChanged), RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(CalendarBase));

        public event EventHandler<SelectionChangedEventArgs>? SelectedDatesChanged
        {
            add => AddHandler(SelectedDatesChangedEvent, value);
            remove => RemoveHandler(SelectedDatesChangedEvent, value);
        }

        public event EventHandler<DateChangedEventArgs>? DisplayDateChanged;

        public event EventHandler<EventArgs>? SelectionModeChanged;

        static CalendarBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarBase), new FrameworkPropertyMetadata(typeof(CalendarBase)));
            FocusableProperty.OverrideMetadata(typeof(CalendarBase), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CalendarBase), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CalendarBase), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            LanguageProperty.OverrideMetadata(typeof(CalendarBase), new FrameworkPropertyMetadata(OnLanguageChanged));

            EventManager.RegisterClassHandler(typeof(CalendarBase), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        protected CalendarBase()
        {
            _build = new(async x => await Dispatcher.BeginInvoke(() => Build(x)));
            _updateAppointments = new(async x => await Dispatcher.Invoke(() => BusyService).WaitAsync<IndeterminateBusy>(async _ => await UpdateAppointmentsAsync(x).ConfigureAwait(false)));
            BlackoutDates = new BlackoutDatesCollection(this);
            SelectedDatesInternal = new Calendars.SelectedDatesCollection(this);
            SetCurrentValue(DisplayDateProperty, DateTime.Now);
            ItemsSource = _appointments;
            BusyService = new BusyService();

            GlobalizationService.Current.TimeZoneChanged += OnTimeZoneChangedCallback;
        }

        ~CalendarBase()
        {
            GlobalizationService.Current.TimeZoneChanged -= OnTimeZoneChangedCallback;
            _updateAppointments.Dispose();
            _build.Dispose();
        }

        private void OnTimeZoneChangedCallback(object? sender, EventArgs e) => UpdateAppointments();

        #region Orientation

        public Orientation ItemsOrientation
        {
            get => (Orientation)GetValue(ItemsOrientationProperty);
            set => SetValue(ItemsOrientationProperty, value);
        }

        public static readonly DependencyProperty ItemsOrientationProperty =
                DependencyProperty.Register(
                        "ItemsOrientation",
                        typeof(Orientation),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(Orientation.Vertical));

        #endregion

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
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(null));

        #endregion

        #region ShowAccurateDate

        public bool ShowAccurateDate
        {
            get => (bool)GetValue(ShowAccurateDateProperty);
            set => SetValue(ShowAccurateDateProperty, value);
        }

        public static readonly DependencyProperty ShowAccurateDateProperty =
            DependencyProperty.Register(
            nameof(ShowAccurateDate),
            typeof(bool),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(false));

        #endregion ShowHeader

        #region AccurateDateForeground

        public Brush AccurateDateForeground
        {
            get => (Brush)GetValue(AccurateDateForegroundProperty);
            set => SetValue(AccurateDateForegroundProperty, value);
        }

        public static readonly DependencyProperty AccurateDateForegroundProperty =
                DependencyProperty.Register(
                        "AccurateDateForeground",
                        typeof(Brush),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(
                                SystemColors.HighlightBrush,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region AccurateDateTemplate

        public DataTemplate AccurateDateTemplate
        {
            get => (DataTemplate)GetValue(AccurateDateTemplateProperty);
            set => SetValue(AccurateDateTemplateProperty, value);
        }

        public static readonly DependencyProperty AccurateDateTemplateProperty =
                DependencyProperty.Register(
                        "AccurateDateTemplate",
                        typeof(DataTemplate),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region AccurateDatePreviewOpacity

        public double AccurateDatePreviewOpacity
        {
            get => (double)GetValue(AccurateDatePreviewOpacityProperty);
            set => SetValue(AccurateDatePreviewOpacityProperty, value);
        }

        public static readonly DependencyProperty AccurateDatePreviewOpacityProperty =
                DependencyProperty.Register(
                        "AccurateDatePreviewOpacity",
                        typeof(double),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(
                                0.56d,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region UseAccurateItemPosition

        public bool UseAccurateItemPosition
        {
            get => (bool)GetValue(UseAccurateItemPositionProperty);
            set => SetValue(UseAccurateItemPositionProperty, value);
        }

        public static readonly DependencyProperty UseAccurateItemPositionProperty =
                DependencyProperty.Register(
                        "UseAccurateItemPosition",
                        typeof(bool),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(false));

        #endregion

        #region InnerBorderBrush

        public Brush InnerBorderBrush
        {
            get => (Brush)GetValue(InnerBorderBrushProperty);
            set => SetValue(InnerBorderBrushProperty, value);
        }

        public static readonly DependencyProperty InnerBorderBrushProperty =
                DependencyProperty.Register(
                        "InnerBorderBrush",
                        typeof(Brush),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region InnerBorderThickess

        public double InnerBorderThickess
        {
            get => (double)GetValue(InnerBorderThickessProperty);
            set => SetValue(InnerBorderThickessProperty, value);
        }

        public static readonly DependencyProperty InnerBorderThickessProperty =
                DependencyProperty.Register(
                        "InnerBorderThickess",
                        typeof(double),
                        typeof(CalendarBase),
                        new FrameworkPropertyMetadata(
                                1d,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region Appointments

        internal static readonly DependencyProperty AppointmentsProperty = DependencyProperty.Register(nameof(Appointments), typeof(IEnumerable), typeof(CalendarBase), new FrameworkPropertyMetadata(null, OnAppointmentsChanged));

        public IEnumerable Appointments
        {
            get => (IEnumerable)GetValue(AppointmentsProperty);
            set => SetValue(AppointmentsProperty, value);
        }

        private static void OnAppointmentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CalendarBase)?.OnAppointmentsChanged(e);
            (d as CalendarBase)?.UpdateAppointments();
        }

        private void OnAppointmentsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged -= OnAppointmentsCollectionChangedCallbackAsync;

                if (e.OldValue is IEnumerable<object> enumerable)
                    enumerable.OfType<IAppointment>().ForEach(x => x.PropertyChanged -= OnItemPropertyChangedCallbackAsync);
            }

            if (e.NewValue is INotifyCollectionChanged notifyCollectionChanged1)
            {
                if (e.NewValue is IEnumerable<object> enumerable)
                {
                    enumerable.OfType<IAppointment>().ForEach(x =>
                    {
                        if (x is INotifyPropertyChanged npc)
                        {
                            npc.PropertyChanged -= OnItemPropertyChangedCallbackAsync;
                            npc.PropertyChanged += OnItemPropertyChangedCallbackAsync;
                        }
                    });
                }

                notifyCollectionChanged1.CollectionChanged -= OnAppointmentsCollectionChangedCallbackAsync;
                notifyCollectionChanged1.CollectionChanged += OnAppointmentsCollectionChangedCallbackAsync;
            }
        }

        private async void OnAppointmentsCollectionChangedCallbackAsync(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<IAppointment>())
                {
                    if (item is INotifyPropertyChanged npc)
                    {
                        npc.PropertyChanged -= OnItemPropertyChangedCallbackAsync;
                        npc.PropertyChanged += OnItemPropertyChangedCallbackAsync;
                    }
                    await SynchronizeAppointmentAsync(item, CancellationToken.None).ConfigureAwait(false);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged npc)
                        npc.PropertyChanged -= OnItemPropertyChangedCallbackAsync;

                    _appointments.RemoveMany(_appointments.Where(x => x.DataContext == item).ToList());
                }
            }
        }

        private async void OnItemPropertyChangedCallbackAsync(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IAppointment appointment && e.PropertyName is (nameof(IAppointment.StartDate)) /* or (nameof(IAppointment.EndDate)) */)
                await SynchronizeAppointmentAsync(appointment, CancellationToken.None).ConfigureAwait(false);
        }

        #endregion Appointments

        #region BlackoutDates

        public BlackoutDatesCollection BlackoutDates { get; }

        #endregion BlackoutDates

        #region CalendarItemStyle

        public static readonly DependencyProperty CalendarItemStyleProperty = DependencyProperty.Register(nameof(CalendarItemStyle), typeof(Style), typeof(CalendarBase));

        public Style CalendarItemStyle
        {
            get => (Style)GetValue(CalendarItemStyleProperty);
            set => SetValue(CalendarItemStyleProperty, value);
        }

        #endregion CalendarItemStyle

        #region HeaderTemplate

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(CalendarBase));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion CalendarItemStyle

        #region ColumnHeaderTemplate

        public static readonly DependencyProperty ColumnHeaderTemplateProperty = DependencyProperty.Register(nameof(ColumnHeaderTemplate), typeof(DataTemplate), typeof(CalendarBase));

        public DataTemplate ColumnHeaderTemplate
        {
            get => (DataTemplate)GetValue(ColumnHeaderTemplateProperty);
            set => SetValue(ColumnHeaderTemplateProperty, value);
        }

        #endregion ColumnHeaderTemplate

        #region RowHeaderTemplate

        public static readonly DependencyProperty RowHeaderTemplateProperty = DependencyProperty.Register(nameof(RowHeaderTemplate), typeof(DataTemplate), typeof(CalendarBase));

        public DataTemplate RowHeaderTemplate
        {
            get => (DataTemplate)GetValue(RowHeaderTemplateProperty);
            set => SetValue(RowHeaderTemplateProperty, value);
        }

        #endregion RowHeaderTemplate

        #region DisplayDateStart

        internal static readonly DependencyPropertyKey DisplayDateStartPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(DisplayDateStart),
                typeof(DateTime?),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisplayDateStartProperty = DisplayDateStartPropertyKey.DependencyProperty;

        public DateTime? DisplayDateStart => (DateTime?)GetValue(DisplayDateStartProperty);

        #endregion DisplayDateStart

        #region DisplayDateEnd

        internal static readonly DependencyPropertyKey DisplayDateEndPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(DisplayDateEnd),
                typeof(DateTime?),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DisplayDateEndProperty = DisplayDateEndPropertyKey.DependencyProperty;

        public DateTime? DisplayDateEnd => (DateTime?)GetValue(DisplayDateEndProperty);

        #endregion DisplayDateEnd

        #region DisplayDate

        internal DateTime CurrentDate
        {
            get => _currentDate.GetValueOrDefault(DisplayDateInternal);
            set => _currentDate = value;
        }

        internal DateTime DisplayDateInternal
        {
            get;
            private set;
        }

        internal DateTime MaximumDateInternal => MaximumDate.GetValueOrDefault(DateTime.MaxValue);

        internal DateTime MinimumDateInternal => MinimumDate.GetValueOrDefault(DateTime.MinValue);

        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        ///
        public DateTime DisplayDate
        {
            get => (DateTime)GetValue(DisplayDateProperty);
            set => SetValue(DisplayDateProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            nameof(DisplayDate),
            typeof(DateTime),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateChanged, CoerceDisplayDate));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            Debug.Assert(c != null);

            var newDate = (DateTime)e.NewValue;
            var oldDate = (DateTime?)e.OldValue;

            if (newDate == default) return;
            c.DisplayDateInternal = newDate;

            if (!oldDate.HasValue || c.MustBeRebuild(oldDate.Value, newDate))
                c.Rebuild();

            c.OnDisplayDateChanged(new DateChangedEventArgs((DateTime)e.OldValue, (DateTime)e.NewValue));
        }

        private void OnDisplayDateChanged(DateChangedEventArgs e) => DisplayDateChanged?.Invoke(this, e);

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            var c = d as CalendarBase;

            var date = (DateTime)value;
            if (c?.MinimumDate != null && date < c.MinimumDate.Value)
                value = c.MinimumDate.Value;
            else if (c?.MaximumDate != null && date > c.MaximumDate.Value)
                value = c.MaximumDate.Value;

            return value;
        }

        #endregion DisplayDate

        #region MaximumDate

        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        ///
        public DateTime? MaximumDate
        {
            get => (DateTime?)GetValue(MaximumDateProperty);
            set => SetValue(MaximumDateProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDateEnd dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumDateProperty = DependencyProperty.Register(nameof(MaximumDate), typeof(DateTime?), typeof(CalendarBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMaximumDateChanged, CoerceMaximumDate));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateEnd.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMaximumDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayDateProperty);
            c.Rebuild();
        }

        private static object CoerceMaximumDate(DependencyObject d, object value)
        {
            var c = d as CalendarBase;

            var date = (DateTime?)value;

            if (date.HasValue)
            {
                if (c?.MinimumDate != null && date.Value < c.MinimumDate.Value)
                {
                    value = c.MinimumDate;
                }

                var maxSelectedDate = c?.SelectedDatesInternal.MaximumDate;
                if (maxSelectedDate != null && date.Value < maxSelectedDate.Value)
                {
                    value = maxSelectedDate;
                }
            }

            return value;
        }

        #endregion MaximumDate

        #region MinimumDate

        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        ///
        public DateTime? MinimumDate
        {
            get => (DateTime?)GetValue(MinimumDateProperty);
            set => SetValue(MinimumDateProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDateStart dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumDateProperty = DependencyProperty.Register(nameof(MinimumDate), typeof(DateTime?), typeof(CalendarBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnMinimumDateChanged, CoerceMinimumDate));

        /// <summary>
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateStart.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnMinimumDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            Debug.Assert(c != null);

            c.CoerceValue(MaximumDateProperty);
            c.CoerceValue(DisplayDateProperty);
            c.Rebuild();
        }

        private static object CoerceMinimumDate(DependencyObject d, object value)
        {
            var c = d as CalendarBase;

            var date = (DateTime?)value;

            if (date.HasValue)
            {
                var minSelectedDate = c?.SelectedDatesInternal.MinimumDate;
                if (minSelectedDate != null && date.Value > minSelectedDate.Value)
                {
                    value = minSelectedDate;
                }
            }

            return value;
        }

        #endregion MinimumDate

        #region FirstDayOfWeek

        /// <summary>
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            set => SetValue(FirstDayOfWeekProperty, value);
        }

        /// <summary>
        /// Identifies the FirstDayOfWeek dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
            nameof(FirstDayOfWeek),
            typeof(DayOfWeek),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek, OnFirstDayOfWeekChanged), IsValidFirstDayOfWeek);

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its FirstDayOfWeek.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            c?.Rebuild();
        }

        internal static bool IsValidFirstDayOfWeek(object value)
        {
            var day = (DayOfWeek)value;

            return day is DayOfWeek.Sunday
                or DayOfWeek.Monday
                or DayOfWeek.Tuesday
                or DayOfWeek.Wednesday
                or DayOfWeek.Thursday
                or DayOfWeek.Friday
                or DayOfWeek.Saturday;
        }

        #endregion FirstDayOfWeek

        #region ShowHeader

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register(
            nameof(ShowHeader),
            typeof(bool),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(true));

        #endregion ShowHeader

        #region LastDayOfWeekIsHighlighted

        public bool LastDayOfWeekIsHighlighted
        {
            get => (bool)GetValue(LastDayOfWeekIsHighlightedProperty);
            set => SetValue(LastDayOfWeekIsHighlightedProperty, value);
        }

        public static readonly DependencyProperty LastDayOfWeekIsHighlightedProperty =
            DependencyProperty.Register(
            nameof(LastDayOfWeekIsHighlighted),
            typeof(bool),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(false));

        #endregion LastDayOfWeekIsHighlighted

        #region CanMoveWithScroll

        public bool CanMoveWithScroll
        {
            get => (bool)GetValue(CanMoveWithScrollProperty);
            set => SetValue(CanMoveWithScrollProperty, value);
        }

        public static readonly DependencyProperty CanMoveWithScrollProperty =
            DependencyProperty.Register(
            nameof(CanMoveWithScroll),
            typeof(bool),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(false));

        #endregion CanMoveWithScroll

        #region Language

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DependencyPropertyHelper.GetValueSource(d, FirstDayOfWeekProperty).BaseValueSource == BaseValueSource.Default && d is CalendarBase c)
            {
                c.SetCurrentValue(FirstDayOfWeekProperty, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
            }
        }

        #endregion Language

        #region SelectedDate

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        ///
        public DateTime? SelectedDateInternal
        {
            get => SelectedDatesInternal.MinimumDate;
            set
            {
                if (DatesSelectionMode != CalendarSelectionMode.None || value == null)
                {
                    var addedDate = value;

                    if (IsValidDateSelection(addedDate))
                    {
                        if (!addedDate.HasValue)
                            SelectedDatesInternal.Clear();
                        else
                        {
                            if (!(SelectedDatesInternal.Count > 0 && SelectedDatesInternal[0] == addedDate.Value))
                            {
                                SelectedDatesInternal.ClearInternal();
                                SelectedDatesInternal.Add(addedDate.Value);
                            }
                        }

                        // We update the current date for only the Single mode.For the other modes it automatically gets updated
                        if (DatesSelectionMode == CalendarSelectionMode.SingleDate && addedDate.HasValue)
                            CurrentDate = addedDate.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        ///
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedDate dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateTime?),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its SelectedDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            Debug.Assert(c != null);

            if ((DateTime?)e.NewValue != c.SelectedDateInternal)
            {
                c.SelectedDateInternal = (DateTime?)e.NewValue;
            }
        }

        #endregion SelectedDate

        #region AccurateDate

        internal static readonly DependencyPropertyKey AccurateDateIsVisiblePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(AccurateDateIsVisible),
        typeof(bool),
        typeof(CalendarBase),
        new FrameworkPropertyMetadata(false, OnAccurateDatePreviewChanged));

        public static readonly DependencyProperty AccurateDateIsVisibleProperty = AccurateDateIsVisiblePropertyKey.DependencyProperty;

        public bool AccurateDateIsVisible => (bool)GetValue(AccurateDateIsVisibleProperty);

        public DateTime? AccurateDate
        {
            get => (DateTime?)GetValue(AccurateDateProperty);
            set => SetValue(AccurateDateProperty, value);
        }

        public static readonly DependencyProperty AccurateDateProperty =
            DependencyProperty.Register(nameof(AccurateDate), typeof(DateTime?),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAccurateDateChanged));

        private static void OnAccurateDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not CalendarBase calendar) return;

            calendar.RefreshAccurateDateControl();
        }

        private void RefreshAccurateDateControl() => SetValue(AccurateDateIsVisiblePropertyKey, UpdateAccurateDateControl(AccurateDate, _accurateDateControl));

        private bool UpdateAccurateDateControl(DateTime? date, ContentControl? contentControl)
        {
            if (!date.HasValue || contentControl is null) return false;

            var item = GetCalendarItemFromDate(date.Value);

            if (item is null) return false;

            Grid.SetColumn(contentControl, Grid.GetColumn(item));
            Grid.SetRow(contentControl, Grid.GetRow(item));
            Grid.SetRowSpan(contentControl, Math.Max(RowsCount - Grid.GetRow(item), 1));
            contentControl.Margin = new Thickness(0, item.GetRelativePositionFromDate(date.Value).Y, 0, 0);

            return true;
        }

        #endregion AccurateDate

        #region AccurateDatePreview

        internal static readonly DependencyPropertyKey AccurateDatePreviewPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(AccurateDatePreview),
                typeof(DateTime?),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(null, OnAccurateDatePreviewChanged));

        public static readonly DependencyProperty AccurateDatePreviewProperty = AccurateDatePreviewPropertyKey.DependencyProperty;

        public DateTime? AccurateDatePreview => (DateTime?)GetValue(AccurateDatePreviewProperty);

        private static void OnAccurateDatePreviewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not CalendarBase calendar) return;

            calendar.RefreshAccurateDatePreviewControl();
        }

        private void RefreshAccurateDatePreviewControl() => UpdateAccurateDateControl(AccurateDatePreview, _accurateDatePreviewControl);

        #endregion AccurateDatePreview

        #region SelectedDatesInternal

        /// <summary>
        /// Gets the dates that are currently selected.
        /// </summary>
        internal Calendars.SelectedDatesCollection SelectedDatesInternal { get; set; }

        internal static readonly DependencyProperty SelectedDatesProperty = DependencyProperty.Register(
            nameof(SelectedDates),
            typeof(IEnumerable),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(new List<DateTime>()));

        /// <summary>
        /// True if the CalendarItem represents a day that falls in the currently displayed month
        /// </summary>
        public IEnumerable SelectedDates
        {
            get => (IEnumerable)GetValue(SelectedDatesProperty);
            set => SetValue(SelectedDatesProperty, value);
        }

        #endregion SelectedDatesInternal

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection mode for the Calendar.
        /// </summary>
        public CalendarSelectionMode DatesSelectionMode
        {
            get => (CalendarSelectionMode)GetValue(DatesSelectionModeProperty);
            set => SetValue(DatesSelectionModeProperty, value);
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DatesSelectionModeProperty =
            DependencyProperty.Register(
            nameof(DatesSelectionMode),
            typeof(CalendarSelectionMode),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(CalendarSelectionMode.SingleDate, OnSelectionModeChanged),
            IsValidSelectionMode);

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            Debug.Assert(c != null);

            c._hoverStart = c._hoverEnd = null;
            c.SelectedDatesInternal.Clear();
            c.OnSelectionModeChanged(EventArgs.Empty);
        }

        private void OnSelectionModeChanged(EventArgs e)
        {
            var handler = SelectionModeChanged;

            handler?.Invoke(this, e);
        }

        private static bool IsValidSelectionMode(object value)
        {
            var mode = (CalendarSelectionMode)value;

            return mode is CalendarSelectionMode.SingleDate
                or CalendarSelectionMode.SingleRange
                or CalendarSelectionMode.MultipleRange
                or CalendarSelectionMode.None;
        }

        #endregion SelectionMode

        #region AppointmentsDisplayMode

        public AppointmentsDisplayMode AppointmentsDisplayMode
        {
            get => (AppointmentsDisplayMode)GetValue(AppointmentsDisplayModeProperty);
            set => SetValue(AppointmentsDisplayModeProperty, value);
        }

        public static readonly DependencyProperty AppointmentsDisplayModeProperty =
            DependencyProperty.Register(
            nameof(AppointmentsDisplayMode),
            typeof(AppointmentsDisplayMode),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(AppointmentsDisplayMode.Range, OnAppointmentsDisplayModeChanged),
            IsValidAppointmentsDisplayMode);

        private static bool IsValidAppointmentsDisplayMode(object value)
        {
            var mode = (AppointmentsDisplayMode)value;

            return mode is AppointmentsDisplayMode.Range or AppointmentsDisplayMode.Cell;
        }

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its FirstDayOfWeek.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnAppointmentsDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CalendarBase;
            c?.UpdateAppointments();
        }

        #endregion AppointmentsDisplayMode

        #region AppointmentsMargin

        public Thickness AppointmentsMargin
        {
            get => (Thickness)GetValue(AppointmentsMarginProperty);
            set => SetValue(AppointmentsMarginProperty, value);
        }

        public static readonly DependencyProperty AppointmentsMarginProperty =
            DependencyProperty.Register(
            nameof(AppointmentsMargin),
            typeof(Thickness),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(new Thickness(0)));

        #endregion AppointmentsMargin

        #region AppointmentsSpacing

        public double AppointmentsSpacing
        {
            get => (double)GetValue(AppointmentsSpacingProperty);
            set => SetValue(AppointmentsSpacingProperty, value);
        }

        public static readonly DependencyProperty AppointmentsSpacingProperty =
            DependencyProperty.Register(
            nameof(AppointmentsSpacing),
            typeof(double),
            typeof(CalendarBase),
            new FrameworkPropertyMetadata(5d));

        #endregion AppointmentsSpacing

        #region ColumnsCount

        internal static readonly DependencyPropertyKey ColumnsCountPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(ColumnsCount),
                typeof(int),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(1));

        public static readonly DependencyProperty ColumnsCountProperty = ColumnsCountPropertyKey.DependencyProperty;

        public int ColumnsCount => (int)GetValue(ColumnsCountProperty);

        #endregion ColumnsCount

        #region RowsCount

        internal static readonly DependencyPropertyKey RowsCountPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(RowsCount),
                typeof(int),
                typeof(CalendarBase),
                new FrameworkPropertyMetadata(1));

        public static readonly DependencyProperty RowsCountProperty = RowsCountPropertyKey.DependencyProperty;

        public int RowsCount => (int)GetValue(RowsCountProperty);

        #endregion RowsCount

        #region BusyService

        internal static readonly DependencyProperty BusyServiceProperty = DependencyProperty.Register(nameof(BusyService), typeof(IBusyService), typeof(CalendarBase), new FrameworkPropertyMetadata(null));

        public IBusyService BusyService
        {
            get => (IBusyService)GetValue(BusyServiceProperty);
            set => SetValue(BusyServiceProperty, value);
        }

        #endregion BusyService

        #region Methods

        #region Build

        public override void OnApplyTemplate()
        {
            _accurateDateControl = GetTemplateChild(ElementAccurateDate) as ContentControl;
            _accurateDatePreviewControl = GetTemplateChild(ElementAccurateDatePreview) as ContentControl;
            Grid = GetTemplateChild(ElementGrid) as Grid;
            DatesItemsControl = GetTemplateChild(ElementDatesItemsControl) as CalendarItemsControl;
            PreviousButton = GetTemplateChild(ElementPreviousButton) as Button;
            NextButton = GetTemplateChild(ElementNextButton) as Button;
            TodayButton = GetTemplateChild(ElementTodayButton) as Button;

            var rowHeadersItemsControl = GetTemplateChild(ElementRowHeaders) as ItemsControl;
            if (rowHeadersItemsControl is not null)
            {
                rowHeadersItemsControl.ItemsSource = _rowHeaders;
                rowHeadersItemsControl.SetBinding(VisibilityProperty, new Binding("Count") { Source = _rowHeaders, Converter = CountToVisibilityConverter.CollapsedIfNotAny });
            }

            var columnHeadersItemsControl = GetTemplateChild(ElementColumnHeaders) as ItemsControl;
            if (columnHeadersItemsControl is not null)
            {
                columnHeadersItemsControl.ItemsSource = _columnHeaders;
                columnHeadersItemsControl.SetBinding(VisibilityProperty, new Binding("Count") { Source = _columnHeaders, Converter = CountToVisibilityConverter.CollapsedIfNotAny });
            }

            if (DatesItemsControl is not null)
            {
                DatesItemsControl.ItemsSource = _displayDates;
                DatesItemsControl.MouseMove += DatesItemsControl_MouseMove;
                DatesItemsControl.MouseLeave += DatesItemsControl_MouseLeave;
                DatesItemsControl.MouseDown += DatesItemsControl_MouseDown;
            }

            if (PreviousButton != null)
            {
                PreviousButton.Click -= PreviousButton_Click;
                PreviousButton.Click += PreviousButton_Click;
            }

            if (NextButton != null)
            {
                NextButton.Click -= NextButton_Click;
                NextButton.Click += NextButton_Click;
            }

            if (TodayButton != null)
            {
                TodayButton.Click -= TodayButton_Click;
                TodayButton.Click += TodayButton_Click;
            }

            CurrentDate = DisplayDate;
            Rebuild();
        }

        public virtual IEnumerable<CalendarItem> GetCalendarItems() => DatesItemsControl?.Items.OfType<CalendarItem>() ?? [];

        public virtual CalendarItem? GetCalendarItemFromDate(DateTime date) => GetCalendarItems().FirstOrDefault(x => x.Period.Contains(date));

        public virtual CalendarItem? GetCalendarItemFromMousePosition()
            => GetCalendarItems().FirstOrDefault(x =>
                {
                    var position = Mouse.GetPosition(x);
                    return position.X >= 0 && position.X <= x.ActualWidth && position.Y >= 0 && position.Y <= x.ActualHeight;
                });

        public virtual CalendarItem? GetCalendarItemFromPosition(Point position)
            => GetCalendarItems().FirstOrDefault(x =>
            {
                var relativeLocation = x.TranslatePoint(new Point(0, 0), this);
                var bounds = new Rect(relativeLocation, x.RenderSize);
                return bounds.Contains(position);
            });

        public virtual DateTime? GetAccurateDateFromPosition(Point position)
        {
            var item = GetCalendarItemFromPosition(position);

            if (item is null) return null;

            var relativeLocation = item.TranslatePoint(new Point(0, 0), this);
            var positionInItem = new Point(position.X - relativeLocation.X, position.Y - relativeLocation.Y);
            return item.GetAccurateDate(positionInItem);
        }

        protected abstract bool MustBeRebuild(DateTime oldDate, DateTime newDate);

        protected internal virtual bool IsInactive(DateTime date) => false;

        protected virtual void BuildCore(CancellationToken cancellationToken)
        {
            if (PreviousButton != null)
                PreviousButton.IsEnabled = CanGoToPreviousDate();
            if (NextButton != null)
                NextButton.IsEnabled = CanGoToNextDate();

            var dates = GetDisplayDates().ToList();

            cancellationToken.ThrowIfCancellationRequested();

            SetValue(RowsCountPropertyKey, dates.MaxOrDefault(x => x.row) + 1);
            SetValue(ColumnsCountPropertyKey, dates.MaxOrDefault(x => x.column) + 1);
            DatesItemsControl?.SetValue(RowsCountPropertyKey, dates.MaxOrDefault(x => x.row) + 1);
            DatesItemsControl?.SetValue(ColumnsCountPropertyKey, dates.MaxOrDefault(x => x.column) + 1);

            cancellationToken.ThrowIfCancellationRequested();

            _columnHeaders.Set(GetColumnHeaders());
            _rowHeaders.Set(GetRowHeaders());

            if (_displayDates.Count != dates.Count)
            {
                _displayDates.Set(dates.Select(x =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var item = new CalendarItem(this, x.date, IntervalUnit);
                    Grid.SetRow(item, x.row);
                    Grid.SetColumn(item, x.column);
                    item.AddHandler(MouseDownEvent, new MouseButtonEventHandler(Cell_MouseDown), true);
                    item.AddHandler(MouseUpEvent, new MouseButtonEventHandler(Cell_MouseUp), true);
                    item.AddHandler(MouseEnterEvent, new MouseEventHandler(Cell_MouseEnter), true);

                    return item;
                }));
            }
            else
            {
                _displayDates.ForEach(x =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var date = dates.FirstOrDefault(y => y.row == Grid.GetRow(x) && y.column == Grid.GetColumn(x));

                    if (date != default)
                        x.SetDate(date.date);
                });
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (GetCalendarItemFromDate(CurrentDate) is CalendarItem calendarItem)
                FocusItem(calendarItem);
        }

        protected void Rebuild()
        {
            if (IsInitialized)
            {
                _build.Cancel();
                _build.Run();
            }
        }

        private void Build(CancellationToken cancellationToken)
        {
            try
            {
                BuildCore(cancellationToken);

                SetValue(DisplayDateStartPropertyKey, _displayDates.MinOrDefault(x => x.Date));
                SetValue(DisplayDateEndPropertyKey, _displayDates.MaxOrDefault(x => x.Date));
                UpdateAppointments();
                RefreshAccurateDateControl();
                RefreshAccurateDatePreviewControl();
            }
            catch (OperationCanceledException)
            {
                // Nothing
            }
        }

        protected virtual IEnumerable<object> GetColumnHeaders() => [];

        protected virtual IEnumerable<object> GetRowHeaders() => [];

        protected abstract IEnumerable<(DateTime date, int row, int column)> GetDisplayDates();

        public abstract TimeUnit IntervalUnit { get; }

        #endregion

        #region Selection

        protected void SelectSingleDate(DateTime newDate)
        {
            if (SelectedDatesInternal.Count > 0)
                SelectedDatesInternal[0] = newDate;
            else
                SelectedDatesInternal.Add(newDate);
        }

        protected virtual void Toggle(DateTime date)
        {
            if (IsValidDateSelection(date))
            {
                switch (DatesSelectionMode)
                {
                    case CalendarSelectionMode.SingleDate:
                        {
                            SelectedDateInternal = !SelectedDateInternal.HasValue || !SelectedDateInternal.Value.Equals(date) ? date : null;
                            break;
                        }

                    case CalendarSelectionMode.MultipleRange:
                        {
                            if (!SelectedDatesInternal.Remove(date))
                                SelectedDatesInternal.Add(date);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
        }

        protected virtual void ProcessSelection(DateTime newDate, bool shift = false)
        {
            if (DatesSelectionMode == CalendarSelectionMode.None || !newDate.IsBetween(MinimumDateInternal, MaximumDateInternal))
                return;

            switch (DatesSelectionMode)
            {
                case CalendarSelectionMode.SingleDate:
                    _hoverStart = _hoverEnd = null;
                    SelectSingleDate(newDate);
                    break;

                default:

                    SelectedDatesInternal.ClearInternal();

                    if (shift)
                    {
                        if (!_hoverStart.HasValue)
                            _hoverStart = CurrentDate;

                        _hoverEnd = newDate;

                        SelectedDatesInternal.AddRange(_hoverStart.Value, newDate);
                    }
                    else
                    {
                        SelectedDateInternal = newDate;
                        _hoverStart = _hoverEnd = newDate;
                    }
                    break;
            }

            SetCurrentDate(newDate);
        }

        protected virtual void ProcessSelection(int offset, TimeUnit unit, bool shift = false)
        {
            var date = GetCurrentDateIncrement(offset, unit);

            if (date.HasValue)
                ProcessSelection(date.Value, shift);
        }

        public void OnSelectedDatesCollectionChanged(DatesSelectionChangedEventArgs datesSelectionChangedEventArgs)
        {
            if (IsSelectionChanged(datesSelectionChangedEventArgs))
            {
                if ((AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection)) &&
                    UIElementAutomationPeer.FromElement(this) is Automation.CalendarAutomationPeer peer)
                    peer.RaiseSelectionEvents(datesSelectionChangedEventArgs);

                CoerceValue(MinimumDateProperty);
                CoerceValue(MaximumDateProperty);
                CoerceValue(DisplayDateProperty);

                SetCurrentValue(SelectedDateProperty, SelectedDateInternal);
                SetCurrentValue(SelectedDatesProperty, SelectedDatesInternal);
                OnSelectedDatesChanged(datesSelectionChangedEventArgs);

                GetCalendarItems().ForEach(x => x.SetValue(CalendarItem.IsSelectedPropertyKey, SelectedDatesInternal.Contains(x.Date)));
            }
        }

        private void OnSelectedDatesChanged(SelectionChangedEventArgs e) => RaiseEvent(e);

        private static bool IsSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != e.RemovedItems.Count)
                return true;

            foreach (DateTime addedDate in e.AddedItems)
            {
                if (!e.RemovedItems.Contains(addedDate))
                    return true;
            }

            return false;
        }

        internal bool IsValidDateSelection(object? value) => value == null || !BlackoutDates.Contains((DateTime)value);

        #endregion

        #region Mouse

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Calendar gets focus move it to the DisplayDate
            var c = (CalendarBase)sender;
            if (!e.Handled && e.OriginalSource == c)
            {
                if (c.GetCalendarItemFromDate(c.CurrentDate) is CalendarItem calendarItem)
                    FocusItem(calendarItem);
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            _isItemPressed = false;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            if (!IsMouseCaptured)
            {
                _isItemPressed = false;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!CanMoveWithScroll) return;
            if (e.Delta < 0)
                GoToNextDate();
            else
                GoToPreviousDate();
        }

        private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not CalendarItem calendarItem
                || e.OriginalSource is DependencyObject d && (d is Button || d.FindVisualParent<Button>() is not null
                                                           || d is ScrollBar || d.FindVisualParent<ScrollBar>() is not null
                                                           || d is CheckBox || d.FindVisualParent<CheckBox>() is not null
                                                           || d is ToggleButton || d.FindVisualParent<ToggleButton>() is not null
                                                           || d is DropDownButton || d.FindVisualParent<DropDownButton>() is not null
                                                           || d is SplitButton || d.FindVisualParent<SplitButton>() is not null
                                                           || d is RadioButton || d.FindVisualParent<RadioButton>() is not null
                                                           || d is ComboBox || d.FindVisualParent<ComboBox>() is not null
                                                           || d is TextBox || d.FindVisualParent<TextBox>() is not null
                                                           || d is NumericUpDown || d.FindVisualParent<NumericUpDown>() is not null)
                || e.RightButton == MouseButtonState.Pressed && calendarItem.IsSelected
                )
                return;

            if (calendarItem.IsBlackedOut)
                _hoverStart = null;
            else
            {
                _isItemPressed = true;
                Mouse.Capture(this, CaptureMode.SubTree);

                calendarItem.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                (var ctrl, var shift) = KeyboardHelper.GetMetaKeyState();

                var selectedDate = calendarItem.Date;

                switch (DatesSelectionMode)
                {
                    case CalendarSelectionMode.None:
                        break;

                    case CalendarSelectionMode.SingleDate:
                        {
                            if (!ctrl)
                                SelectedDateInternal = selectedDate;
                            else
                                Toggle(selectedDate);

                            break;
                        }

                    case CalendarSelectionMode.SingleRange:
                    case CalendarSelectionMode.MultipleRange:
                        {
                            if (!ctrl || DatesSelectionMode == CalendarSelectionMode.SingleRange)
                                SelectedDatesInternal.ClearInternal();

                            if (shift)
                            {
                                if (!_hoverStart.HasValue)
                                    _hoverStart = CurrentDate;

                                _hoverEnd = selectedDate;

                                SelectedDatesInternal.AddRange(_hoverStart.Value, selectedDate);
                            }
                            else
                            {
                                if (ctrl && DatesSelectionMode == CalendarSelectionMode.MultipleRange)
                                    Toggle(selectedDate);
                                else
                                    SelectedDateInternal = selectedDate;
                                _hoverStart = _hoverEnd = selectedDate;
                            }
                            break;
                        }
                }
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not CalendarItem calendarItem || calendarItem.IsBlackedOut) return;

            if (e.LeftButton == MouseButtonState.Pressed && _isItemPressed)
            {
                calendarItem.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                var selectedDate = calendarItem.Date;

                switch (DatesSelectionMode)
                {
                    case CalendarSelectionMode.None:
                        return;

                    case CalendarSelectionMode.SingleDate:
                        {
                            _hoverStart = _hoverEnd = null;
                            SelectSingleDate(selectedDate);

                            return;
                        }

                    default:
                        if (_hoverStart.HasValue)
                        {
                            if (DatesSelectionMode == CalendarSelectionMode.SingleRange)
                                SelectedDatesInternal.ClearInternal();
                            else if (_hoverEnd.HasValue)
                            {
                                SelectedDatesInternal.ClearRangeInternal(_hoverStart.Value, _hoverEnd.Value);
                            }
                            SelectedDatesInternal.AddRange(_hoverStart.Value, selectedDate);
                        }

                        _hoverEnd = selectedDate;
                        break;
                }
            }
        }

        private void Cell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not CalendarItem calendarItem
                || e.OriginalSource is DependencyObject d && (d is Button || d.FindVisualParent<Button>() is not null
                                                           || d is ScrollBar || d.FindVisualParent<ScrollBar>() is not null
                                                           || d is CheckBox || d.FindVisualParent<CheckBox>() is not null
                                                           || d is ToggleButton || d.FindVisualParent<ToggleButton>() is not null
                                                           || d is DropDownButton || d.FindVisualParent<DropDownButton>() is not null
                                                           || d is SplitButton || d.FindVisualParent<SplitButton>() is not null
                                                           || d is RadioButton || d.FindVisualParent<RadioButton>() is not null
                                                           || d is ComboBox || d.FindVisualParent<ComboBox>() is not null
                                                           || d is TextBox || d.FindVisualParent<TextBox>() is not null
                                                           || d is NumericUpDown || d.FindVisualParent<NumericUpDown>() is not null)
                || e.RightButton == MouseButtonState.Pressed && calendarItem.IsSelected
                )
                return;


            _isItemPressed = false;

            var selectedDate = calendarItem.Date;

            SetCurrentDate(selectedDate);
        }

        private void DatesItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ShowAccurateDate) return;

            var child = GetCalendarItemFromMousePosition();

            if (child is null) return;

            var itemMousePosition = e.GetPosition(child);

            var preciseDate = child.GetAccurateDate(itemMousePosition);
            SetValue(AccurateDateProperty, preciseDate);

            SetValue(AccurateDatePreviewPropertyKey, null);
        }

        private void DatesItemsControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowAccurateDate) return;

            var child = GetCalendarItemFromMousePosition();

            if (child is null) return;

            var itemMousePosition = e.GetPosition(child);
            var preciseDate = child.GetAccurateDate(itemMousePosition);

            if (_isItemPressed)
            {
                SetValue(AccurateDateProperty, preciseDate);
                SetValue(AccurateDatePreviewPropertyKey, null);
            }
            else
            {
                SetValue(AccurateDatePreviewPropertyKey, preciseDate);
            }
        }

        private void DatesItemsControl_MouseLeave(object sender, MouseEventArgs e) => SetValue(AccurateDatePreviewPropertyKey, null);

        #endregion

        #region Keyboard

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
                e.Handled = ProcessKey(e);

            base.OnPreviewKeyDown(e);
        }

        protected virtual bool ProcessKey(KeyEventArgs e)
        {
            (var _, var shift) = KeyboardHelper.GetMetaKeyState();

            return e.Key switch
            {
                Key.Up => ProcessUpKey(shift),
                Key.Down => ProcessDownKey(shift),
                Key.Left => ProcessLeftKey(shift),
                Key.Right => ProcessRightKey(shift),
                Key.Enter or Key.Space => ProcessEnterKey(),
                Key.PageDown => ProcessPageDownKey(shift),
                Key.PageUp => ProcessPageUpKey(shift),
                Key.Home => ProcessHomeKey(shift),
                Key.End => ProcessEndKey(shift),
                _ => false,
            };
        }

        protected virtual bool ProcessDownKey(bool shift) => false;

        protected virtual bool ProcessEndKey(bool shift) => false;

        protected virtual bool ProcessEnterKey()
        {
            Toggle(CurrentDate);

            return true;
        }

        protected virtual bool ProcessHomeKey(bool shift) => false;

        protected virtual bool ProcessLeftKey(bool shift) => false;

        protected virtual bool ProcessPageDownKey(bool shift) => false;

        protected virtual bool ProcessPageUpKey(bool shift) => false;

        protected virtual bool ProcessRightKey(bool shift) => false;

        protected virtual bool ProcessUpKey(bool shift) => false;

        #endregion

        #region Buttons

        private void PreviousButton_Click(object sender, RoutedEventArgs e) => GoToPreviousDate();

        private void NextButton_Click(object sender, RoutedEventArgs e) => GoToNextDate();

        private void TodayButton_Click(object sender, RoutedEventArgs e) => GoToToday();

        #endregion

        #region Move to date

        public virtual void MoveDisplayTo(DateTime date)
        {
            SetCurrentValue(DisplayDateProperty, date);

            var item = GetCalendarItemFromDate(CurrentDate);

            if (item is null)
                CurrentDate = date;
            else
                FocusItem(item);
        }

        private void SetCurrentDate(DateTime selectedDate)
        {
            CurrentDate = selectedDate;

            var item = GetCalendarItemFromDate(CurrentDate);

            if (item is null)
                MoveDisplayTo(selectedDate);
            else
                FocusItem(item);
        }

        public virtual void GoToToday() => MoveDisplayTo(DateTime.Today);

        public virtual void GoToPreviousDate() => MoveDisplayTo(GetPreviousDate(DisplayDateInternal));

        public virtual void GoToNextDate() => MoveDisplayTo(GetNextDate(DisplayDateInternal));

        public virtual DateTime GetNextDate(DateTime date) => date.AddDays(1);

        public virtual DateTime GetPreviousDate(DateTime date) => date.AddDays(-1);

        protected virtual bool CanGoToPreviousDate()
            => DisplayDateInternal != DateTime.MinValue && GetPreviousDate(DisplayDateInternal).IsBetween(MinimumDateInternal, MaximumDateInternal);

        protected virtual bool CanGoToNextDate()
            => DisplayDateInternal != DateTime.MaxValue && GetNextDate(DisplayDateInternal).IsBetween(MinimumDateInternal, MaximumDateInternal);

        private static void FocusItem(CalendarItem item)
        {
            if (!item.IsFocused)
                item.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private DateTime? GetCurrentDateIncrement(int offset, TimeUnit timeUnit)
        {
            DateTime date;

            switch (timeUnit)
            {
                case TimeUnit.Millisecond:
                    date = CurrentDate.AddMilliseconds(offset);
                    break;
                case TimeUnit.Second:
                    date = CurrentDate.AddSeconds(offset);
                    break;
                case TimeUnit.Minute:
                    date = CurrentDate.AddMinutes(offset);
                    break;
                case TimeUnit.Hour:
                    date = CurrentDate.AddHours(offset);
                    break;
                case TimeUnit.Day:
                    date = CurrentDate.AddDays(offset);
                    break;
                case TimeUnit.Week:
                    date = CurrentDate.AddDays(offset * 7);
                    break;
                case TimeUnit.Month:
                    date = CurrentDate.AddMonths(offset);
                    break;
                case TimeUnit.Year:
                    date = CurrentDate.AddYears(offset);
                    break;
                default:
                    return null;
            }

            var offsetDirection = offset < 0 ? -1 : 1;
            return BlackoutDates.GetNonBlackoutDate(date, offsetDirection);
        }

        public IEnumerable<DateTime> GetValidDates(DateTime start, DateTime end)
            => DateTimeHelper.Range(DateTimeHelper.Min(start, end), DateTimeHelper.Max(start, end), 1, IntervalUnit).Where(x => IsValidDateSelection(x));

        #endregion

        #region Appointments

        private async Task SynchronizeAppointmentAsync(IAppointment appointment, CancellationToken cancellationToken)
        {
            List<CalendarAppointment> appointmentsToDelete;
            List<CalendarAppointment> appointmentsToAdd = [];

            cancellationToken.ThrowIfCancellationRequested();

            appointmentsToDelete = _appointments.Where(x => Dispatcher.Invoke(() => x.DataContext) == appointment).ToList();

            if (AppointmentMustBeDisplayed(appointment))
                appointmentsToAdd = CreateAppointmentsWrapper(appointment).ToList();

            foreach (var item in appointmentsToDelete)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _appointments.Remove(item);
                await Task.Delay(1.Milliseconds(), cancellationToken).ConfigureAwait(false);
            }

            foreach (var item in appointmentsToAdd)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _appointments.Add(item);
                await Task.Delay(1.Milliseconds(), cancellationToken).ConfigureAwait(false);
            }
        }

        private bool AppointmentMustBeDisplayed(IAppointment appointment)
        {
            var period = new ImmutablePeriod(Dispatcher.Invoke(() => DisplayDateStart ?? DateTime.MinValue), Dispatcher.Invoke(() => DisplayDateEnd ?? DateTime.MaxValue));
            return period.Start < appointment.StartDate && period.End > appointment.EndDate
                   || appointment.StartDate < period.Start && appointment.EndDate > period.End
                   || period.Contains(appointment.StartDate)
                   || period.Contains(appointment.EndDate);
        }

        protected virtual IEnumerable<(int row, int column, int rowSpan, int columnSpan)> GetDisplayedAppointments(IAppointment appointment, IEnumerable<(ImmutablePeriod period, int row, int column)> allDisplayedDates)
            => allDisplayedDates.GroupBy(x => x.column).Select(x => (x.Min(y => y.row), x.Key, x.Count(), 1));

        private IEnumerable<CalendarAppointment> CreateAppointmentsWrapper(IAppointment appointment)
        {
            var availablePeriods = Dispatcher.Invoke(() => GetCalendarItems().Select(x => (x.Period, Grid.GetRow(x), Grid.GetColumn(x))).ToList());
            if (availablePeriods.Count == 0) yield break;

            var dates = availablePeriods.Where(x => x.Period.Start < appointment.StartDate && x.Period.End > appointment.EndDate
                                                    || appointment.StartDate < x.Period.Start && appointment.EndDate > x.Period.End
                                                    || x.Period.Contains(appointment.StartDate)
                                                    || x.Period.Contains(appointment.EndDate)).ToList();

            if (dates.Count == 0) yield break;

            var displayedAppointments = GetDisplayedAppointments(appointment, dates);
            foreach (var (row, column, rowSpan, columnSpan) in displayedAppointments)
            {
                yield return Dispatcher.Invoke(() =>
                {
                    var element = new CalendarAppointment();
                    element.SetValue(ContentControl.ContentProperty, appointment);
                    element.SetValue(DataContextProperty, appointment);
                    element.SetValue(Grid.RowProperty, row);
                    element.SetValue(Grid.ColumnProperty, column);
                    element.SetValue(Grid.RowSpanProperty, rowSpan);
                    element.SetValue(Grid.ColumnSpanProperty, columnSpan);

                    return element;
                });
            }
        }

        protected void UpdateAppointments()
        {
            _updateAppointments.Cancel();
            _updateAppointments.Run();
        }

        private async Task UpdateAppointmentsAsync(CancellationToken cancellationToken)
        {
            var appointments = Dispatcher.Invoke(() => Appointments?.OfType<IAppointment>().ToList());

            try
            {
                _appointments.Clear();

                if (appointments is null) return;

                foreach (var item in appointments)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await SynchronizeAppointmentAsync(item, cancellationToken).ConfigureAwait(false);
                }

                if (Dispatcher.Invoke(() => AppointmentsDisplayMode == AppointmentsDisplayMode.Cell))
                {
                    var calendarItems = GetCalendarItems().ToList();
                    foreach (var item in calendarItems)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        Dispatcher.Invoke(() => item.UpdateAppointments(cancellationToken), DispatcherPriority.Input, cancellationToken);
                        await Task.Delay(1.Milliseconds(), cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Nothing
            }
        }

        #endregion

        protected override bool IsItemItsOwnContainerOverride(object item) => item is CalendarAppointment;

        protected override DependencyObject GetContainerForItemOverride() => new CalendarAppointment();

        protected override AutomationPeer OnCreateAutomationPeer() => new Automation.CalendarAutomationPeer(this);

        public override string ToString() => SelectedDateInternal != null
            ? SelectedDateInternal.Value.ToString(CultureInfo.CurrentCulture.DateTimeFormat)
            : string.Empty;

        #endregion
    }
}
