// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Controls;
using MyNet.Wpf.Controls.HintProxy;

namespace MyNet.Wpf.Presentation.Controls.HintProxy;

public class MultipleTimePickerHintProxy : TimePickerHintProxy
{
    public override bool IsEmpty() => false;

    public MultipleTimePickerHintProxy(TimePicker timePicker) : base(timePicker) { }
}
