// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = Colors1PartName, Type = typeof(ListBox))]
    [TemplatePart(Name = Colors2PartName, Type = typeof(ListBox))]
    [TemplatePart(Name = RecentColorsPartName, Type = typeof(ListBox))]
    [TemplatePart(Name = AddRecentColorButtonPartName, Type = typeof(Button))]
    public class ColorCanvas : ColorPickerBase
    {
        public const string Colors1PartName = "PART_Colors1";
        private const string Colors2PartName = "PART_Colors2";
        private const string RecentColorsPartName = "PART_RecentColors";
        private const string AddRecentColorButtonPartName = "PART_AddRecentColorButton";

        private ListBox? _colorListBox1;
        private ListBox? _colorListBox2;
        private ListBox? _recentColorsListBox;

        static ColorCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(typeof(ColorCanvas)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ColorCanvas), new FrameworkPropertyMetadata(false));
        }

        #region SelectionChangedEvent

        /// <summary>Identifies the <see cref="SelectedColorChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectionChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(SelectionChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<Color?>),
                                               typeof(ColorCanvas));

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color?> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion

        #region ShowNumericValues

        public static readonly DependencyProperty ShowNumericValuesProperty = DependencyProperty.Register(nameof(ShowNumericValues), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowNumericValues
        {
            get => (bool)GetValue(ShowNumericValuesProperty);
            set => SetValue(ShowNumericValuesProperty, value);
        }

        #endregion

        #region ShowRGB

        public static readonly DependencyProperty ShowRGBProperty = DependencyProperty.Register(nameof(ShowRGB), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowRGB
        {
            get => (bool)GetValue(ShowRGBProperty);
            set => SetValue(ShowRGBProperty, value);
        }

        #endregion

        #region ShowHSV

        public static readonly DependencyProperty ShowHSVProperty = DependencyProperty.Register(nameof(ShowHSV), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowHSV
        {
            get => (bool)GetValue(ShowHSVProperty);
            set => SetValue(ShowHSVProperty, value);
        }

        #endregion

        #region ShowTransparency

        public static readonly DependencyProperty ShowTransparencyProperty = DependencyProperty.Register(nameof(ShowTransparency), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowTransparency
        {
            get => (bool)GetValue(ShowTransparencyProperty);
            set => SetValue(ShowTransparencyProperty, value);
        }

        #endregion

        #region ShowHexa

        public static readonly DependencyProperty ShowHexaProperty = DependencyProperty.Register(nameof(ShowHexa), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowHexa
        {
            get => (bool)GetValue(ShowHexaProperty);
            set => SetValue(ShowHexaProperty, value);
        }

        #endregion

        #region HexaIsReadOnly

        public static readonly DependencyProperty HexaIsReadOnlyProperty = DependencyProperty.Register(nameof(HexaIsReadOnly), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(false));

        public bool HexaIsReadOnly
        {
            get => (bool)GetValue(HexaIsReadOnlyProperty);
            set => SetValue(HexaIsReadOnlyProperty, value);
        }

        #endregion

        #region ShowEyeDropper

        public static readonly DependencyProperty ShowEyeDropperProperty = DependencyProperty.Register(nameof(ShowEyeDropper), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowEyeDropper
        {
            get => (bool)GetValue(ShowEyeDropperProperty);
            set => SetValue(ShowEyeDropperProperty, value);
        }

        #endregion

        #region ShowSVPicker

        public static readonly DependencyProperty ShowSVPickerProperty = DependencyProperty.Register(nameof(ShowSVPicker), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        public bool ShowSVPicker
        {
            get => (bool)GetValue(ShowSVPickerProperty);
            set => SetValue(ShowSVPickerProperty, value);
        }

        #endregion

        #region ShowCustomColors1

        /// <summary>Identifies the <see cref="ShowCustomColors1"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCustomColors1Property =
            DependencyProperty.Register(nameof(ShowCustomColors1), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the available <see cref="ColorCanvas"/>.
        /// </summary>
        public bool ShowCustomColors1
        {
            get => (bool)GetValue(ShowCustomColors1Property);
            set => SetValue(ShowCustomColors1Property, value);
        }

        #endregion

        #region CustomColors1Header

        public static readonly DependencyProperty CustomColors1HeaderProperty =
            DependencyProperty.Register(nameof(CustomColors1Header), typeof(string), typeof(ColorCanvas), new PropertyMetadata(null));

        public string CustomColors1Header
        {
            get => (string)GetValue(CustomColors1HeaderProperty);
            set => SetValue(CustomColors1HeaderProperty, value);
        }

        #endregion

        #region CustomColors1ItemsSource

        /// <summary>Identifies the <see cref="CustomColors1ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColors1ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColors1ItemsSource), typeof(IEnumerable), typeof(ColorCanvas), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the available <see cref="ColorCanvas"/>.
        /// </summary>
        public IEnumerable? CustomColors1ItemsSource
        {
            get => (IEnumerable?)GetValue(CustomColors1ItemsSourceProperty);
            set => SetValue(CustomColors1ItemsSourceProperty, value);
        }

        #endregion

        #region ShowCustomColors2

        /// <summary>Identifies the <see cref="ShowCustomColors2"/> dependency property.</summary>
        public static readonly DependencyProperty ShowCustomColors2Property =
            DependencyProperty.Register(nameof(ShowCustomColors2),
                                        typeof(bool),
                                        typeof(ColorCanvas),
                                        new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the recent <see cref="ColorCanvas"/>.
        /// </summary>
        public bool ShowCustomColors2
        {
            get => (bool)GetValue(ShowCustomColors2Property);
            set => SetValue(ShowCustomColors2Property, value);
        }

        #endregion

        #region CustomColors2Header

        public static readonly DependencyProperty CustomColors2HeaderProperty =
            DependencyProperty.Register(nameof(CustomColors2Header), typeof(string), typeof(ColorCanvas), new PropertyMetadata(null));

        public string CustomColors2Header
        {
            get => (string)GetValue(CustomColors2HeaderProperty);
            set => SetValue(CustomColors2HeaderProperty, value);
        }

        #endregion

        #region CustomColors2ItemsSource

        /// <summary>Identifies the <see cref="CustomColors2ItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty CustomColors2ItemsSourceProperty =
            DependencyProperty.Register(nameof(CustomColors2ItemsSource), typeof(IEnumerable), typeof(ColorCanvas), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the recent <see cref="ColorCanvas"/>.
        /// </summary>
        public IEnumerable? CustomColors2ItemsSource
        {
            get => (IEnumerable?)GetValue(CustomColors2ItemsSourceProperty);
            set => SetValue(CustomColors2ItemsSourceProperty, value);
        }

        #endregion

        #region ShowRecentColors

        /// <summary>Identifies the <see cref="ShowRecentColors"/> dependency property.</summary>
        public static readonly DependencyProperty ShowRecentColorsProperty =
            DependencyProperty.Register(nameof(ShowRecentColors), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the standard <see cref="ColorCanvas"/>.
        /// </summary>
        public bool ShowRecentColors
        {
            get => (bool)GetValue(ShowRecentColorsProperty);
            set => SetValue(ShowRecentColorsProperty, value);
        }

        #endregion

        #region RecentColorsItemsSource

        /// <summary>Identifies the <see cref="RecentColorsItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty RecentColorsItemsSourceProperty =
            DependencyProperty.Register(nameof(RecentColorsItemsSource), typeof(IEnumerable), typeof(ColorCanvas), new PropertyMetadata(new ObservableCollection<Color>()));

        /// <summary>
        /// Gets or sets the <see cref="ItemsControl.ItemsSource"/> of the standard <see cref="ColorCanvas"/>.
        /// </summary>
        public IEnumerable? RecentColorsItemsSource
        {
            get => (IEnumerable?)GetValue(RecentColorsItemsSourceProperty);
            set => SetValue(RecentColorsItemsSourceProperty, value);
        }

        #endregion

        #region CanAddRecentColor

        /// <summary>Identifies the <see cref="CanAddRecentColor"/> dependency property.</summary>
        public static readonly DependencyProperty CanAddRecentColorProperty =
            DependencyProperty.Register(nameof(CanAddRecentColor), typeof(bool), typeof(ColorCanvas), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the visibility of the standard <see cref="ColorCanvas"/>.
        /// </summary>
        public bool CanAddRecentColor
        {
            get => (bool)GetValue(CanAddRecentColorProperty);
            set => SetValue(CanAddRecentColorProperty, value);
        }

        #endregion

        #region MaximumRecentColorsCount

        public static readonly DependencyProperty MaximumRecentColorsCountProperty = DependencyProperty.Register(nameof(MaximumRecentColorsCount), typeof(int), typeof(ColorCanvas), new PropertyMetadata(18));

        public int MaximumRecentColorsCount
        {
            get => (int)GetValue(MaximumRecentColorsCountProperty);
            set => SetValue(MaximumRecentColorsCountProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_colorListBox1 != null)
            {
                _colorListBox1.SelectionChanged -= ColorPalette_SelectionChanged;
                _colorListBox1.PreviewKeyDown -= ColorListBox_PreviewKeyDown;
            }

            if (_colorListBox2 != null)
            {
                _colorListBox2.SelectionChanged -= ColorPalette_SelectionChanged;
                _colorListBox2.PreviewKeyDown -= ColorListBox_PreviewKeyDown;
            }

            if (_recentColorsListBox != null)
            {
                _recentColorsListBox.SelectionChanged -= ColorPalette_SelectionChanged;
                _recentColorsListBox.PreviewKeyDown -= ColorListBox_PreviewKeyDown;
            }

            base.OnApplyTemplate();

            _colorListBox1 = Template.FindName(Colors1PartName, this) as ListBox;
            _colorListBox2 = Template.FindName(Colors2PartName, this) as ListBox;
            _recentColorsListBox = Template.FindName(RecentColorsPartName, this) as ListBox;
            var addRecentColorButton = Template.FindName(AddRecentColorButtonPartName, this) as Button;

            if (_colorListBox1 != null)
            {
                _colorListBox1.SelectionChanged += ColorPalette_SelectionChanged;
                _colorListBox1.PreviewKeyDown += ColorListBox_PreviewKeyDown;
            }

            if (_colorListBox2 != null)
            {
                _colorListBox2.SelectionChanged += ColorPalette_SelectionChanged;
                _colorListBox2.PreviewKeyDown += ColorListBox_PreviewKeyDown;
            }

            if (_recentColorsListBox != null)
            {
                _recentColorsListBox.SelectionChanged += ColorPalette_SelectionChanged;
                _recentColorsListBox.PreviewKeyDown += ColorListBox_PreviewKeyDown;
            }

            if (addRecentColorButton is not null)
                addRecentColorButton.Click += AddRecentColorButton_Click;
        }

        private void AddRecentColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedColor is not null)
                AddToRecentColors(SelectedColor.Value);
        }

        private void ColorListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) e.Handled = true;
        }

        protected override void OnSelectedColorChanged(Color? oldValue, Color? newValue)
        {
            base.OnSelectedColorChanged(oldValue, newValue);

            _colorListBox1?.SetCurrentValue(Selector.SelectedValueProperty, newValue);
            _colorListBox2?.SetCurrentValue(Selector.SelectedValueProperty, newValue);
            _recentColorsListBox?.SetCurrentValue(Selector.SelectedValueProperty, newValue);
        }

        private void ColorPalette_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox colorPalette && !_colorIsUpdating)
            {
                var oldValue = SelectedColor;
                SetCurrentValue(SelectedColorProperty, colorPalette.SelectedItem as Color?);
                RaiseEvent(new RoutedPropertyChangedEventArgs<Color?>(oldValue, colorPalette.SelectedItem as Color?, SelectionChangedEvent));
            }
        }

        public void AddToRecentColors(Color color)
        {
            if (RecentColorsItemsSource is ObservableCollection<Color> list)
            {
                if (list.Contains(color))
                {
                    var index = list.IndexOf(color);
                    if (index > 0)
                        list.Move(index, 0);
                }
                else
                {
                    if (MaximumRecentColorsCount > 0 && list.Count >= MaximumRecentColorsCount)
                        list.Remove(list[list.Count - 1]);
                    list.Insert(0, color);
                }

            }
        }
    }
}
