// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using MyNet.Humanizer;
using MyNet.Observable;

namespace MyNet.Wpf.Converters
{
    public class UnitConverter : IValueConverter
    {
        public bool Abbreviation { get; set; } = true;

        public LetterCasing Casing { get; set; } = LetterCasing.Sentence;

        public bool Simplify { get; set; }

        public Enum? MinUnit { get; set; }

        public Enum? MaxUnit { get; set; }

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
            => value is IDisplayValueWithUnit val
                ? Simplify ? val.SimplifyToString(MinUnit, MaxUnit, Abbreviation, parameter?.ToString())?.ApplyCase(Casing) : val.ToString(Abbreviation, parameter?.ToString())?.ApplyCase(Casing)
                : (value?.ToString()?.ApplyCase(Casing));

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
        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
