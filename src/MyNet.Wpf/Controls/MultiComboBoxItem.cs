// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    [Localizability(LocalizationCategory.ComboBox)]
    public class MultiComboBoxItem : ListBoxItem
    {
        static MultiComboBoxItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiComboBoxItem), new FrameworkPropertyMetadata(typeof(MultiComboBoxItem)));
    }
}
