// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows.Media;

namespace MyNet.Wpf.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// This function tries to convert a given string into a Color in the following order:
        ///    1. If the string starts with '#' the function tries to get the color from the hex-code
        ///    2. else the function tries to find the color in the color names Dictionary
        ///    3. If 1. + 2. were not successful the function adds a '#' sign and tries 1. + 2. again
        /// </summary>
        /// <param name="colorName">The localized name of the color, the hex-code of the color or the internal color name</param>
        /// <returns>the Color if successful, else null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2589:Boolean expressions should not be gratuitous", Justification = "result can be null")]
        public static Color? ToColor(this string? colorName)
        {
            Color? result = null;

            try
            {
                // if we don't have a string, we cannot have any Color
                if (string.IsNullOrWhiteSpace(colorName))
                {
                    return null;
                }

                if (!colorName.StartsWith('#') && ResourceLocator.ColorResourcesDictionary.Any(x => string.Equals(x.Value, colorName, StringComparison.OrdinalIgnoreCase)))
                {
                    result = ResourceLocator.ColorResourcesDictionary.FirstOrDefault(x => string.Equals(x.Value, colorName, StringComparison.OrdinalIgnoreCase)).Key;
                }

                result ??= ColorConverter.ConvertFromString(colorName) as Color?;
            }
            catch (FormatException)
            {
                if (colorName != null && !result.HasValue && !colorName.StartsWith('#'))
                {
                    result = ToColor("#" + colorName);
                }
            }

            return result;
        }

        public static string ToHex(this Color color) => color.A != 255 ? string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B) : string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);

        /// <summary>
        /// Searches for the localized name of a given <paramref name="color"/>
        /// </summary>
        /// <param name="color">color</param>
        /// <returns>the local color name or null if the given color doesn't have a name</returns>
        public static string? ToName(this Color color)
            => ResourceLocator.ColorResourcesDictionary.TryGetValue(color, out var name) ? $"{name}" : color.ToHex();

        public static Color ContrastingForegroundColor(this Color color)
            => color.IsLightColor() ? Colors.Black : Colors.White;

        public static bool IsLightColor(this Color color)
        {
            static double rgb_srgb(double d)
            {
                d /= 255.0;
                return (d > 0.03928)
                    ? Math.Pow((d + 0.055) / 1.055, 2.4)
                    : d / 12.92;
            }
            var r = rgb_srgb(color.R);
            var g = rgb_srgb(color.G);
            var b = rgb_srgb(color.B);

            var luminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
            return luminance > 0.179;
        }

        public static bool IsDarkColor(this Color color) => !IsLightColor(color);

        public static Color Darken(this Color color, int amount = 1) => color.ShiftLightness(amount);

        public static Color Lighten(this Color color, int amount = 1) => color.ShiftLightness(-amount);

        public static Color ShiftLightness(this Color color, int amount = 1)
        {
            var lab = color.ToLab();
            var shifted = new Lab(lab.L - LabConstants.Kn * amount, lab.A, lab.B);
            return shifted.ToColor();
        }

        private static Lab ToLab(this Color c)
        {
            var xyz = c.ToXyz();
            return xyz.ToLab();
        }

        private static Lab ToLab(this Xyz xyz)
        {
            static double xyz_lab(double v) => v > LabConstants.E ? Math.Pow(v, 1 / 3.0) : (v * LabConstants.K + 16) / 116;

            var fx = xyz_lab(xyz.X / LabConstants.WhitePointX);
            var fy = xyz_lab(xyz.Y / LabConstants.WhitePointY);
            var fz = xyz_lab(xyz.Z / LabConstants.WhitePointZ);

            var l = 116 * fy - 16;
            var a = 500 * (fx - fy);
            var b = 200 * (fy - fz);
            return new Lab(l, a, b);
        }

        private static Color ToColor(this Lab lab)
        {
            var xyz = lab.ToXyz();

            return xyz.ToColor();
        }

        private static Color ToColor(this Xyz xyz)
        {
            double xyz_rgb(double d) => d > 0.0031308 ? 255.0 * (1.055 * Math.Pow(d, 1.0 / 2.4) - 0.055) : 255.0 * (12.92 * d);

            byte clip(double d) => d < 0 ? (byte)0 : d > 255 ? (byte)255 : (byte)Math.Round(d);
            var r = xyz_rgb(3.2404542 * xyz.X - 1.5371385 * xyz.Y - 0.4985314 * xyz.Z);
            var g = xyz_rgb(-0.9692660 * xyz.X + 1.8760108 * xyz.Y + 0.0415560 * xyz.Z);
            var b = xyz_rgb(0.0556434 * xyz.X - 0.2040259 * xyz.Y + 1.0572252 * xyz.Z);

            return Color.FromRgb(clip(r), clip(g), clip(b));
        }
        private static Xyz ToXyz(this Color c)
        {
            static double rgb_xyz(double v)
            {
                v /= 255;
                return v > 0.04045 ? Math.Pow((v + 0.055) / 1.055, 2.4) : v / 12.92;
            }

            var r = rgb_xyz(c.R);
            var g = rgb_xyz(c.G);
            var b = rgb_xyz(c.B);

            var x = 0.4124564 * r + 0.3575761 * g + 0.1804375 * b;
            var y = 0.2126729 * r + 0.7151522 * g + 0.0721750 * b;
            var z = 0.0193339 * r + 0.1191920 * g + 0.9503041 * b;
            return new Xyz(x, y, z);
        }

        private static Xyz ToXyz(this Lab lab)
        {
            static double lab_xyz(double d) => d > LabConstants.ECubedRoot ? d * d * d : (116 * d - 16) / LabConstants.K;

            var y = (lab.L + 16.0) / 116.0;
            var x = double.IsNaN(lab.A) ? y : y + lab.A / 500.0;
            var z = double.IsNaN(lab.B) ? y : y - lab.B / 200.0;

            y = LabConstants.WhitePointY * lab_xyz(y);
            x = LabConstants.WhitePointX * lab_xyz(x);
            z = LabConstants.WhitePointZ * lab_xyz(z);

            return new Xyz(x, y, z);
        }

        private readonly struct Lab(double l, double a, double b)
        {
            public double L { get; } = l;
            public double A { get; } = a;
            public double B { get; } = b;
        }

        private readonly struct Xyz(double x, double y, double z)
        {
            public double X { get; } = x;
            public double Y { get; } = y;
            public double Z { get; } = z;
        }

        private static class LabConstants
        {
            public const double Kn = 18;

            public const double WhitePointX = 0.95047;
            public const double WhitePointY = 1;
            public const double WhitePointZ = 1.08883;

            public static readonly double ECubedRoot = Math.Pow(E, 1.0 / 3);
            public const double K = 24389 / 27.0;
            public const double E = 216 / 24389.0;
        }
    }
}
