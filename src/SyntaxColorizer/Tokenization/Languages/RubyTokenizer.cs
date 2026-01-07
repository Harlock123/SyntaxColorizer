using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Ruby programming language.
/// </summary>
public class RubyTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static RubyTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow keywords
            { "if", TokenType.ControlKeyword },
            { "elsif", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "unless", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "when", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "until", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "next", TokenType.ControlKeyword },
            { "redo", TokenType.ControlKeyword },
            { "retry", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "yield", TokenType.ControlKeyword },
            { "raise", TokenType.ControlKeyword },
            { "rescue", TokenType.ControlKeyword },
            { "ensure", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },

            // Definition keywords
            { "def", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "module", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "begin", TokenType.Keyword },
            { "do", TokenType.Keyword },
            { "lambda", TokenType.Keyword },
            { "proc", TokenType.Keyword },

            // Modifier keywords
            { "public", TokenType.Keyword },
            { "private", TokenType.Keyword },
            { "protected", TokenType.Keyword },
            { "attr_reader", TokenType.Keyword },
            { "attr_writer", TokenType.Keyword },
            { "attr_accessor", TokenType.Keyword },
            { "attr", TokenType.Keyword },
            { "alias", TokenType.Keyword },
            { "undef", TokenType.Keyword },

            // Other keywords
            { "require", TokenType.Keyword },
            { "require_relative", TokenType.Keyword },
            { "include", TokenType.Keyword },
            { "extend", TokenType.Keyword },
            { "prepend", TokenType.Keyword },
            { "defined?", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "self", TokenType.Keyword },
            { "__FILE__", TokenType.Keyword },
            { "__LINE__", TokenType.Keyword },
            { "__ENCODING__", TokenType.Keyword },
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "then", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "nil", TokenType.Constant },

            // Built-in classes
            { "Array", TokenType.TypeName },
            { "Hash", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Integer", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Symbol", TokenType.TypeName },
            { "Range", TokenType.TypeName },
            { "Regexp", TokenType.TypeName },
            { "Time", TokenType.TypeName },
            { "File", TokenType.TypeName },
            { "IO", TokenType.TypeName },
            { "Dir", TokenType.TypeName },
            { "Exception", TokenType.TypeName },
            { "Object", TokenType.TypeName },
            { "Class", TokenType.TypeName },
            { "Module", TokenType.TypeName },
            { "Proc", TokenType.TypeName },
            { "Method", TokenType.TypeName },
            { "Thread", TokenType.TypeName },
            { "Fiber", TokenType.TypeName },
            { "Struct", TokenType.TypeName },
            { "Enumerable", TokenType.TypeName },
            { "Comparable", TokenType.TypeName },
            { "Kernel", TokenType.TypeName },

            // Common methods
            { "puts", TokenType.Method },
            { "print", TokenType.Method },
            { "p", TokenType.Method },
            { "gets", TokenType.Method },
            { "chomp", TokenType.Method },
            { "each", TokenType.Method },
            { "map", TokenType.Method },
            { "select", TokenType.Method },
            { "reject", TokenType.Method },
            { "reduce", TokenType.Method },
            { "inject", TokenType.Method },
            { "find", TokenType.Method },
            { "any?", TokenType.Method },
            { "all?", TokenType.Method },
            { "none?", TokenType.Method },
            { "empty?", TokenType.Method },
            { "nil?", TokenType.Method },
            { "new", TokenType.Method },
            { "initialize", TokenType.Method }
        };

        _patterns = new List<TokenPattern>
        {
            // Single-line comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Multi-line embedded documentation
            new TokenPattern(@"=begin[\s\S]*?=end", TokenType.MultiLineComment, 100),

            // Here-documents
            new TokenPattern(@"<<[-~]?['\`""]?(\w+)['\`""]?.*?\n[\s\S]*?\n\s*\1", TokenType.String, 98),

            // Regex literals
            new TokenPattern(@"/(?:[^/\\]|\\.)+/[imxo]*", TokenType.Regex, 95),
            new TokenPattern(@"%r\{(?:[^}\\]|\\.)*\}[imxo]*", TokenType.Regex, 95),
            new TokenPattern(@"%r\[(?:[^\]\\]|\\.)*\][imxo]*", TokenType.Regex, 95),
            new TokenPattern(@"%r\((?:[^)\\]|\\.)*\)[imxo]*", TokenType.Regex, 95),

            // Double-quoted strings with interpolation
            new TokenPattern(@"""(?:[^""\\]|\\.|#\{[^}]*\})*""", TokenType.String, 90),

            // Single-quoted strings (no interpolation)
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 90),

            // Percent strings
            new TokenPattern(@"%[qQwWiIxs]?\{(?:[^}\\]|\\.)*\}", TokenType.String, 90),
            new TokenPattern(@"%[qQwWiIxs]?\[(?:[^\]\\]|\\.)*\]", TokenType.String, 90),
            new TokenPattern(@"%[qQwWiIxs]?\((?:[^)\\]|\\.)*\)", TokenType.String, 90),
            new TokenPattern(@"%[qQwWiIxs]?<(?:[^>\\]|\\.)*>", TokenType.String, 90),

            // Symbols
            new TokenPattern(@":\w+[!?]?", TokenType.Constant, 85),
            new TokenPattern(@":""[^""]*""", TokenType.Constant, 85),

            // Instance variables
            new TokenPattern(@"@{1,2}\w+", TokenType.Field, 80),

            // Global variables
            new TokenPattern(@"\$[\w]+", TokenType.ShellVariable, 80),
            new TokenPattern(@"\$[!@&`'+~=/\\,;.<>*$?:""]", TokenType.ShellVariable, 80),

            // Constants (uppercase start)
            new TokenPattern(@"\b[A-Z][A-Z0-9_]+\b", TokenType.Constant, 75),

            // Class/module names (capitalized)
            new TokenPattern(@"\b[A-Z]\w*\b", TokenType.TypeName, 70),

            // Hexadecimal numbers
            new TokenPattern(@"0[xX][0-9a-fA-F_]+", TokenType.Number, 65),

            // Binary numbers
            new TokenPattern(@"0[bB][01_]+", TokenType.Number, 65),

            // Octal numbers
            new TokenPattern(@"0[oO]?[0-7_]+", TokenType.Number, 65),

            // Float numbers
            new TokenPattern(@"\d[\d_]*\.[\d_]+(?:[eE][+-]?[\d_]+)?", TokenType.Number, 60),
            new TokenPattern(@"\d[\d_]*[eE][+-]?[\d_]+", TokenType.Number, 60),

            // Integer numbers
            new TokenPattern(@"\d[\d_]*", TokenType.Number, 55),

            // Method definitions
            new TokenPattern(@"(?<=def\s+)\w+[!?=]?", TokenType.Method, 50),

            // Method calls with parentheses
            new TokenPattern(@"\b\w+[!?]?(?=\s*[\(\.])", TokenType.Method, 45),

            // Operators
            new TokenPattern(@"<=>|<<?|>>?|&&|\|\||[+\-*/%&|^~]=?|[<>=!]=|===?|!~|=~|\*\*=?|\.\.\.?", TokenType.Operator, 40),

            // Identifiers
            new TokenPattern(@"\b[a-z_]\w*[!?]?\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:?]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Ruby;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
