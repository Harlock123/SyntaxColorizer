using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Haskell programming language.
/// </summary>
public class HaskellTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static HaskellTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "then", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "of", TokenType.ControlKeyword },

            // Declaration keywords
            { "module", TokenType.Keyword },
            { "import", TokenType.Keyword },
            { "qualified", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "hiding", TokenType.Keyword },
            { "where", TokenType.Keyword },
            { "let", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "do", TokenType.Keyword },
            { "data", TokenType.Keyword },
            { "type", TokenType.Keyword },
            { "newtype", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "instance", TokenType.Keyword },
            { "deriving", TokenType.Keyword },
            { "default", TokenType.Keyword },
            { "infix", TokenType.Keyword },
            { "infixl", TokenType.Keyword },
            { "infixr", TokenType.Keyword },
            { "foreign", TokenType.Keyword },

            // Other keywords
            { "forall", TokenType.Keyword },
            { "mdo", TokenType.Keyword },
            { "family", TokenType.Keyword },
            { "role", TokenType.Keyword },
            { "pattern", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "stock", TokenType.Keyword },
            { "anyclass", TokenType.Keyword },
            { "via", TokenType.Keyword },
            { "proc", TokenType.Keyword },
            { "rec", TokenType.Keyword },

            // Constants
            { "True", TokenType.Constant },
            { "False", TokenType.Constant },
            { "Nothing", TokenType.Constant },
            { "Just", TokenType.Constant },
            { "Left", TokenType.Constant },
            { "Right", TokenType.Constant },
            { "LT", TokenType.Constant },
            { "EQ", TokenType.Constant },
            { "GT", TokenType.Constant },

            // Built-in types
            { "Int", TokenType.TypeName },
            { "Integer", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "Double", TokenType.TypeName },
            { "Char", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Bool", TokenType.TypeName },
            { "Maybe", TokenType.TypeName },
            { "Either", TokenType.TypeName },
            { "IO", TokenType.TypeName },
            { "Ordering", TokenType.TypeName },
            { "Monad", TokenType.TypeName },
            { "Functor", TokenType.TypeName },
            { "Applicative", TokenType.TypeName },
            { "Monoid", TokenType.TypeName },
            { "Semigroup", TokenType.TypeName },
            { "Foldable", TokenType.TypeName },
            { "Traversable", TokenType.TypeName },
            { "Show", TokenType.TypeName },
            { "Read", TokenType.TypeName },
            { "Eq", TokenType.TypeName },
            { "Ord", TokenType.TypeName },
            { "Num", TokenType.TypeName },
            { "Integral", TokenType.TypeName },
            { "Fractional", TokenType.TypeName },
            { "Floating", TokenType.TypeName },
            { "Bounded", TokenType.TypeName },
            { "Enum", TokenType.TypeName },

            // Common functions
            { "main", TokenType.Method },
            { "print", TokenType.Method },
            { "putStrLn", TokenType.Method },
            { "putStr", TokenType.Method },
            { "getLine", TokenType.Method },
            { "getChar", TokenType.Method },
            { "return", TokenType.Method },
            { "pure", TokenType.Method },
            { "fmap", TokenType.Method },
            { "map", TokenType.Method },
            { "filter", TokenType.Method },
            { "foldr", TokenType.Method },
            { "foldl", TokenType.Method },
            { "head", TokenType.Method },
            { "tail", TokenType.Method },
            { "last", TokenType.Method },
            { "init", TokenType.Method },
            { "null", TokenType.Method },
            { "length", TokenType.Method },
            { "reverse", TokenType.Method },
            { "concat", TokenType.Method },
            { "concatMap", TokenType.Method },
            { "zip", TokenType.Method },
            { "zipWith", TokenType.Method },
            { "take", TokenType.Method },
            { "drop", TokenType.Method },
            { "takeWhile", TokenType.Method },
            { "dropWhile", TokenType.Method },
            { "elem", TokenType.Method },
            { "notElem", TokenType.Method },
            { "show", TokenType.Method },
            { "read", TokenType.Method },
            { "error", TokenType.Method },
            { "undefined", TokenType.Method },
            { "otherwise", TokenType.Constant },
        };

        _patterns = new List<TokenPattern>
        {
            // Pragma
            new TokenPattern(@"\{-#[\s\S]*?#-\}", TokenType.Preprocessor, 100),

            // Block comments (nested)
            new TokenPattern(@"\{-[\s\S]*?-\}", TokenType.MultiLineComment, 95),

            // Line comments
            new TokenPattern(@"--[^\n]*", TokenType.Comment, 90),

            // Character literals
            new TokenPattern(@"'(?:[^'\\]|\\.|\\x[0-9a-fA-F]+)'", TokenType.Character, 85),

            // String literals
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 80),

            // Numbers (hex)
            new TokenPattern(@"0[xX][0-9a-fA-F]+", TokenType.Number, 70),

            // Numbers (octal)
            new TokenPattern(@"0[oO][0-7]+", TokenType.Number, 70),

            // Numbers (binary)
            new TokenPattern(@"0[bB][01]+", TokenType.Number, 70),

            // Numbers (float)
            new TokenPattern(@"\d+\.\d+(?:[eE][+-]?\d+)?", TokenType.Number, 70),

            // Numbers (integer)
            new TokenPattern(@"\d+", TokenType.Number, 65),

            // Type/Constructor (starts with uppercase)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_']*\b", TokenType.TypeName, 50),

            // Operators (including custom)
            new TokenPattern(@"->|<-|=>|::|\\|@|~|=|[+\-*/<>!?&|^$#%:.]+", TokenType.Operator, 40),

            // Identifiers (including primes)
            new TokenPattern(@"\b[a-z_][a-zA-Z0-9_']*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[(){}\[\];,`]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Haskell;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
