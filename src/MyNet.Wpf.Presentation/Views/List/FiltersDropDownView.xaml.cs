// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Presentation.Views.List
{
    public partial class FiltersDropDownView
    {
        public FiltersDropDownView() => InitializeComponent();

        #region PopupContentTemplate

        public static readonly DependencyProperty PopupContentTemplateProperty
            = DependencyProperty.RegisterAttached(nameof(PopupContentTemplate), typeof(DataTemplate), typeof(ExtendedFiltersView));

        public DataTemplate PopupContentTemplate
        {
            get => (DataTemplate)GetValue(PopupContentTemplateProperty);
            set => SetValue(PopupContentTemplateProperty, value);
        }

        #endregion
    }
}
