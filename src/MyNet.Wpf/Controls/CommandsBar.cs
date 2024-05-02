// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    public class CommandsBar : ContentControl
    {
        static CommandsBar() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandsBar), new FrameworkPropertyMetadata(typeof(CommandsBar)));

        #region LeftCommands

        public static readonly DependencyProperty LeftCommandsProperty
            = DependencyProperty.RegisterAttached(nameof(LeftCommands), typeof(object), typeof(CommandsBar));

        public object LeftCommands
        {
            get => GetValue(LeftCommandsProperty);
            set => SetValue(LeftCommandsProperty, value);
        }

        #endregion

        #region RightCommands

        public static readonly DependencyProperty RightCommandsProperty
            = DependencyProperty.RegisterAttached(nameof(RightCommands), typeof(object), typeof(CommandsBar));

        public object RightCommands
        {
            get => GetValue(RightCommandsProperty);
            set => SetValue(RightCommandsProperty, value);
        }

        #endregion
    }
}
