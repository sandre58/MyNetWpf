// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;

namespace MyNet.Wpf.Controls.Toasts.Events
{
    public interface IKeyboardEventHandler
    {
        void Handle(KeyEventArgs eventArgs);
    }
}
