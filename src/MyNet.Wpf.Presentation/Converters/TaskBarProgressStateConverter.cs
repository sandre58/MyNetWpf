// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;
using MyNet.UI;

namespace MyNet.Wpf.Presentation.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public class TaskBarProgressStateConverter
        : IValueConverter
    {
        public static TaskBarProgressStateConverter Default { get; } = new();

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (TaskbarProgressState)value;

            return val switch
            {
                TaskbarProgressState.None => TaskbarItemProgressState.None,
                TaskbarProgressState.Indeterminate => TaskbarItemProgressState.Indeterminate,
                TaskbarProgressState.Normal => TaskbarItemProgressState.Normal,
                TaskbarProgressState.Error => TaskbarItemProgressState.Error,
                TaskbarProgressState.Paused => TaskbarItemProgressState.Paused,
                _ => Binding.DoNothing,
            };
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (TaskbarItemProgressState)value;

            return val switch
            {
                TaskbarItemProgressState.None => TaskbarProgressState.None,
                TaskbarItemProgressState.Indeterminate => TaskbarProgressState.Indeterminate,
                TaskbarItemProgressState.Normal => TaskbarProgressState.Normal,
                TaskbarItemProgressState.Error => TaskbarProgressState.Error,
                TaskbarItemProgressState.Paused => TaskbarProgressState.Paused,
                _ => Binding.DoNothing,
            };
        }
    }
}
