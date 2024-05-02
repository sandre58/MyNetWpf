// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MyNet.UI.ViewModels.List.Grouping;

namespace MyNet.Wpf.Presentation.Converters
{
    public class GroupPropertiesToStringConverter
        : IValueConverter
    {
        public static readonly GroupPropertiesToStringConverter Default = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Binding.DoNothing;

            var properties = value is IEnumerable<IGroupingPropertyViewModel> groupProperties ? groupProperties : [];
            var prefix = parameter?.ToString();

            return string.Join(";", properties.Select(x => $"{prefix}{x.PropertyName}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
