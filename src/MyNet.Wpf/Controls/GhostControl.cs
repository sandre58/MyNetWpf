// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    public class GhostControl : ContentControl
    {
        static GhostControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(GhostControl), new FrameworkPropertyMetadata(typeof(GhostControl)));

        #region ShowGhost

        public static readonly DependencyProperty ShowGhostProperty =
            DependencyProperty.Register(
                nameof(ShowGhost),
                typeof(bool),
                typeof(GhostControl));

        public bool ShowGhost
        {
            get => (bool)GetValue(ShowGhostProperty);
            set => SetValue(ShowGhostProperty, value);
        }

        #endregion Ghost

        #region Ghost

        public static readonly DependencyProperty GhostProperty =
            DependencyProperty.Register(
                nameof(Ghost),
                typeof(object),
                typeof(GhostControl));

        public object Ghost
        {
            get => GetValue(GhostProperty);
            set => SetValue(GhostProperty, value);
        }

        #endregion Ghost

        #region GhostTemplate

        public static readonly DependencyProperty GhostTemplateProperty =
            DependencyProperty.Register(
                nameof(GhostTemplate),
                typeof(DataTemplate),
                typeof(GhostControl));

        public DataTemplate GhostTemplate
        {
            get => (DataTemplate)GetValue(GhostTemplateProperty);
            set => SetValue(GhostTemplateProperty, value);
        }

        #endregion Ghost
    }
}
