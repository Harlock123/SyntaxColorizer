using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for F# programming language.
/// </summary>
public class FSharpTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static FSharpTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "then", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "elif", TokenType.ControlKeyword },
            { "match", TokenType.ControlKeyword },
            { "with", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "done", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "finally", TokenType.ControlKeyword },
            { "raise", TokenType.ControlKeyword },
            { "failwith", TokenType.ControlKeyword },
            { "when", TokenType.ControlKeyword },

            // Declaration keywords
            { "let", TokenType.Keyword },
            { "let!", TokenType.Keyword },
            { "use", TokenType.Keyword },
            { "use!", TokenType.Keyword },
            { "do!", TokenType.Keyword },
            { "return", TokenType.Keyword },
            { "return!", TokenType.Keyword },
            { "yield", TokenType.Keyword },
            { "yield!", TokenType.Keyword },
            { "rec", TokenType.Keyword },
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "fun", TokenType.Keyword },
            { "function", TokenType.Keyword },
            { "mutable", TokenType.Keyword },
            { "inline", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "member", TokenType.Keyword },
            { "override", TokenType.Keyword },
            { "abstract", TokenType.Keyword },
            { "default", TokenType.Keyword },
            { "val", TokenType.Keyword },
            { "new", TokenType.Keyword },

            // Type keywords
            { "type", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "struct", TokenType.Keyword },
            { "interface", TokenType.Keyword },
            { "inherit", TokenType.Keyword },
            { "module", TokenType.Keyword },
            { "namespace", TokenType.Keyword },
            { "open", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "public", TokenType.Keyword },
            { "internal", TokenType.Keyword },

            // Pattern matching
            { "as", TokenType.Keyword },
            { "of", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "to", TokenType.Keyword },
            { "downto", TokenType.Keyword },
            { "upcast", TokenType.Keyword },
            { "downcast", TokenType.Keyword },

            // Computation expressions
            { "async", TokenType.Keyword },
            { "lazy", TokenType.Keyword },
            { "seq", TokenType.Keyword },
            { "query", TokenType.Keyword },

            // Other
            { "begin", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "extern", TokenType.Keyword },
            { "global", TokenType.Keyword },
            { "base", TokenType.Keyword },
            { "assert", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Built-in types
            { "int", TokenType.TypeName },
            { "int32", TokenType.TypeName },
            { "int64", TokenType.TypeName },
            { "uint", TokenType.TypeName },
            { "uint32", TokenType.TypeName },
            { "uint64", TokenType.TypeName },
            { "byte", TokenType.TypeName },
            { "sbyte", TokenType.TypeName },
            { "int16", TokenType.TypeName },
            { "uint16", TokenType.TypeName },
            { "float", TokenType.TypeName },
            { "float32", TokenType.TypeName },
            { "double", TokenType.TypeName },
            { "decimal", TokenType.TypeName },
            { "bool", TokenType.TypeName },
            { "char", TokenType.TypeName },
            { "string", TokenType.TypeName },
            { "unit", TokenType.TypeName },
            { "obj", TokenType.TypeName },
            { "void", TokenType.TypeName },
            { "option", TokenType.TypeName },
            { "list", TokenType.TypeName },
            { "array", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Seq", TokenType.TypeName },
            { "Async", TokenType.TypeName },
            { "Task", TokenType.TypeName },
            { "Result", TokenType.TypeName },
            { "Choice", TokenType.TypeName },

            // Common functions
            { "printfn", TokenType.Method },
            { "printf", TokenType.Method },
            { "sprintf", TokenType.Method },
            { "failwithf", TokenType.Method },
            { "ignore", TokenType.Method },
            { "fst", TokenType.Method },
            { "snd", TokenType.Method },
            { "id", TokenType.Method },
            { "box", TokenType.Method },
            { "unbox", TokenType.Method },
            { "typeof", TokenType.Method },
            { "sizeof", TokenType.Method },
            { "nameof", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments
            new TokenPattern(@"///[^\n]*", TokenType.DocComment, 100),

            // Multi-line comments (nested)
            new TokenPattern(@"\(\*[\s\S]*?\*\)", TokenType.MultiLineComment, 95),

            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 90),

            // Verbatim strings
            new TokenPattern(@"@""(?:[^""]|"""")*""", TokenType.String, 85),

            // Triple-quoted strings
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 85),

            // Regular strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 80),

            // Character literals
            new TokenPattern(@"'(?:[^'\\]|\\.)'", TokenType.Character, 80),

            // Attributes
            new TokenPattern(@"\[<[^\]]+>\]", TokenType.Attribute, 75),

            // Numbers (hex)
            new TokenPattern(@"\b0[xX][0-9a-fA-F]+[uU]?[lLyYsS]?\b", TokenType.Number, 70),

            // Numbers (binary)
            new TokenPattern(@"\b0[bB][01]+[uU]?[lLyYsS]?\b", TokenType.Number, 70),

            // Numbers (octal)
            new TokenPattern(@"\b0[oO][0-7]+[uU]?[lLyYsS]?\b", TokenType.Number, 70),

            // Numbers (float/decimal)
            new TokenPattern(@"\b\d+\.?\d*(?:[eE][+-]?\d+)?[fFmM]?\b", TokenType.Number, 70),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_']*\b", TokenType.TypeName, 50),

            // Operators
            new TokenPattern(@"<-|->|::|\|>|<\||>>|<<|\.\.|:>|:\?>|[+\-*/%=<>!&|^~@?:]+", TokenType.Operator, 40),

            // Identifiers (can include apostrophe)
            new TokenPattern(@"\b[a-z_][a-zA-Z0-9_']*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.<>]+", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.FSharp;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
