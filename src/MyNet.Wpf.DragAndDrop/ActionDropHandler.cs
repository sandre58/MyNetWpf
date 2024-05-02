// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using GongSolutions.Wpf.DragDrop;

namespace MyNet.Wpf.DragAndDrop
{
    public class ActionDropHandler(Action<IDropInfo> onDrop) : DefaultDropHandler
    {
        private readonly Action<IDropInfo> _onDrop = onDrop;

        public override void Drop(IDropInfo dropInfo)
        {
            base.Drop(dropInfo);

            _onDrop.Invoke(dropInfo);
        }
    }
}
