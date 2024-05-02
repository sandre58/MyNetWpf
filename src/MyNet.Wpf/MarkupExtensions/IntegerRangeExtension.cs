// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;
using MyNet.Utilities.Helpers;

namespace MyNet.Wpf.MarkupExtensions
{
    public class IntegerRangeExtension : MarkupExtension
    {
        public IntegerRangeExtension()
        {
        }

        public int Start { get; set; } = 0;

        public int End { get; set; } = 10;

        public int Step { get; set; } = 1;

        public override object? ProvideValue(IServiceProvider serviceProvider) => EnumerableHelper.Range(Start, End, Step);
    }
}
