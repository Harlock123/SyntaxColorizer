using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for XML markup language.
/// </summary>
public class XmlTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static XmlTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>();

        _patterns = new List<TokenPattern>
        {
            // XML declaration <?xml ... ?>
            new TokenPattern(@"<\?xml[^?]*\?>", TokenType.Preprocessor, 100),

            // Processing instructions <?target ... ?>
            new TokenPattern(@"<\?[\w-]+[^?]*\?>", TokenType.Preprocessor, 95),

            // CDATA sections
            new TokenPattern(@"<!\[CDATA\[[\s\S]*?\]\]>", TokenType.String, 90),

            // Comments
            new TokenPattern(@"<!--[\s\S]*?-->", TokenType.Comment, 85),

            // DOCTYPE
            new TokenPattern(@"<!DOCTYPE[^>]*>", TokenType.Preprocessor, 80),

            // Self-closing tags <tag />
            new TokenPattern(@"<[\w:-]+(?:\s+[\w:-]+\s*=\s*(?:""[^""]*""|'[^']*'))*\s*/>", TokenType.XmlTag, 75),

            // Opening tags <tag>
            new TokenPattern(@"<[\w:-]+(?:\s+[\w:-]+\s*=\s*(?:""[^""]*""|'[^']*'))*\s*>", TokenType.XmlTag, 70),

            // Closing tags </tag>
            new TokenPattern(@"</[\w:-]+\s*>", TokenType.XmlTag, 70),

            // Entity references
            new TokenPattern(@"&[\w#]+;", TokenType.Constant, 60),

            // Text content
            new TokenPattern(@"[^<>&]+", TokenType.PlainText, 10),

            // Punctuation
            new TokenPattern(@"[<>&]", TokenType.Punctuation, 5)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Xml;
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

                // For tag matches, break them down into components
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
            // Opening bracket(s)
            yield return new Token(offset, tagNameMatch.Groups[1].Length, TokenType.Punctuation);

            // Tag name
            var nameStart = offset + tagNameMatch.Groups[2].Index;
            yield return new Token(nameStart, tagNameMatch.Groups[2].Length, TokenType.XmlTag);

            var pos = tagNameMatch.Length;

            // Parse attributes
            var attrPattern = new Regex(@"\s+([\w:-]+)(?:\s*=\s*(""[^""]*""|'[^']*'))?");
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
            // Fallback
            yield return new Token(offset, tagText.Length, TokenType.XmlTag);
        }
    }
}
