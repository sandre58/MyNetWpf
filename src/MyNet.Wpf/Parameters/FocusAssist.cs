// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    /// <summary>
    /// Set focus on attached element when target element is loaded
    /// </summary>
    public static class FocusAssist
    {
        #region IsDefault

        public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.RegisterAttached(
              "IsDefault", typeof(bool), typeof(FocusAssist), new PropertyMetadata(OnIsDefaultChanged));

        public static void SetIsDefault(FrameworkElement target, bool value) => target.SetValue(IsDefaultProperty, value);

        public static bool GetIsDefault(FrameworkElement target) => (bool)target.GetValue(IsDefaultProperty);

        private static void OnIsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null || d is not FrameworkElement control || !(bool)e.NewValue)
            {
                return;
            }

            SetFocusOrAddEvents(control);
        }

        #endregion

        #region SetFocus

        public static readonly DependencyProperty SetFocusProperty = DependencyProperty.RegisterAttached(
      "SetFocus", typeof(bool), typeof(FocusAssist), new PropertyMetadata(false, OnSetFocusChanged));

        public static void SetSetFocus(FrameworkElement target, bool value) => target.SetValue(SetFocusProperty, value);

        public static bool GetSetFocus(FrameworkElement target) => (bool)target.GetValue(SetFocusProperty);

        private static void OnSetFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null || d is not FrameworkElement control)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                FocusManager.SetIsFocusScope(control, true);
                var focusedElement = FindDefaultFocusElement(control);
                if (focusedElement != null)
                    SetFocusOrAddEvents(focusedElement);
            }
        }

        #endregion

        private static void SetFocusOrAddEvents(FrameworkElement control)
        {
            if (control.IsVisible && control.IsEnabled && control.IsLoaded)
                SetFocus(control);
            else
                AddFocusEvents(control);
        }

        private static void AddFocusEvents(FrameworkElement control)
        {
            control.Loaded -= Control_Loaded;
            control.IsVisibleChanged -= Control_IsVisibleChanged;
            control.IsEnabledChanged -= Control_IsEnabledChanged;

            control.Loaded += Control_Loaded;
            control.IsVisibleChanged += Control_IsVisibleChanged;
            control.IsEnabledChanged += Control_IsEnabledChanged;
        }

        private static void Control_Loaded(object sender, RoutedEventArgs e) => SetFocus((FrameworkElement)sender);

        private static void Control_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) => SetFocus((FrameworkElement)sender);

        private static void Control_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) => SetFocus((FrameworkElement)sender);

        private static void SetFocus(FrameworkElement control)
        {
            if (!control.IsVisible || !control.IsEnabled || !control.IsLoaded) return;

            _ = control.Focus();
            _ = Keyboard.Focus(control);

            if (control is TextBox textbox)
            {
                textbox.SelectAll();
                textbox.TextChanged += Textbox_TextChanged;
            }
        }

        private static void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                if (e.Changes.Any(x => x.AddedLength > 1))
                {
                    textbox.SelectAll();
                }

                textbox.TextChanged -= Textbox_TextChanged;
            }
        }

        private static FrameworkElement? FindDefaultFocusElement(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return null;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                var result = (child is FrameworkElement foundChild && GetIsDefault(foundChild)) ? foundChild : FindDefaultFocusElement(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;

        }
    }
}
