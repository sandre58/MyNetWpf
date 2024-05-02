// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using MyNet.Wpf.Converters;
using MyNet.Wpf.Helpers;
using MyNet.Wpf.Toasting;
using MyNet.Wpf.Toasting.Settings;

namespace MyNet.Wpf.Controls
{
    internal class ToastsWindow : Window
    {
        static ToastsWindow()
        {
            AllowsTransparencyProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(true));
            BackgroundProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(Brushes.Transparent));
            ShowActivatedProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(false));
            TopmostProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(true));
            VisibilityProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(Visibility.Hidden));
            WindowStateProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(WindowState.Normal));
            WindowStyleProperty.OverrideMetadata(typeof(ToastsWindow), new FrameworkPropertyMetadata(WindowStyle.None));
        }

        public ToastsWindow()
        {
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            WindowStartupLocation = WindowStartupLocation.Manual;
            DataContext = this;
            SetValue(ToastsPropertyKey, new ObservableCollection<Toast>());

            Loaded += NotificationsWindow_Loaded;
            Closing += NotificationsWindow_Closing;
            Toasts.CollectionChanged += Toasts_CollectionChanged;

            var toastsControl = new ToastItemsControl() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            toastsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(Toasts))
            {
                Source = this
            });
            toastsControl.SetBinding(ToastItemsControl.ShouldReverseItemsProperty, new Binding(nameof(EjectDirection))
            {
                Converter = EnumToBooleanConverter.Any,
                ConverterParameter = EjectDirection.ToTop,
                Source = this
            });
            Content = toastsControl;
        }

        #region Toasts

        private static readonly DependencyPropertyKey ToastsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Toasts), typeof(ObservableCollection<Toast>), typeof(ToastsWindow), new PropertyMetadata(new ObservableCollection<Toast>()));

        public static readonly DependencyProperty ToastsProperty = ToastsPropertyKey.DependencyProperty;

        public ObservableCollection<Toast> Toasts => (ObservableCollection<Toast>)GetValue(ToastsProperty);

        #endregion

        #region EjectDirection

        public EjectDirection EjectDirection
        {
            get => (EjectDirection)GetValue(EjectDirectionProperty);
            set => SetValue(EjectDirectionProperty, value);
        }

        public static readonly DependencyProperty EjectDirectionProperty =
                DependencyProperty.Register(
                        nameof(EjectDirection),
                        typeof(EjectDirection),
                        typeof(ToastsWindow),
                        new FrameworkPropertyMetadata(EjectDirection.ToBottom));

        #endregion

        private void Toasts_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            => Visibility = !Toasts.Any() ? Visibility.Collapsed : Visibility.Visible;

        private void NotificationsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var wndHelper = new WindowInteropHelper(this);

            var exStyle = (int)NativeMethods.GetWindowLongWrapper(wndHelper.Handle, (int)NativeMethods.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)NativeMethods.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            _ = NativeMethods.SetWindowLong(wndHelper.Handle, (int)NativeMethods.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void NotificationsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = true;

        public new void Close()
        {
            Closing -= NotificationsWindow_Closing;
            Toasts.CollectionChanged -= Toasts_CollectionChanged;
            base.Close();
        }

        public void AddToast(Toast toast) => Schedulers.WpfScheduler.Current.Schedule(() => Toasts.Add(toast));

        public void RemoveToast(Toast toast)
        {
            if (toast != null)
            {

                Timer? timer = null;
                timer = new Timer(obj =>
                {
                    Schedulers.WpfScheduler.Current.Schedule(() => Toasts.Remove(toast));

                    timer?.Dispose();
                    timer = null;
                }, null, TimeSpan.FromMilliseconds(300), TimeSpan.FromMinutes(1));
            }
        }
    }
}
