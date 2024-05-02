// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MyNet.Wpf.Dialogs;

namespace MyNet.Wpf.Parameters
{
    public static class DialogAssist
    {
        public static readonly DependencyProperty AttachServiceProperty = DependencyProperty.RegisterAttached(
            "AttachService", typeof(OverlayDialogService), typeof(DialogAssist), new PropertyMetadata(OnAttachServiceChanged));

        public static void SetAttachService(Grid target, OverlayDialogService value) => target.SetValue(AttachServiceProperty, value);

        public static OverlayDialogService GetAttachService(Grid target) => (OverlayDialogService)target.GetValue(AttachServiceProperty);

        private static void OnAttachServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid container || e.NewValue is not OverlayDialogService service)
            {
                return;
            }

            service.SetContainer(container);
            service.SetAssociatedControl(GetAssociatedControl(container));
        }

        #region AssociatedControl

        public static readonly DependencyProperty AssociatedControlProperty = DependencyProperty.RegisterAttached(
            "AssociatedControl", typeof(UIElement), typeof(DialogAssist), new PropertyMetadata(OnAssociatedControlChanged));

        public static void SetAssociatedControl(Grid target, UIElement value) => target.SetValue(AssociatedControlProperty, value);

        public static UIElement GetAssociatedControl(Grid target) => (UIElement)target.GetValue(AssociatedControlProperty);

        private static void OnAssociatedControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Grid container || e.NewValue is not UIElement associatedControl || GetAttachService(container) is not OverlayDialogService service)
            {
                return;
            }

            service.SetAssociatedControl(associatedControl);
        }

        #endregion
    }
}
