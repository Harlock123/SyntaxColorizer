using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for TOML configuration files.
/// </summary>
public class TomlTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static TomlTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Boolean values
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },

            // Special float values
            { "inf", TokenType.Constant },
            { "nan", TokenType.Constant },
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Table headers [[array.of.tables]]
            new TokenPattern(@"\[\[[^\]]+\]\]", TokenType.TypeName, 95),

            // Table headers [table.name]
            new TokenPattern(@"\[[^\]]+\]", TokenType.TypeName, 90),

            // Multi-line literal strings (no escapes)
            new TokenPattern(@"'''[\s\S]*?'''", TokenType.String, 85),

            // Multi-line basic strings
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 85),

            // Literal strings (no escapes)
            new TokenPattern(@"'[^'\n]*'", TokenType.String, 80),

            // Basic strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 80),

            // DateTime (RFC 3339)
            new TokenPattern(@"\d{4}-\d{2}-\d{2}[T ]\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})?", TokenType.Number, 75),

            // Local date
            new TokenPattern(@"\d{4}-\d{2}-\d{2}", TokenType.Number, 74),

            // Local time
            new TokenPattern(@"\d{2}:\d{2}:\d{2}(?:\.\d+)?", TokenType.Number, 73),

            // Hexadecimal
            new TokenPattern(@"0x[0-9a-fA-F_]+", TokenType.Number, 70),

            // Octal
            new TokenPattern(@"0o[0-7_]+", TokenType.Number, 70),

            // Binary
            new TokenPattern(@"0b[01_]+", TokenType.Number, 70),

            // Float (with exponent or decimal)
            new TokenPattern(@"[+-]?(?:\d[\d_]*\.[\d_]*|\d[\d_]*[eE][+-]?\d[\d_]*|\.[\d_]+)", TokenType.Number, 70),

            // Integer
            new TokenPattern(@"[+-]?\d[\d_]*", TokenType.Number, 65),

            // Keys (bare keys)
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_-]*\s*(?==)", TokenType.Property, 50),

            // Dotted keys
            new TokenPattern(@"[a-zA-Z_][a-zA-Z0-9_-]*(?:\.[a-zA-Z_][a-zA-Z0-9_-]*)+\s*(?==)", TokenType.Property, 50),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_-]*\b", TokenType.Identifier, 30),

            // Operators
            new TokenPattern(@"[=]", TokenType.Operator, 20),

            // Punctuation
            new TokenPattern(@"[{}\[\],.]", TokenType.Punctuation, 10),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Toml;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
