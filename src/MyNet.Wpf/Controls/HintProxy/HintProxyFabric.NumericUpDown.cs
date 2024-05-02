// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class NumericUpDownHintProxy : IHintProxy
    {
        private readonly NumericUpDown _numericUpDown;
        private bool _disposedValue;

        public NumericUpDownHintProxy(NumericUpDown numericUpDown)
        {
            _numericUpDown = numericUpDown ?? throw new ArgumentNullException(nameof(numericUpDown));

            _numericUpDown.Loaded += NumericUpDownLoaded;
            _numericUpDown.IsVisibleChanged += NumericUpDownIsVisibleChanged;
            _numericUpDown.IsKeyboardFocusWithinChanged += NumericUpDownIsKeyboardFocusWithinChanged;
            _numericUpDown.ValueChanged += NumericUpDownValueChanged;
        }

        public bool IsLoaded => _numericUpDown.IsLoaded;

        public bool IsVisible => _numericUpDown.IsVisible;

        public virtual bool IsEmpty() => !_numericUpDown.Value.HasValue;

        public bool IsFocused() => _numericUpDown.IsKeyboardFocusWithin || _numericUpDown.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void NumericUpDownIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void NumericUpDownLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void NumericUpDownValueChanged(object? sender, RoutedPropertyChangedEventArgs<double?> e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void NumericUpDownIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _numericUpDown.Loaded -= NumericUpDownLoaded;
                    _numericUpDown.IsVisibleChanged -= NumericUpDownIsVisibleChanged;
                    _numericUpDown.IsKeyboardFocusWithinChanged -= NumericUpDownIsKeyboardFocusWithinChanged;
                    _numericUpDown.ValueChanged -= NumericUpDownValueChanged;
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
