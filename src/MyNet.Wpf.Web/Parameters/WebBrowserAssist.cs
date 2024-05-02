// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MyNet.Wpf.Web.Parameters
{
    public static class WebBrowserAssist
    {
        #region Source

        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(object), typeof(WebBrowserAssist), new UIPropertyMetadata(null, SourcePropertyChanged));

        public static object GetSource(WebBrowser obj) => (string)obj.GetValue(SourceProperty);

        public static void SetSource(WebBrowser obj, object value) => obj.SetValue(SourceProperty, value);

        public static void SourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is WebBrowser browser)
            {
                Uri? uri = null;

                if (e.NewValue is string)
                {
                    var uriString = e.NewValue as string;
                    uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
                }
                else if (e.NewValue is Uri)
                {
                    uri = e.NewValue as Uri;
                }

                browser.Source = uri;
            }
        }

        #endregion

        #region DisableJavascriptErrors

        #region SilentJavascriptErrorsContext

        private static readonly DependencyPropertyKey SilentJavascriptErrorsContextKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "SilentJavascriptErrorsContext",
                typeof(SilentJavascriptErrorsContext),
                typeof(WebBrowserAssist),
                new FrameworkPropertyMetadata(null));

        private static void SetSilentJavascriptErrorsContext(DependencyObject depObj, SilentJavascriptErrorsContext? value) => depObj.SetValue(SilentJavascriptErrorsContextKey, value);

        private static SilentJavascriptErrorsContext GetSilentJavascriptErrorsContext(DependencyObject depObj) => (SilentJavascriptErrorsContext)depObj.GetValue(SilentJavascriptErrorsContextKey.DependencyProperty);

        #endregion

        public static readonly DependencyProperty DisableJavascriptErrorsProperty =
            DependencyProperty.RegisterAttached(
                "DisableJavascriptErrors",
                typeof(bool),
                typeof(WebBrowserAssist),
                new FrameworkPropertyMetadata(OnDisableJavascriptErrorsChangedCallback));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static void SetDisableJavascriptErrors(DependencyObject depObj, bool value) => depObj.SetValue(DisableJavascriptErrorsProperty, value);

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static bool GetDisableJavascriptErrors(DependencyObject depObj) => (bool)depObj.GetValue(DisableJavascriptErrorsProperty);

        private static void OnDisableJavascriptErrorsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser webBrowser)
            {
                if (Equals(e.OldValue, e.NewValue)) return;

                var context = GetSilentJavascriptErrorsContext(webBrowser);
                context?.Dispose();

                if (e.NewValue != null)
                {
                    context = new SilentJavascriptErrorsContext(webBrowser);
                    SetSilentJavascriptErrorsContext(webBrowser, context);
                }
                else
                {
                    SetSilentJavascriptErrorsContext(webBrowser, null);
                }
            }
        }

        private sealed class SilentJavascriptErrorsContext : IDisposable
        {
            private bool? _silent;
            private readonly WebBrowser _webBrowser;


            public SilentJavascriptErrorsContext(WebBrowser webBrowser)
            {
                _silent = new bool?();

                _webBrowser = webBrowser;
                _webBrowser.Loaded += OnWebBrowserLoaded;
                _webBrowser.Navigated += OnWebBrowserNavigated;
            }

            private void OnWebBrowserLoaded(object sender, RoutedEventArgs e)
            {
                if (!_silent.HasValue) return;

                SetSilent();
            }

            private void OnWebBrowserNavigated(object sender, NavigationEventArgs e)
            {
                var webBrowser = (WebBrowser)sender;

                if (!_silent.HasValue)
                {
                    _silent = GetDisableJavascriptErrors(webBrowser);
                }

                if (!webBrowser.IsLoaded) return;

                SetSilent();
            }

            /// <summary>
            /// Solution by Simon Mourier on StackOverflow
            /// http://stackoverflow.com/a/6198700/741414
            /// </summary>
            private void SetSilent()
            {
                _webBrowser.Loaded -= OnWebBrowserLoaded;
                _webBrowser.Navigated -= OnWebBrowserNavigated;

                // get an IWebBrowser2 from the document
                if (_webBrowser.Document is IOleServiceProvider sp)
                {
                    var iID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                    var iID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                    _ = sp.QueryService(ref iID_IWebBrowserApp, ref iID_IWebBrowser2, out var webBrowser2);
                    if (webBrowser2 != null)
                    {
                        _ = webBrowser2.GetType().InvokeMember(
                            "Silent",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty,
                            null,
                            webBrowser2,
                            [_silent]);
                    }
                }
            }

            [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface IOleServiceProvider
            {
                [PreserveSig]
                int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
            }

            public void Dispose()
            {
                if (_webBrowser != null)
                {
                    _webBrowser.Loaded -= OnWebBrowserLoaded;
                    _webBrowser.Navigated -= OnWebBrowserNavigated;
                }
            }
        }

        #endregion


    }
}
