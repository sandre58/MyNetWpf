// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MyNet.Wpf.Controls.HintProxy;
using MyNet.Wpf.Controls;

namespace MyNet.Wpf.Presentation.Controls.HintProxy;

public class MultipleNumericUpDownHintProxy : NumericUpDownHintProxy
{
    public override bool IsEmpty() => false;

    public MultipleNumericUpDownHintProxy(NumericUpDown numericUpDown) : base(numericUpDown) { }
}
