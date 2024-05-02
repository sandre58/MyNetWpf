// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Busy;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Parameters
{
    public static class BusyAssist
    {
        public static readonly DependencyProperty AttachServiceProperty = DependencyProperty.RegisterAttached(
            "AttachService", typeof(BusyService), typeof(BusyAssist), new PropertyMetadata(OnAttachServiceChanged));

        public static void SetAttachService(Grid target, BusyService value) => target.SetValue(AttachServiceProperty, value);

        public static BusyService GetAttachService(Grid target) => (BusyService)target.GetValue(AttachServiceProperty);

        private static void OnAttachServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid container || e.NewValue is not BusyService service)
            {
                return;
            }

            service.SetContainer(container);

            if (GetStyle(container) is Style style)
            {
                service.BusyView.SetValue(FrameworkElement.StyleProperty, style);
                service.BusyView.SetValue(BusyControl.AssociatedControlProperty, GetAssociatedControl(container) ?? container);
            }
        }

        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
    "Style", typeof(Style), typeof(BusyAssist), new PropertyMetadata(OnStyleChanged));

        public static void SetStyle(Grid target, Style value) => target.SetValue(StyleProperty, value);

        public static Style GetStyle(Grid target) => (Style)target.GetValue(StyleProperty);

        private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid container || e.NewValue is not Style style || GetAttachService(container) is not BusyService service)
            {
                return;
            }

            service.BusyView.SetValue(FrameworkElement.StyleProperty, style);
        }

        #region AssociatedControl

        public static readonly DependencyProperty AssociatedControlProperty = DependencyProperty.RegisterAttached(
    "AssociatedControl", typeof(UIElement), typeof(BusyAssist), new PropertyMetadata(OnAssociatedControlChanged));

        public static void SetAssociatedControl(Grid target, UIElement value) => target.SetValue(AssociatedControlProperty, value);

        public static UIElement GetAssociatedControl(Grid target) => (UIElement)target.GetValue(AssociatedControlProperty);

        private static void OnAssociatedControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid container || e.NewValue is not UIElement associatedControl || GetAttachService(container) is not BusyService service)
            {
                return;
            }

            service.BusyView.SetValue(BusyControl.AssociatedControlProperty, associatedControl);
        }

        #endregion
    }
}
