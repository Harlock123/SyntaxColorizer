using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Python source code.
/// </summary>
public class PythonTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static PythonTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control keywords
            ["if"] = TokenType.ControlKeyword,
            ["elif"] = TokenType.ControlKeyword,
            ["else"] = TokenType.ControlKeyword,
            ["for"] = TokenType.ControlKeyword,
            ["while"] = TokenType.ControlKeyword,
            ["break"] = TokenType.ControlKeyword,
            ["continue"] = TokenType.ControlKeyword,
            ["return"] = TokenType.ControlKeyword,
            ["pass"] = TokenType.ControlKeyword,
            ["raise"] = TokenType.ControlKeyword,
            ["try"] = TokenType.ControlKeyword,
            ["except"] = TokenType.ControlKeyword,
            ["finally"] = TokenType.ControlKeyword,
            ["with"] = TokenType.ControlKeyword,
            ["yield"] = TokenType.ControlKeyword,
            ["assert"] = TokenType.ControlKeyword,
            ["match"] = TokenType.ControlKeyword,
            ["case"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["def"] = TokenType.Keyword,
            ["class"] = TokenType.Keyword,
            ["lambda"] = TokenType.Keyword,
            ["import"] = TokenType.Keyword,
            ["from"] = TokenType.Keyword,
            ["as"] = TokenType.Keyword,
            ["global"] = TokenType.Keyword,
            ["nonlocal"] = TokenType.Keyword,
            ["async"] = TokenType.Keyword,
            ["await"] = TokenType.Keyword,

            // Other keywords
            ["and"] = TokenType.Keyword,
            ["or"] = TokenType.Keyword,
            ["not"] = TokenType.Keyword,
            ["in"] = TokenType.Keyword,
            ["is"] = TokenType.Keyword,
            ["del"] = TokenType.Keyword,

            // Built-in types
            ["int"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["str"] = TokenType.TypeName,
            ["bool"] = TokenType.TypeName,
            ["list"] = TokenType.TypeName,
            ["dict"] = TokenType.TypeName,
            ["set"] = TokenType.TypeName,
            ["tuple"] = TokenType.TypeName,
            ["bytes"] = TokenType.TypeName,
            ["bytearray"] = TokenType.TypeName,
            ["object"] = TokenType.TypeName,
            ["type"] = TokenType.TypeName,

            // Literals
            ["True"] = TokenType.Constant,
            ["False"] = TokenType.Constant,
            ["None"] = TokenType.Constant,

            // Built-in functions (common ones)
            ["print"] = TokenType.Method,
            ["len"] = TokenType.Method,
            ["range"] = TokenType.Method,
            ["enumerate"] = TokenType.Method,
            ["zip"] = TokenType.Method,
            ["map"] = TokenType.Method,
            ["filter"] = TokenType.Method,
            ["sorted"] = TokenType.Method,
            ["reversed"] = TokenType.Method,
            ["open"] = TokenType.Method,
            ["input"] = TokenType.Method,
            ["isinstance"] = TokenType.Method,
            ["issubclass"] = TokenType.Method,
            ["hasattr"] = TokenType.Method,
            ["getattr"] = TokenType.Method,
            ["setattr"] = TokenType.Method,
            ["delattr"] = TokenType.Method,
            ["super"] = TokenType.Method,
        };

        _patterns = new List<TokenPattern>
        {
            // Triple-quoted strings (must come before regular strings)
            new(@"[rRbBuUfF]{0,2}""""""[\s\S]*?""""""", TokenType.String, 10),
            new(@"[rRbBuUfF]{0,2}'''[\s\S]*?'''", TokenType.String, 10),

            // Comments
            new(CommonPatterns.HashComment, TokenType.Comment, 9),

            // Decorators
            new(@"@[a-zA-Z_][a-zA-Z0-9_.]*", TokenType.Attribute, 8),

            // f-strings (basic support)
            new(@"[fF]""(?:[^""\\]|\\.)*""", TokenType.String, 7),
            new(@"[fF]'(?:[^'\\]|\\.)*'", TokenType.String, 7),

            // Regular strings
            new(@"[rRbBuU]?""(?:[^""\\]|\\.)*""", TokenType.String, 6),
            new(@"[rRbBuU]?'(?:[^'\\]|\\.)*'", TokenType.String, 6),

            // Numbers
            new(@"\b0[xX][0-9a-fA-F_]+\b", TokenType.Number, 5),
            new(@"\b0[oO][0-7_]+\b", TokenType.Number, 5),
            new(@"\b0[bB][01_]+\b", TokenType.Number, 5),
            new(@"\b\d[\d_]*\.\d[\d_]*(?:[eE][+-]?\d[\d_]*)?j?\b", TokenType.Number, 4),
            new(@"\b\d[\d_]*[eE][+-]?\d[\d_]*j?\b", TokenType.Number, 4),
            new(@"\b\d[\d_]*j?\b", TokenType.Number, 4),

            // Identifiers
            new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 2),

            // Operators
            new(@"->|:=|\*\*|//|[+\-*/%=<>!&|^~@:]+", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,.]", TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Python;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
