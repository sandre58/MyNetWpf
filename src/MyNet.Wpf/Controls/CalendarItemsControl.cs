// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    public class CalendarItemsControl : ItemsControl
    {
        static CalendarItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarItemsControl), new FrameworkPropertyMetadata(typeof(CalendarItemsControl)));
            FocusableProperty.OverrideMetadata(typeof(CalendarItemsControl), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CalendarItemsControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CalendarItemsControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
        }

        #region ColumnsCount

        internal static readonly DependencyPropertyKey ColumnsCountPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(ColumnsCount),
                typeof(int),
                typeof(CalendarItemsControl),
                new FrameworkPropertyMetadata(1));

        public static readonly DependencyProperty ColumnsCountProperty = ColumnsCountPropertyKey.DependencyProperty;

        public int ColumnsCount => (int)GetValue(ColumnsCountProperty);

        #endregion ColumnsCount

        #region RowsCount

        internal static readonly DependencyPropertyKey RowsCountPropertyKey = DependencyProperty.RegisterReadOnly(
                nameof(RowsCount),
                typeof(int),
                typeof(CalendarItemsControl),
                new FrameworkPropertyMetadata(1));

        public static readonly DependencyProperty RowsCountProperty = RowsCountPropertyKey.DependencyProperty;

        public int RowsCount => (int)GetValue(RowsCountProperty);

        #endregion RowsCount

        protected override bool IsItemItsOwnContainerOverride(object item) => item is CalendarItem;
    }
}
