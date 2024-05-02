// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Controls;
using MyNet.UI.Layout;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Behaviors
{
    public class GridViewColumnLayout(GridViewColumn element, int defaultIndex) : IColumnLayout
    {
        private bool _isVisible = true;
        private bool _canBeHidden = true;
        private readonly int _defaultIndex = defaultIndex;

        public event EventHandler? IsVisibleChanged;

        public GridViewColumn Column { get; set; } = element;

        public string Width { get; set; } = string.Empty;

        public int Index { get; set; } = defaultIndex;

        public bool CanBeHidden
        {
            get => _canBeHidden;
            set
            {
                if (value != _canBeHidden)
                {
                    _canBeHidden = value;
                    if (!value) IsVisible = true;
                }
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value != _isVisible && CanBeHidden)
                {
                    _isVisible = value;
                    IsVisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsStatic => string.IsNullOrEmpty(Width) || StaticWidth >= 0;

        public double StaticWidth => double.TryParse(Width, out var result) ? result : -1;

        public double Percentage => !IsStatic && !string.IsNullOrEmpty(Width) ? Mulitplier * 100 : 0;

        public double Mulitplier => Width is "*" or "1*"
                    ? 1
                    : Width.EndsWith("*", StringComparison.InvariantCultureIgnoreCase) && double.TryParse(Width[0..^1], out var perc) ? perc : 1;

        public void Reset() => Index = _defaultIndex;

        public string Identifier => !string.IsNullOrEmpty(GridViewColumnAssist.GetPropertyName(Column)) ? GridViewColumnAssist.GetPropertyName(Column) : Column.GetHashCode().ToString(CultureInfo.InvariantCulture);

        public void SetWidth(double allowedSpace, double totalPercentage)
        {
            if (IsVisible)
            {
                if (!string.IsNullOrEmpty(Width) && allowedSpace > 0)
                {
                    Column.Width = IsStatic ? StaticWidth : allowedSpace * (Percentage / totalPercentage);
                }
            }
            else
            {
                Column.Width = 0;
            }
        }

        public override bool Equals(object? obj) => obj is GridViewColumnLayout behavior && Equals(Column, behavior.Column);

        public override int GetHashCode() => Column.GetHashCode();
    }
}
