using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Visual Basic .NET source code.
/// </summary>
public class VisualBasicTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static VisualBasicTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Control keywords
            ["If"] = TokenType.ControlKeyword,
            ["Then"] = TokenType.ControlKeyword,
            ["Else"] = TokenType.ControlKeyword,
            ["ElseIf"] = TokenType.ControlKeyword,
            ["End"] = TokenType.ControlKeyword,
            ["Select"] = TokenType.ControlKeyword,
            ["Case"] = TokenType.ControlKeyword,
            ["For"] = TokenType.ControlKeyword,
            ["Each"] = TokenType.ControlKeyword,
            ["Next"] = TokenType.ControlKeyword,
            ["While"] = TokenType.ControlKeyword,
            ["Wend"] = TokenType.ControlKeyword,
            ["Do"] = TokenType.ControlKeyword,
            ["Loop"] = TokenType.ControlKeyword,
            ["Until"] = TokenType.ControlKeyword,
            ["Exit"] = TokenType.ControlKeyword,
            ["Continue"] = TokenType.ControlKeyword,
            ["Return"] = TokenType.ControlKeyword,
            ["GoTo"] = TokenType.ControlKeyword,
            ["Try"] = TokenType.ControlKeyword,
            ["Catch"] = TokenType.ControlKeyword,
            ["Finally"] = TokenType.ControlKeyword,
            ["Throw"] = TokenType.ControlKeyword,
            ["With"] = TokenType.ControlKeyword,

            // Declaration keywords
            ["Class"] = TokenType.Keyword,
            ["Structure"] = TokenType.Keyword,
            ["Interface"] = TokenType.Keyword,
            ["Enum"] = TokenType.Keyword,
            ["Module"] = TokenType.Keyword,
            ["Namespace"] = TokenType.Keyword,
            ["Imports"] = TokenType.Keyword,
            ["Sub"] = TokenType.Keyword,
            ["Function"] = TokenType.Keyword,
            ["Property"] = TokenType.Keyword,
            ["Event"] = TokenType.Keyword,
            ["Delegate"] = TokenType.Keyword,
            ["Operator"] = TokenType.Keyword,

            // Modifier keywords
            ["Public"] = TokenType.Keyword,
            ["Private"] = TokenType.Keyword,
            ["Protected"] = TokenType.Keyword,
            ["Friend"] = TokenType.Keyword,
            ["Shared"] = TokenType.Keyword,
            ["Static"] = TokenType.Keyword,
            ["ReadOnly"] = TokenType.Keyword,
            ["WriteOnly"] = TokenType.Keyword,
            ["Const"] = TokenType.Keyword,
            ["MustInherit"] = TokenType.Keyword,
            ["MustOverride"] = TokenType.Keyword,
            ["NotInheritable"] = TokenType.Keyword,
            ["NotOverridable"] = TokenType.Keyword,
            ["Overridable"] = TokenType.Keyword,
            ["Overrides"] = TokenType.Keyword,
            ["Overloads"] = TokenType.Keyword,
            ["Shadows"] = TokenType.Keyword,
            ["Partial"] = TokenType.Keyword,
            ["Async"] = TokenType.Keyword,
            ["Await"] = TokenType.Keyword,
            ["Iterator"] = TokenType.Keyword,
            ["Yield"] = TokenType.Keyword,

            // Other keywords
            ["Dim"] = TokenType.Keyword,
            ["As"] = TokenType.Keyword,
            ["New"] = TokenType.Keyword,
            ["Me"] = TokenType.Keyword,
            ["MyBase"] = TokenType.Keyword,
            ["MyClass"] = TokenType.Keyword,
            ["ByVal"] = TokenType.Keyword,
            ["ByRef"] = TokenType.Keyword,
            ["Optional"] = TokenType.Keyword,
            ["ParamArray"] = TokenType.Keyword,
            ["Of"] = TokenType.Keyword,
            ["In"] = TokenType.Keyword,
            ["Out"] = TokenType.Keyword,
            ["Is"] = TokenType.Keyword,
            ["IsNot"] = TokenType.Keyword,
            ["Like"] = TokenType.Keyword,
            ["And"] = TokenType.Keyword,
            ["AndAlso"] = TokenType.Keyword,
            ["Or"] = TokenType.Keyword,
            ["OrElse"] = TokenType.Keyword,
            ["Xor"] = TokenType.Keyword,
            ["Not"] = TokenType.Keyword,
            ["Mod"] = TokenType.Keyword,
            ["TypeOf"] = TokenType.Keyword,
            ["GetType"] = TokenType.Keyword,
            ["NameOf"] = TokenType.Keyword,
            ["CType"] = TokenType.Keyword,
            ["DirectCast"] = TokenType.Keyword,
            ["TryCast"] = TokenType.Keyword,
            ["CBool"] = TokenType.Keyword,
            ["CByte"] = TokenType.Keyword,
            ["CChar"] = TokenType.Keyword,
            ["CDate"] = TokenType.Keyword,
            ["CDbl"] = TokenType.Keyword,
            ["CDec"] = TokenType.Keyword,
            ["CInt"] = TokenType.Keyword,
            ["CLng"] = TokenType.Keyword,
            ["CObj"] = TokenType.Keyword,
            ["CSByte"] = TokenType.Keyword,
            ["CShort"] = TokenType.Keyword,
            ["CSng"] = TokenType.Keyword,
            ["CStr"] = TokenType.Keyword,
            ["CUInt"] = TokenType.Keyword,
            ["CULng"] = TokenType.Keyword,
            ["CUShort"] = TokenType.Keyword,
            ["Inherits"] = TokenType.Keyword,
            ["Implements"] = TokenType.Keyword,
            ["Handles"] = TokenType.Keyword,
            ["AddHandler"] = TokenType.Keyword,
            ["RemoveHandler"] = TokenType.Keyword,
            ["RaiseEvent"] = TokenType.Keyword,
            ["Get"] = TokenType.Keyword,
            ["Set"] = TokenType.Keyword,
            ["Let"] = TokenType.Keyword,
            ["ReDim"] = TokenType.Keyword,
            ["Preserve"] = TokenType.Keyword,
            ["Erase"] = TokenType.Keyword,
            ["Using"] = TokenType.Keyword,
            ["SyncLock"] = TokenType.Keyword,
            ["Declare"] = TokenType.Keyword,
            ["Lib"] = TokenType.Keyword,
            ["Alias"] = TokenType.Keyword,
            ["Call"] = TokenType.Keyword,

            // Built-in types
            ["Boolean"] = TokenType.TypeName,
            ["Byte"] = TokenType.TypeName,
            ["SByte"] = TokenType.TypeName,
            ["Char"] = TokenType.TypeName,
            ["Date"] = TokenType.TypeName,
            ["Decimal"] = TokenType.TypeName,
            ["Double"] = TokenType.TypeName,
            ["Single"] = TokenType.TypeName,
            ["Integer"] = TokenType.TypeName,
            ["UInteger"] = TokenType.TypeName,
            ["Long"] = TokenType.TypeName,
            ["ULong"] = TokenType.TypeName,
            ["Short"] = TokenType.TypeName,
            ["UShort"] = TokenType.TypeName,
            ["Object"] = TokenType.TypeName,
            ["String"] = TokenType.TypeName,

            // Literals
            ["True"] = TokenType.Constant,
            ["False"] = TokenType.Constant,
            ["Nothing"] = TokenType.Constant,
        };

        _patterns = new List<TokenPattern>
        {
            // Comments (VB uses ' for comments)
            new(@"'[^\r\n]*", TokenType.Comment, 10),
            new(@"REM\s[^\r\n]*", TokenType.Comment, 10, RegexOptions.IgnoreCase),

            // XML documentation comments
            new(@"'''[^\r\n]*", TokenType.DocComment, 11),

            // Preprocessor directives
            new(@"#\s*(?:If|Else|ElseIf|End\s+If|Const|Region|End\s+Region|ExternalSource|End\s+ExternalSource|ExternalChecksum|Enable|Disable)[^\r\n]*", TokenType.Preprocessor, 9, RegexOptions.IgnoreCase),

            // Strings
            new(@"""(?:[^""]|"""")*""", TokenType.String, 8),

            // Date literals
            new(@"#[^#]+#", TokenType.Number, 7),

            // Numbers
            new(@"&H[0-9a-fA-F]+[ILSU%&@!#]*", TokenType.Number, 6),
            new(@"&O[0-7]+[ILSU%&@!#]*", TokenType.Number, 6),
            new(@"&B[01]+[ILSU%&@!#]*", TokenType.Number, 6),
            new(@"\b\d+\.\d+(?:[eE][+-]?\d+)?[FRDS!#@]*\b", TokenType.Number, 5),
            new(@"\b\d+[eE][+-]?\d+[FRDS!#@]*\b", TokenType.Number, 5),
            new(@"\b\d+[ILSU%&@!#FRDS]*\b", TokenType.Number, 4),

            // Identifiers (VB is case-insensitive, but we use original case)
            new(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 2),

            // Operators
            new(@"[+\-*/\\^=<>&]+|<>|<=|>=", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,.]", TokenType.Punctuation, 0),

            // Line continuation
            new(@"_\s*$", TokenType.Punctuation, 0, RegexOptions.Multiline),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.VisualBasic;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
