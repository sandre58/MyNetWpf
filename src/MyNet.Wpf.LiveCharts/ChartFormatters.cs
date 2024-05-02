// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using LiveCharts;

namespace MyNet.Wpf.LiveCharts
{
    public static class ChartFormatters
    {
        public static Func<double, string> ValueToPercent => x => (x / 100.0).ToString("P0", CultureInfo.CurrentCulture);

        public static Func<double, string> ValueToPercentWithoutZero => x => string.Format(CultureInfo.CurrentCulture, "{0}", x > 0 ? (x / 100.0).ToString("P0", CultureInfo.CurrentCulture) : string.Empty);

        public static Func<double, string> ToPercent => x => x.ToString("P0", CultureInfo.CurrentCulture);

        public static Func<double, string> ToPercent1 => x => x.ToString("P1", CultureInfo.CurrentCulture);

        public static Func<double, string> ToPercentWithoutZero => x => string.Format(CultureInfo.CurrentCulture, "{0}", x > 0 ? x.ToString("P0", CultureInfo.CurrentCulture) : string.Empty);

        public static Func<double, string> ToValue => x => Math.Round(x, 2).ToString(CultureInfo.CurrentCulture);

        public static Func<double, string> ToValueWithoutZero => x => string.Format(CultureInfo.CurrentCulture, "{0}", x > 0 ? Math.Round(x, 2).ToString(CultureInfo.CurrentCulture) : string.Empty);

        public static Func<ChartPoint, string> ChartPointToValueWithoutZero => x => string.Format(CultureInfo.CurrentCulture, "{0}", x.Y > 0 ? Math.Round(x.Y, 2).ToString(CultureInfo.CurrentCulture) : string.Empty);
    }
}
