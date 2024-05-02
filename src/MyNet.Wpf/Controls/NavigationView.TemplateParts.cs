// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = TemplateElementNavigationViewContentPresenter, Type = typeof(NavigationFrame))]
    [TemplatePart(Name = TemplateElementToggleButton, Type = typeof(Button))]
    [TemplatePart(Name = PreviousButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = NextButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = TemplateElementAutoSuggestBox, Type = typeof(AutoSuggestBox))]
    [TemplatePart(Name = TemplateElementAutoSuggestButton, Type = typeof(Button))]
    public partial class NavigationView
    {
        private const string TemplateElementNavigationViewContentPresenter = "PART_NavigationFrame";
        private const string TemplateElementToggleButton = "PART_ToggleButton";
        private const string TemplateElementAutoSuggestBox = "PART_AutoSuggestBox";
        private const string TemplateElementAutoSuggestButton = "PART_AutoSuggestButton";
        public const string PreviousButtonPartName = "PART_PreviousButton";
        public const string NextButtonPartName = "PART_NextButton";

        private NavigationFrame? _navigationViewContentPresenter;
        private Button? _toggleButton;
        protected AutoSuggestBox? _autoSuggestBox;
        protected Button? _autoSuggestButton;
        private Button? _previousButton;
        private Button? _nextButton;

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _navigationViewContentPresenter = GetTemplateChild(TemplateElementNavigationViewContentPresenter) as NavigationFrame;

            if (GetTemplateChild(TemplateElementAutoSuggestBox) is AutoSuggestBox autoSuggestBox)
            {
                _autoSuggestBox = autoSuggestBox;
                _autoSuggestBox.Provider = this;
                _autoSuggestBox.CommitSelectedItem -= AutoSuggestBoxCommitSelectedItem;
                _autoSuggestBox.CommitSelectedItem += AutoSuggestBoxCommitSelectedItem;
            }

            if (GetTemplateChild(TemplateElementToggleButton) is Button toggleButton)
            {
                _toggleButton = toggleButton;

                _toggleButton.Click -= OnToggleButtonClick;
                _toggleButton.Click += OnToggleButtonClick;
            }

            if (GetTemplateChild(TemplateElementAutoSuggestButton) is Button autoSuggestButton)
            {
                _autoSuggestButton = autoSuggestButton;

                _autoSuggestButton.Click -= OnAutoSuggestButtonClick;
                _autoSuggestButton.Click += OnAutoSuggestButtonClick;
            }

            if (GetTemplateChild(PreviousButtonPartName) is Button previousButton)
            {
                _previousButton = previousButton;

                _previousButton.Click -= OnPreviousButtonClick;
                _previousButton.Click += OnPreviousButtonClick;
            }

            if (GetTemplateChild(NextButtonPartName) is Button nextButton)
            {
                _nextButton = nextButton;

                _nextButton.Click -= OnNextButtonClick;
                _nextButton.Click += OnNextButtonClick;
            }

            foreach (var element in FooterMenuItems)
            {
                if (element is not NavigationViewItem navigationViewItem) continue;

                navigationViewItem.SetNavigationView(this);

                navigationViewItem.Click -= NavigationViewItem_Click;
                navigationViewItem.Click += NavigationViewItem_Click;
            }
        }
    }
}
