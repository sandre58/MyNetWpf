// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Data;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.MarkupExtensions
{
    public class TimeExtension : DateTimeExtension
    {
        public TimeExtension() : base() { }

        public TimeExtension(string path) : base(path) { }

        protected override IValueConverter CreateConverter() => new TimeConverter(Conversion);
    }
}
