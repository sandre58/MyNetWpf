// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Selectors
{
    public class NullTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
            => item is null
                ? NullTemplate
                : base.SelectTemplate(item, container);

        public DataTemplate? NullTemplate { get; set; }
    }
}
