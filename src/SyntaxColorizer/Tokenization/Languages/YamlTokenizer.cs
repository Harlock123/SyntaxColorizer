using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for YAML data format.
/// </summary>
public class YamlTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static YamlTokenizer()
    {
        // YAML keywords (literal values)
        _keywords = new Dictionary<string, TokenType>
        {
            { "true", TokenType.Keyword },
            { "false", TokenType.Keyword },
            { "yes", TokenType.Keyword },
            { "no", TokenType.Keyword },
            { "on", TokenType.Keyword },
            { "off", TokenType.Keyword },
            { "null", TokenType.Keyword },
            { "~", TokenType.Keyword }
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Document markers
            new TokenPattern(@"^---\s*$", TokenType.Punctuation, 95, RegexOptions.Multiline),
            new TokenPattern(@"^\.\.\.\s*$", TokenType.Punctuation, 95, RegexOptions.Multiline),

            // Anchors (&name)
            new TokenPattern(@"&[\w-]+", TokenType.YamlAnchor, 90),

            // Aliases (*name)
            new TokenPattern(@"\*[\w-]+", TokenType.YamlAlias, 90),

            // Tags (!!type or !custom)
            new TokenPattern(@"!![^\s]+|![^\s!][^\s]*", TokenType.YamlTag, 85),

            // Multi-line string indicators
            new TokenPattern(@"[|>][+-]?", TokenType.Operator, 80),

            // Double-quoted strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 75),

            // Single-quoted strings
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 75),

            // Keys (word followed by colon, or quoted string followed by colon)
            new TokenPattern(@"[\w][\w\s-]*(?=\s*:)", TokenType.JsonKey, 70),

            // Boolean and null literals
            new TokenPattern(@"\b(true|false|yes|no|on|off|null)\b", TokenType.Keyword, 65, RegexOptions.IgnoreCase),

            // Numbers (integer, decimal, scientific, hex, octal)
            new TokenPattern(@"-?(?:0x[\da-fA-F]+|0o[0-7]+|0b[01]+|(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?)", TokenType.Number, 60),

            // Special float values
            new TokenPattern(@"\.(inf|Inf|INF|nan|NaN|NAN)", TokenType.Number, 60),

            // Timestamps
            new TokenPattern(@"\d{4}-\d{2}-\d{2}(?:[ T]\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})?)?", TokenType.Number, 55),

            // Structural characters
            new TokenPattern(@"[{}\[\]:,\-?]", TokenType.Punctuation, 50),

            // Unquoted strings/values (anything else that looks like a value)
            new TokenPattern(@"[^\s#:{}\[\],&*!|>'""]+", TokenType.String, 40),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Yaml;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
