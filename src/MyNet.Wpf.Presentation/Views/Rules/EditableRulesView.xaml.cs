// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
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

        public static readonly DependencyProperty EmptyTemplateProperty =
    DependencyProperty.Register(
        nameof(EmptyTemplate), typeof(ControlTemplate), typeof(EditableRulesView));

        public ControlTemplate EmptyTemplate
        {
            get => (ControlTemplate)GetValue(EmptyTemplateProperty);
            set => SetValue(EmptyTemplateProperty, value);
        }
    }
}
