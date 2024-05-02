// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Controls.HintProxy
{
    public class AutoSuggestBoxHintProxy : IHintProxy
    {
        private readonly AutoSuggestBox _autoSuggestBox;
        private bool _disposedValue;

        public AutoSuggestBoxHintProxy(AutoSuggestBox autoSuggestBox)
        {
            ArgumentNullException.ThrowIfNull(autoSuggestBox);

            _autoSuggestBox = autoSuggestBox;
            _autoSuggestBox.TextChanged += AutoSuggestBoxTextChanged;
            _autoSuggestBox.Loaded += AutoSuggestBoxLoaded;
            _autoSuggestBox.IsVisibleChanged += AutoSuggestBoxIsVisibleChanged;
            _autoSuggestBox.IsKeyboardFocusWithinChanged += AutoSuggestBoxIsKeyboardFocusWithinChanged;
        }

        public bool IsLoaded => _autoSuggestBox.IsLoaded;

        public bool IsVisible => _autoSuggestBox.IsVisible;

        public bool IsEmpty() => string.IsNullOrEmpty(_autoSuggestBox.Text);

        public bool IsFocused() => _autoSuggestBox.IsKeyboardFocusWithin || _autoSuggestBox.IsFocused;

        public event EventHandler? ContentChanged;

        public event EventHandler? IsVisibleChanged;

        public event EventHandler? Loaded;
        public event EventHandler? FocusedChanged;

        private void AutoSuggestBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

        private void AutoSuggestBoxLoaded(object sender, RoutedEventArgs e)
            => Loaded?.Invoke(sender, EventArgs.Empty);

        private void AutoSuggestBoxTextChanged(object sender, EventArgs e)
            => ContentChanged?.Invoke(sender, EventArgs.Empty);

        private void AutoSuggestBoxIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
            => FocusedChanged?.Invoke(sender, EventArgs.Empty);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _autoSuggestBox.Loaded -= AutoSuggestBoxLoaded;
                    _autoSuggestBox.IsVisibleChanged -= AutoSuggestBoxIsVisibleChanged;
                    _autoSuggestBox.TextChanged -= AutoSuggestBoxTextChanged;
                    _autoSuggestBox.IsKeyboardFocusWithinChanged -= AutoSuggestBoxIsKeyboardFocusWithinChanged;
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
