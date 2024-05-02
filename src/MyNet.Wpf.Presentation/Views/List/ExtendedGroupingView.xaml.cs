// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Presentation.Views.List
{
    public partial class ExtendedGroupingView
    {
        public ExtendedGroupingView() => InitializeComponent();

        #region ShowActionButton

        public static readonly DependencyProperty ShowActionButtonProperty
            = DependencyProperty.RegisterAttached(nameof(ShowActionButton), typeof(bool), typeof(ExtendedGroupingView), new PropertyMetadata(true));

        public bool ShowActionButton
        {
            get => (bool)GetValue(ShowActionButtonProperty);
            set => SetValue(ShowActionButtonProperty, value);
        }

        #endregion
    }
}
