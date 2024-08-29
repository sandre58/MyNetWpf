// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using MyNet.Observable;
using MyNet.UI.Commands;
using MyNet.UI.Dialogs;
using MyNet.UI.Toasting;
using MyNet.UI.ViewModels.Dialogs;
using MyNet.UI.ViewModels.List;
using MyNet.UI.ViewModels.Workspace;
using MyNet.Utilities;
using MyNet.Utilities.DateTimes;
using MyNet.Utilities.Generator;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class AppointmentsListViewModel : ListViewModel<AppointmentSample>
    {
        public AppointmentsListViewModel(DateTime startDate, DateTime endDate) : base(RandomGenerator.Int(150, 300).Range().Select(x => new AppointmentSample($"Birthday n° {x}", RandomGenerator.Date(startDate, endDate).ToLocalTime(), new TimeSpan(RandomGenerator.Int(1, 8), RandomGenerator.Int(0, 59), 0))).ToList())
            => AddToDateCommand = CommandsManager.Create<DateTime>(async x =>
               {
                   var vm = new PickTimeViewModel() { Time = x.AddHours(16) };
                   var result = await DialogManager.ShowDialogAsync(vm);

                   if (result.IsTrue())
                   {
                       AddItemCore(new AppointmentSample("Event n°" + (Items.Count + 1), vm.Time, new TimeSpan(2, 0, 0)));

                       ToasterManager.ShowSuccess("Event added to " + Items[Items.Count - 1].StartDate.ToString(CultureInfo.CurrentCulture));
                   }
               });

        public ICommand AddToDateCommand { get; }
    }

    internal class CalendarsCalendarsViewModel : NavigableWorkspaceViewModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DisplayDate { get; set; }

        public AppointmentsListViewModel Appointments { get; }

        public CalendarsCalendarsViewModel()
        {
            StartDate = DateTime.Today.AddYears(-10).BeginningOfYear();
            EndDate = DateTime.Today.AddYears(10).EndOfYear();
            DisplayDate = DateTime.Today;
            Appointments = new(StartDate, EndDate);
        }
    }

    internal class PickTimeViewModel : DialogViewModel
    {
        public DateTime Time { get; set; }
    }

    internal class AppointmentSample : EditableObject, IAppointment
    {
        private readonly DateTime _utcStartDate;
        private readonly DateTime _utcEndDate;

        public string Title { get; }

        public DateTime StartDate => _utcStartDate.ToCurrentTime();

        public DateTime EndDate => _utcEndDate.ToCurrentTime();

        public AppointmentSample(string title, DateTime date, TimeSpan duration)
        {
            Title = title;
            _utcStartDate = date;
            _utcEndDate = date.AddFluentTimeSpan(new FluentTimeSpan().Add(duration));
        }

        public override string ToString() => Title;
    }
}
