using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Elixir programming language.
/// </summary>
public class ElixirTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static ElixirTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "unless", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "cond", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "with", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "rescue", TokenType.ControlKeyword },
            { "after", TokenType.ControlKeyword },
            { "raise", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "receive", TokenType.ControlKeyword },

            // Definition keywords
            { "def", TokenType.Keyword },
            { "defp", TokenType.Keyword },
            { "defmodule", TokenType.Keyword },
            { "defmacro", TokenType.Keyword },
            { "defmacrop", TokenType.Keyword },
            { "defguard", TokenType.Keyword },
            { "defguardp", TokenType.Keyword },
            { "defstruct", TokenType.Keyword },
            { "defprotocol", TokenType.Keyword },
            { "defimpl", TokenType.Keyword },
            { "defexception", TokenType.Keyword },
            { "defdelegate", TokenType.Keyword },
            { "defoverridable", TokenType.Keyword },
            { "defcallback", TokenType.Keyword },

            // Other keywords
            { "do", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "fn", TokenType.Keyword },
            { "when", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "require", TokenType.Keyword },
            { "alias", TokenType.Keyword },
            { "use", TokenType.Keyword },
            { "quote", TokenType.Keyword },
            { "unquote", TokenType.Keyword },
            { "unquote_splicing", TokenType.Keyword },
            { "super", TokenType.Keyword },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "nil", TokenType.Constant },

            // Special atoms
            { "__MODULE__", TokenType.Constant },
            { "__DIR__", TokenType.Constant },
            { "__ENV__", TokenType.Constant },
            { "__CALLER__", TokenType.Constant },
            { "__STACKTRACE__", TokenType.Constant },

            // Common types (modules)
            { "String", TokenType.TypeName },
            { "Integer", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "List", TokenType.TypeName },
            { "Map", TokenType.TypeName },
            { "Tuple", TokenType.TypeName },
            { "Atom", TokenType.TypeName },
            { "Function", TokenType.TypeName },
            { "Port", TokenType.TypeName },
            { "PID", TokenType.TypeName },
            { "Reference", TokenType.TypeName },
            { "Binary", TokenType.TypeName },
            { "Bitstring", TokenType.TypeName },
            { "Keyword", TokenType.TypeName },
            { "Range", TokenType.TypeName },
            { "Regex", TokenType.TypeName },
            { "IO", TokenType.TypeName },
            { "File", TokenType.TypeName },
            { "Path", TokenType.TypeName },
            { "System", TokenType.TypeName },
            { "Enum", TokenType.TypeName },
            { "Stream", TokenType.TypeName },
            { "Task", TokenType.TypeName },
            { "Agent", TokenType.TypeName },
            { "GenServer", TokenType.TypeName },
            { "Supervisor", TokenType.TypeName },
            { "Application", TokenType.TypeName },
            { "Process", TokenType.TypeName },
            { "Node", TokenType.TypeName },
            { "Module", TokenType.TypeName },
            { "Kernel", TokenType.TypeName },
            { "Exception", TokenType.TypeName },
            { "RuntimeError", TokenType.TypeName },
            { "ArgumentError", TokenType.TypeName },

            // Common functions
            { "inspect", TokenType.Method },
            { "length", TokenType.Method },
            { "hd", TokenType.Method },
            { "tl", TokenType.Method },
            { "elem", TokenType.Method },
            { "put_elem", TokenType.Method },
            { "tuple_size", TokenType.Method },
            { "map_size", TokenType.Method },
            { "is_atom", TokenType.Method },
            { "is_binary", TokenType.Method },
            { "is_boolean", TokenType.Method },
            { "is_float", TokenType.Method },
            { "is_function", TokenType.Method },
            { "is_integer", TokenType.Method },
            { "is_list", TokenType.Method },
            { "is_map", TokenType.Method },
            { "is_nil", TokenType.Method },
            { "is_number", TokenType.Method },
            { "is_pid", TokenType.Method },
            { "is_port", TokenType.Method },
            { "is_reference", TokenType.Method },
            { "is_tuple", TokenType.Method },
            { "spawn", TokenType.Method },
            { "spawn_link", TokenType.Method },
            { "send", TokenType.Method },
            { "self", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Module attributes (@doc, @spec, etc.)
            new TokenPattern(@"@[a-z_][a-z0-9_]*", TokenType.Attribute, 100),

            // Documentation strings (heredoc)
            new TokenPattern(@"@(?:doc|moduledoc|typedoc)\s+~[sSwwcCdDrRnN]?""""""[\s\S]*?""""""", TokenType.DocComment, 98),

            // Block comments (none in Elixir, but #{ } is interpolation)

            // Line comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 95),

            // Sigils with heredoc
            new TokenPattern(@"~[sSwwcCdDrRnN]?""""""[\s\S]*?""""""", TokenType.String, 92),
            new TokenPattern(@"~[sSwwcCdDrRnN]?'''[\s\S]*?'''", TokenType.String, 92),

            // Sigils (various delimiters)
            new TokenPattern(@"~[sSwwcCdDrRnN]?[/|\""\'\[\]\(\)\{\}<>][^/|\""\'\[\]\(\)\{\}<>]*[/|\""\'\[\]\(\)\{\}<>]", TokenType.String, 90),

            // Heredoc strings
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 88),
            new TokenPattern(@"'''[\s\S]*?'''", TokenType.String, 88),

            // Regular strings
            new TokenPattern(@"""(?:[^""\\#]|\\.|#(?!\{))*""", TokenType.String, 85),

            // Charlists
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 85),

            // Atoms (quoted)
            new TokenPattern(@":""[^""]*""", TokenType.Constant, 82),

            // Atoms
            new TokenPattern(@":[a-zA-Z_][a-zA-Z0-9_]*[?!]?", TokenType.Constant, 80),

            // Numbers (binary)
            new TokenPattern(@"0b[01_]+", TokenType.Number, 75),

            // Numbers (octal)
            new TokenPattern(@"0o[0-7_]+", TokenType.Number, 75),

            // Numbers (hex)
            new TokenPattern(@"0x[0-9a-fA-F_]+", TokenType.Number, 75),

            // Numbers (float with exponent)
            new TokenPattern(@"\d[\d_]*\.\d[\d_]*(?:[eE][+-]?\d[\d_]*)?", TokenType.Number, 70),

            // Numbers (integer)
            new TokenPattern(@"\d[\d_]*", TokenType.Number, 65),

            // Module names (PascalCase with dots)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_]*(?:\.[A-Z][a-zA-Z0-9_]*)*\b", TokenType.TypeName, 55),

            // Function calls with parentheses
            new TokenPattern(@"\b[a-z_][a-z0-9_]*[?!]?(?=\()", TokenType.Method, 52),

            // Variables and function names
            new TokenPattern(@"\b[a-z_][a-z0-9_]*[?!]?\b", TokenType.Identifier, 50),

            // Operators
            new TokenPattern(@"<>|<-|->|=>|\|>|<\||<<<|>>>|~~~|\+\+|--|\.\.\.?|::|[+\-*/%=<>!&|^~@]+", TokenType.Operator, 40),

            // Punctuation
            new TokenPattern(@"[(){}\[\];,.]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Elixir;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
