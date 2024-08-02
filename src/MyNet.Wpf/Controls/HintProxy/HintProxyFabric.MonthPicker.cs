// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class MonthPickerHintProxy : IHintProxy
    {
        private readonly MonthPicker _monthPicker;
        private bool _disposedValue;

        public MonthPickerHintProxy(MonthPicker monthPicker)
        {
            _monthPicker = monthPicker ?? throw new ArgumentNullException(nameof(monthPicker));

            _monthPicker.Loaded += MonthPickerLoaded;
            _monthPicker.IsVisibleChanged += MonthPickerIsVisibleChanged;
            _monthPicker.IsKeyboardFocusWithinChanged += MonthPickerIsKeyboardFocusWithinChanged;
            _monthPicker.SelectedMonthChanged += SelectedMonthChanged;
        }

        public bool IsLoaded => _monthPicker.IsLoaded;

        public bool IsVisible => _monthPicker.IsVisible;

        public virtual bool IsEmpty() => !_monthPicker.SelectedMonth.HasValue;

        public bool IsFocused() => _monthPicker.IsKeyboardFocusWithin || _monthPicker.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void MonthPickerIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void MonthPickerLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void SelectedMonthChanged(object? sender, RoutedPropertyChangedEventArgs<DateTime?> e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void MonthPickerIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _monthPicker.Loaded -= MonthPickerLoaded;
                    _monthPicker.IsVisibleChanged -= MonthPickerIsVisibleChanged;
                    _monthPicker.IsKeyboardFocusWithinChanged -= MonthPickerIsKeyboardFocusWithinChanged;
                    _monthPicker.SelectedMonthChanged -= SelectedMonthChanged;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
