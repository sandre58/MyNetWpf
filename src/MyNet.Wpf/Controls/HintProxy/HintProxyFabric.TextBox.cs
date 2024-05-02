// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls.HintProxy;

public class TextBoxHintProxy : IHintProxy
{
    private readonly TextBox _textBox;
    private bool _disposedValue;

    public bool IsLoaded => _textBox.IsLoaded;

    public bool IsVisible => _textBox.IsVisible;

    public virtual bool IsEmpty() => string.IsNullOrEmpty(_textBox.Text);

    public bool IsFocused() => _textBox.IsKeyboardFocusWithin;

    public event EventHandler? ContentChanged;
    public event EventHandler? IsVisibleChanged;
    public event EventHandler? Loaded;
    public event EventHandler? FocusedChanged;

    public TextBoxHintProxy(TextBox textBox)
    {
        _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        _textBox.TextChanged += TextBoxTextChanged;
        _textBox.Loaded += TextBoxLoaded;
        _textBox.IsVisibleChanged += TextBoxIsVisibleChanged;
        _textBox.IsKeyboardFocusWithinChanged += TextBoxIsKeyboardFocusedChanged;
    }

    private void TextBoxIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        => FocusedChanged?.Invoke(sender, EventArgs.Empty);

    private void TextBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        => IsVisibleChanged?.Invoke(sender, EventArgs.Empty);

    private void TextBoxLoaded(object sender, RoutedEventArgs e)
        => Loaded?.Invoke(sender, EventArgs.Empty);

    private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        => ContentChanged?.Invoke(sender, EventArgs.Empty);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _textBox.TextChanged -= TextBoxTextChanged;
                _textBox.Loaded -= TextBoxLoaded;
                _textBox.IsVisibleChanged -= TextBoxIsVisibleChanged;
                _textBox.IsKeyboardFocusWithinChanged -= TextBoxIsKeyboardFocusedChanged;
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
