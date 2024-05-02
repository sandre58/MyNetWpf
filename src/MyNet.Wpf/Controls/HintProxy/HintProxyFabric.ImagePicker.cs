// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class ImagePickerHintProxy : IHintProxy
    {
        private readonly ImagePicker _imagePicker;
        private bool _disposedValue;

        public ImagePickerHintProxy(ImagePicker imagePicker)
        {
            _imagePicker = imagePicker ?? throw new ArgumentNullException(nameof(imagePicker));

            _imagePicker.Loaded += ImagePickerLoaded;
            _imagePicker.IsVisibleChanged += ImagePickerIsVisibleChanged;
            _imagePicker.IsKeyboardFocusWithinChanged += ImagePickerIsKeyboardFocusWithinChanged;
            _imagePicker.SelectedImageChanged += SelectedImageChanged;
        }

        public bool IsLoaded => _imagePicker.IsLoaded;

        public bool IsVisible => _imagePicker.IsVisible;

        public bool IsEmpty() => _imagePicker.SelectedImage == null && string.IsNullOrEmpty(_imagePicker.ImagePath);

        public bool IsFocused() => _imagePicker.IsKeyboardFocusWithin || _imagePicker.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void ImagePickerIsVisibleChanged(object? sender, DependencyPropertyChangedEventArgs e) => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void ImagePickerLoaded(object? sender, RoutedEventArgs e) => Loaded?.Invoke(sender, EventArgs.Empty);

        private void SelectedImageChanged(object? sender, RoutedPropertyChangedEventArgs<ImageSource?> e) => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void ImagePickerIsKeyboardFocusWithinChanged(object? sender, DependencyPropertyChangedEventArgs e) => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _imagePicker.Loaded -= ImagePickerLoaded;
                    _imagePicker.IsVisibleChanged -= ImagePickerIsVisibleChanged;
                    _imagePicker.IsKeyboardFocusWithinChanged -= ImagePickerIsKeyboardFocusWithinChanged;
                    _imagePicker.SelectedImageChanged -= SelectedImageChanged;
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
