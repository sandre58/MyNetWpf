// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MyNet.Utilities;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.Controls;

[ToolboxItem(true)]
[TemplatePart(Name = ButtonPartName, Type = typeof(Button))]
[TemplatePart(Name = PopupPartName, Type = typeof(Popup))]
public class MonthPicker : Control
{
    public const string ButtonPartName = "PART_Button";
    public const string PopupPartName = "PART_Popup";

    private readonly ContentControl _calendarHostContentControl;
    private readonly CalendarMonthsByYear _calendar;
    private Popup? _popup;
    private Button? _dropDownButton;
    private bool _disablePopupReopen;

    static MonthPicker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthPicker), new FrameworkPropertyMetadata(typeof(MonthPicker)));

    public MonthPicker()
    {
        _calendar = new CalendarMonthsByYear()
        {
            DatesSelectionMode = CalendarSelectionMode.SingleDate
        };
        _calendarHostContentControl = new ContentControl
        {
            Content = _calendar
        };
        InitializeCalendar();
    }

    public static readonly DependencyProperty SelectedMonthProperty = DependencyProperty.Register(
        nameof(SelectedMonth), typeof(DateTime?), typeof(MonthPicker), new FrameworkPropertyMetadata(default(DateTime?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedMonthPropertyChangedCallback, CoerceSelectedMonth));

    private static void SelectedMonthPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var monthPicker = (MonthPicker)dependencyObject;

        if (dependencyPropertyChangedEventArgs.NewValue is DateTime date)
            monthPicker._calendar.DisplayDate = date;
        OnSelectedMonthChanged(monthPicker, dependencyPropertyChangedEventArgs);
    }

    public DateTime? SelectedMonth
    {
        get => (DateTime?)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public static readonly RoutedEvent SelectedMonthChangedEvent =
        EventManager.RegisterRoutedEvent(
            nameof(SelectedMonth),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<DateTime?>),
            typeof(MonthPicker));

    public event RoutedPropertyChangedEventHandler<DateTime?> SelectedMonthChanged
    {
        add => AddHandler(SelectedMonthChangedEvent, value);
        remove => RemoveHandler(SelectedMonthChangedEvent, value);
    }

    private static void OnSelectedMonthChanged(
        DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var instance = (MonthPicker)d;
        var args = new RoutedPropertyChangedEventArgs<DateTime?>(
                (DateTime?)e.OldValue,
                (DateTime?)e.NewValue)
        { RoutedEvent = SelectedMonthChangedEvent };
        instance.RaiseEvent(args);
    }

    private static object? CoerceSelectedMonth(DependencyObject d, object value)
    {
        var date = (DateTime?)value;

        return date?.BeginningOfMonth();
    }

    public static readonly DependencyProperty SelectedMonthFormatProperty = DependencyProperty.Register(
        nameof(SelectedMonthFormat), typeof(string), typeof(MonthPicker), new PropertyMetadata("MMM yyyy"));

    public string SelectedMonthFormat
    {
        get => (string)GetValue(SelectedMonthFormatProperty);
        set => SetValue(SelectedMonthFormatProperty, value);
    }

    public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
        nameof(IsDropDownOpen), typeof(bool), typeof(MonthPicker),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

    public bool IsDropDownOpen
    {
        get => (bool)GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
    {
        var monthPicker = (MonthPicker)d;
        return monthPicker.IsEnabled && (bool)baseValue;
    }

    /// <summary> 
    /// IsDropDownOpenProperty property changed handler.
    /// </summary> 
    /// <param name="d">DatePicker that changed its IsDropDownOpen.</param> 
    /// <param name="e">DependencyPropertyChangedEventArgs.</param>
    private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var monthPicker = (MonthPicker)d;

        var newValue = (bool)e.NewValue;
        if (monthPicker._popup == null || monthPicker._popup.IsOpen == newValue) return;

        monthPicker._popup.IsOpen = newValue;
        if (newValue)
        {
            monthPicker.Dispatcher?.BeginInvoke(DispatcherPriority.Input, new Action(() => monthPicker._calendar.Focus()));
        }
    }

    public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register(
        nameof(CalendarStyle), typeof(Style), typeof(MonthPicker), new PropertyMetadata(default(Style?)));

    public Style? CalendarStyle
    {
        get => (Style?)GetValue(CalendarStyleProperty);
        set => SetValue(CalendarStyleProperty, value);
    }

    public static readonly DependencyProperty CalendarHostContentControlStyleProperty = DependencyProperty.Register(
        nameof(CalendarHostContentControlStyle), typeof(Style), typeof(MonthPicker), new PropertyMetadata(default(Style?)));

    public Style? CalendarHostContentControlStyle
    {
        get => (Style?)GetValue(CalendarHostContentControlStyleProperty);
        set => SetValue(CalendarHostContentControlStyleProperty, value);
    }

    public override void OnApplyTemplate()
    {
        if (_popup != null)
        {
            _popup.RemoveHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopupOnPreviewMouseLeftButtonDown));
            _popup.Opened -= PopupOnOpened;
            _popup.Closed -= PopupOnClosed;
            _popup.Child = null;
        }
        if (_dropDownButton != null)
        {
            _dropDownButton.Click -= DropDownButtonOnClick;
        }

        _popup = GetTemplateChild(PopupPartName) as Popup;
        if (_popup != null)
        {
            _popup.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(PopupOnPreviewMouseLeftButtonDown));
            _popup.Opened += PopupOnOpened;
            _popup.Closed += PopupOnClosed;
            _popup.Child = _calendarHostContentControl;
            if (IsDropDownOpen)
            {
                _popup.IsOpen = true;
            }
        }

        _dropDownButton = GetTemplateChild(ButtonPartName) as Button;
        if (_dropDownButton != null)
        {
            _dropDownButton.Click += DropDownButtonOnClick;
        }

        base.OnApplyTemplate();
    }

    private void SetSelectedMonth(in DateTime month)
        => SetCurrentValue(SelectedMonthProperty, month.BeginningOfMonth());

    private void PopupOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        if (sender is not Popup popup || popup.StaysOpen) return;

        if (_dropDownButton?.InputHitTest(mouseButtonEventArgs.GetPosition(_dropDownButton)) != null)
        {
            // This popup is being closed by a mouse press on the drop down button 
            // The following mouse release will cause the closed popup to immediately reopen. 
            // Raise a flag to block re-opening the popup
            _disablePopupReopen = true;
        }
    }

    private void PopupOnClosed(object? sender, EventArgs eventArgs)
    {
        if (IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }

        if (_calendar.IsKeyboardFocusWithin)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }

    private void PopupOnOpened(object? sender, EventArgs eventArgs)
    {
        if (!IsDropDownOpen)
            SetCurrentValue(IsDropDownOpenProperty, true);

        _calendar?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
    }

    private void InitializeCalendar()
    {
        _calendar.AddHandler(CalendarBase.SelectedDatesChangedEvent, new EventHandler<SelectionChangedEventArgs>(SelectedDatesChangedHandler));
        _calendar.SetBinding(ForegroundProperty, GetBinding(ForegroundProperty));
        _calendar.SetBinding(StyleProperty, GetBinding(CalendarStyleProperty));
        _calendar.SetBinding(CalendarBase.SelectedDateProperty, GetBinding(SelectedMonthProperty, new NullableDateTimeToCurrentDateConverter()));
        _calendarHostContentControl.SetBinding(StyleProperty, GetBinding(CalendarHostContentControlStyleProperty));
    }

    private void SelectedDatesChangedHandler(object? sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        SetCurrentValue(IsDropDownOpenProperty, false);

        if (SelectedMonth == null && _calendar.SelectedDate.HasValue)
            SetSelectedMonth(_calendar.SelectedDate.Value);
    }

    private void DropDownButtonOnClick(object sender, RoutedEventArgs routedEventArgs) => TogglePopup();

    private void TogglePopup()
    {
        if (!IsLoaded) return;
        if (IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
        else
        {
            if (_disablePopupReopen)
            {
                _disablePopupReopen = false;
            }
            else
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }
        }
    }

    private Binding GetBinding(DependencyProperty property, IValueConverter? converter = null)
    {
        var binding = new Binding(property.Name)
        {
            Source = this,
            Converter = converter
        };
        return binding;
    }
}
