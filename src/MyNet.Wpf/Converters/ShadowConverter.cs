// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Effects;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Converters
{
    public class ShadowConverter : IValueConverter
    {
        public static readonly ShadowConverter Default = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value switch
            {
                Elevation elevation => Clone(Convert(elevation)),
                _ => null
            };

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();

        public static DropShadowEffect? Convert(Elevation elevation) => ElevationAssist.GetDropShadow(elevation);

        private static DropShadowEffect? Clone(DropShadowEffect? dropShadowEffect)
            => dropShadowEffect is null
                ? null
                : new DropShadowEffect
                {
                    BlurRadius = dropShadowEffect.BlurRadius,
                    Color = dropShadowEffect.Color,
                    Direction = dropShadowEffect.Direction,
                    Opacity = dropShadowEffect.Opacity,
                    RenderingBias = dropShadowEffect.RenderingBias,
                    ShadowDepth = dropShadowEffect.ShadowDepth
                };
    }
}
