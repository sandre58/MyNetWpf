// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Effects;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Parameters;

public enum Elevation
{
    Dp0,
    Dp1,
    Dp2,
    Dp3,
    Dp4,
    Dp5,
    Dp6,
    Dp7,
    Dp8,
    Dp12,
    Dp16,
    Dp24
}

internal static class ElevationInfo
{
    private static readonly IDictionary<Elevation, DropShadowEffect?> ShadowsDictionary;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3963:\"static\" fields should be initialized inline", Justification = "I doesn't want because resources muste be created")]
    static ElevationInfo()
    {
        _ = Application.Current.Resources;

        ShadowsDictionary = new Dictionary<Elevation, DropShadowEffect?>
        {
            { Elevation.Dp0, null },
            { Elevation.Dp1, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation1") },
            { Elevation.Dp2, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation2") },
            { Elevation.Dp3, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation3") },
            { Elevation.Dp4, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation4") },
            { Elevation.Dp5, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation5") },
            { Elevation.Dp6, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation6") },
            { Elevation.Dp7, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation7") },
            { Elevation.Dp8, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation8") },
            { Elevation.Dp12, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation12") },
            { Elevation.Dp16, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation16") },
            { Elevation.Dp24, WpfHelper.GetResourceOrDefault<DropShadowEffect>("MyNet.Shadows.Elevation24") }
        };
    }

    public static DropShadowEffect? GetDropShadow(Elevation elevation) => ShadowsDictionary[elevation];
}

public static class ElevationAssist
{

    public static readonly DependencyProperty ElevationProperty = DependencyProperty.RegisterAttached(
        "Elevation",
        typeof(Elevation),
        typeof(ElevationAssist),
        new FrameworkPropertyMetadata(default(Elevation), FrameworkPropertyMetadataOptions.AffectsRender));

    public static void SetElevation(DependencyObject element, Elevation value) => element.SetValue(ElevationProperty, value);
    public static Elevation GetElevation(DependencyObject element) => (Elevation)element.GetValue(ElevationProperty);

    public static DropShadowEffect? GetDropShadow(Elevation elevation) => ElevationInfo.GetDropShadow(elevation);
}
