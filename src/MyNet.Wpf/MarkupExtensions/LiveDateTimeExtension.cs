﻿// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using System.Windows.Threading;
using MyNet.Humanizer;
using MyNet.Utilities;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.MarkupExtensions
{
    public enum LiveDateTimeKind
    {
        Current,

        Local,

        Utc
    }

    public class LiveDateTimeExtension : GlobalizationExtensionBase<Binding>
    {
        public virtual DateTime UtcDate => DateTime.UtcNow;

        public LiveDateTimeExtension() : base(true, true) { }

        protected override Binding CreateBinding() => new(nameof(UtcDate))
        {
            Source = this,
            Mode = BindingMode.OneWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };

        public string? Format { get => Binding.ConverterParameter?.ToString(); set => Binding.ConverterParameter = value; }

        public LetterCasing Casing { get; set; } = LetterCasing.Normal;

        public LiveDateTimeKind Kind { get; set; } = LiveDateTimeKind.Current;

        public double Interval { get; set; } = 1;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding.Converter = new DateTimeToStringConverter(Kind switch
            {
                LiveDateTimeKind.Utc => DateTimeToStringConverterKind.Utc,
                LiveDateTimeKind.Local => DateTimeToStringConverterKind.Local,
                _ => DateTimeToStringConverterKind.Current
            }, Casing);

            var value = base.ProvideValue(serviceProvider);

            var timer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = Interval.Seconds()
            };
            timer.Tick += UpdateTargetHandler;
            timer.Start();

            return value;
        }
    }
}
