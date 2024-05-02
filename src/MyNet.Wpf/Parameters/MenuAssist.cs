// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Parameters
{
    public static class MenuAssist
    {
        #region CloseOnClick

        public static readonly DependencyProperty CloseOnClickProperty = DependencyProperty.RegisterAttached(
            "CloseOnClick",
            typeof(bool),
            typeof(MenuAssist),
            new PropertyMetadata(false, OnCloseOnClickCallback));

        public static bool GetCloseOnClick(ButtonBase item) => (bool)item.GetValue(CloseOnClickProperty);

        public static void SetCloseOnClick(ButtonBase item, bool value) => item.SetValue(CloseOnClickProperty, value);

        private static void OnCloseOnClickCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ButtonBase button) return;

            button.Click += button_Click;

            void button_Click(object sender, RoutedEventArgs e)
            {
                var contextMenu = button.FindVisualParent<ContextMenu>();

                if (contextMenu is not null)
                    contextMenu.IsOpen = false;
                else if (button.FindVisualParent<DropDownButton>() is DropDownButton dropDownButton)
                {
                    dropDownButton.IsPopupOpen = false;
                }
            }
        }

        #endregion CloseOnClick

        #region CollapseDisabledItems

        public static readonly DependencyProperty CollapseDisabledItemsProperty = DependencyProperty.RegisterAttached(
            "CollapseDisabledItems",
            typeof(bool),
            typeof(MenuAssist),
            new PropertyMetadata(false));

        public static bool GetCollapseDisabledItems(MenuBase item) => (bool)item.GetValue(CollapseDisabledItemsProperty);

        public static void SetCollapseDisabledItems(MenuBase item, bool value) => item.SetValue(CollapseDisabledItemsProperty, value);

        #endregion CollapseDisabledItems

        #region IconTemplate

        public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.RegisterAttached(
            "IconTemplate",
            typeof(DataTemplate),
            typeof(MenuAssist),
            new PropertyMetadata(null));

        public static DataTemplate GetIconTemplate(MenuItem item) => (DataTemplate)item.GetValue(IconTemplateProperty);

        public static void SetIconTemplate(MenuItem item, DataTemplate value) => item.SetValue(IconTemplateProperty, value);

        #endregion IconTemplate
    }
}
