using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for PowerShell scripting language.
/// </summary>
public class PowerShellTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static PowerShellTokenizer()
    {
        // PowerShell keywords
        _keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "elseif", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "foreach", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "until", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "continue", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "exit", TokenType.ControlKeyword },
            { "throw", TokenType.ControlKeyword },
            { "try", TokenType.ControlKeyword },
            { "catch", TokenType.ControlKeyword },
            { "finally", TokenType.ControlKeyword },
            { "trap", TokenType.ControlKeyword },

            // Keywords
            { "function", TokenType.Keyword },
            { "filter", TokenType.Keyword },
            { "param", TokenType.Keyword },
            { "begin", TokenType.Keyword },
            { "process", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "enum", TokenType.Keyword },
            { "using", TokenType.Keyword },
            { "namespace", TokenType.Keyword },
            { "module", TokenType.Keyword },
            { "workflow", TokenType.Keyword },
            { "parallel", TokenType.Keyword },
            { "sequence", TokenType.Keyword },
            { "inlinescript", TokenType.Keyword },
            { "configuration", TokenType.Keyword },
            { "data", TokenType.Keyword },
            { "dynamicparam", TokenType.Keyword },
            { "hidden", TokenType.Keyword },
            { "static", TokenType.Keyword },

            // Operators as keywords
            { "in", TokenType.Keyword },
            { "and", TokenType.Keyword },
            { "or", TokenType.Keyword },
            { "not", TokenType.Keyword },
            { "band", TokenType.Keyword },
            { "bor", TokenType.Keyword },
            { "bnot", TokenType.Keyword },
            { "bxor", TokenType.Keyword },

            // Constants
            { "$true", TokenType.Constant },
            { "$false", TokenType.Constant },
            { "$null", TokenType.Constant }
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#.*$", TokenType.Comment, 100, RegexOptions.Multiline),
            new TokenPattern(@"<#[\s\S]*?#>", TokenType.MultiLineComment, 100),

            // Here-strings
            new TokenPattern(@"@""[\s\S]*?""@", TokenType.String, 95),
            new TokenPattern(@"@'[\s\S]*?'@", TokenType.String, 95),

            // Double-quoted strings (with variable expansion)
            new TokenPattern(@"""(?:[^""`$]|`.|(\$(?:\{[^}]+\}|[\w:]+)))*""", TokenType.String, 90),

            // Single-quoted strings (literal)
            new TokenPattern(@"'[^']*'", TokenType.String, 90),

            // Cmdlets (Verb-Noun pattern)
            new TokenPattern(@"\b(?:Get|Set|New|Remove|Add|Clear|Export|Import|Start|Stop|Restart|Invoke|Enable|Disable|Test|Update|Read|Write|Out|Format|Select|Where|Sort|Group|Measure|Compare|ConvertTo|ConvertFrom|Copy|Move|Rename|Join|Split|Wait|Register|Unregister|Push|Pop|Enter|Exit|Show|Hide|Find|Search|Expand|Compress|Trace|Debug|Assert|Limit|Lock|Unlock|Protect|Unprotect|Publish|Unpublish|Install|Uninstall|Initialize|Mount|Dismount|Backup|Restore|Suspend|Resume|Checkpoint|Undo|Redo|Reset|Repair|Resolve|Revoke|Save|Send|Submit|Sync|Use|Block|Unblock|Grant|Deny|Request|Receive|Close|Open|Connect|Disconnect|Watch|Complete|Confirm|Approve|Deny)-\w+\b", TokenType.PowerShellCmdlet, 85),

            // Variables
            new TokenPattern(@"\$\{[^}]+\}", TokenType.ShellVariable, 80),
            new TokenPattern(@"\$[\w:]+", TokenType.ShellVariable, 80),
            new TokenPattern(@"\$\?|\$\$|\$\^", TokenType.ShellVariable, 80),

            // Parameters
            new TokenPattern(@"-[\w]+:?", TokenType.PowerShellParameter, 75),

            // Type accelerators and .NET types
            new TokenPattern(@"\[[\w.]+(?:\[\])?\]", TokenType.TypeName, 70),

            // Numbers
            new TokenPattern(@"0x[0-9a-fA-F]+(?:L|l)?", TokenType.Number, 65),
            new TokenPattern(@"\d+(?:\.\d+)?(?:e[+-]?\d+)?(?:d|D|l|L|kb|mb|gb|tb|pb)?", TokenType.Number, 65),

            // Operators
            new TokenPattern(@"-(?:eq|ne|gt|ge|lt|le|like|notlike|match|notmatch|replace|contains|notcontains|in|notin|split|join|is|isnot|as|f|and|or|not|band|bor|bnot|bxor|shl|shr)\b", TokenType.Operator, 60, RegexOptions.IgnoreCase),
            new TokenPattern(@"[+\-*/%]=?|[<>=!]=?|&&|\|\||[&|^~]|\.\.", TokenType.Operator, 55),

            // Special operators
            new TokenPattern(@"@\(|@\{|\$\(|::", TokenType.Operator, 50),

            // Splatting
            new TokenPattern(@"@\w+", TokenType.ShellVariable, 50),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][\w]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,.|]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.PowerShell;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
