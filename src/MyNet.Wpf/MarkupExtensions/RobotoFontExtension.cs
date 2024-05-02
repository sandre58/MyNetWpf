// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;
using System.Windows.Media;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(FontFamily))]
    public class RobotoFontExtension : MarkupExtension
    {
        private static readonly Lazy<FontFamily> Roboto = new(() => WpfHelper.GetResource<FontFamily>("MyNet.Font.Family.Roboto") ?? new FontFamily());

        public override object ProvideValue(IServiceProvider serviceProvider) => Roboto.Value;
    }
}
