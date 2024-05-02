// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyNet.Wpf.Converters
{
    public class MultipleBooleanToVisibilityConverter : IMultiValueConverter
    {
        private enum Operator
        {
            And,
            Or,
        }

        private readonly Operator _operator;

        private readonly Visibility _falseVisibility;

        public static readonly MultipleBooleanToVisibilityConverter AndHidden = new(Operator.And, Visibility.Hidden);

        public static readonly MultipleBooleanToVisibilityConverter AndCollapse = new(Operator.And, Visibility.Collapsed);

        public static readonly MultipleBooleanToVisibilityConverter OrHidden = new(Operator.Or, Visibility.Hidden);

        public static readonly MultipleBooleanToVisibilityConverter OrCollapse = new(Operator.Or, Visibility.Collapsed);

        private MultipleBooleanToVisibilityConverter(Operator @operator, Visibility falseVisibility) => (_operator, _falseVisibility) = (@operator, falseVisibility);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value is not bool) return Visibility.Visible;

                var result = (bool)value;
                if (_operator == Operator.Or && result)
                {
                    return Visibility.Visible;
                }

                if (_operator == Operator.And && !result)
                {
                    return _falseVisibility;
                }
            }
            return (_operator == Operator.And) ? Visibility.Visible : _falseVisibility;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
