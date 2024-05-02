// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public class Interval : IInterval
    {
        private DispatcherTimer? _timer;

        public bool IsRunning => _timer != null && _timer.IsEnabled;

        public void Invoke(TimeSpan frequency, Action action, Dispatcher dispatcher)
        {
            _timer = new DispatcherTimer(frequency, DispatcherPriority.Normal, (sender, args) => action(), dispatcher);
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
            _timer = null;
        }
    }
}
