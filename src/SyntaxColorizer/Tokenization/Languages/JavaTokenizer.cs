using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Java source code.
/// </summary>
public class JavaTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static JavaTokenizer()
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
            ["throw"] = TokenType.ControlKeyword,
            ["try"] = TokenType.ControlKeyword,
            ["catch"] = TokenType.ControlKeyword,
            ["finally"] = TokenType.ControlKeyword,
            ["assert"] = TokenType.ControlKeyword,
            ["yield"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["class"] = TokenType.Keyword,
            ["interface"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["record"] = TokenType.Keyword,
            ["extends"] = TokenType.Keyword,
            ["implements"] = TokenType.Keyword,
            ["package"] = TokenType.Keyword,
            ["import"] = TokenType.Keyword,

            // Modifier keywords
            ["public"] = TokenType.Keyword,
            ["private"] = TokenType.Keyword,
            ["protected"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["final"] = TokenType.Keyword,
            ["abstract"] = TokenType.Keyword,
            ["native"] = TokenType.Keyword,
            ["strictfp"] = TokenType.Keyword,
            ["synchronized"] = TokenType.Keyword,
            ["transient"] = TokenType.Keyword,
            ["volatile"] = TokenType.Keyword,
            ["sealed"] = TokenType.Keyword,
            ["non-sealed"] = TokenType.Keyword,
            ["permits"] = TokenType.Keyword,

            // Other keywords
            ["new"] = TokenType.Keyword,
            ["this"] = TokenType.Keyword,
            ["super"] = TokenType.Keyword,
            ["instanceof"] = TokenType.Keyword,
            ["throws"] = TokenType.Keyword,
            ["var"] = TokenType.Keyword,

            // Built-in types
            ["boolean"] = TokenType.TypeName,
            ["byte"] = TokenType.TypeName,
            ["char"] = TokenType.TypeName,
            ["short"] = TokenType.TypeName,
            ["int"] = TokenType.TypeName,
            ["long"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["double"] = TokenType.TypeName,
            ["void"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["null"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments
            new(@"/\*\*[\s\S]*?\*/", TokenType.DocComment, 11),

            // Multi-line comments
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),

            // Annotations
            new(@"@[a-zA-Z_][a-zA-Z0-9_]*(?:\.[a-zA-Z_][a-zA-Z0-9_]*)*", TokenType.Attribute, 8),

            // Text blocks (Java 15+)
            new(@"""""""[\s\S]*?""""""", TokenType.String, 7),

            // Strings
            new(CommonPatterns.DoubleQuotedString, TokenType.String, 6),

            // Character literals
            new(CommonPatterns.SingleQuotedString, TokenType.Character, 6),

            // Numbers
            new(CommonPatterns.HexNumber, TokenType.Number, 5),
            new(CommonPatterns.BinaryNumber, TokenType.Number, 5),
            new(@"\b\d+_*\d*[lLfFdD]?\b", TokenType.Number, 4),
            new(CommonPatterns.FloatingPoint, TokenType.Number, 4),

            // Identifiers
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"->|::|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(CommonPatterns.Punctuation, TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Java;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
