// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using MyNet.Wpf.Toasting.Lifetime.Clear;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public sealed class CountBasedLifetimeSupervisor(MaximumToastCount maximumNotificationCount) : IToastLifetimeSupervisor
    {
        private readonly int _maximumToastCount = maximumNotificationCount.Count;
        private readonly ToastList _toasts = [];

        public void PushToast(Toast toast)
        {
            if (_disposed)
            {
                Debug.WriteLine($"Warn ToastNotifications {this}.{nameof(PushToast)} is already disposed");
                return;
            }

            var numberOfToastsToClose = Math.Max(_toasts.Count - _maximumToastCount, 0);

            var toastsToRemove = _toasts
                .OrderBy(x => x.Key)
                .Take(numberOfToastsToClose)
                .Select(x => x.Value)
                .ToList();

            foreach (var n in toastsToRemove)
                CloseToast(n.Toast);

            _ = _toasts.Add(toast);
            toast.CloseRequest += Toast_CloseRequest;
            RequestShowToast(new ShowToastEventArgs(toast));
        }

        public void CloseToast(Toast toast)
        {
            _ = _toasts.TryRemove(toast.Id, out var removedToast);

            if (removedToast is not null)
            {
                removedToast.Toast.Close();
                removedToast.Toast.CloseRequest -= Toast_CloseRequest;
            }
        }

        private void Toast_CloseRequest(object? sender, EventArgs e) => RequestCloseToast(new CloseToastEventArgs((Toast)sender!));

        private void RequestShowToast(ShowToastEventArgs e) => ShowToastRequested?.Invoke(this, e);

        private void RequestCloseToast(CloseToastEventArgs e) => CloseToastRequested?.Invoke(this, e);


        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _toasts?.Clear();
        }

        public void ClearMessages(IClearStrategy clearStrategy)
        {
            var notifications = clearStrategy.GetToastsToRemove(_toasts.Select(x => x.Value.Toast));
            foreach (var notification in notifications)
            {
                CloseToast(notification);
            }
        }

        public event EventHandler<ShowToastEventArgs>? ShowToastRequested;
        public event EventHandler<CloseToastEventArgs>? CloseToastRequested;
    }
}
