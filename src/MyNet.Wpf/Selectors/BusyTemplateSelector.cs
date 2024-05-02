// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.UI.Busy.Models;

namespace MyNet.Wpf.Selectors
{
    public class BusyTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            => item is ProgressionBusy
                ? ProgressionBusyTemplate
                : item is DeterminateBusy
                ? DeterminateBusyTemplate
                : item is IndeterminateBusy
                ? IndeterminateBusyTemplate
                : base.SelectTemplate(item, container);

        public DataTemplate? IndeterminateBusyTemplate { get; set; }

        public DataTemplate? DeterminateBusyTemplate { get; set; }

        public DataTemplate? ProgressionBusyTemplate { get; set; }
    }
}
