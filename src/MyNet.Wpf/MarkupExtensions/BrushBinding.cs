// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.MarkupExtensions
{
    public class BrushBinding : Binding
    {
        public BrushBinding(string path) : base(path) { }

        public BrushBinding() { }

        public double Opacity
        {
            get => ConverterParameter is double opacity ? opacity : 1.0;
            set
            {
                ConverterParameter = value;
                Converter = BrushConverter.Opacity;
            }
        }

        public bool Contrast
        {
            get => Converter == BrushConverter.Contrast;
            set
            {
                if (value)
                    Converter = BrushConverter.Contrast;
            }
        }

        public PropertyPath SelfPath
        {
            get => Path;
            set
            {
                Path = value;
                RelativeSource = RelativeSource.Self;
            }
        }

        public PropertyPath InheritPath
        {
            get => Path;
            set
            {
                Path = value;
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1);
            }
        }
    }
}
