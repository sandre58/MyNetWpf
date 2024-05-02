// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Parameters
{
    public static class TransitionAssist
    {
        public static readonly DependencyProperty DisableTransitionsProperty = DependencyProperty.RegisterAttached(
            "DisableTransitions", typeof(bool), typeof(TransitionAssist), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetDisableTransitions(DependencyObject element, bool value) => element.SetValue(DisableTransitionsProperty, value);

        public static bool GetDisableTransitions(DependencyObject element) => (bool)element.GetValue(DisableTransitionsProperty);
    }
}
