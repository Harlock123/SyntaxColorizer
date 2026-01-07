using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Scala programming language.
/// </summary>
public class ScalaTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static ScalaTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "match", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "yield", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "finally", TokenType.ControlKeyword },

            // Declaration keywords
            { "val", TokenType.Keyword },
            { "var", TokenType.Keyword },
            { "def", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "trait", TokenType.Keyword },
            { "object", TokenType.Keyword },
            { "extends", TokenType.Keyword },
            { "with", TokenType.Keyword },
            { "type", TokenType.Keyword },
            { "package", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "new", TokenType.Keyword },
            { "given", TokenType.Keyword },
            { "using", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "export", TokenType.Keyword },

            // Modifiers
            { "abstract", TokenType.Keyword },
            { "final", TokenType.Keyword },
            { "sealed", TokenType.Keyword },
            { "implicit", TokenType.Keyword },
            { "lazy", TokenType.Keyword },
            { "override", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "protected", TokenType.Keyword },
            { "public", TokenType.Keyword },
            { "inline", TokenType.Keyword },
            { "opaque", TokenType.Keyword },
            { "transparent", TokenType.Keyword },
            { "open", TokenType.Keyword },
            { "infix", TokenType.Keyword },

            // Other keywords
            { "this", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "forSome", TokenType.Keyword },
            { "macro", TokenType.Keyword },
            { "then", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "extension", TokenType.Keyword },
            { "derives", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Built-in types
            { "Any", TokenType.TypeName },
            { "AnyRef", TokenType.TypeName },
            { "AnyVal", TokenType.TypeName },
            { "Nothing", TokenType.TypeName },
            { "Null", TokenType.TypeName },
            { "Unit", TokenType.TypeName },
            { "Boolean", TokenType.TypeName },
            { "Byte", TokenType.TypeName },
            { "Short", TokenType.TypeName },
            { "Int", TokenType.TypeName },
            { "Long", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Double", TokenType.TypeName },
            { "Char", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Symbol", TokenType.TypeName },
            { "List", TokenType.TypeName },
            { "Seq", TokenType.TypeName },
            { "Set", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "Vector", TokenType.TypeName },
            { "Array", TokenType.TypeName },
            { "Option", TokenType.TypeName },
            { "Some", TokenType.TypeName },
            { "None", TokenType.TypeName },
            { "Either", TokenType.TypeName },
            { "Left", TokenType.TypeName },
            { "Right", TokenType.TypeName },
            { "Try", TokenType.TypeName },
            { "Success", TokenType.TypeName },
            { "Failure", TokenType.TypeName },
            { "Future", TokenType.TypeName },
            { "Promise", TokenType.TypeName },
            { "Tuple", TokenType.TypeName },
            { "Function", TokenType.TypeName },
            { "PartialFunction", TokenType.TypeName },

            // Common methods
            { "println", TokenType.Method },
            { "print", TokenType.Method },
            { "require", TokenType.Method },
            { "assert", TokenType.Method }
        };

        _patterns = new List<TokenPattern>
        {
            // Single-line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 100),

            // Multi-line comments (including nested)
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 100),

            // ScalaDoc comments
            new TokenPattern(@"/\*\*[\s\S]*?\*/", TokenType.DocComment, 100),

            // Multi-line strings (triple-quoted)
            new TokenPattern(@"""""""[\s\S]*?""""""", TokenType.String, 95),

            // Interpolated strings
            new TokenPattern(@"[sf]""(?:[^""\\]|\\.|(\$\{[^}]*\})|(\$\w+))*""", TokenType.String, 92),

            // Regular strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 90),

            // Character literals
            new TokenPattern(@"'(?:[^'\\]|\\.)'", TokenType.Character, 90),

            // Symbol literals
            new TokenPattern(@"'\w+", TokenType.Constant, 85),

            // Annotations
            new TokenPattern(@"@\w+(?:\.\w+)*", TokenType.Attribute, 85),

            // Hexadecimal numbers
            new TokenPattern(@"0[xX][0-9a-fA-F_]+[Ll]?", TokenType.Number, 80),

            // Float/Double numbers
            new TokenPattern(@"\d[\d_]*\.[\d_]+(?:[eE][+-]?[\d_]+)?[fFdD]?", TokenType.Number, 75),
            new TokenPattern(@"\d[\d_]*[eE][+-]?[\d_]+[fFdD]?", TokenType.Number, 75),
            new TokenPattern(@"\d[\d_]*[fFdD]", TokenType.Number, 75),

            // Long/Integer numbers
            new TokenPattern(@"\d[\d_]*[Ll]?", TokenType.Number, 70),

            // Type parameters
            new TokenPattern(@"\[\s*[A-Z]\w*(?:\s*,\s*[A-Z]\w*)*\s*\]", TokenType.TypeName, 65),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z]\w*\b", TokenType.TypeName, 60),

            // Method calls
            new TokenPattern(@"\b[a-z_]\w*(?=\s*[\[(])", TokenType.Method, 55),

            // Operators
            new TokenPattern(@"=>|<-|::|[+\-*/%]=?|&&|\|\||[&|^~]=?|<<?=?|>>?>?=?|===?|!=|!", TokenType.Operator, 45),

            // Underscore wildcard
            new TokenPattern(@"\b_\b", TokenType.Keyword, 40),

            // Identifiers
            new TokenPattern(@"\b[a-z_]\w*\b", TokenType.Identifier, 30),

            // Backtick identifiers
            new TokenPattern(@"`[^`]+`", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:@#]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Scala;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
