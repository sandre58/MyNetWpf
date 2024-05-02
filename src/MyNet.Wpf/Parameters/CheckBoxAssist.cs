// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class CheckBoxAssist
    {
        #region IndeterminatePath

        public static readonly DependencyProperty IndeterminatePathProperty = DependencyProperty.RegisterAttached(
            "IndeterminatePath",
            typeof(Geometry),
            typeof(CheckBoxAssist),
            new PropertyMetadata(null));

        public static Geometry GetIndeterminatePath(DependencyObject item) => (Geometry)item.GetValue(IndeterminatePathProperty);

        public static void SetIndeterminatePath(DependencyObject item, Geometry value) => item.SetValue(IndeterminatePathProperty, value);

        #endregion

        #region UncheckedPath

        public static readonly DependencyProperty UncheckedPathProperty = DependencyProperty.RegisterAttached(
            "UncheckedPath",
            typeof(Geometry),
            typeof(CheckBoxAssist),
            new PropertyMetadata(null));

        public static Geometry GetUncheckedPath(DependencyObject item) => (Geometry)item.GetValue(UncheckedPathProperty);

        public static void SetUncheckedPath(DependencyObject item, Geometry value) => item.SetValue(UncheckedPathProperty, value);

        #endregion

        #region CheckedPath

        public static readonly DependencyProperty CheckedPathProperty = DependencyProperty.RegisterAttached(
            "CheckedPath",
            typeof(Geometry),
            typeof(CheckBoxAssist),
            new PropertyMetadata(null));

        public static Geometry GetCheckedPath(DependencyObject item) => (Geometry)item.GetValue(CheckedPathProperty);

        public static void SetCheckedPath(DependencyObject item, Geometry value) => item.SetValue(CheckedPathProperty, value);

        #endregion

        #region ShowCheck

        public static readonly DependencyProperty ShowCheckProperty = DependencyProperty.RegisterAttached(
            "ShowCheck",
            typeof(bool),
            typeof(CheckBoxAssist),
            new PropertyMetadata(true));

        public static bool GetShowCheck(DependencyObject item) => (bool)item.GetValue(ShowCheckProperty);

        public static void SetShowCheck(DependencyObject item, bool value) => item.SetValue(ShowCheckProperty, value);

        #endregion
    }
}
