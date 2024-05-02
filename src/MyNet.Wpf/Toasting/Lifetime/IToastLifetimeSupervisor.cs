// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using MyNet.Wpf.Toasting.Lifetime.Clear;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public interface IToastLifetimeSupervisor : IDisposable
    {
        void PushToast(Toast toast);

        void CloseToast(Toast toast);

        event EventHandler<ShowToastEventArgs>? ShowToastRequested;
        event EventHandler<CloseToastEventArgs>? CloseToastRequested;

        void ClearMessages(IClearStrategy clearStrategy);
    }
}
