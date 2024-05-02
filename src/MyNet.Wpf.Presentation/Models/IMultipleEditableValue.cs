// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MyNet.Wpf.Presentation.Models
{
    public interface IMultipleEditableValue : INotifyPropertyChanged
    {
        bool IsActive { get; set; }

        bool IsMultiple { get; }

        bool IsEmpty();
    }
}
