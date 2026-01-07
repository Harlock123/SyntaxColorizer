using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Go programming language.
/// </summary>
public class GoTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static GoTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow keywords
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "default", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "range", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "goto", TokenType.ControlKeyword },
            { "fallthrough", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "select", TokenType.ControlKeyword },

            // Declaration keywords
            { "var", TokenType.Keyword },
            { "const", TokenType.Keyword },
            { "type", TokenType.Keyword },
            { "func", TokenType.Keyword },
            { "package", TokenType.Keyword },
            { "import", TokenType.Keyword },

            // Type keywords
            { "struct", TokenType.Keyword },
            { "interface", TokenType.Keyword },
            { "map", TokenType.Keyword },
            { "chan", TokenType.Keyword },

            // Other keywords
            { "defer", TokenType.Keyword },
            { "go", TokenType.Keyword },

            // Built-in types
            { "bool", TokenType.TypeName },
            { "byte", TokenType.TypeName },
            { "complex64", TokenType.TypeName },
            { "complex128", TokenType.TypeName },
            { "error", TokenType.TypeName },
            { "float32", TokenType.TypeName },
            { "float64", TokenType.TypeName },
            { "int", TokenType.TypeName },
            { "int8", TokenType.TypeName },
            { "int16", TokenType.TypeName },
            { "int32", TokenType.TypeName },
            { "int64", TokenType.TypeName },
            { "rune", TokenType.TypeName },
            { "string", TokenType.TypeName },
            { "uint", TokenType.TypeName },
            { "uint8", TokenType.TypeName },
            { "uint16", TokenType.TypeName },
            { "uint32", TokenType.TypeName },
            { "uint64", TokenType.TypeName },
            { "uintptr", TokenType.TypeName },
            { "any", TokenType.TypeName },
            { "comparable", TokenType.TypeName },

            // Built-in functions
            { "append", TokenType.Method },
            { "cap", TokenType.Method },
            { "clear", TokenType.Method },
            { "close", TokenType.Method },
            { "complex", TokenType.Method },
            { "copy", TokenType.Method },
            { "delete", TokenType.Method },
            { "imag", TokenType.Method },
            { "len", TokenType.Method },
            { "make", TokenType.Method },
            { "max", TokenType.Method },
            { "min", TokenType.Method },
            { "new", TokenType.Method },
            { "panic", TokenType.Method },
            { "print", TokenType.Method },
            { "println", TokenType.Method },
            { "real", TokenType.Method },
            { "recover", TokenType.Method },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "nil", TokenType.Constant },
            { "iota", TokenType.Constant }
        };

        _patterns = new List<TokenPattern>
        {
            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 100),

            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 100),

            // Raw string literals (backtick)
            new TokenPattern(@"`[^`]*`", TokenType.String, 95),

            // Interpreted string literals
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 90),

            // Rune/character literals
            new TokenPattern(@"'(?:[^'\\]|\\.)'", TokenType.Character, 90),

            // Imaginary numbers
            new TokenPattern(@"\d+(?:\.\d+)?i", TokenType.Number, 85),

            // Hexadecimal numbers
            new TokenPattern(@"0[xX][0-9a-fA-F_]+", TokenType.Number, 80),

            // Octal numbers
            new TokenPattern(@"0[oO][0-7_]+", TokenType.Number, 80),

            // Binary numbers
            new TokenPattern(@"0[bB][01_]+", TokenType.Number, 80),

            // Float numbers
            new TokenPattern(@"\d+\.\d+(?:[eE][+-]?\d+)?", TokenType.Number, 75),
            new TokenPattern(@"\d+[eE][+-]?\d+", TokenType.Number, 75),

            // Integer numbers
            new TokenPattern(@"\d[0-9_]*", TokenType.Number, 70),

            // Package qualifier (e.g., fmt.Println)
            new TokenPattern(@"\b[a-z][a-zA-Z0-9_]*(?=\.)", TokenType.Namespace, 65),

            // Function calls
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*(?=\s*\()", TokenType.Method, 60),

            // Type names (capitalized identifiers often indicate exported types)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_]*\b", TokenType.TypeName, 55),

            // Operators
            new TokenPattern(@":=|<-|\.\.\.|\+\+|--|&&|\|\||<<|>>|&\^|[+\-*/%&|^<>=!]=?", TokenType.Operator, 50),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 40),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:&*]", TokenType.Punctuation, 30),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Go;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
