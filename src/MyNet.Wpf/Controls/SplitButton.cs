// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ButtonName, Type = typeof(ToggleButton))]
    public class SplitButton : DropDownButton
    {
        private const string ButtonName = "PART_Button";

        private ToggleButton? _button;

        static SplitButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _button = GetTemplateChild(ButtonName) as ToggleButton;
            if (_button is not null)
            {
                _button.Checked += Button_Checked;
            }
        }

        private void Button_Checked(object sender, RoutedEventArgs e)
        {
            if (_button is null || IsCheckedEnabled || !IsChecked) return;

            _button.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        }

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SplitButton), new UIPropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion Command

        #region CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        #endregion CommandParameter

        #region IsChecked

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(SplitButton), new UIPropertyMetadata(false));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion IsChecked

        #region IsCheckedEnabled

        public static readonly DependencyProperty IsCheckedEnabledProperty = DependencyProperty.Register(nameof(IsCheckedEnabled), typeof(bool), typeof(SplitButton), new UIPropertyMetadata(false));

        public bool IsCheckedEnabled
        {
            get => (bool)GetValue(IsCheckedEnabledProperty);
            set => SetValue(IsCheckedEnabledProperty, value);
        }

        #endregion IsCheckedEnabled

        #region ToggleButtonWidth

        public static readonly DependencyProperty ToggleButtonWidthProperty = DependencyProperty.Register(nameof(ToggleButtonWidth), typeof(double), typeof(SplitButton), new UIPropertyMetadata(double.NaN));

        [TypeConverter(typeof(LengthConverter))]
        public double ToggleButtonWidth
        {
            get => (double)GetValue(ToggleButtonWidthProperty);
            set => SetValue(ToggleButtonWidthProperty, value);
        }

        #endregion ToggleButtonWidth

        #region ToggleButtonHeight

        public static readonly DependencyProperty ToggleButtonHeightProperty = DependencyProperty.Register(nameof(ToggleButtonHeight), typeof(double), typeof(SplitButton), new UIPropertyMetadata(double.NaN));

        [TypeConverter(typeof(LengthConverter))]
        public double ToggleButtonHeight
        {
            get => (double)GetValue(ToggleButtonHeightProperty);
            set => SetValue(ToggleButtonHeightProperty, value);
        }

        #endregion ToggleButtonHeight

        #region InnerSpacing

        public static readonly DependencyProperty InnerSpacingProperty = DependencyProperty.Register(nameof(InnerSpacing), typeof(GridLength), typeof(SplitButton), new UIPropertyMetadata(new GridLength(1)));

        public GridLength InnerSpacing
        {
            get => (GridLength)GetValue(InnerSpacingProperty);
            set => SetValue(InnerSpacingProperty, value);
        }

        #endregion InnerSpacing
    }
}
