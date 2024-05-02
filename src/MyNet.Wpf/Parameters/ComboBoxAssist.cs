// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.UI.Commands;

namespace MyNet.Wpf.Parameters
{
    public static class ComboBoxAssist
    {
        #region ShowDropDownPopup

        public static readonly DependencyProperty ShowDropDownPopupProperty = DependencyProperty.RegisterAttached(
            "ShowDropDownPopup",
            typeof(bool),
            typeof(ComboBoxAssist),
            new PropertyMetadata(true));

        public static bool GetShowDropDownPopup(DependencyObject item) => (bool)item.GetValue(ShowDropDownPopupProperty);

        public static void SetShowDropDownPopup(DependencyObject item, bool value) => item.SetValue(ShowDropDownPopupProperty, value);

        #endregion

        #region ShowCaret

        public static readonly DependencyProperty ShowCaretProperty = DependencyProperty.RegisterAttached(
            "ShowCaret",
            typeof(bool),
            typeof(ComboBoxAssist),
            new PropertyMetadata(true));

        public static bool GetShowCaret(DependencyObject item) => (bool)item.GetValue(ShowCaretProperty);

        public static void SetShowCaret(DependencyObject item, bool value) => item.SetValue(ShowCaretProperty, value);

        #endregion

        #region ShowUpDownButton

        public static readonly DependencyProperty ShowUpDownButtonProperty = DependencyProperty.RegisterAttached(
            "ShowUpDownButton",
            typeof(bool),
            typeof(ComboBoxAssist),
            new PropertyMetadata(false));

        public static bool GetShowUpDownButton(DependencyObject item) => (bool)item.GetValue(ShowUpDownButtonProperty);

        public static void SetShowUpDownButton(DependencyObject item, bool value) => item.SetValue(ShowUpDownButtonProperty, value);

        #endregion

        #region UpCommand

        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.RegisterAttached(
            "UpCommand",
            typeof(ICommand),
            typeof(ComboBoxAssist),
            new PropertyMetadata(UpCommand));

        public static ICommand GetUpCommand(DependencyObject item) => (ICommand)item.GetValue(UpCommandProperty);

        public static void SetUpCommand(DependencyObject item, ICommand value) => item.SetValue(UpCommandProperty, value);

        public static ICommand UpCommand => CommandsManager.Create<object>(Up, CanUp);

        private static void Up(object? obj)
        {
            if (obj is ComboBox cb && cb.SelectedIndex < cb.Items.Count - 1)
            {
                cb.SelectedIndex += 1;
            }
        }

        private static bool CanUp(object? obj) => obj is ComboBox cb && cb.SelectedIndex < cb.Items.Count - 1;

        #endregion

        #region DownCommand

        public static readonly DependencyProperty DownCommandProperty = DependencyProperty.RegisterAttached(
            "DownCommand",
            typeof(ICommand),
            typeof(ComboBoxAssist),
            new PropertyMetadata(DownCommand));

        public static ICommand GetDownCommand(DependencyObject item) => (ICommand)item.GetValue(DownCommandProperty);

        public static void SetDownCommand(DependencyObject item, ICommand value) => item.SetValue(DownCommandProperty, value);

        public static ICommand DownCommand => CommandsManager.Create<object>(Down, CanDown);

        private static void Down(object? obj)
        {
            if (obj is ComboBox cb && cb.SelectedIndex > 0)
            {
                cb.SelectedIndex -= 1;
            }
        }

        private static bool CanDown(object? obj) => obj is ComboBox cb && cb.SelectedIndex > 0;

        #endregion

        #region AttachedProperty : ShowSelectedItem

        public static readonly DependencyProperty ShowSelectedItemProperty = DependencyProperty.RegisterAttached(
            "ShowSelectedItem",
            typeof(bool),
            typeof(ComboBoxAssist),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetShowSelectedItem(DependencyObject element)
            => (bool)element.GetValue(ShowSelectedItemProperty);

        public static void SetShowSelectedItem(DependencyObject element, bool value)
            => element.SetValue(ShowSelectedItemProperty, value);
        #endregion

        #region AttachedProperty : MaxLength

        public static readonly DependencyProperty MaxLengthProperty =
           DependencyProperty.RegisterAttached(
               name: "MaxLength",
               propertyType: typeof(int),
               ownerType: typeof(ComboBoxAssist),
               defaultMetadata: new FrameworkPropertyMetadata(defaultValue: 0)
               );
        public static int GetMaxLength(DependencyObject element) => (int)element.GetValue(MaxLengthProperty);
        public static void SetMaxLength(DependencyObject element, int value) => element.SetValue(MaxLengthProperty, value);
        #endregion
    }
}
