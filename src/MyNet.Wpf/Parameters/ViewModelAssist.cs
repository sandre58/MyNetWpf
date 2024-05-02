// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using MyNet.UI.Locators;

namespace MyNet.Wpf.Parameters
{
    public static class ViewModelAssist
    {
        public static readonly DependencyProperty AutoWireProperty = DependencyProperty.RegisterAttached(
            "AutoWire",
            typeof(bool),
            typeof(ViewModelAssist),
            new PropertyMetadata(false, AutoWireChanged));

        public static bool GetAutoWire(DependencyObject item) => (bool)item.GetValue(AutoWireProperty);

        public static void SetAutoWire(DependencyObject item, bool value) => item.SetValue(AutoWireProperty, value);

        private static void AutoWireChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d) && (bool)e.NewValue)
            {
                var viewModel = ViewModelManager.GetViewModel(d.GetType());

                Bind(d, viewModel);
            }
        }

        /// <summary>
        /// Sets the DataContext of a View
        /// </summary>
        /// <param name="view">The View to set the DataContext on</param>
        /// <param name="viewModel">The object to use as the DataContext for the View</param>
        private static void Bind(object view, object? viewModel)
        {
            if (view is FrameworkElement element)
            {
                element.DataContext = viewModel;
            }
        }
    }
}
