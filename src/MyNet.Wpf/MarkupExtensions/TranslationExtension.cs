// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows.Data;
using MyNet.Wpf.Converters;
using MyNet.Humanizer;

namespace MyNet.Wpf.MarkupExtensions
{
    public class TranslationExtension : AbstractTranslationExtension
    {
        public TranslationExtension() : base() { }

        public TranslationExtension(string path) : this() => Path = path;

        protected override Binding CreateBinding() => new();

        public string? Format { get => Binding.ConverterParameter?.ToString(); set => Binding.ConverterParameter = value; }

        public bool Plural { get; set; }

        public bool Abbreviation { get; set; }

        public bool Initials { get; set; }

        public LetterCasing Casing { get; set; } = LetterCasing.Normal;

        protected override IValueConverter CreateConverter() => new StringConverter(Casing, Plural, Abbreviation, Initials);
    }
}
