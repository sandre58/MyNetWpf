// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class TimePickerHintProxy : IHintProxy
    {
        private readonly TimePicker _timePicker;
        private bool _disposedValue;

        public TimePickerHintProxy(TimePicker timePicker)
        {
            _timePicker = timePicker ?? throw new ArgumentNullException(nameof(timePicker));

            _timePicker.Loaded += TimePickerLoaded;
            _timePicker.IsVisibleChanged += TimePickerIsVisibleChanged;
            _timePicker.IsKeyboardFocusWithinChanged += TimePickerIsKeyboardFocusWithinChanged;
            _timePicker.SelectedTimeChanged += SelectedTimeChanged;
        }

        public bool IsLoaded => _timePicker.IsLoaded;

        public bool IsVisible => _timePicker.IsVisible;

        public virtual bool IsEmpty() => !_timePicker.SelectedTime.HasValue;

        public bool IsFocused() => _timePicker.IsKeyboardFocusWithin || _timePicker.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void TimePickerIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void TimePickerLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void SelectedTimeChanged(object? sender, RoutedPropertyChangedEventArgs<DateTime?> e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void TimePickerIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timePicker.Loaded -= TimePickerLoaded;
                    _timePicker.IsVisibleChanged -= TimePickerIsVisibleChanged;
                    _timePicker.IsKeyboardFocusWithinChanged -= TimePickerIsKeyboardFocusWithinChanged;
                    _timePicker.SelectedTimeChanged -= SelectedTimeChanged;
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
