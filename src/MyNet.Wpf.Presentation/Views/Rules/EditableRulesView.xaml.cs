// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using MyNet.UI.Resources;

namespace MyNet.Wpf.Presentation.Views.Rules
{
    public partial class EditableRulesView
    {
        public EditableRulesView() => InitializeComponent();

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header), typeof(string), typeof(EditableRulesView),
                new PropertyMetadata(UiResources.AddRule));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
    }
}
