// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using System.Windows.Markup;
using MyNet.Utilities.Localization;

namespace MyNet.Wpf.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class GlobalizationExtensionBase<T> : MarkupExtension
        where T : BindingBase
    {
        private T? _binding;

        protected GlobalizationExtensionBase(bool updateOnCultureChanged, bool updateOnTimeZoneChanged) : base()
        {
            ResourceLocator.Initialize();
            UpdateOnCultureChanged = updateOnCultureChanged;
            UpdateOnTimeZoneChanged = updateOnTimeZoneChanged;
        }

        protected EventHandler? UpdateTargetHandler { get; private set; }

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

        public bool UpdateOnCultureChanged { get; set; }

        public bool UpdateOnTimeZoneChanged { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding.ProvideValue(serviceProvider) is BindingExpressionBase expression)
            {
                void handler(object? o, EventArgs e)
                {
                    var wr = new WeakReference<BindingExpressionBase>(expression);
                    if (wr.TryGetTarget(out var target))
                    {
                        if (expression is not BindingExpression bindinExpression || bindinExpression.Status != BindingStatus.Detached)
                            expression.UpdateTarget();
                    }
                    else
                    {
                        GlobalizationService.Current.CultureChanged -= UpdateTargetHandler;
                        GlobalizationService.Current.TimeZoneChanged -= UpdateTargetHandler;
                    }
                };
                UpdateTargetHandler = new(handler);

                if (UpdateOnCultureChanged)
                    GlobalizationService.Current.CultureChanged += UpdateTargetHandler;

                if (UpdateOnTimeZoneChanged)
                    GlobalizationService.Current.TimeZoneChanged += UpdateTargetHandler;

                return expression;
            }

            return this;
        }
    }
}
