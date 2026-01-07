using System.Text;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Formatting;

/// <summary>
/// Provides basic code formatting/indentation for supported languages.
/// </summary>
public static class BasicCodeFormatter
{
    /// <summary>
    /// Formats the given code with proper indentation.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <param name="language">The programming language.</param>
    /// <param name="indentSize">Number of spaces per indent level.</param>
    /// <param name="useSpaces">Whether to use spaces (true) or tabs (false).</param>
    /// <returns>The formatted code.</returns>
    public static string Format(string code, SyntaxLanguage language, int indentSize = 4, bool useSpaces = true)
    {
        if (string.IsNullOrEmpty(code))
            return code;

        return language switch
        {
            SyntaxLanguage.CSharp or
            SyntaxLanguage.Java or
            SyntaxLanguage.JavaScript or
            SyntaxLanguage.TypeScript or
            SyntaxLanguage.C or
            SyntaxLanguage.Cpp or
            SyntaxLanguage.Php or
            SyntaxLanguage.Rust or
            SyntaxLanguage.Kotlin or
            SyntaxLanguage.Swift or
            SyntaxLanguage.Scala => FormatCStyle(code, indentSize, useSpaces),

            SyntaxLanguage.Python => FormatPython(code, indentSize, useSpaces),

            SyntaxLanguage.VisualBasic => FormatVisualBasic(code, indentSize, useSpaces),

            SyntaxLanguage.MsSql or
            SyntaxLanguage.OracleSql => FormatSql(code, indentSize, useSpaces),

            SyntaxLanguage.Html => FormatHtml(code, indentSize, useSpaces),

            SyntaxLanguage.Css => FormatCss(code, indentSize, useSpaces),

            SyntaxLanguage.Json => FormatJson(code, indentSize, useSpaces),

            SyntaxLanguage.Yaml => FormatYaml(code, indentSize, useSpaces),

            SyntaxLanguage.Xml => FormatXml(code, indentSize, useSpaces),

            SyntaxLanguage.Bash => FormatBash(code, indentSize, useSpaces),

            SyntaxLanguage.PowerShell => FormatPowerShell(code, indentSize, useSpaces),

            SyntaxLanguage.Go => FormatGo(code, indentSize, useSpaces),

            SyntaxLanguage.Ruby => FormatRuby(code, indentSize, useSpaces),

            SyntaxLanguage.Dockerfile => FormatDockerfile(code, indentSize, useSpaces),

            SyntaxLanguage.Markdown => code, // Markdown formatting preserves original structure

            _ => code // No formatting for unknown languages
        };
    }

    private static string FormatCStyle(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Track if we're inside a multi-line comment or string
        var inMultiLineComment = false;
        var inVerbatimString = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmedLine = line.Trim();

            // Skip empty lines but preserve them
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Handle multi-line comment tracking
            if (!inVerbatimString)
            {
                if (trimmedLine.Contains("/*") && !trimmedLine.Contains("*/"))
                    inMultiLineComment = true;
                else if (trimmedLine.Contains("*/"))
                    inMultiLineComment = false;
            }

            // Handle verbatim string tracking (C#)
            if (trimmedLine.Contains("@\"") && !inMultiLineComment)
            {
                var atQuoteCount = CountOccurrences(trimmedLine, "@\"");
                var quoteCount = CountOccurrences(trimmedLine, "\"") - atQuoteCount;
                if (atQuoteCount > 0 && quoteCount % 2 != 0)
                    inVerbatimString = true;
            }
            else if (inVerbatimString && trimmedLine.Contains("\""))
            {
                inVerbatimString = false;
            }

            // Count braces for indent adjustment (only if not in comment/string)
            var openBraces = 0;
            var closeBraces = 0;

            if (!inMultiLineComment && !inVerbatimString)
            {
                CountBraces(trimmedLine, out openBraces, out closeBraces);
            }

            // Decrease indent BEFORE this line if it starts with closing brace
            var startsWithClose = trimmedLine.StartsWith('}') ||
                                   trimmedLine.StartsWith(')') && HasOnlyClosingParens(trimmedLine) ||
                                   trimmedLine.StartsWith(']');

            if (startsWithClose && indentLevel > 0)
            {
                indentLevel--;
            }

            // Apply current indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Adjust indent level for next line
            indentLevel += openBraces - closeBraces;

            // Re-add the indent we subtracted if line started with close brace
            // (because we already counted it in closeBraces)
            if (startsWithClose)
            {
                indentLevel++;
            }

            // Ensure indent level doesn't go negative
            if (indentLevel < 0)
                indentLevel = 0;
        }

