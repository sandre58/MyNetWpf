// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using MyNet.UI.Commands;
using MyNet.UI.Resources;
using MyNet.UI.Toasting;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Helpers;
using MyNet.Utilities.Logging;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Commands
{
    public static class AppCommands
    {
        public static ICommand CopyInClipboardCommand { get; } = CommandsManager.Create<object>(x =>
        {
            try
            {
                Clipboard.SetDataObject(x?.ToString());
                ToasterManager.ShowInformation(MessageResources.CopyInClipBoardSuccess);
            }
            catch (Exception)
            {
                ToasterManager.ShowError(MessageResources.CopyInClipBoardError);
            }
        });

        public static ICommand OpenFileCommand { get; } = CommandsManager.CreateNotNull<string>(x =>
        {
            try
            {
                ProcessHelper.Start(x);
            }
            catch (Exception e)
            {
                ToasterManager.ShowError(MessageResources.FileXOpenError.FormatWith(x));
                LogManager.Error(e);
            }
        });


        public static ICommand ComboBoxDownCommand => CommandsManager.Create<object>(ComboBoxDown, CanComboBoxDown);

        private static void ComboBoxDown(object? obj)
        {
            if (obj is ComboBox cb && cb.SelectedIndex > 0)
            {
                cb.SelectedIndex -= 1;
            }
        }

        private static bool CanComboBoxDown(object? obj) => obj is ComboBox cb && cb.SelectedIndex > 0;

        public static ICommand ComboBoxUpCommand => CommandsManager.Create<object>(ComboBoxUp, CanComboBoxUp);

        private static void ComboBoxUp(object? obj)
        {
            if (obj is ComboBox cb && cb.SelectedIndex < cb.Items.Count - 1)
            {
                cb.SelectedIndex += 1;
            }
        }

        private static bool CanComboBoxUp(object? obj) => obj is ComboBox cb && cb.SelectedIndex < cb.Items.Count - 1;

        public static ICommand NextHourCommand => CreateNextCommand(x => x.AddHours(1));

        public static ICommand PreviousHourCommand => CreatePreviousCommand(x => x.AddHours(-1));

        public static ICommand NextDayCommand => CreateNextCommand(x => x.NextDay().BeginningOfDay());

        public static ICommand PreviousDayCommand => CreatePreviousCommand(x => x.PreviousDay().BeginningOfDay());

        public static ICommand NextMonthCommand => CreateNextCommand(x => x.NextMonth().BeginningOfMonth());

        public static ICommand PreviousMonthCommand => CreatePreviousCommand(x => x.PreviousMonth().BeginningOfMonth());

        public static ICommand NextYearCommand => CreateNextCommand(x => x.NextYear().BeginningOfYear());

        public static ICommand PreviousYearCommand => CreatePreviousCommand(x => x.PreviousYear().BeginningOfYear());

        private static ICommand CreateNextCommand(Func<DateTime, DateTime> newDate) => CommandsManager.Create<object>(x => AddToSelectedDate(x, newDate), x => CanNextDate(x, newDate));

        private static ICommand CreatePreviousCommand(Func<DateTime, DateTime> newDate) => CommandsManager.Create<object>(x => AddToSelectedDate(x, newDate), x => CanPreviousDate(x, newDate));

        private static void AddToSelectedDate(object? obj, Func<DateTime, DateTime> newDate)
        {
            if (obj is DatePicker dt && dt.SelectedDate.HasValue)
                dt.SelectedDate = newDate(dt.SelectedDate.Value);
            if (obj is TimePicker tp && tp.SelectedTime.HasValue)
                tp.SelectedTime = newDate(tp.SelectedTime.Value);
            if (obj is MonthPicker mp && mp.SelectedMonth.HasValue)
                mp.SelectedMonth = newDate(mp.SelectedMonth.Value);
        }

        private static bool CanNextDate(object? obj, Func<DateTime, DateTime> newDate) =>
            obj is DatePicker dt && dt.SelectedDate.HasValue && (!dt.DisplayDateEnd.HasValue || newDate(dt.SelectedDate.Value) < dt.DisplayDateEnd)
            || obj is TimePicker tp && tp.SelectedTime.HasValue && newDate(tp.SelectedTime.Value).TimeOfDay < tp.SelectedTime.Value.EndOfDay().TimeOfDay
            || obj is MonthPicker mp && mp.SelectedMonth.HasValue;

        private static bool CanPreviousDate(object? obj, Func<DateTime, DateTime> newDate) =>
            obj is DatePicker dt && dt.SelectedDate.HasValue && (!dt.DisplayDateStart.HasValue || newDate(dt.SelectedDate.Value) > dt.DisplayDateStart)
            || obj is TimePicker tp && tp.SelectedTime.HasValue && newDate(tp.SelectedTime.Value).TimeOfDay >= tp.SelectedTime.Value.BeginningOfDay().TimeOfDay
            || obj is MonthPicker mp && mp.SelectedMonth.HasValue;
    }
}
