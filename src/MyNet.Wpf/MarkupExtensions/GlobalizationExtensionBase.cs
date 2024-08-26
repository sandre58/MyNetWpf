// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MyNet.Utilities.Localization;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MyNet.Wpf.MarkupExtensions
{
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class GlobalizationExtensionBase<T> : MarkupExtension
        where T : BindingBase
    {
        private T? _binding;
        private readonly bool _updateOnCultureChanged;
        private readonly bool _updateOnTimeZoneChanged;
        private Func<bool>? _updateTarget;

        protected GlobalizationExtensionBase(bool updateOnCultureChanged, bool updateOnTimeZoneChanged) : base()
        {
            ResourceLocator.Initialize();
            _updateOnCultureChanged = updateOnCultureChanged;
            _updateOnTimeZoneChanged = updateOnTimeZoneChanged;
        }

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

        protected virtual bool UpdateTarget() => _updateTarget?.Invoke() ?? false;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding.ProvideValue(serviceProvider) is BindingExpressionBase expression)
            {
                _updateTarget = () =>
                {
                    var wr = new WeakReference<BindingExpressionBase>(expression);
                    if (wr.TryGetTarget(out var target))
                    {
                        if (target is not BindingExpression bindinExpression || bindinExpression.Status != BindingStatus.Detached)
                        {
                            target.UpdateTarget();
                            return true;
                        }
                    }

                    return false;
                };
                void handler(object? o, EventArgs e)
                {
                    if (!UpdateTarget())
                    {
                        GlobalizationService.Current.CultureChanged -= handler;
                        GlobalizationService.Current.TimeZoneChanged -= handler;
                    }
                }

                if (_updateOnCultureChanged)
                    GlobalizationService.Current.CultureChanged += handler;

                if (_updateOnTimeZoneChanged)
                    GlobalizationService.Current.TimeZoneChanged += handler;

                return expression;
            }

            return this;
        }
    }
}
