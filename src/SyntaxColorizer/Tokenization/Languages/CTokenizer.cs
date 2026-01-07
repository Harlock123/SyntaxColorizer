using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for C source code.
/// </summary>
public class CTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static CTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control keywords
            ["if"] = TokenType.ControlKeyword,
            ["else"] = TokenType.ControlKeyword,
            ["switch"] = TokenType.ControlKeyword,
            ["case"] = TokenType.ControlKeyword,
            ["default"] = TokenType.ControlKeyword,
            ["for"] = TokenType.ControlKeyword,
            ["while"] = TokenType.ControlKeyword,
            ["do"] = TokenType.ControlKeyword,
            ["break"] = TokenType.ControlKeyword,
            ["continue"] = TokenType.ControlKeyword,
            ["return"] = TokenType.ControlKeyword,
            ["goto"] = TokenType.ControlKeyword,

            // Type keywords
            ["struct"] = TokenType.Keyword,
            ["union"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["typedef"] = TokenType.Keyword,

            // Storage class specifiers
            ["auto"] = TokenType.Keyword,
            ["register"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["extern"] = TokenType.Keyword,
            ["inline"] = TokenType.Keyword,
            ["_Noreturn"] = TokenType.Keyword,

            // Type qualifiers
            ["const"] = TokenType.Keyword,
            ["volatile"] = TokenType.Keyword,
            ["restrict"] = TokenType.Keyword,
            ["_Atomic"] = TokenType.Keyword,

            // Other keywords
            ["sizeof"] = TokenType.Keyword,
            ["_Alignof"] = TokenType.Keyword,
            ["_Alignas"] = TokenType.Keyword,
            ["_Generic"] = TokenType.Keyword,
            ["_Static_assert"] = TokenType.Keyword,
            ["_Thread_local"] = TokenType.Keyword,

            // Built-in types
            ["void"] = TokenType.TypeName,
            ["char"] = TokenType.TypeName,
            ["short"] = TokenType.TypeName,
            ["int"] = TokenType.TypeName,
            ["long"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["double"] = TokenType.TypeName,
            ["signed"] = TokenType.TypeName,
            ["unsigned"] = TokenType.TypeName,
            ["_Bool"] = TokenType.TypeName,
            ["_Complex"] = TokenType.TypeName,
            ["_Imaginary"] = TokenType.TypeName,

            // C99/C11 types
            ["int8_t"] = TokenType.TypeName,
            ["int16_t"] = TokenType.TypeName,
            ["int32_t"] = TokenType.TypeName,
            ["int64_t"] = TokenType.TypeName,
            ["uint8_t"] = TokenType.TypeName,
            ["uint16_t"] = TokenType.TypeName,
            ["uint32_t"] = TokenType.TypeName,
            ["uint64_t"] = TokenType.TypeName,
            ["size_t"] = TokenType.TypeName,
            ["ptrdiff_t"] = TokenType.TypeName,
            ["intptr_t"] = TokenType.TypeName,
            ["uintptr_t"] = TokenType.TypeName,
            ["bool"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["NULL"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // Multi-line comments
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),

            // Preprocessor directives
            new(@"#\s*(?:include|define|undef|if|ifdef|ifndef|else|elif|endif|error|pragma|line|warning)[^\r\n]*", TokenType.Preprocessor, 8),

            // Include paths
            new(@"<[a-zA-Z0-9_./]+\.h>", TokenType.String, 7),

            // Strings
            new(CommonPatterns.DoubleQuotedString, TokenType.String, 6),

            // Character literals
            new(CommonPatterns.SingleQuotedString, TokenType.Character, 6),

            // Numbers
            new(CommonPatterns.HexNumber, TokenType.Number, 5),
            new(@"\b0[0-7]+[uUlL]*\b", TokenType.Number, 5),
            new(CommonPatterns.FloatingPoint, TokenType.Number, 4),
            new(CommonPatterns.ScientificNotation, TokenType.Number, 4),
            new(CommonPatterns.Integer, TokenType.Number, 4),

            // Identifiers
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"->|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(CommonPatterns.Punctuation, TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.C;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
