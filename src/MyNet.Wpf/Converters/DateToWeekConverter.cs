// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Utilities;

namespace MyNet.Wpf.Converters
{
    public sealed class DateToWeekConverter : IValueConverter
    {
        private enum Mode
        {
            StartWeek,
            EndWeek
        }
        private readonly Mode _type;


        public static readonly DateToWeekConverter ToStartWeek = new(Mode.StartWeek);

        public static readonly DateToWeekConverter ToEndWeek = new(Mode.EndWeek);

        private DateToWeekConverter(Mode type) => _type = type;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is not DateTime date ? Binding.DoNothing : _type == Mode.StartWeek ? date.BeginningOfWeek() : date.EndOfWeek();

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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotSupportedException();
    }
}
