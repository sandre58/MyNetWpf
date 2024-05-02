// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Parameters
{
    public static class CardAssist
    {
        #region Style

        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
            "Style",
            typeof(object),
            typeof(CardAssist),
            new PropertyMetadata(null));

        public static Style GetStyle(DependencyObject item) => (Style)item.GetValue(StyleProperty);

        public static void SetStyle(DependencyObject item, Style value) => item.SetValue(StyleProperty, value);

        #endregion Style
    }
}