        // Remove trailing newline if original didn't have one
        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static void CountBraces(string line, out int open, out int close)
    {
        open = 0;
        close = 0;

        var inString = false;
        var inChar = false;
        var stringChar = '\0';
        var escaped = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            // Handle escape sequences
            if (escaped)
            {
                escaped = false;
                continue;
            }

            if (c == '\\' && (inString || inChar))
            {
                escaped = true;
                continue;
            }

            // Handle string/char literals
            if (!inString && !inChar)
            {
                if (c == '"')
                {
                    inString = true;
                    stringChar = '"';
                    continue;
                }
                if (c == '\'')
                {
                    inChar = true;
                    stringChar = '\'';
                    continue;
                }

                // Check for single-line comment
                if (c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                {
                    // Rest of line is comment, stop counting
                    break;
                }
            }
            else
            {
                if (c == stringChar)
                {
                    inString = false;
                    inChar = false;
                    stringChar = '\0';
                }
                continue;
            }

            // Count braces only outside strings/comments
            if (c == '{')
                open++;
            else if (c == '}')
                close++;
        }
    }

    private static bool HasOnlyClosingParens(string line)
    {
        var trimmed = line.Trim();
        foreach (var c in trimmed)
        {
            if (c != ')' && c != ';' && c != ',' && !char.IsWhiteSpace(c))
                return false;
        }
        return trimmed.Contains(')');
    }

