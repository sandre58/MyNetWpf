// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Presentation.Views.List
{
    /// <summary>
    /// Interaction logic for ExtendedFiltersView.xaml
    /// </summary>
    public partial class ExtendedFiltersView
    {
        public ExtendedFiltersView() => InitializeComponent();

        #region ShowAdvancedFilters

        public static readonly DependencyProperty ShowAdvancedFiltersProperty
            = DependencyProperty.RegisterAttached(nameof(ShowAdvancedFilters), typeof(bool), typeof(ExtendedFiltersView), new PropertyMetadata(false));

        public bool ShowAdvancedFilters
        {
            get => (bool)GetValue(ShowAdvancedFiltersProperty);
            set => SetValue(ShowAdvancedFiltersProperty, value);
        }

        #endregion

        #region DropDownHeight

        public static readonly DependencyProperty DropDownHeightProperty
            = DependencyProperty.RegisterAttached(nameof(DropDownHeight), typeof(double), typeof(ExtendedFiltersView), new PropertyMetadata(400.0D));

        public double DropDownHeight
        {
            get => (double)GetValue(DropDownHeightProperty);
            set => SetValue(DropDownHeightProperty, value);
        }

        #endregion

        #region DropDownWidth

        public static readonly DependencyProperty DropDownWidthProperty
            = DependencyProperty.RegisterAttached(nameof(DropDownWidth), typeof(double), typeof(ExtendedFiltersView), new PropertyMetadata(450.0D));

        public double DropDownWidth
        {
            get => (double)GetValue(DropDownWidthProperty);
            set => SetValue(DropDownWidthProperty, value);
        }

        #endregion

        #region SpeedFilters

        public static readonly DependencyProperty SpeedFiltersProperty
            = DependencyProperty.RegisterAttached(nameof(SpeedFilters), typeof(object), typeof(ExtendedFiltersView));

        public object SpeedFilters
        {
            get => GetValue(SpeedFiltersProperty);
            set => SetValue(SpeedFiltersProperty, value);
        }

        #endregion
    }
}
