// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using MyNet.Humanizer;
using MyNet.Observable.Translatables;

namespace MyNet.Wpf.Extensions
{
    internal static class WpfExtensions
    {
        public static void SetTranslatableResourceBinding(this FrameworkElement element, DependencyProperty dp, string key, LetterCasing characterCasing = LetterCasing.Normal)
            => element.SetBinding(dp, new Binding(nameof(TranslatableString.Value))
            {
                Source = new TranslatableString(key, characterCasing)
            });

        public static void AddAdorner<TAdorner>(this UIElement element, TAdorner adorner) where TAdorner : Adorner
        {
            ArgumentNullException.ThrowIfNull(adorner);

            var adornerLayer = AdornerLayer.GetAdornerLayer(element)
                ?? throw new InvalidOperationException("This element has no adorner layer.");

            adornerLayer.Add(adorner);
        }

        public static void RemoveAdorners<TAdorner>(this UIElement element) where TAdorner : Adorner
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer?.GetAdorners(element);

            if (adorners is null)
            {
                return;
            }

            foreach (var adorner in adorners.OfType<TAdorner>())
            {
                adornerLayer!.Remove(adorner);
            }
        }
    }
}
