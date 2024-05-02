// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    /// <summary>
    /// Converts a null value to Visibility.Visible and any other value to Visibility.Collapsed
    /// </summary>
    public partial class HtmlToPlainTextConverter
        : IValueConverter
    {
        public static readonly HtmlToPlainTextConverter Default = new();

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
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) => HtmlToPlainText(value?.ToString());

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
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();

        private static string HtmlToPlainText(string? html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);

            //Remove tag whitespace/line breaks
            text = TagWhiteRegex().Replace(text, "><");
            //Replace <br /> with line breaks
            text = LineRegex().Replace(text, Environment.NewLine);
            //Strip formatting
            text = StripFormattingRegex().Replace(text, string.Empty);
            return text;
        }

        [GeneratedRegex("UP", RegexOptions.IgnoreCase, "fr-FR")]
        private static partial Regex TagWhiteRegex();

        [GeneratedRegex("UP", RegexOptions.IgnoreCase, "fr-FR")]
        private static partial Regex StripFormattingRegex();

        [GeneratedRegex("UP", RegexOptions.IgnoreCase, "fr-FR")]
        private static partial Regex LineRegex();
    }
}
