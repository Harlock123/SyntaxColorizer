using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Rust source code.
/// </summary>
public class RustTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static RustTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control keywords
            ["if"] = TokenType.ControlKeyword,
            ["else"] = TokenType.ControlKeyword,
            ["match"] = TokenType.ControlKeyword,
            ["for"] = TokenType.ControlKeyword,
            ["while"] = TokenType.ControlKeyword,
            ["loop"] = TokenType.ControlKeyword,
            ["break"] = TokenType.ControlKeyword,
            ["continue"] = TokenType.ControlKeyword,
            ["return"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["fn"] = TokenType.Keyword,
            ["struct"] = TokenType.Keyword,
            ["enum"] = TokenType.Keyword,
            ["trait"] = TokenType.Keyword,
            ["impl"] = TokenType.Keyword,
            ["type"] = TokenType.Keyword,
            ["mod"] = TokenType.Keyword,
            ["use"] = TokenType.Keyword,
            ["crate"] = TokenType.Keyword,
            ["extern"] = TokenType.Keyword,
            ["macro_rules"] = TokenType.Keyword,
            ["union"] = TokenType.Keyword,

            // Modifier keywords
            ["pub"] = TokenType.Keyword,
            ["const"] = TokenType.Keyword,
            ["static"] = TokenType.Keyword,
            ["mut"] = TokenType.Keyword,
            ["ref"] = TokenType.Keyword,
            ["unsafe"] = TokenType.Keyword,
            ["async"] = TokenType.Keyword,
            ["await"] = TokenType.Keyword,
            ["dyn"] = TokenType.Keyword,
            ["move"] = TokenType.Keyword,

            // Other keywords
            ["let"] = TokenType.Keyword,
            ["as"] = TokenType.Keyword,
            ["in"] = TokenType.Keyword,
            ["where"] = TokenType.Keyword,
            ["Self"] = TokenType.Keyword,
            ["self"] = TokenType.Keyword,
            ["super"] = TokenType.Keyword,

            // Built-in types
            ["bool"] = TokenType.TypeName,
            ["char"] = TokenType.TypeName,
            ["str"] = TokenType.TypeName,
            ["i8"] = TokenType.TypeName,
            ["i16"] = TokenType.TypeName,
            ["i32"] = TokenType.TypeName,
            ["i64"] = TokenType.TypeName,
            ["i128"] = TokenType.TypeName,
            ["isize"] = TokenType.TypeName,
            ["u8"] = TokenType.TypeName,
            ["u16"] = TokenType.TypeName,
            ["u32"] = TokenType.TypeName,
            ["u64"] = TokenType.TypeName,
            ["u128"] = TokenType.TypeName,
            ["usize"] = TokenType.TypeName,
            ["f32"] = TokenType.TypeName,
            ["f64"] = TokenType.TypeName,

            // Common types from std
            ["String"] = TokenType.TypeName,
            ["Vec"] = TokenType.TypeName,
            ["Option"] = TokenType.TypeName,
            ["Result"] = TokenType.TypeName,
            ["Box"] = TokenType.TypeName,
            ["Rc"] = TokenType.TypeName,
            ["Arc"] = TokenType.TypeName,
            ["Cell"] = TokenType.TypeName,
            ["RefCell"] = TokenType.TypeName,
            ["HashMap"] = TokenType.TypeName,
            ["HashSet"] = TokenType.TypeName,

            // Literals
            ["true"] = TokenType.Constant,
            ["false"] = TokenType.Constant,

            // Common macros
            ["println"] = TokenType.Method,
            ["print"] = TokenType.Method,
            ["eprintln"] = TokenType.Method,
            ["eprint"] = TokenType.Method,
            ["format"] = TokenType.Method,
            ["panic"] = TokenType.Method,
            ["assert"] = TokenType.Method,
            ["assert_eq"] = TokenType.Method,
            ["assert_ne"] = TokenType.Method,
            ["debug_assert"] = TokenType.Method,
            ["vec"] = TokenType.Method,
            ["todo"] = TokenType.Method,
            ["unimplemented"] = TokenType.Method,
            ["unreachable"] = TokenType.Method,
        };

        _patterns = new List<TokenPattern>
        {
            // Documentation comments
            new(@"///[^\r\n]*", TokenType.DocComment, 11),
            new(@"//![^\r\n]*", TokenType.DocComment, 11),

            // Multi-line comments (can be nested in Rust)
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Single-line comments
            new(CommonPatterns.SingleLineComment, TokenType.Comment, 9),

            // Attributes
            new(@"#!\??\[[^\]]*\]", TokenType.Attribute, 8),
            new(@"#\[[^\]]*\]", TokenType.Attribute, 8),

            // Raw strings
            new(@"r#*""[^""]*""#*", TokenType.String, 7),

            // Byte strings
            new(@"b""(?:[^""\\]|\\.)*""", TokenType.String, 6),
            new(@"b'(?:[^'\\]|\\.)*'", TokenType.Character, 6),

            // Regular strings
            new(CommonPatterns.DoubleQuotedString, TokenType.String, 6),

            // Character literals
            new(CommonPatterns.SingleQuotedString, TokenType.Character, 6),

            // Lifetimes
            new(@"'[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Parameter, 5),

            // Numbers
            new(@"\b0[xX][0-9a-fA-F_]+(?:i8|i16|i32|i64|i128|isize|u8|u16|u32|u64|u128|usize)?\b", TokenType.Number, 4),
            new(@"\b0[oO][0-7_]+(?:i8|i16|i32|i64|i128|isize|u8|u16|u32|u64|u128|usize)?\b", TokenType.Number, 4),
            new(@"\b0[bB][01_]+(?:i8|i16|i32|i64|i128|isize|u8|u16|u32|u64|u128|usize)?\b", TokenType.Number, 4),
            new(@"\b\d[\d_]*\.\d[\d_]*(?:[eE][+-]?\d[\d_]*)?(?:f32|f64)?\b", TokenType.Number, 4),
            new(@"\b\d[\d_]*[eE][+-]?\d[\d_]*(?:f32|f64)?\b", TokenType.Number, 4),
            new(@"\b\d[\d_]*(?:i8|i16|i32|i64|i128|isize|u8|u16|u32|u64|u128|usize|f32|f64)?\b", TokenType.Number, 4),

            // Macro invocations (with !)
            new(@"\b[a-zA-Z_][a-zA-Z0-9_]*!", TokenType.Method, 3),

            // Identifiers
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"=>|->|\.\.=?|\?\.|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,.]", TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Rust;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
