// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using MyNet.UI.Notifications;
using MyNet.UI.Toasting.Settings;

namespace MyNet.Wpf.Toasting
{
    public class Toast(INotification notification, ToastSettings settings, Action<INotification>? onClick = null, Action? onClose = null)
    {
        private readonly Action? _onClose = onClose;

        internal int Id { get; set; }

        public bool CanClose { get; set; } = true;

        public INotification Notification { get; } = notification;

        public ToastSettings Settings { get; } = settings;

        public Action<INotification>? OnClick { get; } = onClick;

        public event EventHandler? CloseRequest;

        public void Close()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
            _onClose?.Invoke();
        }
        public override bool Equals(object? obj) => obj != null && GetType() == obj.GetType() && Id == ((Toast)obj).Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
}
