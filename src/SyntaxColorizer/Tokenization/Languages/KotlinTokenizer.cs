using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Kotlin programming language.
/// </summary>
public class KotlinTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static KotlinTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow keywords
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "when", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "finally", TokenType.ControlKeyword },

            // Declaration keywords
            { "val", TokenType.Keyword },
            { "var", TokenType.Keyword },
            { "fun", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "interface", TokenType.Keyword },
            { "object", TokenType.Keyword },
            { "package", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "typealias", TokenType.Keyword },
            { "constructor", TokenType.Keyword },
            { "init", TokenType.Keyword },
            { "companion", TokenType.Keyword },

            // Modifier keywords
            { "public", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "protected", TokenType.Keyword },
            { "internal", TokenType.Keyword },
            { "open", TokenType.Keyword },
            { "final", TokenType.Keyword },
            { "abstract", TokenType.Keyword },
            { "sealed", TokenType.Keyword },
            { "override", TokenType.Keyword },
            { "lateinit", TokenType.Keyword },
            { "lazy", TokenType.Keyword },
            { "inline", TokenType.Keyword },
            { "noinline", TokenType.Keyword },
            { "crossinline", TokenType.Keyword },
            { "reified", TokenType.Keyword },
            { "external", TokenType.Keyword },
            { "annotation", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "data", TokenType.Keyword },
            { "inner", TokenType.Keyword },
            { "tailrec", TokenType.Keyword },
            { "operator", TokenType.Keyword },
            { "infix", TokenType.Keyword },
            { "suspend", TokenType.Keyword },
            { "expect", TokenType.Keyword },
            { "actual", TokenType.Keyword },
            { "const", TokenType.Keyword },
            { "vararg", TokenType.Keyword },

            // Other keywords
            { "this", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "is", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "as?", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "!in", TokenType.Keyword },
            { "out", TokenType.Keyword },
            { "get", TokenType.Keyword },
            { "set", TokenType.Keyword },
            { "by", TokenType.Keyword },
            { "where", TokenType.Keyword },
            { "typeof", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Built-in types
            { "Any", TokenType.TypeName },
            { "Unit", TokenType.TypeName },
            { "Nothing", TokenType.TypeName },
            { "Int", TokenType.TypeName },
            { "Long", TokenType.TypeName },
            { "Short", TokenType.TypeName },
            { "Byte", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Double", TokenType.TypeName },
            { "Boolean", TokenType.TypeName },
            { "Char", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Array", TokenType.TypeName },
            { "List", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "MutableList", TokenType.TypeName },
            { "MutableSet", TokenType.TypeName },
            { "MutableMap", TokenType.TypeName },
            { "Pair", TokenType.TypeName },
            { "Triple", TokenType.TypeName },
            { "Sequence", TokenType.TypeName },
            { "Comparable", TokenType.TypeName },
            { "Iterable", TokenType.TypeName },
            { "Iterator", TokenType.TypeName },
            { "Collection", TokenType.TypeName },

            // Common functions
            { "println", TokenType.Method },
            { "print", TokenType.Method },
            { "readLine", TokenType.Method },
            { "listOf", TokenType.Method },
            { "mutableListOf", TokenType.Method },
            { "setOf", TokenType.Method },
            { "mutableSetOf", TokenType.Method },
            { "mapOf", TokenType.Method },
            { "mutableMapOf", TokenType.Method },
            { "arrayOf", TokenType.Method },
            { "sequenceOf", TokenType.Method },
            { "emptyList", TokenType.Method },
            { "emptySet", TokenType.Method },
            { "emptyMap", TokenType.Method },
            { "require", TokenType.Method },
            { "check", TokenType.Method },
            { "error", TokenType.Method },
            { "TODO", TokenType.Method },
            { "run", TokenType.Method },
            { "let", TokenType.Method },
            { "also", TokenType.Method },
            { "apply", TokenType.Method },
            { "with", TokenType.Method },
            { "takeIf", TokenType.Method },
            { "takeUnless", TokenType.Method },
            { "repeat", TokenType.Method }
        };

        _patterns = new List<TokenPattern>
        {
            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 100),

            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 100),

            // Strings
            new TokenPattern(@"""[^""]*""", TokenType.String, 90),

            // Character literals
            new TokenPattern(@"'[^']*'", TokenType.Character, 90),

            // Annotations
            new TokenPattern(@"@\w+", TokenType.Attribute, 85),

            // Numbers
            new TokenPattern(@"\b\d+\.?\d*\b", TokenType.Number, 70),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z]\w*\b", TokenType.TypeName, 50),

            // Operators
            new TokenPattern(@"[+\-*/%=<>!&|]+", TokenType.Operator, 40),

            // Identifiers
            new TokenPattern(@"\b[a-z_]\w*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:?]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Kotlin;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
