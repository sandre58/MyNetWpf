// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MyNet.UI.Dialogs;
using MyNet.UI.Dialogs.Settings;
using MyNet.UI.Resources;
using MyNet.Utilities;
using MyNet.Utilities.Extensions;
using MyNet.Utilities.IO.FileExtensions;
using MyNet.Utilities.Logging;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Resources;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ButtonPartName, Type = typeof(Button))]
    [TemplatePart(Name = TextBoxPartName, Type = typeof(TextBox))]
    [DefaultEvent("SelectedImageChanged")]
    public class ImagePicker : Control
    {
        public const string TextBoxPartName = "PART_TextBox";
        private const string ButtonPartName = "PART_Button";

        private TextBox? _textBox;
        private ButtonBase? _dropDownButton;
        protected bool _imageIsUpdating;

        static ImagePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImagePicker), new FrameworkPropertyMetadata(typeof(ImagePicker)));
            EventManager.RegisterClassHandler(typeof(ImagePicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ImagePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ImagePicker), new FrameworkPropertyMetadata(false));
        }

        public ImagePicker() => Errors = [];

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Datepicker gets focus move it to the TextBox
            var picker = (ImagePicker)sender;
            if (!e.Handled && picker._textBox != null)
            {
                if (e.OriginalSource == picker)
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (e.OriginalSource == picker._textBox)
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        #region Errors

        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register(nameof(Errors), typeof(ObservableCollection<string>), typeof(ImagePicker), new PropertyMetadata(null));

        public ObservableCollection<string> Errors
        {
            get => (ObservableCollection<string>)GetValue(ErrorsProperty);
            private set => SetValue(ErrorsProperty, value);
        }

        #endregion

        #region HasErrors

        internal static readonly DependencyPropertyKey HasErrorsPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasErrors),
                typeof(bool),
                typeof(ImagePicker),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasErrorsProperty = HasErrorsPropertyKey.DependencyProperty;

        public bool HasErrors => (bool)GetValue(HasErrorsProperty);

        #endregion HasErrors

        #region SelectedImage

        /// <summary>Identifies the <see cref="SelectedImage"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedImageProperty
            = DependencyProperty.Register(nameof(SelectedImage),
                                          typeof(ImageSource),
                                          typeof(ImagePicker),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedImagePropertyChanged));

        /// <summary>
        /// Gets or sets the selected <see cref="ImageSource"/>.
        /// </summary>
        public ImageSource? SelectedImage
        {
            get => (ImageSource)GetValue(SelectedImageProperty);
            set => SetValue(SelectedImageProperty, value);
        }

        private static void OnSelectedImagePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ImagePicker imagePicker && e.OldValue != e.NewValue && !imagePicker._imageIsUpdating)
            {
                imagePicker._imageIsUpdating = true;
                try
                {
                    imagePicker.OnSelectedImageChanged(e.OldValue as ImageSource, e.NewValue as ImageSource);
                }
                finally
                {
                    imagePicker._imageIsUpdating = false;
                }
            }
        }

        /// <summary>Identifies the <see cref="SelectedImageChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectedImageChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(SelectedImageChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<ImageSource?>),
                                               typeof(ImagePicker));

        /// <summary>
        ///     Occurs when the <see cref="SelectedImage" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ImageSource?> SelectedImageChanged
        {
            add => AddHandler(SelectedImageChangedEvent, value);
            remove => RemoveHandler(SelectedImageChangedEvent, value);
        }

        #endregion

        #region ImagePath

        /// <summary>Identifies the <see cref="ImagePath"/> dependency property.</summary>
        public static readonly DependencyProperty ImagePathProperty
            = DependencyProperty.Register(nameof(ImagePath),
                                          typeof(string),
                                          typeof(ImagePicker),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnImagePathPropertyChanged));

        /// <summary>
        /// Gets or sets the name of the <see cref="SelectedImage"/>.
        /// </summary>
        public string? ImagePath
        {
            get => (string?)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        private static void OnImagePathPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ImagePicker imagePicker)
            {
                imagePicker.OnImagePathChanged(e.NewValue?.ToString());
            }
        }

        protected virtual void OnImagePathChanged(string? imagePath)
        {
            Errors.Clear();
            SetValue(HasErrorsPropertyKey, false);

            if (_imageIsUpdating) return;

            if (string.IsNullOrEmpty(imagePath))
            {
                SetCurrentValue(SelectedImageProperty, null);
            }
            else if (!File.Exists(imagePath))
            {
                Errors.Add(MessageResources.FileXNotFoundError.FormatWith(imagePath));
                SetValue(HasErrorsPropertyKey, true);
            }
            else
            {
                try
                {
                    var image = File.ReadAllBytes(imagePath).ToBitmap();
                    SetCurrentValue(SelectedImageProperty, image);
                }
                catch (Exception ex)
                {
                    LogManager.Error(ex);
                    Errors.Add(WpfResources.XIsNotAValidImageError.FormatWith(imagePath));
                    SetValue(HasErrorsPropertyKey, true);
                }
            }
        }

        #endregion

        #region SelectedImageTemplate

        /// <summary>Identifies the <see cref="SelectedImageTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedImageTemplateProperty
            = DependencyProperty.Register(nameof(SelectedImageTemplate), typeof(DataTemplate), typeof(ImagePicker), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> for the <see cref="ImagePicker.SelectedImage"/>
        /// </summary>
        public DataTemplate? SelectedImageTemplate
        {
            get => (DataTemplate?)GetValue(SelectedImageTemplateProperty);
            set => SetValue(SelectedImageTemplateProperty, value);
        }

        #endregion

        protected virtual void OnSelectedImageChanged(ImageSource? oldValue, ImageSource? newValue) => RaiseEvent(new RoutedPropertyChangedEventArgs<ImageSource?>(oldValue, newValue, SelectedImageChangedEvent));

        public override void OnApplyTemplate()
        {
            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_ClickAsync;
            }

            base.OnApplyTemplate();

            _textBox = GetTemplateChild(TextBoxPartName) as TextBox;
            _dropDownButton = GetTemplateChild(ButtonPartName) as Button;

            if (_dropDownButton != null)
            {
                _dropDownButton.Click += DropDownButton_ClickAsync;
            }
        }

        private async void DropDownButton_ClickAsync(object? sender, RoutedEventArgs e)
        {
            var result = await DialogManager.ShowOpenFileDialogAsync(new OpenFileDialogSettings
            {
                Multiselect = false,
                InitialDirectory = string.Empty,
                Filters = FileExtensionInfoProvider.AllImages.GetFileFilters(x => x.Translate())
            });

            if (result.result.IsTrue() && !string.IsNullOrEmpty(result.filename))
                SetCurrentValue(ImagePathProperty, result.filename);
        }
    }
}
