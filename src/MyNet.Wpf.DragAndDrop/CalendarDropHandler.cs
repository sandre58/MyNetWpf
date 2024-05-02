// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;
using MyNet.Observable;
using MyNet.Utilities;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.DragAndDrop
{
    public class CalendarDropHandler : DefaultDropHandler
    {
        private readonly Action<IEnumerable<IAppointment>, DateTime> _dropOnDate;
        private readonly Func<IEnumerable<IAppointment>, bool> _canDrag;
        private readonly Type _dropTargetAdorner;

        public CalendarDropHandler(Action<IEnumerable<IAppointment>, DateTime> dropOnDate)
            : this(dropOnDate, _ => true, typeof(CalendarDropTargetAdorner)) { }

        public CalendarDropHandler(Action<IEnumerable<IAppointment>, DateTime> dropOnDate,
                                   Func<IEnumerable<IAppointment>, bool> canDrag)
            : this(dropOnDate, canDrag, typeof(CalendarDropTargetAdorner)) { }

        public CalendarDropHandler(Action<IEnumerable<IAppointment>, DateTime> dropOnDate,
                                   Func<IEnumerable<IAppointment>, bool> canDrag,
                                   Type dropTargetAdorner)
        {
            _dropOnDate = dropOnDate;
            _canDrag = canDrag;
            _dropTargetAdorner = dropTargetAdorner;
        }

        public override void DragOver(IDropInfo dropInfo)
        {
            var data = ExtractData(dropInfo.Data).OfType<CalendarAppointment>().Select(x => x.DataContext).OfType<IAppointment>().ToList();
            if (CanAcceptData(dropInfo) && CanDrag(data))
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = _dropTargetAdorner;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.None;
            }
        }

        /// <inheritdoc />
        public override void Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null) return;

            var data = ExtractData(dropInfo.Data).OfType<CalendarAppointment>().Select(x => x.DataContext).OfType<IAppointment>().ToList();
            var calendarBase = dropInfo.DragInfo.VisualSource.CastIn<CalendarHoursByDay>();
            var date = calendarBase.GetAccurateDateFromPosition(dropInfo.DropPosition);

            if (!date.HasValue) return;

            _dropOnDate.Invoke(data, date.Value);
        }

        public virtual bool CanDrag(IEnumerable<IAppointment> appointments) => _canDrag.Invoke(appointments);
    }
}