    private static int CountOccurrences(string text, string pattern)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(pattern, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += pattern.Length;
        }
        return count;
    }

    private static string FormatPython(string code, int indentSize, bool useSpaces)
    {
        // Python formatting is tricky because indentation IS the syntax
        // We'll just normalize existing indentation to use consistent spacing
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                result.AppendLine();
                continue;
            }

            // Count leading whitespace
            var leadingSpaces = 0;
            foreach (var c in line)
            {
                if (c == ' ')
                    leadingSpaces++;
                else if (c == '\t')
                    leadingSpaces += indentSize;
                else
                    break;
            }

            // Calculate indent level (assuming original used some consistent indentation)
            var indentLevel = leadingSpaces / indentSize;
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));

            result.AppendLine(currentIndent + line.TrimStart());
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatVisualBasic(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Keywords that increase indent
        var increaseIndentKeywords = new[]
        {
            "Sub ", "Function ", "If ", "ElseIf ", "Else", "For ", "While ", "Do ",
            "Select ", "Case ", "Try", "Catch", "Finally", "With ", "Using ",
            "Class ", "Structure ", "Module ", "Namespace ", "Interface ", "Enum ",
            "Property ", "Get", "Set"
        };

        // Keywords that decrease indent
        var decreaseIndentKeywords = new[]
        {
            "End Sub", "End Function", "End If", "Next", "Wend", "Loop",
            "End Select", "End Try", "End With", "End Using",
            "End Class", "End Structure", "End Module", "End Namespace",
            "End Interface", "End Enum", "End Property", "End Get", "End Set"
        };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Check for decrease indent keywords first
            var shouldDecrease = decreaseIndentKeywords.Any(k =>
                trimmedLine.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            if (shouldDecrease && indentLevel > 0)
                indentLevel--;

            // Check for mid-block keywords (Else, ElseIf, Case, Catch, Finally)
            var isMidBlock = trimmedLine.StartsWith("Else", StringComparison.OrdinalIgnoreCase) ||
                            trimmedLine.StartsWith("ElseIf", StringComparison.OrdinalIgnoreCase) ||
                            trimmedLine.StartsWith("Case ", StringComparison.OrdinalIgnoreCase) ||
                            trimmedLine.StartsWith("Catch", StringComparison.OrdinalIgnoreCase) ||
                            trimmedLine.StartsWith("Finally", StringComparison.OrdinalIgnoreCase);

            var outputIndent = indentLevel;
            if (isMidBlock && outputIndent > 0)
                outputIndent--;

            // Apply indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, outputIndent));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for increase indent keywords
            var shouldIncrease = increaseIndentKeywords.Any(k =>
                trimmedLine.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            // Special case: single-line If...Then without block
            if (trimmedLine.StartsWith("If ", StringComparison.OrdinalIgnoreCase) &&
                trimmedLine.Contains(" Then ", StringComparison.OrdinalIgnoreCase) &&
                !trimmedLine.EndsWith(" Then", StringComparison.OrdinalIgnoreCase))
            {
                shouldIncrease = false;
            }

            if (shouldIncrease)
                indentLevel++;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatSql(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // SQL keywords that typically start at base indent
        var baseKeywords = new[]
        {
            "SELECT", "FROM", "WHERE", "GROUP BY", "HAVING", "ORDER BY",
            "INSERT", "UPDATE", "DELETE", "CREATE", "ALTER", "DROP",
            "BEGIN", "END", "IF", "ELSE", "WHILE", "DECLARE", "SET"
        };

        // Keywords that increase indent
        var increaseKeywords = new[] { "BEGIN", "CASE" };
        var decreaseKeywords = new[] { "END", "END;" };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            var upperLine = trimmedLine.ToUpperInvariant();

            // Check for decrease
            if (decreaseKeywords.Any(k => upperLine.StartsWith(k)))
            {
                if (indentLevel > 0) indentLevel--;
            }

            // Apply indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for increase
            if (increaseKeywords.Any(k => upperLine.StartsWith(k) || upperLine.EndsWith(k)))
            {
                // Don't increase if this is BEGIN...END on same line
                if (!(upperLine.Contains("BEGIN") && upperLine.Contains("END")))
                    indentLevel++;
            }
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatHtml(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Self-closing and void elements that don't need closing tags
        var voidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input",
            "link", "meta", "param", "source", "track", "wbr"
        };

        // Inline elements that shouldn't affect indentation
        var inlineElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "abbr", "b", "bdo", "br", "cite", "code", "dfn", "em", "i",
            "img", "kbd", "q", "samp", "small", "span", "strong", "sub", "sup",
            "var", "button", "input", "label", "select", "textarea"
        };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Check for closing tags at start of line
            var closingTagMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"^</(\w+)");
            if (closingTagMatch.Success)
            {
                var tagName = closingTagMatch.Groups[1].Value;
                if (!inlineElements.Contains(tagName) && indentLevel > 0)
                    indentLevel--;
            }

            // Apply current indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for opening tags (increase indent)
            var openingTags = System.Text.RegularExpressions.Regex.Matches(trimmedLine, @"<(\w+)(?:\s[^>]*)?>(?!</)");
            var closingTags = System.Text.RegularExpressions.Regex.Matches(trimmedLine, @"</(\w+)>");

            foreach (System.Text.RegularExpressions.Match match in openingTags)
            {
                var tagName = match.Groups[1].Value;
                // Don't increase indent for void/self-closing or inline elements
                if (!voidElements.Contains(tagName) && !inlineElements.Contains(tagName))
                {
                    // Check if this tag is closed on the same line
                    var closedOnSameLine = closingTags.Cast<System.Text.RegularExpressions.Match>()
                        .Any(c => c.Groups[1].Value.Equals(tagName, StringComparison.OrdinalIgnoreCase));

                    // Check for self-closing syntax
                    var selfClosing = trimmedLine.Contains("/>");

                    if (!closedOnSameLine && !selfClosing)
                        indentLevel++;
                }
            }

            // Ensure indent level doesn't go negative
            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatCss(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Count braces for indent adjustment
            var openBraces = trimmedLine.Count(c => c == '{');
            var closeBraces = trimmedLine.Count(c => c == '}');

            // Decrease indent if line starts with closing brace
            if (trimmedLine.StartsWith('}') && indentLevel > 0)
            {
                indentLevel--;
            }

            // Apply current indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Adjust indent level for next line
            indentLevel += openBraces - closeBraces;

            // Re-add the indent we subtracted if line started with close brace
            if (trimmedLine.StartsWith('}'))
            {
                indentLevel++;
            }

            // Ensure indent level doesn't go negative
            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatJson(string code, int indentSize, bool useSpaces)
    {
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";
        var inString = false;
        var escaped = false;

        for (var i = 0; i < code.Length; i++)
        {
            var c = code[i];

            // Handle escape sequences in strings
            if (escaped)
            {
                result.Append(c);
                escaped = false;
                continue;
            }

            if (c == '\\' && inString)
            {
                result.Append(c);
                escaped = true;
                continue;
            }

            // Handle string boundaries
            if (c == '"')
            {
                inString = !inString;
                result.Append(c);
                continue;
            }

            // If inside string, just append
            if (inString)
            {
                result.Append(c);
                continue;
            }

            // Handle structural characters
            switch (c)
            {
                case '{':
                case '[':
                    result.Append(c);
                    indentLevel++;
                    result.AppendLine();
                    result.Append(string.Concat(Enumerable.Repeat(indent, indentLevel)));
                    break;

                case '}':
                case ']':
                    indentLevel = Math.Max(0, indentLevel - 1);
                    result.AppendLine();
                    result.Append(string.Concat(Enumerable.Repeat(indent, indentLevel)));
                    result.Append(c);
                    break;

                case ',':
                    result.Append(c);
                    result.AppendLine();
                    result.Append(string.Concat(Enumerable.Repeat(indent, indentLevel)));
                    break;

                case ':':
                    result.Append(c);
                    result.Append(' ');
                    break;

                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    // Skip whitespace outside strings (we're re-formatting)
                    break;

                default:
                    result.Append(c);
                    break;
            }
        }

        return result.ToString();
    }

    private static string FormatYaml(string code, int indentSize, bool useSpaces)
    {
        // YAML formatting is similar to Python - indentation is significant
        // We normalize the indentation while preserving the structure
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                result.AppendLine();
                continue;
            }

            // Count leading whitespace
            var leadingSpaces = 0;
            foreach (var c in line)
            {
                if (c == ' ')
                    leadingSpaces++;
                else if (c == '\t')
                    leadingSpaces += indentSize;
                else
                    break;
            }

            // Calculate indent level (YAML commonly uses 2-space indentation)
            var indentLevel = leadingSpaces / 2; // Assume 2-space original
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));

            result.AppendLine(currentIndent + line.TrimStart());
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatXml(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Self-closing elements
        var selfClosingPattern = new System.Text.RegularExpressions.Regex(@"<[\w:-]+[^>]*/>");

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Check for closing tags at start of line
            var closingTagMatch = System.Text.RegularExpressions.Regex.Match(trimmedLine, @"^</[\w:-]+");
            if (closingTagMatch.Success && indentLevel > 0)
            {
                indentLevel--;
            }

            // Apply current indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for opening tags (increase indent)
            var openingTags = System.Text.RegularExpressions.Regex.Matches(trimmedLine, @"<[\w:-]+(?:\s[^>]*)?>(?!</)");
            var closingTags = System.Text.RegularExpressions.Regex.Matches(trimmedLine, @"</[\w:-]+>");

            foreach (System.Text.RegularExpressions.Match match in openingTags)
            {
                // Skip self-closing tags and processing instructions
                if (match.Value.EndsWith("/>") || match.Value.StartsWith("<?") || match.Value.StartsWith("<!"))
                    continue;

                var tagName = System.Text.RegularExpressions.Regex.Match(match.Value, @"<([\w:-]+)").Groups[1].Value;

                // Check if this tag is closed on the same line
                var closedOnSameLine = closingTags.Cast<System.Text.RegularExpressions.Match>()
                    .Any(c => c.Value.Contains(tagName));

                if (!closedOnSameLine)
                    indentLevel++;
            }

            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatBash(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Keywords that increase indent
        var increaseKeywords = new[] { "then", "do", "{", "(" };
        var decreaseKeywords = new[] { "fi", "done", "esac", "}", ")" };
        var midBlockKeywords = new[] { "else", "elif" };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Skip comments for structure analysis
            var isComment = trimmedLine.StartsWith('#');

            // Check for decrease keywords
            var startsWithDecrease = !isComment && decreaseKeywords.Any(k =>
                trimmedLine.StartsWith(k) || trimmedLine == k);

            if (startsWithDecrease && indentLevel > 0)
            {
                indentLevel--;
            }

            // Check for mid-block keywords
            var isMidBlock = !isComment && midBlockKeywords.Any(k =>
                trimmedLine.StartsWith(k + " ") || trimmedLine == k);

            var outputIndent = indentLevel;
            if (isMidBlock && outputIndent > 0)
                outputIndent--;

            // Apply indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, outputIndent));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for increase keywords
            if (!isComment)
            {
                var endsWithIncrease = increaseKeywords.Any(k =>
                    trimmedLine.EndsWith(k) || trimmedLine.EndsWith(k + ";"));

                // Also check for case patterns ending with )
                if (trimmedLine.EndsWith(")") && !trimmedLine.Contains("$(") && !trimmedLine.Contains("$(("))
                {
                    // Could be a case pattern
                    if (System.Text.RegularExpressions.Regex.IsMatch(trimmedLine, @"^[\w\s|*?-]+\)$"))
                        endsWithIncrease = true;
                }

                if (endsWithIncrease)
                    indentLevel++;
            }

            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatPowerShell(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Keywords that increase indent
        var increaseKeywords = new[] { "{", "(" };
        var decreaseKeywords = new[] { "}", ")" };

        // Block keywords
        var blockStartKeywords = new[]
        {
            "if", "else", "elseif", "switch", "while", "for", "foreach", "do",
            "try", "catch", "finally", "function", "filter", "begin", "process", "end"
        };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Skip comments for structure analysis
            var isComment = trimmedLine.StartsWith('#') && !trimmedLine.StartsWith("#>");

            // Check for decrease keywords at start
            var startsWithDecrease = !isComment &&
                (trimmedLine.StartsWith('}') || trimmedLine.StartsWith(')'));

            if (startsWithDecrease && indentLevel > 0)
            {
                indentLevel--;
            }

            // Check for mid-block keywords
            var isMidBlock = !isComment &&
                (trimmedLine.StartsWith("else", StringComparison.OrdinalIgnoreCase) ||
                 trimmedLine.StartsWith("elseif", StringComparison.OrdinalIgnoreCase) ||
                 trimmedLine.StartsWith("catch", StringComparison.OrdinalIgnoreCase) ||
                 trimmedLine.StartsWith("finally", StringComparison.OrdinalIgnoreCase));

            var outputIndent = indentLevel;
            if (isMidBlock && outputIndent > 0)
                outputIndent--;

            // Apply indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, outputIndent));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for increase keywords at end
            if (!isComment)
            {
                var endsWithIncrease = trimmedLine.EndsWith('{') || trimmedLine.EndsWith('(');

                if (endsWithIncrease)
                    indentLevel++;
            }

            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatGo(string code, int indentSize, bool useSpaces)
    {
        // Go uses C-style braces for blocks
        // Go also uses tabs by default (gofmt standard), but we respect the useSpaces parameter
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Track if we're inside a multi-line comment or string
        var inMultiLineComment = false;
        var inRawString = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmedLine = line.Trim();

            // Skip empty lines but preserve them
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Handle multi-line comment tracking
            if (!inRawString)
            {
                if (trimmedLine.Contains("/*") && !trimmedLine.Contains("*/"))
                    inMultiLineComment = true;
                else if (trimmedLine.Contains("*/"))
                    inMultiLineComment = false;
            }

            // Handle raw string tracking (backtick strings in Go)
            if (trimmedLine.Contains('`') && !inMultiLineComment)
            {
                var backtickCount = trimmedLine.Count(c => c == '`');
                if (backtickCount % 2 != 0)
                    inRawString = !inRawString;
            }

            // Count braces for indent adjustment (only if not in comment/string)
            var openBraces = 0;
            var closeBraces = 0;

            if (!inMultiLineComment && !inRawString)
            {
                CountBraces(trimmedLine, out openBraces, out closeBraces);
            }

            // Decrease indent BEFORE this line if it starts with closing brace
            var startsWithClose = trimmedLine.StartsWith('}') || trimmedLine.StartsWith(')');

            if (startsWithClose && indentLevel > 0)
            {
                indentLevel--;
            }

            // Apply current indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Adjust indent level for next line
            indentLevel += openBraces - closeBraces;

            // Re-add the indent we subtracted if line started with close brace
            if (startsWithClose)
            {
                indentLevel++;
            }

            // Ensure indent level doesn't go negative
            if (indentLevel < 0)
                indentLevel = 0;
        }

        // Remove trailing newline if original didn't have one
        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatRuby(string code, int indentSize, bool useSpaces)
    {
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indentLevel = 0;
        var indent = useSpaces ? new string(' ', indentSize) : "\t";

        // Keywords that increase indent
        var increaseKeywords = new[]
        {
            "def", "class", "module", "if", "unless", "case", "while", "until",
            "for", "begin", "do", "elsif", "else", "when", "rescue", "ensure"
        };

        // Keywords that decrease indent
        var decreaseKeywords = new[] { "end", "elsif", "else", "when", "rescue", "ensure" };

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // Skip comments for structure analysis
            var isComment = trimmedLine.StartsWith('#');

            // Check for decrease keywords at start
            var startsWithDecrease = !isComment && decreaseKeywords.Any(k =>
                trimmedLine == k ||
                trimmedLine.StartsWith(k + " ") ||
                trimmedLine.StartsWith(k + "\t"));

            if (startsWithDecrease && indentLevel > 0)
            {
                indentLevel--;
            }

            // Apply indentation
            var currentIndent = string.Concat(Enumerable.Repeat(indent, indentLevel));
            result.AppendLine(currentIndent + trimmedLine);

            // Check for increase keywords at start or end
            if (!isComment)
            {
                var shouldIncrease = false;

                // Check for block-starting keywords
                foreach (var kw in increaseKeywords)
                {
                    if (trimmedLine == kw ||
                        trimmedLine.StartsWith(kw + " ") ||
                        trimmedLine.StartsWith(kw + "\t") ||
                        trimmedLine.EndsWith(" " + kw) ||
                        trimmedLine.EndsWith("\t" + kw))
                    {
                        shouldIncrease = true;
                        break;
                    }
                }

                // Check for block with do
                if (trimmedLine.EndsWith(" do") || trimmedLine.EndsWith("|"))
                {
                    shouldIncrease = true;
                }

                // Don't increase for single-line if/unless/while with then or modifier form
                if (trimmedLine.Contains(" then ") && !trimmedLine.EndsWith(" then"))
                {
                    shouldIncrease = false;
                }

                // Check for one-liner (ends with end)
                if (trimmedLine.EndsWith(" end") || trimmedLine == "end")
                {
                    shouldIncrease = false;
                }

                if (shouldIncrease)
                    indentLevel++;
            }

            if (indentLevel < 0)
                indentLevel = 0;
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }

    private static string FormatDockerfile(string code, int indentSize, bool useSpaces)
    {
        // Dockerfile formatting is relatively simple - mainly handle line continuations
        var lines = code.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
        var result = new StringBuilder();
        var indent = useSpaces ? new string(' ', indentSize) : "\t";
        var inContinuation = false;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                inContinuation = false;
                continue;
            }

            // Apply indentation for continuation lines
            if (inContinuation)
            {
                result.AppendLine(indent + trimmedLine);
            }
            else
            {
                result.AppendLine(trimmedLine);
            }

            // Check if line continues (ends with backslash)
            inContinuation = trimmedLine.EndsWith('\\');
        }

        var formatted = result.ToString();
        if (!code.EndsWith('\n') && formatted.EndsWith(Environment.NewLine))
        {
            formatted = formatted.TrimEnd('\r', '\n');
        }

        return formatted;
    }
}
