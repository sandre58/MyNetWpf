// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Presentation.Views.List
{
    public partial class PaginationView
    {
        public PaginationView() => InitializeComponent();

        #region ShowPageSize

        public static readonly DependencyProperty ShowPageSizeProperty
            = DependencyProperty.RegisterAttached(nameof(ShowPageSize), typeof(bool), typeof(PaginationView), new PropertyMetadata(true));

        public bool ShowPageSize
        {
            get => (bool)GetValue(ShowPageSizeProperty);
            set => SetValue(ShowPageSizeProperty, value);
        }

        #endregion
    }
}
