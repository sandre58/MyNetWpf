// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class ColorPickerHintProxy : IHintProxy
    {
        private readonly ColorPicker _colorPicker;
        private bool _disposedValue;

        public ColorPickerHintProxy(ColorPicker colorPicker)
        {
            _colorPicker = colorPicker ?? throw new ArgumentNullException(nameof(colorPicker));

            _colorPicker.Loaded += ColorPickerLoaded;
            _colorPicker.IsVisibleChanged += ColorPickerIsVisibleChanged;
            _colorPicker.IsKeyboardFocusWithinChanged += ColorPickerIsKeyboardFocusWithinChanged;
            _colorPicker.SelectedColorChanged += ColorNameChanged;
        }

        public bool IsLoaded => _colorPicker.IsLoaded;

        public bool IsVisible => _colorPicker.IsVisible;

        public bool IsEmpty() => string.IsNullOrEmpty(_colorPicker.ColorName);

        public bool IsFocused() => _colorPicker.IsKeyboardFocusWithin || _colorPicker.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void ColorPickerIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void ColorPickerLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void ColorNameChanged(object? sender, RoutedPropertyChangedEventArgs<Color?> e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void ColorPickerIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _colorPicker.Loaded -= ColorPickerLoaded;
                    _colorPicker.IsVisibleChanged -= ColorPickerIsVisibleChanged;
                    _colorPicker.IsKeyboardFocusWithinChanged -= ColorPickerIsKeyboardFocusWithinChanged;
                    _colorPicker.SelectedColorChanged -= ColorNameChanged;
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
