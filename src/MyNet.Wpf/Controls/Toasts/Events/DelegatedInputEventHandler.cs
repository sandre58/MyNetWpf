// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace MyNet.Wpf.Controls.Toasts.Events
{
    public class DelegatedInputEventHandler(Action<KeyEventArgs> action) : IKeyboardEventHandler
    {
        private readonly Action<KeyEventArgs> _action = action;

        public void Handle(KeyEventArgs eventArgs) => _action(eventArgs);
    }
}
