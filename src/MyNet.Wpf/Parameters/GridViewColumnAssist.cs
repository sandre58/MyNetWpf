// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Behaviors;

namespace MyNet.Wpf.Parameters
{
    public static class GridViewColumnAssist
    {
        #region ResizeBehavior

        internal static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.RegisterAttached("ResizeBehavior", typeof(GridViewColumnLayout), typeof(GridViewColumnAssist));

        internal static GridViewColumnLayout? GetResizeBehavior(GridViewColumn obj) => (GridViewColumnLayout?)obj.GetValue(ResizeBehaviorProperty);

        internal static void SetResizeBehavior(GridViewColumn obj, GridViewColumnLayout value) => obj.SetValue(ResizeBehaviorProperty, value);

        #endregion

        #region Width

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached("Width", typeof(string), typeof(GridViewColumnAssist), new PropertyMetadata(string.Empty));

        public static string GetWidth(GridViewColumn obj) => (string)obj.GetValue(WidthProperty);

        public static void SetWidth(GridViewColumn obj, string value) => obj.SetValue(WidthProperty, value);

        #endregion

        #region CanBeHidden

        public static readonly DependencyProperty CanBeHiddenProperty = DependencyProperty.RegisterAttached(
            "CanBeHidden",
            typeof(bool),
            typeof(GridViewColumnAssist),
            new PropertyMetadata(true));

        public static bool GetCanBeHidden(GridViewColumn item) => (bool)item.GetValue(CanBeHiddenProperty);

        public static void SetCanBeHidden(GridViewColumn item, bool value) => item.SetValue(CanBeHiddenProperty, value);

        #endregion

        #region PropertyName

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
            "PropertyName",
            typeof(string),
            typeof(GridViewColumnAssist));

        public static string GetPropertyName(GridViewColumn item) => (string)item.GetValue(PropertyNameProperty);

        public static void SetPropertyName(GridViewColumn item, string value) => item.SetValue(PropertyNameProperty, value);

        #endregion

        #region CanSort

        public static readonly DependencyProperty CanSortProperty = DependencyProperty.RegisterAttached(
            "CanSort",
            typeof(bool),
            typeof(GridViewColumnAssist),
            new PropertyMetadata(true));

        public static bool GetCanSort(GridViewColumn item) => (bool)item.GetValue(CanSortProperty);

        public static void SetCanSort(GridViewColumn item, bool value) => item.SetValue(CanSortProperty, value);

        #endregion
    }
}
