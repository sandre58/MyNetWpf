// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using System.Windows.Threading;
using MyNet.Humanizer;
using MyNet.Utilities;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.MarkupExtensions
{
    public class LiveDateTimeExtension : GlobalizationExtensionBase<Binding>
    {
        public virtual DateTime UtcDate => DateTime.UtcNow;

        public LiveDateTimeExtension() : base(true, true) { }

        protected override Binding CreateBinding() => new(nameof(UtcDate))
        {
            Source = this,
            Mode = BindingMode.OneWay,
        };

        public string? Format { get => Binding.ConverterParameter?.ToString(); set => Binding.ConverterParameter = value; }

        public LetterCasing Casing { get; set; } = LetterCasing.Normal;

        public DateTimeConverterKind Kind { get; set; } = DateTimeConverterKind.Current;

        public double Interval { get; set; } = 1;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Binding.Converter = new DateTimeToStringConverter(Kind switch
            {
                DateTimeConverterKind.Utc => DateTimeToStringConverterKind.Utc,
                DateTimeConverterKind.Local => DateTimeToStringConverterKind.Local,
                _ => DateTimeToStringConverterKind.Current
            }, Casing);

            var timer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = Interval.Seconds()
            };
            timer.Tick += (sender, e) => UpdateTarget();
            timer.Start();

            return base.ProvideValue(serviceProvider);
        }
    }
}
