// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels.Dialogs;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Generator;
using MyNet.Utilities.Helpers;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class CalendarsCalendarsViewModel : NavigableWorkspaceViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DisplayDate { get; set; }

        public ICommand AddCommand { get; }

        public ObservableCollection<AppointmentSample> Appointments { get; } = [];

        public CalendarsCalendarsViewModel()
        {
            StartDate = DateTime.Today.AddYears(-1).BeginningOfYear();
            EndDate = DateTime.Today.AddYears(1).EndOfYear();
            DisplayDate = DateTime.Today;
            EnumerableHelper.Iteration(RandomGenerator.Int(150, 300), x => Appointments.Add(new AppointmentSample($"Birthday n° {x}", RandomGenerator.Date(StartDate, EndDate).ToLocalTime(), new TimeSpan(RandomGenerator.Int(1, 8), RandomGenerator.Int(0, 59), 0))));

            AddCommand = CommandsManager.Create<DateTime>(async x =>
            {
                var vm = new PickTimeViewModel() { Time = x.AddHours(16) };
                var result = await DialogManager.ShowDialogAsync(vm);

                if (result.IsTrue())
                {
                    Appointments.Add(new AppointmentSample("Event n°" + (Appointments.Count + 1), vm.Time, new TimeSpan(2, 0, 0)));

                    ToasterManager.ShowSuccess("Event added to " + Appointments[Appointments.Count - 1].StartDate.ToString(CultureInfo.CurrentCulture));
                }
            });
        }
    }

    internal class PickTimeViewModel : DialogViewModel
    {
        public DateTime Time { get; set; }
    }

    internal class AppointmentSample : EditableObject, IAppointment
    {
        public string Title { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public AppointmentSample(string title, DateTime date, TimeSpan duration)
        {
            Title = title;
            StartDate = date;
            EndDate = date.AddFluentTimeSpan(new FluentTimeSpan().Add(duration));
        }

        public override string ToString() => Title;
    }
}
