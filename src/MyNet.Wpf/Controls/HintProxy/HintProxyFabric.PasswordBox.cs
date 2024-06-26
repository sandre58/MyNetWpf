﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls.HintProxy;

public class PasswordBoxHintProxy : IHintProxy
{
    private readonly PasswordBox _passwordBox;
    private bool _disposedValue;

    public bool IsEmpty() => string.IsNullOrEmpty(_passwordBox.Password);

    public bool IsLoaded => _passwordBox.IsLoaded;

    public bool IsVisible => _passwordBox.IsVisible;

    public bool IsFocused() => _passwordBox.IsKeyboardFocusWithin;

    public event EventHandler? ContentChanged;
    public event EventHandler? IsVisibleChanged;
    public event EventHandler? Loaded;
    public event EventHandler? FocusedChanged;

    public PasswordBoxHintProxy(PasswordBox passwordBox)
    {
        ArgumentNullException.ThrowIfNull(passwordBox);

        _passwordBox = passwordBox;
        _passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
        _passwordBox.Loaded += PasswordBoxLoaded;
        _passwordBox.IsVisibleChanged += PasswordBoxIsVisibleChanged;
        _passwordBox.IsKeyboardFocusWithinChanged += PasswordBoxIsKeyboardFocusedChanged;
    }

    private void PasswordBoxIsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        => FocusedChanged?.Invoke(this, EventArgs.Empty);

    private void PasswordBoxIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        => IsVisibleChanged?.Invoke(this, EventArgs.Empty);

    private void PasswordBoxLoaded(object sender, RoutedEventArgs e)
        => Loaded?.Invoke(this, EventArgs.Empty);

    private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        => ContentChanged?.Invoke(this, EventArgs.Empty);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
                _passwordBox.Loaded -= PasswordBoxLoaded;
                _passwordBox.IsVisibleChanged -= PasswordBoxIsVisibleChanged;
                _passwordBox.IsKeyboardFocusWithinChanged -= PasswordBoxIsKeyboardFocusedChanged;
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
