using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Objective-C programming language.
/// </summary>
public class ObjectiveCTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static ObjectiveCTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow (C)
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "default", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "goto", TokenType.ControlKeyword },

            // C keywords
            { "auto", TokenType.Keyword },
            { "const", TokenType.Keyword },
            { "extern", TokenType.Keyword },
            { "register", TokenType.Keyword },
            { "static", TokenType.Keyword },
            { "volatile", TokenType.Keyword },
            { "inline", TokenType.Keyword },
            { "restrict", TokenType.Keyword },
            { "sizeof", TokenType.Keyword },
            { "typedef", TokenType.Keyword },
            { "struct", TokenType.Keyword },
            { "union", TokenType.Keyword },
            { "enum", TokenType.Keyword },

            // Objective-C keywords
            { "@interface", TokenType.Keyword },
            { "@implementation", TokenType.Keyword },
            { "@end", TokenType.Keyword },
            { "@protocol", TokenType.Keyword },
            { "@optional", TokenType.Keyword },
            { "@required", TokenType.Keyword },
            { "@class", TokenType.Keyword },
            { "@public", TokenType.Keyword },
            { "@private", TokenType.Keyword },
            { "@protected", TokenType.Keyword },
            { "@package", TokenType.Keyword },
            { "@property", TokenType.Keyword },
            { "@synthesize", TokenType.Keyword },
            { "@dynamic", TokenType.Keyword },
            { "@selector", TokenType.Keyword },
            { "@encode", TokenType.Keyword },
            { "@defs", TokenType.Keyword },
            { "@synchronized", TokenType.Keyword },
            { "@autoreleasepool", TokenType.Keyword },
            { "@try", TokenType.ControlKeyword },
            { "@catch", TokenType.ControlKeyword },
            { "@finally", TokenType.ControlKeyword },
            { "@throw", TokenType.ControlKeyword },
            { "@import", TokenType.Keyword },
            { "@available", TokenType.Keyword },
            { "@compatibility_alias", TokenType.Keyword },

            // Property attributes
            { "nonatomic", TokenType.Keyword },
            { "atomic", TokenType.Keyword },
            { "strong", TokenType.Keyword },
            { "weak", TokenType.Keyword },
            { "assign", TokenType.Keyword },
            { "copy", TokenType.Keyword },
            { "retain", TokenType.Keyword },
            { "readonly", TokenType.Keyword },
            { "readwrite", TokenType.Keyword },
            { "getter", TokenType.Keyword },
            { "setter", TokenType.Keyword },
            { "nullable", TokenType.Keyword },
            { "nonnull", TokenType.Keyword },
            { "null_resettable", TokenType.Keyword },
            { "null_unspecified", TokenType.Keyword },
            { "class", TokenType.Keyword },

            // Other keywords
            { "self", TokenType.Keyword },
            { "super", TokenType.Keyword },
            { "id", TokenType.TypeName },
            { "instancetype", TokenType.TypeName },
            { "Class", TokenType.TypeName },
            { "SEL", TokenType.TypeName },
            { "IMP", TokenType.TypeName },
            { "BOOL", TokenType.TypeName },
            { "Protocol", TokenType.TypeName },
            { "in", TokenType.Keyword },
            { "out", TokenType.Keyword },
            { "inout", TokenType.Keyword },
            { "bycopy", TokenType.Keyword },
            { "byref", TokenType.Keyword },
            { "oneway", TokenType.Keyword },
            { "__block", TokenType.Keyword },
            { "__weak", TokenType.Keyword },
            { "__strong", TokenType.Keyword },
            { "__unsafe_unretained", TokenType.Keyword },
            { "__autoreleasing", TokenType.Keyword },
            { "__bridge", TokenType.Keyword },
            { "__bridge_retained", TokenType.Keyword },
            { "__bridge_transfer", TokenType.Keyword },
            { "__kindof", TokenType.Keyword },

            // C types
            { "void", TokenType.TypeName },
            { "char", TokenType.TypeName },
            { "short", TokenType.TypeName },
            { "int", TokenType.TypeName },
            { "long", TokenType.TypeName },
            { "float", TokenType.TypeName },
            { "double", TokenType.TypeName },
            { "signed", TokenType.TypeName },
            { "unsigned", TokenType.TypeName },
            { "_Bool", TokenType.TypeName },
            { "_Complex", TokenType.TypeName },
            { "_Imaginary", TokenType.TypeName },

            // Foundation types
            { "NSObject", TokenType.TypeName },
            { "NSString", TokenType.TypeName },
            { "NSArray", TokenType.TypeName },
            { "NSDictionary", TokenType.TypeName },
            { "NSSet", TokenType.TypeName },
            { "NSNumber", TokenType.TypeName },
            { "NSInteger", TokenType.TypeName },
            { "NSUInteger", TokenType.TypeName },
            { "CGFloat", TokenType.TypeName },
            { "NSData", TokenType.TypeName },
            { "NSDate", TokenType.TypeName },
            { "NSError", TokenType.TypeName },
            { "NSURL", TokenType.TypeName },
            { "NSMutableArray", TokenType.TypeName },
            { "NSMutableDictionary", TokenType.TypeName },
            { "NSMutableString", TokenType.TypeName },
            { "NSMutableSet", TokenType.TypeName },
            { "NSValue", TokenType.TypeName },
            { "NSNotification", TokenType.TypeName },
            { "NSNotificationCenter", TokenType.TypeName },
            { "NSUserDefaults", TokenType.TypeName },
            { "NSBundle", TokenType.TypeName },
            { "NSFileManager", TokenType.TypeName },
            { "NSRange", TokenType.TypeName },
            { "NSRect", TokenType.TypeName },
            { "NSPoint", TokenType.TypeName },
            { "NSSize", TokenType.TypeName },
            { "CGRect", TokenType.TypeName },
            { "CGPoint", TokenType.TypeName },
            { "CGSize", TokenType.TypeName },

            // Constants
            { "YES", TokenType.Constant },
            { "NO", TokenType.Constant },
            { "nil", TokenType.Constant },
            { "Nil", TokenType.Constant },
            { "NULL", TokenType.Constant },
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },

            // Common methods
            { "alloc", TokenType.Method },
            { "init", TokenType.Method },
            { "new", TokenType.Method },
            // Note: "copy" and "retain" already defined as property attributes above
            { "mutableCopy", TokenType.Method },
            { "dealloc", TokenType.Method },
            { "release", TokenType.Method },
            { "autorelease", TokenType.Method },
            { "description", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Preprocessor directives
            new TokenPattern(@"#\s*(?:import|include|define|undef|ifdef|ifndef|if|else|elif|endif|pragma|error|warning|line)[^\n]*", TokenType.Preprocessor, 100),

            // Block comments
            new TokenPattern(@"/\*[\s\S]*?\*/", TokenType.MultiLineComment, 95),

            // Line comments
            new TokenPattern(@"//[^\n]*", TokenType.Comment, 90),

            // Objective-C string literals @"..."
            new TokenPattern(@"@""(?:[^""\\]|\\.)*""", TokenType.String, 88),

            // C string literals
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 85),

            // Character literals
            new TokenPattern(@"'(?:[^'\\]|\\.)'", TokenType.Character, 85),

            // Objective-C @ keywords
            new TokenPattern(@"@[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Keyword, 82),

            // Boxed expressions @(...)
            new TokenPattern(@"@\(", TokenType.Operator, 80),

            // Array/Dictionary literals @[...] @{...}
            new TokenPattern(@"@[\[\{]", TokenType.Operator, 80),

            // Numbers (hex)
            new TokenPattern(@"0[xX][0-9a-fA-F]+[uUlL]*", TokenType.Number, 75),

            // Numbers (float)
            new TokenPattern(@"\d+\.\d*(?:[eE][+-]?\d+)?[fFlL]?", TokenType.Number, 75),
            new TokenPattern(@"\.\d+(?:[eE][+-]?\d+)?[fFlL]?", TokenType.Number, 75),

            // Numbers (integer)
            new TokenPattern(@"\d+[uUlL]*", TokenType.Number, 70),

            // Type names (NS prefix, CG prefix, etc.)
            new TokenPattern(@"\b(?:NS|CG|CF|UI|CA|CI|CL|MK|AV|SK|SC|WK|GC)[A-Z][a-zA-Z0-9]*\b", TokenType.TypeName, 55),

            // Generic type (protocol conformance)
            new TokenPattern(@"<[^>]+>", TokenType.TypeName, 50),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 40),

            // Operators
            new TokenPattern(@"->|[+\-*/%=<>!&|^~?:]+", TokenType.Operator, 30),

            // Punctuation
            new TokenPattern(@"[(){}\[\];,.]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.ObjectiveC;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
