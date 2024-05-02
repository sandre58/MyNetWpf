// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Wpf.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MyNet.Wpf.MarkupExtensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MarkupExtensionReturnType(typeof(ContextMenu))]
    public class TextContextMenuExtension : MarkupExtension
    {
        public override object? ProvideValue(IServiceProvider serviceProvider) => DefaultContextMenu.Value;

        private static readonly ThreadLocal<TextContextMenu> DefaultContextMenu = new(() => new TextContextMenu());
    }
}
