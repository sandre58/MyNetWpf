// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using MaterialDesignThemes.Wpf;
using MyNet.Wpf.Extensions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    /// <summary>
    /// Represents a specialized context menu that contains commands for editing text.
    /// </summary>
    public class TextContextMenu : ContextMenu
    {
        private readonly MenuItem _spellCheckingMenuItem;

        static TextContextMenu() => DefaultStyleKeyProperty.OverrideMetadata(typeof(TextContextMenu), new FrameworkPropertyMetadata(typeof(TextContextMenu)));

        /// <summary>
        /// Initializes a new instance of the TextContextMenu class.
        /// </summary>
        public TextContextMenu()
        {
            _spellCheckingMenuItem = new MenuItem
            {
                Visibility = Visibility.Collapsed,
                Icon = new PackIcon { Kind = PackIconKind.Spellcheck },
            };
            _spellCheckingMenuItem.SetTranslatableResourceBinding(HeaderedItemsControl.HeaderProperty, "SpellChecking");

            var proofingSeparator = new Separator();
            _ = proofingSeparator.SetBinding(VisibilityProperty, new Binding
            {
                Path = new PropertyPath(VisibilityProperty),
                Source = _spellCheckingMenuItem
            });

            _ = Items.Add(_spellCheckingMenuItem);
            _ = Items.Add(proofingSeparator);
            AddItem(ApplicationCommands.Cut, PackIconKind.ContentCut);
            AddItem(ApplicationCommands.Copy, PackIconKind.ContentCopy);
            AddItem(ApplicationCommands.Paste, PackIconKind.ContentPaste);
            AddSeparator();
            AddItem(ApplicationCommands.Undo, PackIconKind.Undo);
            AddItem(ApplicationCommands.Redo, PackIconKind.Redo);
            AddSeparator();
            AddItem(ApplicationCommands.SelectAll, PackIconKind.SelectAll);
        }

        private void AddSeparator() => Items.Add(new Separator());

        private void AddItem(RoutedUICommand command, PackIconKind kind)
        {
            var item = new MenuItem
            {
                Command = command,
                Icon = new PackIcon { Kind = kind },
            };
            item.SetTranslatableResourceBinding(HeaderedItemsControl.HeaderProperty, command.Name);

            Items.Add(item);
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);

            var index = Items.IndexOf(_spellCheckingMenuItem);

            // Remove default spell cheking (before my spell checking item)
            if (index > 0)
            {
                for (var i = index - 1; i >= 0; i--)
                {
                    Items.RemoveAt(i);
                }
            }

            UpdateSpellCheckingMenuItem(PlacementTarget as Control);
        }

        private void UpdateSpellCheckingMenuItem(Control? target)
        {
            _spellCheckingMenuItem.Items.Clear();

            SpellingError? spellingError = null;

            if (target is TextBox textBox)
            {
                spellingError = textBox.GetSpellingError(textBox.CaretIndex);
            }
            else if (target is RichTextBox richTextBox)
            {
                spellingError = richTextBox.GetSpellingError(richTextBox.CaretPosition);
            }

            if (spellingError != null)
            {
                if (spellingError.Suggestions.Any())
                {
                    foreach (var suggestion in spellingError.Suggestions)
                    {
                        var menuItem = new MenuItem
                        {
                            Header = suggestion,
                            Command = EditingCommands.CorrectSpellingError,
                            CommandParameter = suggestion,
                            CommandTarget = target
                        };
                        _ = _spellCheckingMenuItem.Items.Add(menuItem);
                    }

                }
                else
                {
                    var nothingMenuItem = new MenuItem
                    {
                        IsEnabled = false
                    };
                    nothingMenuItem.SetTranslatableResourceBinding(HeaderedItemsControl.HeaderProperty, "NoSpellSuggestions");
                    _ = _spellCheckingMenuItem.Items.Add(nothingMenuItem);
                }

                _ = _spellCheckingMenuItem.Items.Add(new Separator());

                var ignoreAllMenuItem = new MenuItem
                {
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandTarget = target
                };
                ignoreAllMenuItem.SetTranslatableResourceBinding(HeaderedItemsControl.HeaderProperty, "IgnoreAll");

                _ = _spellCheckingMenuItem.Items.Add(ignoreAllMenuItem);

                _spellCheckingMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _spellCheckingMenuItem.Visibility = Visibility.Collapsed;
            }
        }

    }
}
