// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public interface IInterval
    {
        bool IsRunning { get; }
        void Invoke(TimeSpan frequency, Action action, Dispatcher dispatcher);
        void Stop();
    }
}
