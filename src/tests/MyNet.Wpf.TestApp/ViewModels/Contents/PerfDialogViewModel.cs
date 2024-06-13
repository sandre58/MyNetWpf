// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using MyNet.UI.ViewModels.Dialogs;
using MyNet.Utilities.Helpers;

namespace MyNet.Wpf.TestApp.ViewModels.Contents
{
    internal class PerfDialogViewModel : DialogViewModel
    {
        public ObservableCollection<int>? List { get; } = new(EnumerableHelper.Range(1, 1000, 1));
    }
}
