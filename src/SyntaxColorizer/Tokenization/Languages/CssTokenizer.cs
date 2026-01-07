using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for CSS stylesheets.
/// </summary>
public class CssTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static CssTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>();

        _patterns = new List<TokenPattern>
        {
            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.Comment, 100),

            // Single-line comments (non-standard but common in preprocessors)
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 95),

            // @-rules (import, media, keyframes, etc.)
            new TokenPattern(@"@[\w-]+", TokenType.Keyword, 90),

            // ID selectors
            new TokenPattern(@"#[\w-]+", TokenType.CssSelector, 85),

            // Class selectors
            new TokenPattern(@"\.[\w-]+", TokenType.CssSelector, 85),

            // Pseudo-classes and pseudo-elements
            new TokenPattern(@"::?[\w-]+(?:\([^)]*\))?", TokenType.CssSelector, 80),

            // Attribute selectors
            new TokenPattern(@"\[[^\]]+\]", TokenType.CssSelector, 80),

            // URL function
            new TokenPattern(@"url\s*\([^)]*\)", TokenType.String, 75),

            // Strings (double quoted)
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 70),

            // Strings (single quoted)
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 70),

            // Hex colors
            new TokenPattern(@"#(?:[0-9a-fA-F]{8}|[0-9a-fA-F]{6}|[0-9a-fA-F]{4}|[0-9a-fA-F]{3})\b", TokenType.Number, 65),

            // Numbers with units
            new TokenPattern(@"-?(?:\d+\.?\d*|\.\d+)(?:px|em|rem|%|vh|vw|vmin|vmax|ch|ex|cm|mm|in|pt|pc|deg|rad|grad|turn|s|ms|Hz|kHz|dpi|dpcm|dppx)\b", TokenType.CssUnit, 60),

            // Plain numbers
            new TokenPattern(@"-?(?:\d+\.?\d*|\.\d+)", TokenType.Number, 55),

            // CSS functions (calc, rgb, rgba, hsl, var, etc.)
            new TokenPattern(@"[\w-]+\s*\(", TokenType.Method, 50),

            // !important
            new TokenPattern(@"!important\b", TokenType.Keyword, 45, RegexOptions.IgnoreCase),

            // Property names (followed by colon)
            new TokenPattern(@"[\w-]+(?=\s*:)", TokenType.CssProperty, 40),

            // CSS variables
            new TokenPattern(@"--[\w-]+", TokenType.Field, 35),

            // Element/tag selectors and property values (identifiers)
            new TokenPattern(@"[\w-]+", TokenType.CssValue, 30),

            // Operators and punctuation
            new TokenPattern(@"[{}();:,>+~*\/=]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Css;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
