// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using MyNet.UI.Theming;

namespace MyNet.Wpf.Controls;

/// <summary>
/// Formats and display a fragment of the source code.
/// </summary>
[ToolboxItem(true)]
public class CodeBlock : System.Windows.Controls.ContentControl
{
    static CodeBlock() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CodeBlock), new FrameworkPropertyMetadata(typeof(CodeBlock)));

    /// <summary>
    /// Property for <see cref="SyntaxContent"/>.
    /// </summary>
    public static readonly DependencyProperty SyntaxContentProperty = DependencyProperty.Register(nameof(SyntaxContent),
        typeof(object), typeof(CodeBlock),
        new PropertyMetadata(null));

    /// <summary>
    /// Formatted <see cref="System.Windows.Controls.ContentControl.Content"/>.
    /// </summary>
    public object SyntaxContent
    {
        get => GetValue(SyntaxContentProperty);
        internal set => SetValue(SyntaxContentProperty, value);
    }

    /// <summary>
    /// Creates new instance and assigns <see cref="ButtonCommand"/> default action.
    /// </summary>
    public CodeBlock() => ThemeManager.ThemeChanged += ThemeOnChanged;

    private void ThemeOnChanged(object? sender, ThemeChangedEventArgs e) => UpdateSyntax(e.CurrentTheme.Base == ThemeBase.Light);

    /// <summary>
    /// This method is invoked when the Content property changes.
    /// </summary>
    /// <param name="oldContent">The old value of the Content property.</param>
    /// <param name="newContent">The new value of the Content property.</param>
    protected override void OnContentChanged(object oldContent, object newContent) => UpdateSyntax(ThemeManager.CurrentTheme?.Base == ThemeBase.Light);

    protected virtual void UpdateSyntax(bool isLightTheme) => SyntaxContent = Syntax.Highlighter.Format(Syntax.Highlighter.Clean(Content as string ?? string.Empty), isLightTheme);
}
