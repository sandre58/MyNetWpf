// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using MyNet.Wpf.Converters;
using MyNet.Humanizer;

namespace MyNet.Wpf.MarkupExtensions
{
    public class UnitTranslationExtension : AbstractTranslationExtension
    {
        public UnitTranslationExtension() : base() { }

        public UnitTranslationExtension(string path) : this() => Path = path;

        protected override Binding CreateBinding() => new();

        public string? Format { get => Binding.ConverterParameter?.ToString(); set => Binding.ConverterParameter = value; }

        public bool Abbreviation { get; set; } = true;

        public Enum? MinUnit { get; set; }

        public Enum? MaxUnit { get; set; }

        public LetterCasing Casing { get; set; } = LetterCasing.Sentence;

        public bool Simplify { get; set; }

        protected override IValueConverter CreateConverter() => new UnitConverter
        {
            Casing = Casing,
            MaxUnit = MaxUnit,
            MinUnit = MinUnit,
            Abbreviation = Abbreviation,
            Simplify = Simplify
        };
    }
}
