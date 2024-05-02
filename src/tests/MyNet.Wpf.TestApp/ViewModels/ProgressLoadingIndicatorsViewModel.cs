// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using MyNet.UI.Busy;
using MyNet.UI.Busy.Models;
using MyNet.UI.Commands;
using MyNet.UI.Extensions;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class ProgressLoadingIndicatorsViewModel : NavigableWorkspaceViewModel
    {
        public IBusyService SmallBusyService { get; private set; }

        public int CountStep { get; set; } = 5;

        public int DurationStep { get; set; } = 2000;

        public ICommand WaitIndeterminateCommand { get; set; }

        public ICommand WaitDeterminateCommand { get; set; }

        public ICommand WaitProgressionCommand { get; set; }

        public IBusyService BusyService2 { get; }

        public ProgressLoadingIndicatorsViewModel(IBusyServiceFactory busyServiceFactory)
        {
            BusyService = busyServiceFactory.Create();
            BusyService2 = busyServiceFactory.Create();
            SmallBusyService = BusyManager.Create();
            WaitDeterminateCommand = CommandsManager.CreateNotNull<IBusyService>(async x => await x.WaitDeterminateAsync(e =>
            {
                using var cancel = new CancellationTokenSource();
                e.CancelAction = () => cancel.Cancel();

                try
                {
                    e.Message = "Start...";
                    for (var i = 1; i <= CountStep; i++)
                    {
                        cancel.Token.ThrowIfCancellationRequested();
                        Thread.Sleep(DurationStep);
                        cancel.Token.ThrowIfCancellationRequested();
                        e.Message = "End of Step " + i;
                        e.Value = i * e.Maximum / CountStep;
                    }
                }
                catch (OperationCanceledException)
                {
                    ToasterManager.ShowError("Operation has been cancelled");
                }
            }).ConfigureAwait(false));

            WaitIndeterminateCommand = CommandsManager.CreateNotNull<IBusyService>(async x => await x.WaitIndeterminateAsync(e =>
            {
                using var cancel = new CancellationTokenSource();
                e.CancelAction = () => cancel.Cancel();

                try
                {
                    e.Message = "Start...";
                    for (var i = 1; i <= CountStep; i++)
                    {
                        cancel.Token.ThrowIfCancellationRequested();
                        Thread.Sleep(DurationStep);
                        cancel.Token.ThrowIfCancellationRequested();
                        e.Message = "End of Step " + i;
                    }
                }
                catch (OperationCanceledException)
                {
                    ToasterManager.ShowError("Operation has been cancelled");
                }
            }).ConfigureAwait(false));

            WaitProgressionCommand = CommandsManager.CreateNotNull<IBusyService>(async x => await x.WaitAsync<ProgressionBusy>(e =>
            {
                using var cancel = new CancellationTokenSource();
                e.CancelAction = () => cancel.Cancel();
                e.Title = "Loading Data";
                try
                {
                    for (var i = 1; i <= CountStep; i++)
                    {
                        e.Messages = ["Execution of Step " + i];
                        cancel.Token.ThrowIfCancellationRequested();
                        Thread.Sleep(DurationStep);
                        cancel.Token.ThrowIfCancellationRequested();
                        e.Value = (double)i / CountStep;
                    }
                }
                catch (OperationCanceledException)
                {
                    ToasterManager.ShowError("Operation has been cancelled");
                }
            }).ConfigureAwait(false));
        }
    }
}
