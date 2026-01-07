using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for JSON data format.
/// </summary>
public class JsonTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static JsonTokenizer()
    {
        // JSON keywords (literal values)
        _keywords = new Dictionary<string, TokenType>
        {
            { "true", TokenType.Keyword },
            { "false", TokenType.Keyword },
            { "null", TokenType.Keyword }
        };

        _patterns = new List<TokenPattern>
        {
            // Strings (double quoted only in JSON)
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 100),

            // Property keys (string followed by colon) - handled specially in Tokenize override
            // We'll mark strings before colons as keys

            // Numbers (integer, decimal, scientific notation)
            new TokenPattern(@"-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?", TokenType.Number, 80),

            // Boolean and null literals
            new TokenPattern(@"\b(true|false|null)\b", TokenType.Keyword, 70),

            // Structural characters
            new TokenPattern(@"[{}\[\]:,]", TokenType.Punctuation, 60),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Json;
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

                // Check if this string is a property key (followed by colon)
                if (matched.Type == TokenType.String)
                {
                    var afterString = matched.StartIndex + matched.Length;
                    var remaining = text.AsSpan(afterString);
                    var colonIndex = 0;

                    // Skip whitespace to find colon
                    while (colonIndex < remaining.Length && char.IsWhiteSpace(remaining[colonIndex]))
                        colonIndex++;

                    if (colonIndex < remaining.Length && remaining[colonIndex] == ':')
                    {
                        // This is a property key
                        yield return new Token(matched.StartIndex, matched.Length, TokenType.JsonKey);
                    }
                    else
                    {
                        yield return matched;
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
}
