// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.Controls.VisualStates
{
    internal class IndicatorVisualStateGroupNames
    {
        private static IndicatorVisualStateGroupNames? _internalActiveStates;
        private static IndicatorVisualStateGroupNames? _sizeStates;

        public static IndicatorVisualStateGroupNames ActiveStates =>
_internalActiveStates ??= new IndicatorVisualStateGroupNames(nameof(ActiveStates));

        public static IndicatorVisualStateGroupNames SizeStates =>
_sizeStates ??= new IndicatorVisualStateGroupNames(nameof(SizeStates));

        private IndicatorVisualStateGroupNames(string name) => Name = name;

        public string Name { get; }
    }
}
