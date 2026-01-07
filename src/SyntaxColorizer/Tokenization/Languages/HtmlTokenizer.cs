using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for HTML markup.
/// </summary>
public class HtmlTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static HtmlTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>();

        _patterns = new List<TokenPattern>
        {
            // HTML comments
            new TokenPattern(@"<!--[\s\S]*?-->", TokenType.Comment, 100),

            // DOCTYPE
            new TokenPattern(@"<!DOCTYPE[^>]*>", TokenType.Preprocessor, 95, RegexOptions.IgnoreCase),

            // Script tags with content (treat content as string for now)
            new TokenPattern(@"<script\b[^>]*>[\s\S]*?</script>", TokenType.XmlTag, 90, RegexOptions.IgnoreCase),

            // Style tags with content
            new TokenPattern(@"<style\b[^>]*>[\s\S]*?</style>", TokenType.XmlTag, 90, RegexOptions.IgnoreCase),

            // Self-closing tags
            new TokenPattern(@"<\s*[\w:-]+(?:\s+[\w:-]+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+))*\s*/\s*>", TokenType.XmlTag, 85),

            // Opening tags with attributes
            new TokenPattern(@"<\s*[\w:-]+(?:\s+[\w:-]+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+))*\s*>", TokenType.XmlTag, 80),

            // Closing tags
            new TokenPattern(@"</\s*[\w:-]+\s*>", TokenType.XmlTag, 80),

            // Attribute values (double quoted)
            new TokenPattern(@"""[^""]*""", TokenType.XmlAttributeValue, 70),

            // Attribute values (single quoted)
            new TokenPattern(@"'[^']*'", TokenType.XmlAttributeValue, 70),

            // HTML entities
            new TokenPattern(@"&(?:#\d+|#x[\da-fA-F]+|[a-zA-Z]+);", TokenType.Constant, 60),

            // Common punctuation
            new TokenPattern(@"[<>=/]", TokenType.Punctuation, 10),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Html;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;

    public override IEnumerable<Token> Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        var position = 0;

        while (position < text.Length)
        {
            Token? bestMatch = null;
            var bestPriority = -1;

            foreach (var pattern in Patterns)
            {
                var match = pattern.Regex.Match(text, position);
                if (match.Success && match.Index == position && pattern.Priority > bestPriority)
                {
                    bestMatch = new Token(match.Index, match.Length, pattern.Type);
                    bestPriority = pattern.Priority;
                }
            }

            if (bestMatch.HasValue)
            {
                var matched = bestMatch.Value;
                // For tag matches, we can break them down further
                if (matched.Type == TokenType.XmlTag)
                {
                    foreach (var token in TokenizeTag(text.Substring(matched.StartIndex, matched.Length), matched.StartIndex))
                    {
                        yield return token;
                    }
                }
                else
                {
                    yield return matched;
                }
                position = matched.StartIndex + matched.Length;
            }
            else
            {
                // No pattern matched, emit single character as plain text
                yield return new Token(position, 1, TokenType.PlainText);
                position++;
            }
        }
    }

    private IEnumerable<Token> TokenizeTag(string tagText, int offset)
    {
        // Match the tag structure
        var tagNameMatch = Regex.Match(tagText, @"^(</?)\s*([\w:-]+)");
        if (tagNameMatch.Success)
        {
            // Opening bracket
            yield return new Token(offset, tagNameMatch.Groups[1].Length, TokenType.Punctuation);

            // Tag name
            var nameStart = offset + tagNameMatch.Groups[2].Index;
            yield return new Token(nameStart, tagNameMatch.Groups[2].Length, TokenType.XmlTag);

            var pos = tagNameMatch.Length;

            // Parse attributes
            var attrPattern = new Regex(@"\s+([\w:-]+)(?:\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+))?");
            var attrMatches = attrPattern.Matches(tagText, pos);

            foreach (Match attrMatch in attrMatches)
            {
                // Attribute name
                var attrNameGroup = attrMatch.Groups[1];
                yield return new Token(offset + attrNameGroup.Index, attrNameGroup.Length, TokenType.XmlAttribute);

                // Attribute value if present
                if (attrMatch.Groups[2].Success)
                {
                    var attrValueGroup = attrMatch.Groups[2];
                    // Find the = sign
                    var eqPos = tagText.IndexOf('=', attrNameGroup.Index + attrNameGroup.Length);
                    if (eqPos >= 0 && eqPos < attrValueGroup.Index)
                    {
                        yield return new Token(offset + eqPos, 1, TokenType.Punctuation);
                    }
                    yield return new Token(offset + attrValueGroup.Index, attrValueGroup.Length, TokenType.XmlAttributeValue);
                }

                pos = attrMatch.Index + attrMatch.Length;
            }

            // Closing bracket(s)
            var closeMatch = Regex.Match(tagText, @"\s*(/?)>$");
            if (closeMatch.Success)
            {
                if (closeMatch.Groups[1].Length > 0)
                {
                    yield return new Token(offset + closeMatch.Groups[1].Index, 1, TokenType.Punctuation);
                }
                yield return new Token(offset + tagText.Length - 1, 1, TokenType.Punctuation);
            }
        }
        else
        {
            // Fallback - return as single tag token
            yield return new Token(offset, tagText.Length, TokenType.XmlTag);
        }
    }
}
