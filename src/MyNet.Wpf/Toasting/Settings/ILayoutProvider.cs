// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MyNet.Wpf.Toasting.Settings
{
    public interface ILayoutProvider : IDisposable
    {
        bool TopMost { get; }

        double Width { get; }

        Window? ParentWindow { get; }

        Point GetPosition(double actualPopupWidth, double actualPopupHeight);

        double GetHeight();

        EjectDirection EjectDirection { get; }

        event EventHandler? UpdatePositionRequested;
        event EventHandler? UpdateEjectDirectionRequested;
        event EventHandler? UpdateHeightRequested;
    }
}
