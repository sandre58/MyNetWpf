// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;

namespace MyNet.Wpf.TestApp.Views
{
    /// <summary>
    /// Interaction logic for Page1View.xaml
    /// </summary>
    public partial class InputsCalendarsView
    {
        public InputsCalendarsView() => InitializeComponent();

        public DateTime? Month { get; set; } = DateTime.Today;

        public DateTime? Year { get; set; } = DateTime.Today;
    }
}
