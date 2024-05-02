// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using MyNet.Utilities;
using MyNet.Utilities.Helpers;

namespace MyNet.Wpf.MarkupExtensions
{
    public class HourRangeExtension : IntegerRangeExtension
    {
        public HourRangeExtension()
        {
        }

        public override object? ProvideValue(IServiceProvider serviceProvider) => EnumerableHelper.Range(Start, End, Step).Select(x => x.Hours().TimeSpan);
    }
}
