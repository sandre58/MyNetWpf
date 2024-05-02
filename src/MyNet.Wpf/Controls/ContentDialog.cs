// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyNet.UI;
using MyNet.UI.Busy;
using MyNet.UI.Dialogs.Models;
using MyNet.Wpf.Dialogs;

namespace MyNet.Wpf.Controls
{
    public class ContentDialog : HeaderedContentControl, IOverlayDialog, IDialogViewModel
    {
        static ContentDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(typeof(ContentDialog)));

        #region CloseOnClickAway

        public static readonly DependencyProperty CloseOnClickAwayProperty = DependencyProperty.Register(nameof(CloseOnClickAway), typeof(bool), typeof(ContentDialog), new PropertyMetadata(false));

        public bool CloseOnClickAway
        {
            get => (bool)GetValue(CloseOnClickAwayProperty);
            set => SetValue(CloseOnClickAwayProperty, value);
        }

        #endregion

        #region FocusOnShow

        public static readonly DependencyProperty FocusOnShowProperty = DependencyProperty.Register(nameof(FocusOnShow), typeof(bool), typeof(ContentDialog), new PropertyMetadata(true));

        public bool FocusOnShow
        {
            get => (bool)GetValue(FocusOnShowProperty);
            set => SetValue(FocusOnShowProperty, value);
        }

        #endregion

        #region ShowHeader

        public static readonly DependencyProperty ShowHeaderProperty = DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(ContentDialog), new PropertyMetadata(true));

        public bool ShowHeader
        {
            get => (bool)GetValue(ShowHeaderProperty);
            set => SetValue(ShowHeaderProperty, value);
        }

        #endregion

        #region ShowCloseButton

        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(ContentDialog), new PropertyMetadata(false));

        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        #endregion

        #region HeaderHeight

        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register("HeaderHeight", typeof(double), typeof(ContentDialog), new FrameworkPropertyMetadata(40.0D));

        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }

        #endregion

        #region HeaderPadding

        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register("HeaderPadding", typeof(Thickness), typeof(ContentDialog), new FrameworkPropertyMetadata(null));

        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set => SetValue(HeaderPaddingProperty, value);
        }

        #endregion

        #region HeaderFontSize

        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register("HeaderFontSize", typeof(double), typeof(ContentDialog), new FrameworkPropertyMetadata(12.0D));

        public double HeaderFontSize
        {
            get => (double)GetValue(HeaderFontSizeProperty);
            set => SetValue(HeaderFontSizeProperty, value);
        }

        #endregion

        #region HeaderBackground

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ContentDialog), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush HeaderBackground
        {
            get => (Brush)GetValue(HeaderBackgroundProperty);
            set => SetValue(HeaderBackgroundProperty, value);
        }

        #endregion

        #region HeaderForeground

        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(ContentDialog), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush HeaderForeground
        {
            get => (Brush)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }

        #endregion

        #region Footer

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(object), typeof(ContentDialog), new FrameworkPropertyMetadata(null));

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        #endregion

        #region FooterTemplate

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(ContentDialog), new FrameworkPropertyMetadata(null));

        public DataTemplate FooterTemplate
        {
            get => (DataTemplate)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        #endregion

        #region FooterPadding

        public static readonly DependencyProperty FooterPaddingProperty = DependencyProperty.Register("FooterPadding", typeof(Thickness), typeof(ContentDialog), new FrameworkPropertyMetadata(null));

        public Thickness FooterPadding
        {
            get => (Thickness)GetValue(BusyServiceProperty);
            set => SetValue(BusyServiceProperty, value);
        }

        #endregion

        #region BusyService

        public static readonly DependencyProperty BusyServiceProperty = DependencyProperty.Register("BusyService", typeof(IBusyService), typeof(ContentDialog), new FrameworkPropertyMetadata(null));

        public IBusyService BusyService
        {
            get => (IBusyService)GetValue(BusyServiceProperty);
            set => SetValue(BusyServiceProperty, value);
        }

        #endregion

        #region Dialog

        public event CancelEventHandler? CloseRequest;

        string? IDialogViewModel.Title { get => Header?.ToString(); set => Header = value; }

        public bool? DialogResult { get; protected set; }

        public virtual bool LoadWhenDialogOpening => true;

        void IDialogViewModel.Load()
        {
            // Method intentionally left empty.
        }

        Task<bool> IClosable.CanCloseAsync() => Task.FromResult(true);

        public virtual void Close()
        {
            var e = new CancelEventArgs();
            CloseRequest?.Invoke(this, e);
        }

        #endregion
    }
}
