// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.Controls.VisualStates
{
    internal class IndicatorVisualStateNames
    {
        private static IndicatorVisualStateNames? _activeState;
        private static IndicatorVisualStateNames? _inactiveState;

        public static IndicatorVisualStateNames ActiveState =>
_activeState ??= new IndicatorVisualStateNames("Active");

        public static IndicatorVisualStateNames InactiveState =>
_inactiveState ??= new IndicatorVisualStateNames("Inactive");

        private IndicatorVisualStateNames(string name) => Name = name;

        public string Name { get; }
    }
}
