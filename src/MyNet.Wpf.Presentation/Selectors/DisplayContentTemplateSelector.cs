// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyNet.UI.ViewModels;

namespace MyNet.Wpf.Presentation.Selectors
{
    public class DataTemplatesCollection : Collection<DataTemplate>
    {
        public DataTemplatesCollection() : base() { }

        public DataTemplatesCollection(IList<DataTemplate> list) : base(list) { }
    }

    public class DisplayContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IListViewModel listViewModel || listViewModel.Display.Mode is null) return base.SelectTemplate(item, container);

            if (EmptySourceTemplate is not null && listViewModel.SourceCount == 0 && !listViewModel.Display.Mode.OverrideEmptySourceTemplate) return EmptySourceTemplate;
            if (EmptyItemsTemplate is not null && listViewModel.Count == 0 && !listViewModel.Display.Mode.OverrideEmptyItemsTemplate) return EmptyItemsTemplate;

            var allowedDisplayModes = listViewModel.Display.AllowedModes;
            var displayModeIndex = allowedDisplayModes.IndexOf(listViewModel.Display.Mode);

            return Templates is null || displayModeIndex < 0 || displayModeIndex >= Templates.Count
                ? base.SelectTemplate(item, container)
                : Templates[displayModeIndex];
        }

        public DataTemplatesCollection? Templates { get; set; }

        public DataTemplate? EmptyItemsTemplate { get; set; }

        public DataTemplate? EmptySourceTemplate { get; set; }
    }
}
