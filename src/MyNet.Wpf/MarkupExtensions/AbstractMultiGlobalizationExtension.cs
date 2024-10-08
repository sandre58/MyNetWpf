﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;

namespace MyNet.Wpf.MarkupExtensions
{
    public abstract class AbstractMultiGlobalizationExtension : GlobalizationExtensionBase<MultiBinding>
    {
        protected AbstractMultiGlobalizationExtension(bool updateOnCultureChanged, bool updateOnTimeZoneChanged)
            : base(updateOnCultureChanged, updateOnTimeZoneChanged) { }

        protected override MultiBinding CreateBinding() => new();

        public object? ConverterParameter { get => Binding.ConverterParameter; set => Binding.ConverterParameter = value; }

        protected abstract IMultiValueConverter CreateConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding.Converter = CreateConverter();

            return base.ProvideValue(serviceProvider);
        }

    }
}
