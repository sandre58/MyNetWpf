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
