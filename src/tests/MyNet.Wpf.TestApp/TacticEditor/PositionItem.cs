// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MyNet.Wpf.TestApp.TacticEditor
{
    internal class PositionItem : ListBoxItem
    {
        public IPositionWrapper? Position { get; internal set; }
    }
}
