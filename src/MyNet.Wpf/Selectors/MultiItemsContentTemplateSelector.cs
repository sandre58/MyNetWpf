// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Selectors
{
    public class MultiItemsDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not IEnumerable<object> collection) return base.SelectTemplate(item, container);
            var count = collection.Count();
            return count == 0
                ? EmptyTemplate ?? WpfHelper.GetResource<DataTemplate>("MyNet.DataTemplates.NoItems.Large")
                : count == 1
                ? SingleTemplate
                : ManyTemplate ?? WpfHelper.GetResource<DataTemplate>("MyNet.DataTemplates.ManyItems.Large");
        }

        public DataTemplate? EmptyTemplate { get; set; }

        public DataTemplate? ManyTemplate { get; set; }

        public DataTemplate? SingleTemplate { get; set; }
    }
}
