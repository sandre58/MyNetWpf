// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using MyNet.UI.Toasting.Settings;
using MyNet.Wpf.Toasting.Lifetime.Clear;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public sealed class TimeAndCountBasedLifetimeSupervisor(TimeSpan toastLifetime, MaximumToastCount maximumNotificationCount, Func<Dispatcher> dispatcher) : IToastLifetimeSupervisor
    {
        private readonly TimeSpan _toastLifetime = toastLifetime;
        private readonly int _maximumToastsCount = maximumNotificationCount.Count;

        private readonly Func<Dispatcher> _dispatcher = dispatcher;
        private readonly ToastList _toasts = [];
        private Queue<Toast>? _toastPending;

        private readonly Interval _interval = new();

        public void PushToast(Toast toast)
        {
            if (_disposed)
            {
                Debug.WriteLine($"Warn ToastNotifications {this}.{nameof(PushToast)} is already disposed");
                return;
            }

            if (!_interval.IsRunning)
                TimerStart();

            if (_toasts.Count == _maximumToastsCount)
            {
                _toastPending ??= new Queue<Toast>();
                _toastPending.Enqueue(toast);
                return;
            }

            var numberOfNotificationsToClose = Math.Max(_toasts.Count - _maximumToastsCount + 1, 0);

            var toastsToRemove = _toasts
                .OrderBy(x => x.Key)
                .Take(numberOfNotificationsToClose)
                .Select(x => x.Value)
                .ToList();

            foreach (var t in toastsToRemove)
                CloseToast(t.Toast);

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

            if (_toastPending != null && _toastPending.Count != 0)
            {
                var not = _toastPending.Dequeue();
                PushToast(not);
            }
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _interval.Stop();
            _toasts?.Clear();
            _toastPending?.Clear();
        }

        private void Toast_CloseRequest(object? sender, EventArgs e) => RequestCloseToast(new CloseToastEventArgs((Toast)sender!));

        private void RequestShowToast(ShowToastEventArgs e) => ShowToastRequested?.Invoke(this, e);

        private void RequestCloseToast(CloseToastEventArgs e) => CloseToastRequested?.Invoke(this, e);

        private void TimerStart() => _interval.Invoke(TimeSpan.FromMilliseconds(200), OnTimerTick, _dispatcher());

        private void TimerStop() => _interval.Stop();

        private void OnTimerTick()
        {
            var now = DateTime.Now.TimeOfDay;

            var toastsToRemove = _toasts
                .Where(x => (x.Value.Toast.Settings.ClosingStrategy == ToastClosingStrategy.AutoClose || x.Value.Toast.Settings.ClosingStrategy == ToastClosingStrategy.Both) && x.Value.Toast.CanClose && x.Value.CreatedDate + _toastLifetime <= now)
                .Select(x => x.Value)
                .ToList();

            foreach (var t in toastsToRemove)
                CloseToast(t.Toast);

            if (_toasts.IsEmpty)
                TimerStop();
        }

        public void ClearMessages(IClearStrategy clearStrategy)
        {
            var toasts = clearStrategy.GetToastsToRemove(_toasts.Select(x => x.Value.Toast));
            foreach (var toast in toasts)
            {
                CloseToast(toast);
            }
        }

        public event EventHandler<ShowToastEventArgs>? ShowToastRequested;
        public event EventHandler<CloseToastEventArgs>? CloseToastRequested;
    }
}
