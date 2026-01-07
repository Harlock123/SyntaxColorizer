using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for PHP source code.
/// </summary>
public class PhpTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static PhpTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Control keywords
            ["if"] = TokenType.ControlKeyword,
            ["else"] = TokenType.ControlKeyword,
            ["elseif"] = TokenType.ControlKeyword,
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
            ["match"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["class"] = TokenType.Keyword,
            ["interface"] = TokenType.Keyword,
            ["trait"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["extends"] = TokenType.Keyword,
            ["implements"] = TokenType.Keyword,
            ["namespace"] = TokenType.Keyword,
            ["use"] = TokenType.Keyword,
            ["function"] = TokenType.Keyword,
            ["fn"] = TokenType.Keyword,
            ["const"] = TokenType.Keyword,
            ["var"] = TokenType.Keyword,

            // Modifier keywords
            ["public"] = TokenType.Keyword,
            ["private"] = TokenType.Keyword,
            ["protected"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["final"] = TokenType.Keyword,
            ["abstract"] = TokenType.Keyword,
            ["readonly"] = TokenType.Keyword,

            // Other keywords
            ["new"] = TokenType.Keyword,
            ["clone"] = TokenType.Keyword,
            ["instanceof"] = TokenType.Keyword,
            ["as"] = TokenType.Keyword,
            ["and"] = TokenType.Keyword,
            ["or"] = TokenType.Keyword,
            ["xor"] = TokenType.Keyword,
            ["global"] = TokenType.Keyword,
            ["echo"] = TokenType.Keyword,
            ["print"] = TokenType.Keyword,
            ["require"] = TokenType.Keyword,
            ["require_once"] = TokenType.Keyword,
            ["include"] = TokenType.Keyword,
            ["include_once"] = TokenType.Keyword,
            ["list"] = TokenType.Keyword,
            ["array"] = TokenType.Keyword,
            ["empty"] = TokenType.Keyword,
            ["isset"] = TokenType.Keyword,
            ["unset"] = TokenType.Keyword,
            ["die"] = TokenType.Keyword,
            ["exit"] = TokenType.Keyword,
            ["eval"] = TokenType.Keyword,
            ["insteadof"] = TokenType.Keyword,

            // Built-in types
            ["int"] = TokenType.TypeName,
            ["float"] = TokenType.TypeName,
            ["string"] = TokenType.TypeName,
            ["bool"] = TokenType.TypeName,
            ["boolean"] = TokenType.TypeName,
            ["object"] = TokenType.TypeName,
            ["callable"] = TokenType.TypeName,
            ["iterable"] = TokenType.TypeName,
            ["void"] = TokenType.TypeName,
            ["mixed"] = TokenType.TypeName,
            ["never"] = TokenType.TypeName,
            ["self"] = TokenType.TypeName,
            ["parent"] = TokenType.TypeName,

            // Literals (case-insensitive, so no need for uppercase variants)
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,
            ["null"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // PHP tags
            new(@"<\?php|\?>|<\?=", TokenType.Preprocessor, 11),

            // Multi-line comments
            new(@"/\*\*[\s\S]*?\*/", TokenType.DocComment, 10),
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),
            new(CommonPatterns.HashComment, TokenType.Comment, 9),

            // Attributes (PHP 8+)
            new(@"#\[[^\]]+\]", TokenType.Attribute, 8),

            // Heredoc and Nowdoc
            new(@"<<<['""]?([a-zA-Z_][a-zA-Z0-9_]*)['""]?[\s\S]*?^\1;?$", TokenType.String, 7, RegexOptions.Multiline),

            // Double-quoted strings with variables
            new(@"""(?:[^""\\$]|\\.|\$[a-zA-Z_])*""", TokenType.String, 6),

            // Single-quoted strings
            new(@"'(?:[^'\\]|\\.)*'", TokenType.String, 6),

            // Variables
            new(@"\$[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Identifier, 5),

            // Numbers
            new(CommonPatterns.HexNumber, TokenType.Number, 4),
            new(@"\b0[bB][01_]+\b", TokenType.Number, 4),
            new(@"\b0[oO][0-7_]+\b", TokenType.Number, 4),
            new(CommonPatterns.FloatingPoint, TokenType.Number, 4),
            new(@"\b\d[\d_]*\b", TokenType.Number, 4),

            // Identifiers
            new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 2),

            // Operators
            new(@"=>|->|\?\?|<=>|\.\.\.|[+\-*/%=<>!&|^~?:.]+", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,@]", TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Php;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
