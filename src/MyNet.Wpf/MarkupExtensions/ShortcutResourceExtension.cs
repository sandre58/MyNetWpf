// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using System.Windows.Markup;
using MyNet.Humanizer;
using MyNet.Observable.Attributes;
using MyNet.Observable.Translatables;
using MyNet.Utilities.Extensions;

namespace MyNet.Wpf.MarkupExtensions
{
    public class ShortcutResourceExtension : ResourceExtension
    {
        public ShortcutResourceExtension()
        { }

        public ShortcutResourceExtension(string key) : base(key) { }

        public ShortcutResourceExtension(string key, string shortcutKey) : base(key) => ShortcutKey = shortcutKey;

        [ConstructorArgument("shortcutKey")]
        public string ShortcutKey { get; set; } = string.Empty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(nameof(TranslatableString.Value))
            {
                Source = new ShortcutTranslatableString(Key, ShortcutKey, Casing)
            };

            return binding.ProvideValue(serviceProvider);
        }
    }

    internal class ShortcutTranslatableString(string key, string shortcutKey, LetterCasing casing = LetterCasing.Normal) : TranslatableString(key, casing)
    {
        public string ShortcutKey { get; set; } = shortcutKey;

        [UpdateOnCultureChanged]
        public override string? Value => $"{base.Value} ({ShortcutKey.Translate()})";
    }

}
