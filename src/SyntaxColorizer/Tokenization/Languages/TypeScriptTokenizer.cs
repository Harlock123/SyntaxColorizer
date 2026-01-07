using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for TypeScript source code.
/// </summary>
public class TypeScriptTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static TypeScriptTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control keywords (inherited from JavaScript)
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
            ["throw"] = TokenType.ControlKeyword,
            ["try"] = TokenType.ControlKeyword,
            ["catch"] = TokenType.ControlKeyword,
            ["finally"] = TokenType.ControlKeyword,
            ["with"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["function"] = TokenType.Keyword,
            ["class"] = TokenType.Keyword,
            ["extends"] = TokenType.Keyword,
            ["var"] = TokenType.Keyword,
            ["let"] = TokenType.Keyword,
            ["const"] = TokenType.Keyword,
            ["import"] = TokenType.Keyword,
            ["export"] = TokenType.Keyword,
            ["from"] = TokenType.Keyword,
            ["as"] = TokenType.Keyword,

            // TypeScript-specific keywords
            ["interface"] = TokenType.Keyword,
            ["type"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["namespace"] = TokenType.Keyword,
            ["module"] = TokenType.Keyword,
            ["declare"] = TokenType.Keyword,
            ["abstract"] = TokenType.Keyword,
            ["implements"] = TokenType.Keyword,
            ["public"] = TokenType.Keyword,
            ["private"] = TokenType.Keyword,
            ["protected"] = TokenType.Keyword,
            ["readonly"] = TokenType.Keyword,
            ["override"] = TokenType.Keyword,
            ["is"] = TokenType.Keyword,
            ["keyof"] = TokenType.Keyword,
            ["infer"] = TokenType.Keyword,
            ["asserts"] = TokenType.Keyword,
            ["satisfies"] = TokenType.Keyword,

            // Other keywords
            ["new"] = TokenType.Keyword,
            ["this"] = TokenType.Keyword,
            ["super"] = TokenType.Keyword,
            ["typeof"] = TokenType.Keyword,
            ["instanceof"] = TokenType.Keyword,
            ["in"] = TokenType.Keyword,
            ["of"] = TokenType.Keyword,
            ["delete"] = TokenType.Keyword,
            ["void"] = TokenType.Keyword,
            ["async"] = TokenType.Keyword,
            ["await"] = TokenType.Keyword,
            ["yield"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["get"] = TokenType.Keyword,
            ["set"] = TokenType.Keyword,
            ["debugger"] = TokenType.Keyword,

            // TypeScript types
            ["string"] = TokenType.TypeName,
            ["number"] = TokenType.TypeName,
            ["boolean"] = TokenType.TypeName,
            ["symbol"] = TokenType.TypeName,
            ["bigint"] = TokenType.TypeName,
            ["object"] = TokenType.TypeName,
            ["any"] = TokenType.TypeName,
            ["unknown"] = TokenType.TypeName,
            ["never"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["null"] = TokenType.Constant,
            ["undefined"] = TokenType.Constant,
            ["NaN"] = TokenType.Constant,
            ["Infinity"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // Multi-line comments
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),

            // Decorators
            new(@"@[a-zA-Z_$][a-zA-Z0-9_$]*", TokenType.Attribute, 8),

            // Regex literals
            new(@"/(?![/*])(?:[^/\\]|\\.)+/[gimsuy]*", TokenType.Regex, 7),

            // Template literals
            new(@"`(?:[^`\\$]|\\.|\$(?!\{)|\$\{[^}]*\})*`", TokenType.String, 6),

            // Strings
            new(CommonPatterns.DoubleQuotedString, TokenType.String, 6),
            new(CommonPatterns.SingleQuotedString, TokenType.String, 6),

            // Numbers
            new(@"\b0[xX][0-9a-fA-F]+n?\b", TokenType.Number, 5),
            new(@"\b0[oO][0-7]+n?\b", TokenType.Number, 5),
            new(@"\b0[bB][01]+n?\b", TokenType.Number, 5),
            new(@"\b\d+n\b", TokenType.Number, 5),
            new(CommonPatterns.FloatingPoint, TokenType.Number, 4),
            new(CommonPatterns.Integer, TokenType.Number, 4),

            // Identifiers
            new(@"\b[a-zA-Z_$][a-zA-Z0-9_$]*\b", TokenType.Identifier, 2),

            // Operators
            new(@"=>|\.{3}|\?\?|\?\.|\?\?=|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,.<>]", TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.TypeScript;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
