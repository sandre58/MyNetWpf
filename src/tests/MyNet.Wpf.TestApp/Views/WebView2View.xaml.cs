// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace MyNet.Wpf.TestApp.Views
{
    /// <summary>
    /// Interaction logic for WebView.xaml
    /// </summary>
    public partial class WebView2View
    {
        public WebView2View() => InitializeComponent();

        private void WorkspaceView_Loaded(object sender, System.Windows.RoutedEventArgs e) => webView.Source = new Uri(Path.Combine(Environment.CurrentDirectory, "WebView", "RichText.html"));

        private async void GetTextButton_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = await webView.ExecuteScriptAsync("quill.getText()");
            resultTextBlock.Text = text;
        }

        private async void GetContentButton_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = await webView.ExecuteScriptAsync("quill.getContents()");
            resultTextBlock.Text = text;
        }

        private async void GetHtmlButton_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = await webView.ExecuteScriptAsync("quill.container.innerHtml");
            resultTextBlock.Text = text;
        }
    }
}
