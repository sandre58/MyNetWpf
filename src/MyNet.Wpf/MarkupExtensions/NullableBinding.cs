// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Converters;
using System.Windows.Data;

namespace MyNet.Wpf.MarkupExtensions
{
    public class NullableBinding : Binding
    {
        public NullableBinding(string path) : base(path) => Converter = NullableConverter.Default;

        public NullableBinding() => Converter = NullableConverter.Default;
    }
}
