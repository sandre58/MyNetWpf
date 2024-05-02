// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Converters;
using System.Windows.Data;

namespace MyNet.Wpf.MarkupExtensions
{
    public class NotNullableBinding : Binding
    {
        public NotNullableBinding(string path) : base(path) => Converter = NotNullableConverter.Default;

        public NotNullableBinding() => Converter = NotNullableConverter.Default;
    }
}
