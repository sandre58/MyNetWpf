// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;
using MyNet.Wpf.Web.Resources;

namespace MyNet.Wpf.Web.Controls
{
    [TemplatePart(Name = WebViewName, Type = typeof(WebView2))]
    public class GoogleMapsWebView : Control
    {
        private const string WebViewName = "PART_WebView";

        private WebView2? _webView;

        static GoogleMapsWebView() => DefaultStyleKeyProperty.OverrideMetadata(typeof(GoogleMapsWebView), new FrameworkPropertyMetadata(typeof(GoogleMapsWebView)));

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _webView = GetTemplateChild(WebViewName) as WebView2;

            await UpdateUrlAsync(Url).ConfigureAwait(false);
        }

        #region Url


        public static readonly DependencyProperty UrlProperty
            = DependencyProperty.RegisterAttached(nameof(Url), typeof(string), typeof(GoogleMapsWebView), new PropertyMetadata(string.Empty, OnUrlChangedAsync));

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        private static async void OnUrlChangedAsync(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not GoogleMapsWebView control) return;

            await control.UpdateUrlAsync(e.NewValue?.ToString()).ConfigureAwait(false);
        }

        private async Task UpdateUrlAsync(string? url)
        {
            if (_webView is null) return;

            await _webView.EnsureCoreWebView2Async().ConfigureAwait(false);
            var str = string.IsNullOrEmpty(url) ? "about:blank" : GoogleResources.GoogleMapsFrame.Replace("$url", url);

            _ = Observable.Threading.Scheduler.GetUIOrCurrent().Schedule(() => _webView.NavigateToString(str));
        }

        #endregion
    }
}
