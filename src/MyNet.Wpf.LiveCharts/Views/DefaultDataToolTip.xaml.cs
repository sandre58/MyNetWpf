// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;

namespace MyNet.Wpf.LiveCharts.Views
{
    /// <summary>
    /// Interaction logic for TrainingStatisticsView.xaml
    /// </summary>
    public partial class DefaultDataToolTip : IChartTooltip
    {
        private TooltipData? _data;

        public DefaultDataToolTip()
        {
            InitializeComponent();

            DataContext = this;
        }


        /// <summary>
        /// The selection mode property
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            "SelectionMode", typeof(TooltipSelectionMode?), typeof(DefaultDataToolTip),
            new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the tooltip selection mode, default is null, if this property is null LiveCharts will decide the selection mode based on the series (that fired the tooltip) preferred section mode
        /// </summary>
        public TooltipSelectionMode? SelectionMode
        {
            get => (TooltipSelectionMode?)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(
    "ShowHeader", typeof(bool), typeof(DefaultDataToolTip),
    new PropertyMetadata(true));

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public TooltipData Data
        {
            get => _data ?? new TooltipData();
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
