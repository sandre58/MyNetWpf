// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class DatePickerHintProxy : IHintProxy
    {
        private readonly DatePicker _datePicker;
        private bool _disposedValue;

        public DatePickerHintProxy(DatePicker datePicker)
        {
            _datePicker = datePicker ?? throw new ArgumentNullException(nameof(datePicker));

            _datePicker.Loaded += DatePickerLoaded;
            _datePicker.IsVisibleChanged += DatePickerIsVisibleChanged;
            _datePicker.IsKeyboardFocusWithinChanged += DatePickerIsKeyboardFocusWithinChanged;
            _datePicker.SelectedDateChanged += SelectedDateChanged;
        }

        public bool IsLoaded => _datePicker.IsLoaded;

        public bool IsVisible => _datePicker.IsVisible;

        public virtual bool IsEmpty() => string.IsNullOrEmpty(_datePicker.Text);

        public bool IsFocused() => _datePicker.IsKeyboardFocusWithin || _datePicker.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void DatePickerIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void DatePickerLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void SelectedDateChanged(object? sender, SelectionChangedEventArgs e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void DatePickerIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _datePicker.Loaded -= DatePickerLoaded;
                    _datePicker.IsVisibleChanged -= DatePickerIsVisibleChanged;
                    _datePicker.IsKeyboardFocusWithinChanged -= DatePickerIsKeyboardFocusWithinChanged;
                    _datePicker.SelectedDateChanged -= SelectedDateChanged;
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
