// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls.HintProxy;

public static partial class HintProxyFabric
{
    private sealed class HintProxyBuilder(Func<Control?, bool> canBuild, Func<Control, IHintProxy> build)
    {
        private readonly Func<Control?, bool> _canBuild = canBuild ?? throw new ArgumentNullException(nameof(canBuild));
        private readonly Func<Control, IHintProxy> _build = build ?? throw new ArgumentNullException(nameof(build));

        public bool CanBuild(Control? control) => _canBuild(control);
        public IHintProxy? Build(Control control) => _build(control);
    }

    private static readonly List<HintProxyBuilder> Builders = [];

    static HintProxyFabric()
    {
        Builders.Add(new HintProxyBuilder(c => c is ComboBox, c => new ComboBoxHintProxy((ComboBox)c)));
        Builders.Add(new HintProxyBuilder(c => c is TextBox, c => new TextBoxHintProxy((TextBox)c)));
        Builders.Add(new HintProxyBuilder(c => c is RichTextBox, c => new RichTextBoxHintProxy((RichTextBox)c)));
        Builders.Add(new HintProxyBuilder(c => c is PasswordBox, c => new PasswordBoxHintProxy((PasswordBox)c)));
        Builders.Add(new HintProxyBuilder(c => c is MultiComboBox, c => new MultiComboBoxHintProxy((MultiComboBox)c)));
        Builders.Add(new HintProxyBuilder(c => c is NumericUpDown, c => new NumericUpDownHintProxy((NumericUpDown)c)));
        Builders.Add(new HintProxyBuilder(c => c is DatePicker, c => new DatePickerHintProxy((DatePicker)c)));
        Builders.Add(new HintProxyBuilder(c => c is TimePicker, c => new TimePickerHintProxy((TimePicker)c)));
        Builders.Add(new HintProxyBuilder(c => c is MonthPicker, c => new MonthPickerHintProxy((MonthPicker)c)));
        Builders.Add(new HintProxyBuilder(c => c is ColorPicker, c => new ColorPickerHintProxy((ColorPicker)c)));
        Builders.Add(new HintProxyBuilder(c => c is ImagePicker, c => new ImagePickerHintProxy((ImagePicker)c)));
        Builders.Add(new HintProxyBuilder(c => c is AutoSuggestBox, c => new AutoSuggestBoxHintProxy((AutoSuggestBox)c)));
    }

    public static void RegisterBuilder(Func<Control?, bool> canBuild, Func<Control, IHintProxy> build)
        => Builders.Add(new HintProxyBuilder(canBuild, build));

    public static IHintProxy? Get(Control? control)
    {
        if (control is null) return null;
        var builder = Builders.LastOrDefault(v => v.CanBuild(control));
        return builder?.Build(control);
    }
}
