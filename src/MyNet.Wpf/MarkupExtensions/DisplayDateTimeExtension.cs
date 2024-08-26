// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using MyNet.Humanizer;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.MarkupExtensions
{
    public class DisplayDateTimeExtension : AbstractGlobalizationExtension
    {
        public DisplayDateTimeExtension() : base(true, true) { }

        public DisplayDateTimeExtension(string path) : this() => Path = new PropertyPath(path);

        protected override Binding CreateBinding() => new()
        {
            Mode = BindingMode.OneWay,
        };

        public string? Format { get => Binding.ConverterParameter?.ToString(); set => Binding.ConverterParameter = value; }

        public LetterCasing Casing { get; set; } = LetterCasing.Normal;

        public DateTimeToStringConverterKind Kind { get; set; } = DateTimeToStringConverterKind.Default;

        protected override IValueConverter CreateConverter() => new DateTimeToStringConverter(Kind, Casing);
    }
}
