using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Dart programming language.
/// </summary>
public class DartTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static DartTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "default", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "finally", TokenType.ControlKeyword },
            { "rethrow", TokenType.ControlKeyword },
            { "assert", TokenType.ControlKeyword },
            { "yield", TokenType.ControlKeyword },

            // Declaration keywords
            { "class", TokenType.Keyword },
            { "abstract", TokenType.Keyword },
            { "extends", TokenType.Keyword },
            { "implements", TokenType.Keyword },
            { "mixin", TokenType.Keyword },
            { "with", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "typedef", TokenType.Keyword },
            { "extension", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "export", TokenType.Keyword },
            { "library", TokenType.Keyword },
            { "part", TokenType.Keyword },

            // Variable/function keywords
            { "var", TokenType.Keyword },
            { "final", TokenType.Keyword },
            { "const", TokenType.Keyword },
            { "late", TokenType.Keyword },
            { "required", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "covariant", TokenType.Keyword },
            { "external", TokenType.Keyword },
            { "factory", TokenType.Keyword },
            { "operator", TokenType.Keyword },
            { "get", TokenType.Keyword },
            { "set", TokenType.Keyword },

            // Async keywords
            { "async", TokenType.Keyword },
            { "await", TokenType.Keyword },
            { "sync", TokenType.Keyword },

            // Other keywords
            { "new", TokenType.Keyword },
            { "this", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "is", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "on", TokenType.Keyword },
            { "show", TokenType.Keyword },
            { "hide", TokenType.Keyword },
            { "deferred", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Built-in types
            { "void", TokenType.TypeName },
            { "dynamic", TokenType.TypeName },
            { "Object", TokenType.TypeName },
            { "int", TokenType.TypeName },
            { "double", TokenType.TypeName },
            { "num", TokenType.TypeName },
            { "bool", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "List", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "Iterable", TokenType.TypeName },
            { "Iterator", TokenType.TypeName },
            { "Future", TokenType.TypeName },
            { "Stream", TokenType.TypeName },
            { "Function", TokenType.TypeName },
            { "Symbol", TokenType.TypeName },
            { "Type", TokenType.TypeName },
            { "Null", TokenType.TypeName },
            { "Never", TokenType.TypeName },
            { "Duration", TokenType.TypeName },
            { "DateTime", TokenType.TypeName },
            { "Uri", TokenType.TypeName },
            { "RegExp", TokenType.TypeName },
            { "Pattern", TokenType.TypeName },
            { "Match", TokenType.TypeName },
            { "Comparable", TokenType.TypeName },
            { "Exception", TokenType.TypeName },
            { "Error", TokenType.TypeName },

            // Common methods
            { "print", TokenType.Method },
            { "main", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments
            new TokenPattern(@"///[^\n]*", TokenType.DocComment, 100),

            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 95),

            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 90),

            // Raw strings
            new TokenPattern(@"r""[^""]*""", TokenType.String, 85),
            new TokenPattern(@"r'[^']*'", TokenType.String, 85),

            // Multi-line strings
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 85),
            new TokenPattern(@"'''[\s\S]*?'''", TokenType.String, 85),

            // Regular strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 80),
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 80),

            // Annotations
            new TokenPattern(@"@[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Attribute, 75),

            // Numbers (hex)
            new TokenPattern(@"\b0[xX][0-9a-fA-F]+\b", TokenType.Number, 70),

            // Numbers (decimal/float)
            new TokenPattern(@"\b\d+\.?\d*(?:[eE][+-]?\d+)?\b", TokenType.Number, 70),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_]*\b", TokenType.TypeName, 50),

            // Operators
            new TokenPattern(@"=>|->|\?\?|\.\.\.?|\?\.|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 40),

            // Identifiers
            new TokenPattern(@"\b[a-z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.<>]+", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Dart;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
