// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reactive.Joins;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MyNet.Wpf.Syntax;

/// <summary>
/// Formats a string of code into <see cref="TextBox"/> control.
/// <para>Implementation and regex patterns inspired by <see href="https://github.com/antoniandre/simple-syntax-highlighter"/>.</para>
/// </summary>
public static class Highlighter
{
    private const string EndlinePattern = /* language=regex */ "(\n)";

    private const string TabPattern = /* language=regex */ "(\t)";

    private const string QuotePattern = /* language=regex */ "(\"(?:\\\"|[^\"])*\")|('(?:\\'|[^'])*')";

    private const string CommentPattern = /* language=regex */ @"(\/\/.*?(?:\n|$)|\/\*.*?\*\/)";

    private const string TagPattern = /* language=regex */ @"(<\/?)([a-zA-Z\-:]+)(.*?)(\/?>)";

    private const string EntityPattern = /* language=regex */ @"(&[a-zA-Z0-9#]+;)";

    public static TextBlock Format(string code, bool lightTheme = true)
    {
        var returnText = new TextBlock();

        var pattern = string.Empty;
        pattern += EndlinePattern;
        pattern += "|" + TabPattern;
        pattern += "|" + QuotePattern;
        pattern += "|" + CommentPattern;
        pattern += "|" + EntityPattern;
        pattern += "|" + TagPattern;

        Regex rgx = new(pattern);

        Group codeMatched;

        foreach (var match in rgx.Matches(code).OfType<Match>())
        {
            foreach (var group in match.Groups)
            {
                // Remove whole matches
                if (group is Match)
                    continue;

                // Cast to group
                codeMatched = (Group)group;

                // Remove empty groups
                if (string.IsNullOrEmpty(codeMatched.Value))
                    continue;

                if (codeMatched.Value.Contains('\t'))
                {
                    returnText.Inlines.Add(Line("  ", Brushes.Transparent));
                }
                else if (codeMatched.Value.Contains("/*") || codeMatched.Value.Contains("//"))
                {
                    returnText.Inlines.Add(Line(codeMatched.Value, Brushes.Orange));
                }
                else if (codeMatched.Value.Contains('<') || codeMatched.Value.Contains('>'))
                {
                    returnText.Inlines.Add(Line(codeMatched.Value,
                        lightTheme ? Brushes.DarkCyan : Brushes.CornflowerBlue));
                }
                else if (codeMatched.Value.Contains('"'))
                {
                    var attributeArray = codeMatched.Value.Split('"');
                    attributeArray = attributeArray.Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray();

                    if (attributeArray.Length % 2 == 0)
                    {
                        for (var i = 0; i < attributeArray.Length; i += 2)
                        {
                            returnText.Inlines.Add(Line(attributeArray[i],
                                lightTheme ? Brushes.DarkSlateGray : Brushes.WhiteSmoke));
                            returnText.Inlines.Add(Line("\"",
                                lightTheme ? Brushes.DarkCyan : Brushes.CornflowerBlue));
                            returnText.Inlines.Add(Line(attributeArray[i + 1], Brushes.Coral));
                            returnText.Inlines.Add(Line("\"",
                                lightTheme ? Brushes.DarkCyan : Brushes.CornflowerBlue));
                        }
                    }
                    else
                    {
                        returnText.Inlines.Add(Line(codeMatched.Value,
                            lightTheme ? Brushes.DarkSlateGray : Brushes.WhiteSmoke));
                    }
                }
                else if (codeMatched.Value.Contains('\''))
                {
                    var attributeArray = codeMatched.Value.Split('\'');
                    attributeArray = attributeArray.Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray();

                    if (attributeArray.Length % 2 == 0)
                    {
                        for (var i = 0; i < attributeArray.Length; i += 2)
                        {
                            returnText.Inlines.Add(Line(attributeArray[i],
                                lightTheme ? Brushes.DarkSlateGray : Brushes.WhiteSmoke));
                            returnText.Inlines.Add(
                                Line("'", lightTheme ? Brushes.DarkCyan : Brushes.CornflowerBlue));
                            returnText.Inlines.Add(Line(attributeArray[i + 1], Brushes.Coral));
                            returnText.Inlines.Add(
                                Line("'", lightTheme ? Brushes.DarkCyan : Brushes.CornflowerBlue));
                        }
                    }
                    else
                    {
                        returnText.Inlines.Add(Line(codeMatched.Value,
                            lightTheme ? Brushes.DarkSlateGray : Brushes.WhiteSmoke));
                    }
                }
                else
                {
                    returnText.Inlines.Add(Line(codeMatched.Value,
                        lightTheme ? Brushes.CornflowerBlue : Brushes.Aqua));
                }
            }
        }

        return returnText;
    }

    public static string Clean(string code)
    {
        code = code.Replace(@"\n", "\n");
        code = code.Replace(@"\t", "\t");
        code = code.Replace("&lt;", "<");
        code = code.Replace("&gt;", ">");
        code = code.Replace("&amp;", "&");
        code = code.Replace("&quot;", "\"");
        code = code.Replace("&apos;", "'");

        return code;
    }

    private static Run Line(string line, SolidColorBrush brush) => new(line) { Foreground = brush };
}
