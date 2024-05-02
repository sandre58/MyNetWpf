// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace MyNet.Wpf.DragAndDrop
{
    public class FileDropHandler(Action<IDropInfo, StringCollection> dropAction) : IDropTarget
    {
        private readonly Action<IDropInfo, StringCollection> _dropAction = dropAction;

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            dropInfo.Effects = dropInfo.Data is IDataObject dataObject && dataObject.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DataObject dataObject && dataObject.ContainsFileDropList())
            {
                _dropAction.Invoke(dropInfo, dataObject.GetFileDropList());
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }
    }
}
