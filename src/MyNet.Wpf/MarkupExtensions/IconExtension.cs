// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MaterialDesignThemes.Wpf;
using MyNet.Wpf.Controls;
using System;
using System.Windows.Markup;
using System.Windows.Media;
using Wpf.Ui.Controls;
using PackIcon = MyNet.Wpf.Controls.PackIcon;

namespace MyNet.Wpf.MarkupExtensions
{
    public enum IconSizeMode
    {
        Tool = 18,

        Small = 12,

        Medium = 24,

        Large = 36

    }

    public class IconExtension : MarkupExtension
    {
        public IconExtension()
        { }

        public IconExtension(Geometry data) => Data = data;

        public IconExtension(PackIconKind kind) => Kind = kind;

        public IconExtension(SymbolRegular symbol) => Symbol = symbol;

        public IconExtension(Geometry data, double size)
        {
            Data = data;
            Size = size;
        }

        public IconExtension(PackIconKind kind, double size)
        {
            Kind = kind;
            Size = size;
        }

        public IconExtension(SymbolRegular symbol, double size, bool isFilled)
        {
            Symbol = symbol;
            Size = size;
            IsFilled = isFilled;
        }

        [ConstructorArgument("data")]
        public Geometry? Data { get; set; }

        [ConstructorArgument("kind")]
        public PackIconKind? Kind { get; set; }

        [ConstructorArgument("symbol")]
        public SymbolRegular? Symbol { get; set; }

        [ConstructorArgument("size")]
        public double? Size { get; set; }

        [ConstructorArgument("isFilled")]
        public bool IsFilled { get; set; }

        public IconSizeMode? SizeMode { get; set; }

        private double? GetSize() => Size.HasValue ? Size : SizeMode.HasValue ? (int)SizeMode.Value : (double?)null;

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            var size = GetSize();

            if (Kind.HasValue)
            {
                var result = new PackIcon { Kind = Kind.Value, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                if (size.HasValue)
                {
                    result.Height = size.Value;
                    result.Width = size.Value;
                }

                return result;
            }


            if (Data != null)
            {
                var result = new GeometryIcon { Data = Data, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                if (size.HasValue)
                {
                    result.Height = size.Value;
                    result.Width = size.Value;
                }

                return result;
            }

            if (Symbol != null)
            {
                var result = new Controls.SymbolIcon { Symbol = Symbol.Value, Filled = IsFilled, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, VerticalAlignment = System.Windows.VerticalAlignment.Center };

                if (size.HasValue)
                {
                    result.Height = size.Value;
                    result.Width = size.Value;
                    result.FontSize = size.Value;
                }

                return result;
            }


            return null;
        }
    }

    public class ToolIconExtension : IconExtension
    {
        public ToolIconExtension() => SizeMode = IconSizeMode.Tool;

        public ToolIconExtension(Geometry data) : base(data) { }

        public ToolIconExtension(PackIconKind kind) : base(kind) { }
    }

    public class SmallIconExtension : IconExtension
    {
        public SmallIconExtension() => SizeMode = IconSizeMode.Small;

        public SmallIconExtension(Geometry data) : base(data) { }

        public SmallIconExtension(PackIconKind kind) : base(kind) { }
    }

    public class MediumIconExtension : IconExtension
    {
        public MediumIconExtension() => SizeMode = IconSizeMode.Medium;

        public MediumIconExtension(Geometry data) : base(data) { }

        public MediumIconExtension(PackIconKind kind) : base(kind) { }
    }

    public class LargeIconExtension : IconExtension
    {
        public LargeIconExtension() => SizeMode = IconSizeMode.Large;

        public LargeIconExtension(Geometry data) : base(data) { }

        public LargeIconExtension(PackIconKind kind) : base(kind) { }
    }

}
