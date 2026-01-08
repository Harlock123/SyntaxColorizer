using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for SCSS/Sass/Less CSS preprocessors.
/// </summary>
public class ScssTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static ScssTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // At-rules (SCSS/Sass)
            { "@import", TokenType.Keyword },
            { "@use", TokenType.Keyword },
            { "@forward", TokenType.Keyword },
            { "@mixin", TokenType.Keyword },
            { "@include", TokenType.Keyword },
            { "@function", TokenType.Keyword },
            { "@return", TokenType.Keyword },
            { "@extend", TokenType.Keyword },
            { "@at-root", TokenType.Keyword },
            { "@error", TokenType.Keyword },
            { "@warn", TokenType.Keyword },
            { "@debug", TokenType.Keyword },
            { "@if", TokenType.ControlKeyword },
            { "@else", TokenType.ControlKeyword },
            { "@each", TokenType.ControlKeyword },
            { "@for", TokenType.ControlKeyword },
            { "@while", TokenType.ControlKeyword },
            { "@content", TokenType.Keyword },

            // CSS at-rules
            { "@media", TokenType.Keyword },
            { "@keyframes", TokenType.Keyword },
            { "@font-face", TokenType.Keyword },
            { "@supports", TokenType.Keyword },
            { "@page", TokenType.Keyword },
            { "@charset", TokenType.Keyword },
            { "@namespace", TokenType.Keyword },
            { "@layer", TokenType.Keyword },
            { "@container", TokenType.Keyword },
            { "@property", TokenType.Keyword },
            { "@scope", TokenType.Keyword },

            // Less specific
            { "@plugin", TokenType.Keyword },
            { "@import-once", TokenType.Keyword },

            // Boolean values
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // CSS keywords
            { "important", TokenType.Keyword },
            { "default", TokenType.Keyword },
            { "global", TokenType.Keyword },
            { "optional", TokenType.Keyword },
            { "from", TokenType.Keyword },
            { "to", TokenType.Keyword },
            { "through", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "only", TokenType.Keyword },

            // Common values
            { "inherit", TokenType.Constant },
            { "initial", TokenType.Constant },
            { "unset", TokenType.Constant },
            { "revert", TokenType.Constant },
            { "none", TokenType.Constant },
            { "auto", TokenType.Constant },
            { "transparent", TokenType.Constant },
            { "currentColor", TokenType.Constant },

            // Common functions
            { "rgb", TokenType.Method },
            { "rgba", TokenType.Method },
            { "hsl", TokenType.Method },
            { "hsla", TokenType.Method },
            { "url", TokenType.Method },
            { "calc", TokenType.Method },
            { "var", TokenType.Method },
            { "min", TokenType.Method },
            { "max", TokenType.Method },
            { "clamp", TokenType.Method },
            { "attr", TokenType.Method },

            // SCSS/Sass functions
            { "lighten", TokenType.Method },
            { "darken", TokenType.Method },
            { "saturate", TokenType.Method },
            { "desaturate", TokenType.Method },
            { "mix", TokenType.Method },
            { "adjust-hue", TokenType.Method },
            { "complement", TokenType.Method },
            { "invert", TokenType.Method },
            { "grayscale", TokenType.Method },
            { "percentage", TokenType.Method },
            { "round", TokenType.Method },
            { "ceil", TokenType.Method },
            { "floor", TokenType.Method },
            { "abs", TokenType.Method },
            { "length", TokenType.Method },
            { "nth", TokenType.Method },
            { "join", TokenType.Method },
            { "append", TokenType.Method },
            { "zip", TokenType.Method },
            { "index", TokenType.Method },
            { "map-get", TokenType.Method },
            { "map-merge", TokenType.Method },
            { "map-keys", TokenType.Method },
            { "map-values", TokenType.Method },
            { "map-has-key", TokenType.Method },
            { "type-of", TokenType.Method },
            { "unit", TokenType.Method },
            { "unitless", TokenType.Method },
            { "comparable", TokenType.Method },
            { "if", TokenType.Method },
            { "unquote", TokenType.Method },
            { "quote", TokenType.Method },
            { "str-length", TokenType.Method },
            { "str-insert", TokenType.Method },
            { "str-index", TokenType.Method },
            { "str-slice", TokenType.Method },
            { "to-upper-case", TokenType.Method },
            { "to-lower-case", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 100),

            // Single-line comments (SCSS/Less)
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 95),

            // Interpolation #{...}
            new TokenPattern(@"#\{[^}]*\}", TokenType.Identifier, 90),

            // Variables ($var or @var for Less)
            new TokenPattern(@"[\$@][a-zA-Z_][a-zA-Z0-9_-]*", TokenType.Identifier, 85),

            // At-rules
            new TokenPattern(@"@[a-zA-Z_][a-zA-Z0-9_-]*", TokenType.Keyword, 82),

            // Strings (double quotes)
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 80),

            // Strings (single quotes)
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 80),

            // URLs
            new TokenPattern(@"url\([^)]*\)", TokenType.String, 78),

            // Hex colors
            new TokenPattern(@"#[0-9a-fA-F]{3,8}\b", TokenType.Number, 75),

            // Numbers with units
            new TokenPattern(@"-?\d+\.?\d*(?:px|em|rem|%|vh|vw|vmin|vmax|ch|ex|cm|mm|in|pt|pc|deg|rad|grad|turn|s|ms|Hz|kHz|dpi|dpcm|dppx|fr)?", TokenType.Number, 70),

            // Class selectors
            new TokenPattern(@"\.[a-zA-Z_][a-zA-Z0-9_-]*", TokenType.TypeName, 60),

            // ID selectors
            new TokenPattern(@"#[a-zA-Z_][a-zA-Z0-9_-]*", TokenType.TypeName, 58),

            // Pseudo-classes and pseudo-elements
            new TokenPattern(@"::?[a-zA-Z_][a-zA-Z0-9_-]*(?:\([^)]*\))?", TokenType.Keyword, 55),

            // Attribute selectors
            new TokenPattern(@"\[[^\]]+\]", TokenType.Attribute, 52),

            // Property names
            new TokenPattern(@"[a-zA-Z_-][a-zA-Z0-9_-]*\s*(?=:)", TokenType.Property, 50),

            // Element/tag names at start of selector
            new TokenPattern(@"\b(?:html|body|div|span|p|a|img|ul|ol|li|table|tr|td|th|form|input|button|header|footer|nav|section|article|aside|main|h[1-6])\b", TokenType.TypeName, 45),

            // Identifiers (functions, mixins, etc.)
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_-]*\b", TokenType.Identifier, 40),

            // Operators
            new TokenPattern(@"[+\-*/%=<>!&|~^]+|:", TokenType.Operator, 30),

            // Punctuation
            new TokenPattern(@"[(){}\[\];,.]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Scss;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
