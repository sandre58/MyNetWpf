// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNet.Wpf.Parameters
{
    public static class GroupBoxAssist
    {
        #region MoreContent

        public static readonly DependencyProperty MoreContentProperty = DependencyProperty.RegisterAttached(
            "MoreContent",
            typeof(object),
            typeof(GroupBoxAssist),
            new PropertyMetadata(null));

        public static object GetMoreContent(GroupBox item) => item.GetValue(MoreContentProperty);

        public static void SetMoreContent(GroupBox item, object value) => item.SetValue(MoreContentProperty, value);

        #endregion MoreContent

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(GroupBoxAssist),
            new PropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject item) => (ICommand)item.GetValue(CommandProperty);

        public static void SetCommand(DependencyObject item, ICommand value) => item.SetValue(CommandProperty, value);

        #endregion Command

        #region CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(GroupBoxAssist),
            new PropertyMetadata(null));

        public static object GetCommandParameter(DependencyObject item) => item.GetValue(CommandParameterProperty);

        public static void SetCommandParameter(DependencyObject item, object value) => item.SetValue(CommandParameterProperty, value);

        #endregion CommandParameter
    }
}
