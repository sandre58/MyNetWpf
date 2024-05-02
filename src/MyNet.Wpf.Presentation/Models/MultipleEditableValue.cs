// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MyNet.Observable;
using MyNet.Observable.Attributes;

namespace MyNet.Wpf.Presentation.Models
{
    public class MultipleEditableValue<T> : EditableObject, IMultipleEditableValue
    {
        public T? Value { get; set; }

        public bool IsActive { get; set; }

        [CanSetIsModified(false)]
        public bool IsMultiple { get; private set; }

        public void Reset(IEnumerable<T?> values)
        {
            var distinctValues = values.Distinct().ToList();
            IsMultiple = distinctValues.Count > 1;
            Value = distinctValues.Count == 1 ? distinctValues[0] : default;
            IsActive = false;
        }

        public void Reset(T? value)
        {
            IsMultiple = false;
            Value = value;
            IsActive = false;
        }

        public T? GetActiveValue() => IsActive ? Value : default;

        public bool IsEmpty() => Value is null || string.IsNullOrEmpty(Value.ToString()) || Value is TimeSpan ts && ts == TimeSpan.MinValue;
    }
}
