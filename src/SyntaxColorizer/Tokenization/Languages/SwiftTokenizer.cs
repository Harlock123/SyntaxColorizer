using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Swift programming language.
/// </summary>
public class SwiftTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static SwiftTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "guard", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "default", TokenType.ControlKeyword },
            { "where", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "repeat", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "fallthrough", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "throws", TokenType.ControlKeyword },
            { "rethrows", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "defer", TokenType.ControlKeyword },

            // Declaration keywords
            { "let", TokenType.Keyword },
            { "var", TokenType.Keyword },
            { "func", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "struct", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "protocol", TokenType.Keyword },
            { "extension", TokenType.Keyword },
            { "typealias", TokenType.Keyword },
            { "associatedtype", TokenType.Keyword },
            { "init", TokenType.Keyword },
            { "deinit", TokenType.Keyword },
            { "subscript", TokenType.Keyword },
            { "operator", TokenType.Keyword },
            { "precedencegroup", TokenType.Keyword },
            { "import", TokenType.Keyword },

            // Modifiers
            { "public", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "fileprivate", TokenType.Keyword },
            { "internal", TokenType.Keyword },
            { "open", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "final", TokenType.Keyword },
            { "override", TokenType.Keyword },
            { "required", TokenType.Keyword },
            { "convenience", TokenType.Keyword },
            { "lazy", TokenType.Keyword },
            { "weak", TokenType.Keyword },
            { "unowned", TokenType.Keyword },
            { "mutating", TokenType.Keyword },
            { "nonmutating", TokenType.Keyword },
            { "dynamic", TokenType.Keyword },
            { "indirect", TokenType.Keyword },
            { "inout", TokenType.Keyword },
            { "async", TokenType.Keyword },
            { "await", TokenType.Keyword },
            { "actor", TokenType.Keyword },
            { "nonisolated", TokenType.Keyword },
            { "isolated", TokenType.Keyword },

            // Other keywords
            { "self", TokenType.Keyword },
            { "Self", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "is", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "get", TokenType.Keyword },
            { "set", TokenType.Keyword },
            { "willSet", TokenType.Keyword },
            { "didSet", TokenType.Keyword },
            { "some", TokenType.Keyword },
            { "any", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "nil", TokenType.Constant },

            // Built-in types
            { "Int", TokenType.TypeName },
            { "Int8", TokenType.TypeName },
            { "Int16", TokenType.TypeName },
            { "Int32", TokenType.TypeName },
            { "Int64", TokenType.TypeName },
            { "UInt", TokenType.TypeName },
            { "UInt8", TokenType.TypeName },
            { "UInt16", TokenType.TypeName },
            { "UInt32", TokenType.TypeName },
            { "UInt64", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Double", TokenType.TypeName },
            { "Bool", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Character", TokenType.TypeName },
            { "Array", TokenType.TypeName },
            { "Dictionary", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Optional", TokenType.TypeName },
            { "Result", TokenType.TypeName },
            { "Void", TokenType.TypeName },
            { "Never", TokenType.TypeName },
            { "Any", TokenType.TypeName },
            { "AnyObject", TokenType.TypeName },
            { "Error", TokenType.TypeName },
            { "Codable", TokenType.TypeName },
            { "Encodable", TokenType.TypeName },
            { "Decodable", TokenType.TypeName },
            { "Hashable", TokenType.TypeName },
            { "Equatable", TokenType.TypeName },
            { "Comparable", TokenType.TypeName },
            { "Identifiable", TokenType.TypeName },

            // Common functions
            { "print", TokenType.Method },
            { "debugPrint", TokenType.Method },
            { "fatalError", TokenType.Method },
            { "precondition", TokenType.Method },
            { "assert", TokenType.Method },
            { "preconditionFailure", TokenType.Method },
            { "assertionFailure", TokenType.Method }
        };

        _patterns = new List<TokenPattern>
        {
            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 100),

            // Multi-line comments (including nested)
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 100),

            // Documentation comments
            new TokenPattern(@"///[^\n]*", TokenType.DocComment, 100),

            // Multi-line strings
            new TokenPattern(@"""""""[\s\S]*?""""""", TokenType.String, 95),

            // Regular strings with interpolation
            new TokenPattern(@"""(?:[^""\\]|\\.|(\\\([^)]*\)))*""", TokenType.String, 90),

            // Attributes
            new TokenPattern(@"@\w+", TokenType.Attribute, 85),

            // Compiler directives
            new TokenPattern(@"#(?:if|elseif|else|endif|sourceLocation|warning|error|available|selector|keyPath|colorLiteral|fileLiteral|imageLiteral)\b[^\n]*", TokenType.Preprocessor, 85),

            // Hexadecimal numbers
            new TokenPattern(@"0x[0-9a-fA-F_]+(?:\.[0-9a-fA-F_]+)?(?:[pP][+-]?[0-9_]+)?", TokenType.Number, 80),

            // Binary numbers
            new TokenPattern(@"0b[01_]+", TokenType.Number, 80),

            // Octal numbers
            new TokenPattern(@"0o[0-7_]+", TokenType.Number, 80),

            // Float numbers
            new TokenPattern(@"\d[\d_]*\.[\d_]+(?:[eE][+-]?[\d_]+)?", TokenType.Number, 75),
            new TokenPattern(@"\d[\d_]*[eE][+-]?[\d_]+", TokenType.Number, 75),

            // Integer numbers
            new TokenPattern(@"\d[\d_]*", TokenType.Number, 70),

            // Generic type parameters
            new TokenPattern(@"<\s*\w+(?:\s*:\s*\w+)?(?:\s*,\s*\w+(?:\s*:\s*\w+)?)*\s*>", TokenType.TypeName, 65),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z]\w*\b", TokenType.TypeName, 60),

            // Function calls
            new TokenPattern(@"\b[a-z_]\w*(?=\s*[\(<])", TokenType.Method, 55),

            // Optional chaining and nil coalescing
            new TokenPattern(@"\?\?|\?\.|\?", TokenType.Operator, 50),

            // Operators
            new TokenPattern(@"\.\.\.|\.\.<|->|[+\-*/%]=?|&&|\|\||[&|^~]=?|<<?=?|>>?=?|===?|!==?|!", TokenType.Operator, 45),

            // Identifiers
            new TokenPattern(@"\b[a-z_]\w*\b", TokenType.Identifier, 30),

            // Backtick identifiers
            new TokenPattern(@"`\w+`", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:@#]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Swift;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
