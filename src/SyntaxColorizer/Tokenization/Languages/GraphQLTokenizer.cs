using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for GraphQL query language.
/// </summary>
public class GraphQLTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static GraphQLTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Operation types
            { "query", TokenType.Keyword },
            { "mutation", TokenType.Keyword },
            { "subscription", TokenType.Keyword },
            { "fragment", TokenType.Keyword },
            { "on", TokenType.Keyword },

            // Schema definition
            { "type", TokenType.Keyword },
            { "interface", TokenType.Keyword },
            { "union", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "scalar", TokenType.Keyword },
            { "input", TokenType.Keyword },
            { "extend", TokenType.Keyword },
            { "schema", TokenType.Keyword },
            { "directive", TokenType.Keyword },
            { "implements", TokenType.Keyword },
            { "repeatable", TokenType.Keyword },

            // Built-in scalars
            { "Int", TokenType.TypeName },
            { "Float", TokenType.TypeName },
            { "String", TokenType.TypeName },
            { "Boolean", TokenType.TypeName },
            { "ID", TokenType.TypeName },

            // Constants
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "null", TokenType.Constant },

            // Directive locations
            { "QUERY", TokenType.Constant },
            { "MUTATION", TokenType.Constant },
            { "SUBSCRIPTION", TokenType.Constant },
            { "FIELD", TokenType.Constant },
            { "FRAGMENT_DEFINITION", TokenType.Constant },
            { "FRAGMENT_SPREAD", TokenType.Constant },
            { "INLINE_FRAGMENT", TokenType.Constant },
            { "VARIABLE_DEFINITION", TokenType.Constant },
            { "SCHEMA", TokenType.Constant },
            { "SCALAR", TokenType.Constant },
            { "OBJECT", TokenType.Constant },
            { "FIELD_DEFINITION", TokenType.Constant },
            { "ARGUMENT_DEFINITION", TokenType.Constant },
            { "INTERFACE", TokenType.Constant },
            { "UNION", TokenType.Constant },
            { "ENUM", TokenType.Constant },
            { "ENUM_VALUE", TokenType.Constant },
            { "INPUT_OBJECT", TokenType.Constant },
            { "INPUT_FIELD_DEFINITION", TokenType.Constant },
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Block strings (triple quotes)
            new TokenPattern(@"""""""""[\s\S]*?""""""""", TokenType.String, 90),

            // String literals
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 85),

            // Directives (@directive)
            new TokenPattern(@"@[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Attribute, 80),

            // Variables ($variable)
            new TokenPattern(@"\$[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Identifier, 75),

            // Fragment spread (...)
            new TokenPattern(@"\.\.\.", TokenType.Operator, 70),

            // Numbers (float)
            new TokenPattern(@"-?\d+\.\d+(?:[eE][+-]?\d+)?", TokenType.Number, 65),

            // Numbers (integer)
            new TokenPattern(@"-?\d+", TokenType.Number, 60),

            // Type names (PascalCase)
            new TokenPattern(@"\b[A-Z][a-zA-Z0-9_]*\b", TokenType.TypeName, 50),

            // Field names / identifiers
            new TokenPattern(@"\b[a-z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 40),

            // Operators
            new TokenPattern(@"[=!:|&]", TokenType.Operator, 30),

            // Punctuation
            new TokenPattern(@"[(){}\[\],]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.GraphQL;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
