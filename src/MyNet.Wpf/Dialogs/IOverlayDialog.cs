// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MyNet.Wpf.Dialogs
{
    public interface IOverlayDialog
    {
        bool CloseOnClickAway { get; }

        bool FocusOnShow { get; }

        VerticalAlignment VerticalAlignment { get; }

        HorizontalAlignment HorizontalAlignment { get; }
    }
}
