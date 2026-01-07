using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for C# source code.
/// </summary>
public class CSharpTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static CSharpTokenizer()
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
            ["foreach"] = TokenType.ControlKeyword,
            ["while"] = TokenType.ControlKeyword,
            ["do"] = TokenType.ControlKeyword,
            ["break"] = TokenType.ControlKeyword,
            ["continue"] = TokenType.ControlKeyword,
            ["return"] = TokenType.ControlKeyword,
            ["throw"] = TokenType.ControlKeyword,
            ["try"] = TokenType.ControlKeyword,
            ["catch"] = TokenType.ControlKeyword,
            ["finally"] = TokenType.ControlKeyword,
            ["goto"] = TokenType.ControlKeyword,
            ["yield"] = TokenType.ControlKeyword,

            // Type keywords
            ["class"] = TokenType.Keyword,
            ["struct"] = TokenType.Keyword,
            ["interface"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["record"] = TokenType.Keyword,
            ["delegate"] = TokenType.Keyword,
            ["namespace"] = TokenType.Keyword,
            ["using"] = TokenType.Keyword,

            // Modifier keywords
            ["public"] = TokenType.Keyword,
            ["private"] = TokenType.Keyword,
            ["protected"] = TokenType.Keyword,
            ["internal"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["readonly"] = TokenType.Keyword,
            ["const"] = TokenType.Keyword,
            ["volatile"] = TokenType.Keyword,
            ["abstract"] = TokenType.Keyword,
            ["virtual"] = TokenType.Keyword,
            ["override"] = TokenType.Keyword,
            ["sealed"] = TokenType.Keyword,
            ["partial"] = TokenType.Keyword,
            ["async"] = TokenType.Keyword,
            ["await"] = TokenType.Keyword,
            ["extern"] = TokenType.Keyword,
            ["unsafe"] = TokenType.Keyword,
            ["fixed"] = TokenType.Keyword,
            ["ref"] = TokenType.Keyword,
            ["out"] = TokenType.Keyword,
            ["in"] = TokenType.Keyword,
            ["params"] = TokenType.Keyword,
            ["required"] = TokenType.Keyword,
            ["file"] = TokenType.Keyword,
            ["scoped"] = TokenType.Keyword,

            // Other keywords
            ["new"] = TokenType.Keyword,
            ["this"] = TokenType.Keyword,
            ["base"] = TokenType.Keyword,
            ["typeof"] = TokenType.Keyword,
            ["sizeof"] = TokenType.Keyword,
            ["nameof"] = TokenType.Keyword,
            ["is"] = TokenType.Keyword,
            ["as"] = TokenType.Keyword,
            ["lock"] = TokenType.Keyword,
            ["checked"] = TokenType.Keyword,
            ["unchecked"] = TokenType.Keyword,
            ["stackalloc"] = TokenType.Keyword,
            ["where"] = TokenType.Keyword,
            ["when"] = TokenType.Keyword,
            ["with"] = TokenType.Keyword,
            ["init"] = TokenType.Keyword,
            ["get"] = TokenType.Keyword,
            ["set"] = TokenType.Keyword,
            ["add"] = TokenType.Keyword,
            ["remove"] = TokenType.Keyword,
            ["value"] = TokenType.Keyword,
            ["var"] = TokenType.Keyword,
            ["dynamic"] = TokenType.Keyword,
            ["global"] = TokenType.Keyword,
            ["event"] = TokenType.Keyword,
            ["implicit"] = TokenType.Keyword,
            ["explicit"] = TokenType.Keyword,
            ["operator"] = TokenType.Keyword,

            // Built-in types
            ["bool"] = TokenType.TypeName,
            ["byte"] = TokenType.TypeName,
            ["sbyte"] = TokenType.TypeName,
            ["char"] = TokenType.TypeName,
            ["decimal"] = TokenType.TypeName,
            ["double"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["int"] = TokenType.TypeName,
            ["uint"] = TokenType.TypeName,
            ["long"] = TokenType.TypeName,
            ["ulong"] = TokenType.TypeName,
            ["short"] = TokenType.TypeName,
            ["ushort"] = TokenType.TypeName,
            ["object"] = TokenType.TypeName,
            ["string"] = TokenType.TypeName,
            ["void"] = TokenType.TypeName,
            ["nint"] = TokenType.TypeName,
            ["nuint"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["null"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments (must come before regular comments)
            new(@"///[^\r\n]*", TokenType.DocComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),

            // Multi-line comments
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 9),

            // Preprocessor directives
            new(@"#\s*(?:if|else|elif|endif|define|undef|warning|error|line|region|endregion|pragma|nullable)[^\r\n]*", TokenType.Preprocessor, 8),

            // Verbatim strings
            new(@"@""(?:[^""]|"""")*""", TokenType.String, 7),

            // Interpolated strings (basic support)
            new(@"\$""(?:[^""\\]|\\.)*""", TokenType.String, 7),
            new(@"\$@""(?:[^""]|"""")*""", TokenType.String, 7),
            new(@"@\$""(?:[^""]|"""")*""", TokenType.String, 7),

            // Regular strings
            new(CommonPatterns.DoubleQuotedString, TokenType.String, 6),

            // Character literals
            new(CommonPatterns.SingleQuotedString, TokenType.Character, 6),

            // Attributes
            new(@"\[\s*[a-zA-Z_][a-zA-Z0-9_]*(?:\s*\([^)]*\))?\s*\]", TokenType.Attribute, 5),

            // Numbers
            new(CommonPatterns.HexNumber, TokenType.Number, 4),
            new(CommonPatterns.BinaryNumber, TokenType.Number, 4),
            new(CommonPatterns.FloatingPoint, TokenType.Number, 4),
            new(CommonPatterns.ScientificNotation, TokenType.Number, 4),
            new(CommonPatterns.Integer, TokenType.Number, 4),

            // Identifiers (keywords are checked in base class)
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"=>|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(CommonPatterns.Punctuation, TokenType.Punctuation, 0),

            // Whitespace (captured but typically not displayed)
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.CSharp;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
