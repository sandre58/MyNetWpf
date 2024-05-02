// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MyNet.UI.ViewModels.Import;

namespace MyNet.Wpf.Presentation.Selectors
{
    public class ImportSourceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? DatabaseTemplate { get; set; }

        public DataTemplate? FileTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            => ((KeyValuePair<ImportSource, ImportSourceViewModel>)item).Key switch
            {
                ImportSource.Database => DatabaseTemplate,
                ImportSource.File => FileTemplate,
                _ => (DataTemplate?)base.SelectTemplate(item, container)
            };
    }
}
