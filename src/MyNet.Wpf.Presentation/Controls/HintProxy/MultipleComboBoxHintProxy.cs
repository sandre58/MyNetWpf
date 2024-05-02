// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MyNet.Wpf.Controls.HintProxy;

namespace MyNet.Wpf.Presentation.Controls.HintProxy;

public class MultipleComboBoxHintProxy : ComboBoxHintProxy
{
    public override bool IsEmpty() => false;

    public MultipleComboBoxHintProxy(ComboBox comboBox) : base(comboBox) { }
}
