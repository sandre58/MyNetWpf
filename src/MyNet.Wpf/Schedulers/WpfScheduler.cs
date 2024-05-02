// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MyNet.Wpf.Schedulers
{
    /// <summary>
    /// Represents an object that schedules units of work on a <see cref="System.Windows.Threading.Dispatcher"/>.
    /// </summary>
    /// <remarks>
    /// This scheduler type is typically used indirectly through the <see cref="System.Reactive.Linq.DispatcherObservable.ObserveOnDispatcher&lt;TSource&gt;(IObservable&lt;TSource&gt;)"/> and <see cref="System.Reactive.Linq.DispatcherObservable.SubscribeOnDispatcher&lt;TSource&gt;(IObservable&lt;TSource&gt;)"/> methods that use the Dispatcher on the calling thread.
    /// </remarks>
    public class WpfScheduler : LocalScheduler, ISchedulerPeriodic
    {
        private static WpfScheduler _current = null!;

        /// <summary>
        /// Gets the scheduler that schedules work on the <see cref="System.Windows.Threading.Dispatcher"/> for the current thread.
        /// </summary>
        public static WpfScheduler Current
        {
            get
            {
                if (_current == null)
                {
                    var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
                    dispatcher ??= Application.Current.Dispatcher;

                    _current = new WpfScheduler(dispatcher);
                }

                return _current;
            }
        }

        /// <summary>
        /// Constructs a WpfScheduler that schedules units of work on the given <see cref="System.Windows.Threading.Dispatcher"/>.
        /// </summary>
        /// <param name="dispatcher">Dispatcher to schedule work on.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> is null.</exception>
        public WpfScheduler(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            Priority = DispatcherPriority.DataBind;
        }

        /// <summary>
        /// Constructs a WpfScheduler that schedules units of work on the given <see cref="System.Windows.Threading.Dispatcher"/> at the given priority.
        /// </summary>
        /// <param name="dispatcher">Dispatcher to schedule work on.</param>
        /// <param name="priority">Priority at which units of work are scheduled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> is null.</exception>
        public WpfScheduler(Dispatcher dispatcher, DispatcherPriority priority)
        {
            Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            Priority = priority;
        }

        /// <summary>
        /// Gets the <see cref="System.Windows.Threading.Dispatcher"/> associated with the WpfScheduler.
        /// </summary>
        public Dispatcher Dispatcher { get; }

        /// <summary>
        /// Gets the priority at which work items will be dispatched.
        /// </summary>
        public DispatcherPriority Priority { get; }

        /// <summary>
        /// Schedules an action to be executed on the dispatcher.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="action">Action to be executed.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            var d = new SingleAssignmentDisposable();

            Dispatcher.Invoke(() =>
                {
                    if (!d.IsDisposed)
                    {
                        d.Disposable = action(this, state);
                    }
                }
                , Priority
            );

            return d;
        }

        /// <summary>
        /// Schedules an action to be executed after dueTime on the dispatcher, using a <see cref="DispatcherTimer"/> object.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="dueTime">Relative time after which to execute the action.</param>
        /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            var dt = Scheduler.Normalize(dueTime);
            if (dt.Ticks == 0)
                return Schedule(state, action);

            var d = new MultipleAssignmentDisposable();

            var timer = new DispatcherTimer(
                Priority, Dispatcher
            );

            timer.Tick += (s, e) =>
            {
                var t = Interlocked.Exchange(ref timer, default);
                if (t != null)
                {
                    try
                    {
                        d.Disposable = action(this, state);
                    }
                    finally
                    {
                        t.Stop();
                        action = (_, _) => Disposable.Empty;
                    }
                }
            };

            timer.Interval = dt;
            timer.Start();

            d.Disposable = Disposable.Create(() =>
            {
                var t = Interlocked.Exchange(ref timer, default);
                if (t != null)
                {
                    t.Stop();
                    action = (_, _) => Disposable.Empty;
                }
            });

            return d;
        }

        /// <summary>
        /// Schedules a periodic piece of work on the dispatcher, using a <see cref="DispatcherTimer"/> object.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">Initial state passed to the action upon the first iteration.</param>
        /// <param name="period">Period for running the work periodically.</param>
        /// <param name="action">Action to be executed, potentially updating the state.</param>
        /// <returns>The disposable object used to cancel the scheduled recurring action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="period"/> is less than TimeSpan.Zero.</exception>
        public IDisposable SchedulePeriodic<TState>(TState state, TimeSpan period, Func<TState, TState> action)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(period, TimeSpan.Zero);
            ArgumentNullException.ThrowIfNull(action);

            var timer = new DispatcherTimer(
                Priority, Dispatcher
            );

            var state1 = state;

            timer.Tick += (s, e) => state1 = action(state1);

            timer.Interval = period;
            timer.Start();

            return Disposable.Create(() =>
            {
                var t = Interlocked.Exchange(ref timer, default);
                if (t != null)
                {
                    t.Stop();
                    action = _ => _;
                }
            });
        }
    }
}
