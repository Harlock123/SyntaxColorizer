using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Groovy programming language.
/// </summary>
public class GroovyTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static GroovyTokenizer()
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
            { "assert", TokenType.ControlKeyword },

            // Declaration keywords
            { "class", TokenType.Keyword },
            { "interface", TokenType.Keyword },
            { "trait", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "extends", TokenType.Keyword },
            { "implements", TokenType.Keyword },
            { "package", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "def", TokenType.Keyword },
            { "var", TokenType.Keyword },
            { "final", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "abstract", TokenType.Keyword },
            { "synchronized", TokenType.Keyword },
            { "volatile", TokenType.Keyword },
            { "transient", TokenType.Keyword },
            { "native", TokenType.Keyword },
            { "strictfp", TokenType.Keyword },

            // Access modifiers
            { "public", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "protected", TokenType.Keyword },

            // Other keywords
            { "new", TokenType.Keyword },
            { "this", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "instanceof", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "with", TokenType.Keyword },

            // Groovy-specific
            { "it", TokenType.Keyword },
            { "delegate", TokenType.Keyword },
            { "owner", TokenType.Keyword },
            { "lazy", TokenType.Keyword },
            { "mixin", TokenType.Keyword },
            { "category", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Built-in types
            { "void", TokenType.TypeName },
            { "boolean", TokenType.TypeName },
            { "byte", TokenType.TypeName },
            { "char", TokenType.TypeName },
            { "short", TokenType.TypeName },
            { "int", TokenType.TypeName },
            { "long", TokenType.TypeName },
            { "float", TokenType.TypeName },
            { "double", TokenType.TypeName },
            { "Boolean", TokenType.TypeName },
            { "Byte", TokenType.TypeName },
            { "Character", TokenType.TypeName },
            { "Short", TokenType.TypeName },
            { "Integer", TokenType.TypeName },
            { "Long", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Double", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Object", TokenType.TypeName },
            { "List", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Collection", TokenType.TypeName },
            { "Closure", TokenType.TypeName },
            { "BigInteger", TokenType.TypeName },
            { "BigDecimal", TokenType.TypeName },
            { "File", TokenType.TypeName },
            { "Date", TokenType.TypeName },
            { "Pattern", TokenType.TypeName },
            { "Matcher", TokenType.TypeName },
            { "GString", TokenType.TypeName },
            { "Range", TokenType.TypeName },
            { "Binding", TokenType.TypeName },
            { "Script", TokenType.TypeName },

            // Common methods
            { "println", TokenType.Method },
            { "print", TokenType.Method },
            { "each", TokenType.Method },
            { "eachWithIndex", TokenType.Method },
            { "collect", TokenType.Method },
            { "find", TokenType.Method },
            { "findAll", TokenType.Method },
            { "grep", TokenType.Method },
            { "any", TokenType.Method },
            { "every", TokenType.Method },
            { "inject", TokenType.Method },
            { "groupBy", TokenType.Method },
            { "sort", TokenType.Method },
            { "reverse", TokenType.Method },
            { "unique", TokenType.Method },
            { "flatten", TokenType.Method },
            { "join", TokenType.Method },
            { "split", TokenType.Method },
            { "tokenize", TokenType.Method },
            { "withReader", TokenType.Method },
            { "withWriter", TokenType.Method },
            { "execute", TokenType.Method },
            { "getText", TokenType.Method },
            { "readLines", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments
            new TokenPattern(@"/\*\*[\s\S]*?\*/", TokenType.DocComment, 100),

            // Multi-line comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 95),

            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 90),

            // Shebang
            new TokenPattern(@"^#![^\n]*", TokenType.Comment, 90),

            // Triple-quoted strings (GString)
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 85),
            new TokenPattern(@"'''[\s\S]*?'''", TokenType.String, 85),

            // Slashy strings (regex)
            new TokenPattern(@"/(?:[^/\\]|\\.)+/", TokenType.String, 85),

            // Dollar slashy strings
            new TokenPattern(@"\$/[\s\S]*?/\$", TokenType.String, 85),

            // Regular strings
            new TokenPattern(@"""(?:[^""\\$]|\\.|\$[^{]|\$\{[^}]*\})*""", TokenType.String, 80),
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 80),

            // Annotations
            new TokenPattern(@"@[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Attribute, 75),

            // Numbers (hex)
            new TokenPattern(@"\b0[xX][0-9a-fA-F]+[gGlLiIdDfF]?\b", TokenType.Number, 70),

            // Numbers (binary)
            new TokenPattern(@"\b0[bB][01]+[gGlLiIdDfF]?\b", TokenType.Number, 70),

            // Numbers (octal)
            new TokenPattern(@"\b0[0-7]+[gGlLiIdDfF]?\b", TokenType.Number, 70),

            // Numbers (decimal/float)
            new TokenPattern(@"\b\d+\.?\d*(?:[eE][+-]?\d+)?[gGlLiIdDfF]?\b", TokenType.Number, 70),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_]*\b", TokenType.TypeName, 50),

            // Operators
            new TokenPattern(@"<=>|<>|\.\.\.?|\*\.|\.@|\?:|&\.|->|~=|\*\*|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 40),

            // Identifiers
            new TokenPattern(@"\b[a-z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.]+", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Groovy;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
