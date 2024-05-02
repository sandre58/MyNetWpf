// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

namespace MyNet.Wpf.Toasting.Lifetime
{
    public readonly struct MaximumToastCount
    {
        public static MaximumToastCount UnlimitedToasts() => new(int.MaxValue);

        public static MaximumToastCount FromCount(int count) => new(count);

        internal int Count { get; }

        private MaximumToastCount(int count) => Count = count;
    }
}
