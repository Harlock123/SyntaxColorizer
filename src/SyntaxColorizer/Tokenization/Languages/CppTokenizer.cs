using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for C++ source code.
/// </summary>
public class CppTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static CppTokenizer()
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
            ["throw"] = TokenType.ControlKeyword,
            ["try"] = TokenType.ControlKeyword,
            ["catch"] = TokenType.ControlKeyword,
            ["co_await"] = TokenType.ControlKeyword,
            ["co_return"] = TokenType.ControlKeyword,
            ["co_yield"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["class"] = TokenType.Keyword,
            ["struct"] = TokenType.Keyword,
            ["union"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["namespace"] = TokenType.Keyword,
            ["using"] = TokenType.Keyword,
            ["typedef"] = TokenType.Keyword,
            ["template"] = TokenType.Keyword,
            ["typename"] = TokenType.Keyword,
            ["concept"] = TokenType.Keyword,
            ["requires"] = TokenType.Keyword,
            ["module"] = TokenType.Keyword,
            ["import"] = TokenType.Keyword,
            ["export"] = TokenType.Keyword,

            // Access specifiers
            ["public"] = TokenType.Keyword,
            ["private"] = TokenType.Keyword,
            ["protected"] = TokenType.Keyword,
            ["friend"] = TokenType.Keyword,

            // Storage class specifiers
            ["auto"] = TokenType.Keyword,
            ["register"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["extern"] = TokenType.Keyword,
            ["mutable"] = TokenType.Keyword,
            ["thread_local"] = TokenType.Keyword,
            ["inline"] = TokenType.Keyword,
            ["virtual"] = TokenType.Keyword,
            ["explicit"] = TokenType.Keyword,

            // Type qualifiers
            ["const"] = TokenType.Keyword,
            ["volatile"] = TokenType.Keyword,
            ["constexpr"] = TokenType.Keyword,
            ["consteval"] = TokenType.Keyword,
            ["constinit"] = TokenType.Keyword,

            // Other keywords
            ["new"] = TokenType.Keyword,
            ["delete"] = TokenType.Keyword,
            ["this"] = TokenType.Keyword,
            ["operator"] = TokenType.Keyword,
            ["sizeof"] = TokenType.Keyword,
            ["alignof"] = TokenType.Keyword,
            ["alignas"] = TokenType.Keyword,
            ["decltype"] = TokenType.Keyword,
            ["typeid"] = TokenType.Keyword,
            ["noexcept"] = TokenType.Keyword,
            ["static_assert"] = TokenType.Keyword,
            ["static_cast"] = TokenType.Keyword,
            ["dynamic_cast"] = TokenType.Keyword,
            ["const_cast"] = TokenType.Keyword,
            ["reinterpret_cast"] = TokenType.Keyword,
            ["final"] = TokenType.Keyword,
            ["override"] = TokenType.Keyword,

            // Built-in types
            ["void"] = TokenType.TypeName,
            ["bool"] = TokenType.TypeName,
            ["char"] = TokenType.TypeName,
            ["char8_t"] = TokenType.TypeName,
            ["char16_t"] = TokenType.TypeName,
            ["char32_t"] = TokenType.TypeName,
            ["wchar_t"] = TokenType.TypeName,
            ["short"] = TokenType.TypeName,
            ["int"] = TokenType.TypeName,
            ["long"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["double"] = TokenType.TypeName,
            ["signed"] = TokenType.TypeName,
            ["unsigned"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["nullptr"] = TokenType.Constant,
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
            new(@"<[a-zA-Z0-9_./]+(?:\.h|\.hpp)?>", TokenType.String, 7),

            // Raw string literals
            new(@"R""[a-zA-Z0-9_]*\([\s\S]*?\)[a-zA-Z0-9_]*""", TokenType.String, 7),

            // Strings
            new(@"(?:u8|u|U|L)?""(?:[^""\\]|\\.)*""", TokenType.String, 6),

            // Character literals
            new(@"(?:u8|u|U|L)?'(?:[^'\\]|\\.)*'", TokenType.Character, 6),

            // Numbers
            new(@"\b0[xX][0-9a-fA-F']+[uUlLzZ]*\b", TokenType.Number, 5),
            new(@"\b0[bB][01']+[uUlLzZ]*\b", TokenType.Number, 5),
            new(@"\b0[0-7']+[uUlLzZ]*\b", TokenType.Number, 5),
            new(@"\b\d[\d']*\.\d[\d']*(?:[eE][+-]?\d[\d']*)?[fFlL]?\b", TokenType.Number, 4),
            new(@"\b\d[\d']*[eE][+-]?\d[\d']*[fFlL]?\b", TokenType.Number, 4),
            new(@"\b\d[\d']*[uUlLzZ]*\b", TokenType.Number, 4),

            // Identifiers
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"->|\.\*|->|::|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(CommonPatterns.Punctuation, TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Cpp;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
