// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Utilities.Localization;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace MyNet.Wpf.MarkupExtensions
{
    public abstract class TranslationExtensionBase<T> : MarkupExtension
        where T : BindingBase
    {
        private T? _binding;

        protected TranslationExtensionBase() : base() => ResourceLocator.Initialize();

        protected T Binding
        {
            get
            {
                _binding ??= CreateBinding();

                return _binding;
            }
        }

        protected abstract T CreateBinding();

        public object TargetNullValue { get => Binding.TargetNullValue; set => Binding.TargetNullValue = value; }

        public int Delay { get => Binding.Delay; set => Binding.Delay = value; }

        public object FallbackValue { get => Binding.FallbackValue; set => Binding.FallbackValue = value; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding.ProvideValue(serviceProvider) is BindingExpressionBase expression)
            {
                void handler(object? o, EventArgs e)
                {
                    var wr = new WeakReference<BindingExpressionBase>(expression);
                    if (wr.TryGetTarget(out var target))
                    {
                        if (target is not BindingExpression bindinExpression || bindinExpression.Status != BindingStatus.Detached)
                            target.UpdateTarget();
                    }
                    else
                        CultureInfoService.Current.CultureChanged -= handler;
                }

                CultureInfoService.Current.CultureChanged += handler;

                return expression;
            }

            return this;
        }
    }
}
