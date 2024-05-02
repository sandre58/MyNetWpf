// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Models;
using MyNet.UI.Dialogs.Settings;

namespace MyNet.Wpf.Dialogs
{
    public class MessageBoxFactory : IMessageBoxFactory
    {
        public IMessageBox Create(string message,
                                  string? title,
                                  MessageSeverity severity,
                                  MessageBoxResultOption buttons,
                                  MessageBoxResult defaultResut)
            => new MessageBoxViewModel(message, title, severity, buttons, defaultResut);
    }
}
