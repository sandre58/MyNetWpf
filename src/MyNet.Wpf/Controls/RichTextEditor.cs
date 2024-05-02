// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using MyNet.Observable.Translatables;
using MyNet.UI.Commands;
using MyNet.Utilities;
using MyNet.Wpf.Parameters;
using MyNet.Wpf.Resources;
using MyNet.Xaml;
using MyNet.Xaml.Html;

namespace MyNet.Wpf.Controls
{
    [TemplatePart(Name = ColorMenuName, Type = typeof(MenuBase))]
    [TemplatePart(Name = FontSizeComboBoxName, Type = typeof(ComboBox))]
    [TemplatePart(Name = FontFamilyMenuName, Type = typeof(MenuBase))]
    public class RichTextEditor : RichTextBox
    {
        private const string FontSizeComboBoxName = "PART_FontSizeComboBox";
        private const string FontFamilyMenuName = "PART_FontFamilyMenu";
        private const string ColorMenuName = "PART_ColorMenu";

        private bool _textChanging;

        private MenuBase? _colorMenu;
        private ComboBox? _fontSizeComboBox;
        private MenuBase? _fontFamilyMenu;

        static RichTextEditor() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextEditor), new FrameworkPropertyMetadata(typeof(RichTextEditor)));

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            _textChanging = true;
            SetValue(HtmlProperty, GetHtmlText(Document));
            _textChanging = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _colorMenu = GetTemplateChild(ColorMenuName) as MenuBase;
            _fontSizeComboBox = GetTemplateChild(FontSizeComboBoxName) as ComboBox;
            _fontFamilyMenu = GetTemplateChild(FontFamilyMenuName) as MenuBase;

            if (_fontSizeComboBox is not null)
            {
                _fontSizeComboBox.LostFocus += FontSizeComboBox_LostFocus;
                _fontSizeComboBox.SelectionChanged += FontSizeComboBox_LostFocus;
                ComboBoxAssist.SetDownCommand(_fontSizeComboBox, ComboBoxAssist.DownCommand);
                ComboBoxAssist.SetUpCommand(_fontSizeComboBox, ComboBoxAssist.UpCommand);
                _fontSizeComboBox.ItemsSource = new List<int> { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
                _fontSizeComboBox.SelectedIndex = 4;
            }

            if (_fontFamilyMenu is not null)
            {
                var command = CommandsManager.CreateNotNull<FontFamily>(x => Selection?.ApplyPropertyValue(FontFamilyProperty, x));
                var families = Fonts.SystemFontFamilies.OrderBy(x => x.ToString()).Select(x => new MenuItem() { Header = x.ToString(), DataContext = x, CommandParameter = x, Command = command });
                families.ForEach(x => _fontFamilyMenu.Items.Add(x));
            }

            if (_colorMenu is not null)
            {
                var command = CommandsManager.CreateNotNull<Brush>(x => Selection?.ApplyPropertyValue(ForegroundProperty, x));

                var brushes = new List<DisplayWrapper<Brush>>
                {
                    new(new SolidColorBrush(Colors.Black), new TranslatableString(nameof(ColorResources.Black))),
                    new(new SolidColorBrush(Colors.White), new TranslatableString(nameof(ColorResources.White))),
                    new(new SolidColorBrush(Colors.DarkRed), new TranslatableString(nameof(ColorResources.DarkRed))),
                    new(new SolidColorBrush(Colors.Red), new TranslatableString(nameof(ColorResources.Red))),
                    new(new SolidColorBrush(Colors.Orange), new TranslatableString(nameof(ColorResources.Orange))),
                    new(new SolidColorBrush(Colors.Yellow), new TranslatableString(nameof(ColorResources.Yellow))),
                    new(new SolidColorBrush(Colors.DarkGreen), new TranslatableString(nameof(ColorResources.DarkGreen))),
                    new(new SolidColorBrush(Colors.Green), new TranslatableString(nameof(ColorResources.Green))),
                    new(new SolidColorBrush(Colors.LightGreen), new TranslatableString(nameof(ColorResources.LightGreen))),
                    new(new SolidColorBrush(Colors.DarkBlue), new TranslatableString(nameof(ColorResources.DarkBlue))),
                    new(new SolidColorBrush(Colors.Blue), new TranslatableString(nameof(ColorResources.Blue))),
                    new(new SolidColorBrush(Colors.LightBlue), new TranslatableString(nameof(ColorResources.LightBlue))),
                    new(new SolidColorBrush(Colors.Purple), new TranslatableString(nameof(ColorResources.Purple))),
                    new(new SolidColorBrush(Colors.Pink), new TranslatableString(nameof(ColorResources.Pink)))
            };

                var items = brushes.Select(x => new MenuItem() { DataContext = x, CommandParameter = x.Item, Command = command });
                items.ForEach(x => _colorMenu.Items.Add(x));
            }
        }

        #region Text

        public static readonly DependencyProperty HtmlProperty =
          DependencyProperty.Register(nameof(Html), typeof(string), typeof(RichTextEditor),
          new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHtmlChanged));

        public string Html
        {
            get => (string)GetValue(HtmlProperty);
            set => SetValue(HtmlProperty, value);
        }
        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextEditor richTextEditor || richTextEditor._textChanging) return;

            richTextEditor.Document.Blocks.Clear();

            if (GetSection(e.NewValue?.ToString()) is Section section)
                richTextEditor.Document.Blocks.Add(section);
        }


        #endregion

        #region AvailableFonts

        public static readonly DependencyProperty AvailableFontsProperty =
          DependencyProperty.Register("AvailableFonts", typeof(Collection<string>), typeof(RichTextEditor),
          new PropertyMetadata(new Collection<string>(
              [
                  "Arial",
                  "Courier New",
                  "Tahoma",
                  "Times New Roman"
              ]
        )));

        public Collection<string> AvailableFonts
        {
            get => (Collection<string>)GetValue(AvailableFontsProperty);
            set => SetValue(AvailableFontsProperty, value);
        }

        #endregion

        private static string GetHtmlText(FlowDocument? document)
        {
            if (document is null) return string.Empty;

            var tr = new TextRange(document.ContentStart, document.ContentEnd);

            using var ms = new MemoryStream();
            tr.Save(ms, DataFormats.Xaml);
            var xamlText = Encoding.UTF8.GetString(ms.ToArray());
            return HtmlFromXamlConverter.ConvertXamlToHtml(xamlText, false);
        }

        private static Section? GetSection(string? htmlText)
        {
            if (!string.IsNullOrEmpty(htmlText))
            {
                var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(htmlText, false);
                using var xamlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
                var parser = new ParserContext();
                parser.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                parser.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                return XamlReader.Load(xamlMemoryStream, parser) as Section;
            }

            return new();
        }

        private void FontSizeComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(_fontSizeComboBox?.Text, out var fontSize))
                Selection?.ApplyPropertyValue(FontSizeProperty, fontSize);
        }
    }
}
