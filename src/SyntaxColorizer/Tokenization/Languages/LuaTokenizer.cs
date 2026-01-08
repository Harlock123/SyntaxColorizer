using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Lua programming language.
/// </summary>
public class LuaTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static LuaTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "then", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "elseif", TokenType.ControlKeyword },
            { "end", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "repeat", TokenType.ControlKeyword },
            { "until", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "goto", TokenType.ControlKeyword },

            // Keywords
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "local", TokenType.Keyword },
            { "function", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "nil", TokenType.Constant },

            // Built-in types/metatables
            { "self", TokenType.Keyword },
            { "metatable", TokenType.TypeName },

            // Common standard library
            { "print", TokenType.Method },
            { "pairs", TokenType.Method },
            { "ipairs", TokenType.Method },
            { "next", TokenType.Method },
            { "type", TokenType.Method },
            { "tostring", TokenType.Method },
            { "tonumber", TokenType.Method },
            { "error", TokenType.Method },
            { "assert", TokenType.Method },
            { "pcall", TokenType.Method },
            { "xpcall", TokenType.Method },
            { "require", TokenType.Method },
            { "loadfile", TokenType.Method },
            { "dofile", TokenType.Method },
            { "setmetatable", TokenType.Method },
            { "getmetatable", TokenType.Method },
            { "rawget", TokenType.Method },
            { "rawset", TokenType.Method },
            { "rawequal", TokenType.Method },
            { "select", TokenType.Method },
            { "unpack", TokenType.Method },
            { "collectgarbage", TokenType.Method },

            // Standard libraries
            { "string", TokenType.TypeName },
            { "table", TokenType.TypeName },
            { "math", TokenType.TypeName },
            { "io", TokenType.TypeName },
            { "os", TokenType.TypeName },
            { "debug", TokenType.TypeName },
            { "coroutine", TokenType.TypeName },
            { "package", TokenType.TypeName },
            { "utf8", TokenType.TypeName },
        };

        _patterns = new List<TokenPattern>
        {
            // Long comments --[[ ]]
            new TokenPattern(@"--\[\[[\s\S]*?\]\]", TokenType.MultiLineComment, 100),

            // Single-line comments
            new TokenPattern(@"--[^\n]*", TokenType.Comment, 95),

            // Long strings [[ ]]
            new TokenPattern(@"\[\[[\s\S]*?\]\]", TokenType.String, 90),

            // Double-quoted strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 85),

            // Single-quoted strings
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 85),

            // Numbers (hex)
            new TokenPattern(@"\b0[xX][0-9a-fA-F]+\b", TokenType.Number, 70),

            // Numbers (decimal/float)
            new TokenPattern(@"\b\d+\.?\d*(?:[eE][+-]?\d+)?\b", TokenType.Number, 70),

            // Labels (::name::)
            new TokenPattern(@"::[a-zA-Z_][a-zA-Z0-9_]*::", TokenType.Attribute, 60),

            // Operators
            new TokenPattern(@"\.\.\.?|[+\-*/%^#=<>~]=?|~=", TokenType.Operator, 40),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.:]+", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Lua;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
