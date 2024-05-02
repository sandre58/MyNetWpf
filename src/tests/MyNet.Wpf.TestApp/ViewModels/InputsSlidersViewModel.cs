// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Observable;
using MyNet.UI.ViewModels.Workspace;

namespace MyNet.Wpf.TestApp.ViewModels
{
    internal class InputsSlidersViewModel : NavigableWorkspaceViewModel
    {
        public SliderViewModel DiscreteHorizontal { get; } = new();
        public SliderViewModel DiscreteVertical { get; } = new() { Maximum = 100000, TickFrequency = 10000, Value = 50000 };
    }

    internal class SliderViewModel : ObservableObject
    {
        public double Minimum { get; set; }

        public double Maximum { get; set; } = 100.0;

        public double TickFrequency { get; set; } = 10.0;

        public double Value { get; set; } = 50.0;
    }
}
