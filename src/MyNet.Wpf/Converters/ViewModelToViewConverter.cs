// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MyNet.UI.Locators;

namespace MyNet.Wpf.Converters
{
    public class ViewModelToViewConverter : IValueConverter
    {
        public static readonly ViewModelToViewConverter Default = new();

        #region Public Methods and Operators

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var view = ViewManager.GetView(value.GetType());

                if (view is FrameworkElement element)
                    element.DataContext = value;

                return view;
            }

            return value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        #endregion Public Methods and Operators
    }
}
