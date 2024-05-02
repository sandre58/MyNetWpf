// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;

namespace MyNet.Wpf.Toasting.Lifetime
{
    public class ShowToastEventArgs(Toast toast) : EventArgs
    {
        public Toast Toast { get; } = toast;
    }
}
