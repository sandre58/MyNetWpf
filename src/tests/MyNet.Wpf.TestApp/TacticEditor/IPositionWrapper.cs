// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.TestApp.TacticEditor
{
    public interface IPositionWrapper
    {
        Position Position { get; }

        double OffsetX { get; set; }

        double OffsetY { get; set; }
    }
}
