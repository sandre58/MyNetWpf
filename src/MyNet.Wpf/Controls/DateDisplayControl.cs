// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Extensions;
using MyNet.Humanizer;
using static MaterialDesignThemes.Wpf.CalendarFormatInfo;

namespace MyNet.Wpf.Controls
{
    internal class DateDisplayControl : Control
    {
        static DateDisplayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateDisplayControl), new FrameworkPropertyMetadata(typeof(DateDisplayControl)));
            LanguageProperty.OverrideMetadata(typeof(DateDisplayControl), new FrameworkPropertyMetadata(OnLanguageChanged));
        }

        public DateDisplayControl() => SetCurrentValue(DisplayDateProperty, DateTime.Today);

        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
            nameof(DisplayDate), typeof(DateTime), typeof(DateDisplayControl), new PropertyMetadata(default(DateTime), DisplayDatePropertyChangedCallback, DisplayDateCoerceValue));

        private static object DisplayDateCoerceValue(DependencyObject d, object baseValue)
        {
            if (d is FrameworkElement element &&
                element.Language.GetSpecificCulture() is CultureInfo culture &&
                baseValue is DateTime displayDate)
            {
                if (displayDate < culture.Calendar.MinSupportedDateTime)
                {
                    return culture.Calendar.MinSupportedDateTime;
                }
                if (displayDate > culture.Calendar.MaxSupportedDateTime)
                {
                    return culture.Calendar.MaxSupportedDateTime;
                }
            }
            return baseValue;
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DateDisplayControl)d).UpdateComponents();

        private static void DisplayDatePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => ((DateDisplayControl)dependencyObject).UpdateComponents();

        public DateTime DisplayDate
        {
            get => (DateTime)GetValue(DisplayDateProperty);
            set => SetValue(DisplayDateProperty, value);
        }

        private static readonly DependencyPropertyKey ComponentOneContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ComponentOneContent), typeof(string), typeof(DateDisplayControl),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentOneContentProperty =
            ComponentOneContentPropertyKey.DependencyProperty;

        public string? ComponentOneContent
        {
            get => (string)GetValue(ComponentOneContentProperty);
            private set => SetValue(ComponentOneContentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ComponentTwoContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ComponentTwoContent), typeof(string), typeof(DateDisplayControl),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentTwoContentProperty =
            ComponentTwoContentPropertyKey.DependencyProperty;

        public string? ComponentTwoContent
        {
            get => (string?)GetValue(ComponentTwoContentProperty);
            private set => SetValue(ComponentTwoContentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ComponentThreeContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ComponentThreeContent), typeof(string), typeof(DateDisplayControl),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ComponentThreeContentProperty =
            ComponentThreeContentPropertyKey.DependencyProperty;

        public string? ComponentThreeContent
        {
            get => (string?)GetValue(ComponentThreeContentProperty);
            private set => SetValue(ComponentThreeContentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey IsDayInFirstComponentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsDayInFirstComponent), typeof(bool), typeof(DateDisplayControl),
                new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsDayInFirstComponentProperty =
            IsDayInFirstComponentPropertyKey.DependencyProperty;

        public bool IsDayInFirstComponent
        {
            get => (bool)GetValue(IsDayInFirstComponentProperty);
            private set => SetValue(IsDayInFirstComponentPropertyKey, value);
        }

        private void UpdateComponents()
        {
            var culture = Language.GetSpecificCulture();
            var dateTimeFormatInfo = culture.DateTimeFormat;
            var minDateTime = dateTimeFormatInfo.Calendar.MinSupportedDateTime;
            var maxDateTime = dateTimeFormatInfo.Calendar.MaxSupportedDateTime;

            if (DisplayDate < minDateTime)
            {
                SetDisplayDateOfCalendar(minDateTime);

                // return to avoid second formatting of the same value
                return;
            }

            if (DisplayDate > maxDateTime)
            {
                SetDisplayDateOfCalendar(maxDateTime);

                // return to avoid second formatting of the same value
                return;
            }

            var monthDayPattern = dateTimeFormatInfo.MonthDayPattern.Replace("MMMM", "MMM");
            var dayOfWeekStyle = DayOfWeekStyle.Parse(dateTimeFormatInfo.LongDatePattern);
            var displayDate = DisplayDate;
            ComponentOneContent = FormatDate(dayOfWeekStyle.IsFirst ? monthDayPattern : dayOfWeekStyle.Pattern, displayDate, culture);
            ComponentTwoContent = FormatDate(dayOfWeekStyle.IsFirst ? dayOfWeekStyle.Pattern + dayOfWeekStyle.Separator : monthDayPattern + dayOfWeekStyle.Separator, displayDate, culture);
            ComponentThreeContent = FormatDate("yyyy", displayDate, culture);
        }

        private static string FormatDate(string format, DateTime displayDate, CultureInfo culture) => string.IsNullOrEmpty(format) ? string.Empty : displayDate.ToString(format, culture).ToTitle();

        private void SetDisplayDateOfCalendar(DateTime displayDate)
        {
            var calendarControl = this.GetVisualAncestry().OfType<System.Windows.Controls.Calendar>().FirstOrDefault();

            if (calendarControl != null)
            {
                calendarControl.DisplayDate = displayDate;
            }
        }
    }
}
