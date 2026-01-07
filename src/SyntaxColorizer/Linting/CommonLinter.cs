using System.Text.RegularExpressions;

namespace SyntaxColorizer.Linting;

/// <summary>
/// A generic linter that provides common checks across multiple languages.
/// </summary>
public class CommonLinter : BaseLinter
{
    private readonly SyntaxLanguage _language;
    private readonly List<LintingRule> _rules;

    public CommonLinter(SyntaxLanguage language)
    {
        _language = language;
        _rules = CreateCommonRules().ToList();
    }

    public override SyntaxLanguage Language => _language;

    protected override IEnumerable<LintingRule> GetRules() => _rules;

    private static IEnumerable<LintingRule> CreateCommonRules()
    {
        // Trailing whitespace
        yield return new LintingRule
        {
            Pattern = new Regex(@"[ \t]+$", RegexOptions.Multiline),
            Message = "Trailing whitespace detected",
            Severity = LintingSeverity.Suggestion,
            Code = "COMMON001",
            Source = "CommonLinter"
        };

        // Multiple consecutive blank lines
        yield return new LintingRule
        {
            Pattern = new Regex(@"\n{3,}"),
            Message = "Multiple consecutive blank lines",
            Severity = LintingSeverity.Suggestion,
            Code = "COMMON002",
            Source = "CommonLinter"
        };

        // TODO comments
        yield return new LintingRule
        {
            Pattern = new Regex(@"(?://|#|/\*|\*)\s*TODO\b[^\r\n]*", RegexOptions.IgnoreCase),
            Message = "TODO comment found",
            Severity = LintingSeverity.Information,
            Code = "COMMON003",
            Source = "CommonLinter"
        };

        // FIXME comments
        yield return new LintingRule
        {
            Pattern = new Regex(@"(?://|#|/\*|\*)\s*FIXME\b[^\r\n]*", RegexOptions.IgnoreCase),
            Message = "FIXME comment found",
            Severity = LintingSeverity.Warning,
            Code = "COMMON004",
            Source = "CommonLinter"
        };

        // HACK comments
        yield return new LintingRule
        {
            Pattern = new Regex(@"(?://|#|/\*|\*)\s*HACK\b[^\r\n]*", RegexOptions.IgnoreCase),
            Message = "HACK comment found - consider refactoring",
            Severity = LintingSeverity.Warning,
            Code = "COMMON005",
            Source = "CommonLinter"
        };

        // Very long lines (over 120 characters)
        yield return new LintingRule
        {
            Pattern = new Regex(@"^.{121,}$", RegexOptions.Multiline),
            Message = "Line exceeds 120 characters",
            Severity = LintingSeverity.Suggestion,
            Code = "COMMON006",
            Source = "CommonLinter"
        };

        // Tab and space mixing (tabs after spaces)
        yield return new LintingRule
        {
            Pattern = new Regex(@"^ +\t", RegexOptions.Multiline),
            Message = "Mixed tabs and spaces in indentation",
            Severity = LintingSeverity.Warning,
            Code = "COMMON007",
            Source = "CommonLinter"
        };
    }

    /// <summary>
    /// Adds a custom rule to this linter.
    /// </summary>
    public void AddRule(LintingRule rule)
    {
        _rules.Add(rule);
    }

    /// <summary>
    /// Removes all rules with the specified code.
    /// </summary>
    public void RemoveRule(string code)
    {
        _rules.RemoveAll(r => r.Code == code);
    }
}

/// <summary>
/// Linter for C# specific checks.
/// </summary>
public class CSharpLinter : BaseLinter
{
    private static readonly List<LintingRule> _rules;

    static CSharpLinter()
    {
        _rules = new List<LintingRule>
        {
            // Using statement without braces
            new LintingRule
            {
                Pattern = new Regex(@"\busing\s*\([^)]+\)\s*[^{]", RegexOptions.Multiline),
                Message = "Consider using braces with 'using' statement",
                Severity = LintingSeverity.Suggestion,
                Code = "CS001",
                Source = "CSharpLinter"
            },

            // Empty catch block
            new LintingRule
            {
                Pattern = new Regex(@"catch\s*(?:\([^)]*\))?\s*\{\s*\}"),
                Message = "Empty catch block - consider logging or handling the exception",
                Severity = LintingSeverity.Warning,
                Code = "CS002",
                Source = "CSharpLinter"
            },

            // Console.WriteLine in production code (might want to be configurable)
            new LintingRule
            {
                Pattern = new Regex(@"Console\.(?:Write|WriteLine)\s*\("),
                Message = "Console output detected - consider using proper logging",
                Severity = LintingSeverity.Information,
                Code = "CS003",
                Source = "CSharpLinter"
            },

            // Potential null reference
            new LintingRule
            {
                Pattern = new Regex(@"\.\w+\s*\.\s*\w+\s*\("),
                Message = "Potential null reference - consider null checking",
                Severity = LintingSeverity.Information,
                Code = "CS004",
                Source = "CSharpLinter"
            },

            // Magic numbers
            new LintingRule
            {
                Pattern = new Regex(@"[=<>]\s*(?!0|1|-1)\d{2,}\b"),
                Message = "Magic number detected - consider using a named constant",
                Severity = LintingSeverity.Suggestion,
                Code = "CS005",
                Source = "CSharpLinter"
            }
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.CSharp;

    protected override IEnumerable<LintingRule> GetRules() => _rules;
}

/// <summary>
/// Linter for JavaScript/TypeScript specific checks.
/// </summary>
public class JavaScriptLinter : BaseLinter
{
    private readonly SyntaxLanguage _language;
    private static readonly List<LintingRule> _rules;

    static JavaScriptLinter()
    {
        _rules = new List<LintingRule>
        {
            // var usage (prefer let/const)
            new LintingRule
            {
                Pattern = new Regex(@"\bvar\s+\w+"),
                Message = "Consider using 'let' or 'const' instead of 'var'",
                Severity = LintingSeverity.Suggestion,
                Code = "JS001",
                Source = "JavaScriptLinter"
            },

            // == instead of ===
            new LintingRule
            {
                Pattern = new Regex(@"[^=!]==[^=]"),
                Message = "Consider using '===' instead of '=='",
                Severity = LintingSeverity.Warning,
                Code = "JS002",
                Source = "JavaScriptLinter"
            },

            // != instead of !==
            new LintingRule
            {
                Pattern = new Regex(@"!=[^=]"),
                Message = "Consider using '!==' instead of '!='",
                Severity = LintingSeverity.Warning,
                Code = "JS003",
                Source = "JavaScriptLinter"
            },

            // console.log
            new LintingRule
            {
                Pattern = new Regex(@"console\.(?:log|warn|error|info)\s*\("),
                Message = "Console statement detected - remove before production",
                Severity = LintingSeverity.Information,
                Code = "JS004",
                Source = "JavaScriptLinter"
            },

            // alert/confirm/prompt
            new LintingRule
            {
                Pattern = new Regex(@"\b(?:alert|confirm|prompt)\s*\("),
                Message = "Browser dialog detected - consider using a modal component",
                Severity = LintingSeverity.Warning,
                Code = "JS005",
                Source = "JavaScriptLinter"
            },

            // debugger statement
            new LintingRule
            {
                Pattern = new Regex(@"\bdebugger\b"),
                Message = "Debugger statement detected - remove before production",
                Severity = LintingSeverity.Warning,
                Code = "JS006",
                Source = "JavaScriptLinter"
            }
        };
    }

    public JavaScriptLinter(SyntaxLanguage language = SyntaxLanguage.JavaScript)
    {
        _language = language;
    }

    public override SyntaxLanguage Language => _language;

    protected override IEnumerable<LintingRule> GetRules() => _rules;
}

/// <summary>
/// Linter for SQL specific checks.
/// </summary>
public class SqlLinter : BaseLinter
{
    private readonly SyntaxLanguage _language;
    private static readonly List<LintingRule> _rules;

    static SqlLinter()
    {
        _rules = new List<LintingRule>
        {
            // SELECT *
            new LintingRule
            {
                Pattern = new Regex(@"\bSELECT\s+\*\s+FROM\b", RegexOptions.IgnoreCase),
                Message = "Avoid SELECT * - specify column names explicitly",
                Severity = LintingSeverity.Warning,
                Code = "SQL001",
                Source = "SqlLinter"
            },

            // Missing WHERE clause on UPDATE/DELETE
            new LintingRule
            {
                Pattern = new Regex(@"\b(?:UPDATE|DELETE)\s+\w+\s*(?:SET[^;]*)?;?\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline),
                Message = "UPDATE/DELETE without WHERE clause - this affects all rows",
                Severity = LintingSeverity.Error,
                Code = "SQL002",
                Source = "SqlLinter"
            },

            // NOLOCK hint
            new LintingRule
            {
                Pattern = new Regex(@"\bWITH\s*\(\s*NOLOCK\s*\)", RegexOptions.IgnoreCase),
                Message = "NOLOCK can cause dirty reads - use with caution",
                Severity = LintingSeverity.Information,
                Code = "SQL003",
                Source = "SqlLinter"
            },

            // Cursor usage
            new LintingRule
            {
                Pattern = new Regex(@"\bDECLARE\s+\w+\s+CURSOR\b", RegexOptions.IgnoreCase),
                Message = "Consider set-based operations instead of cursors for better performance",
                Severity = LintingSeverity.Suggestion,
                Code = "SQL004",
                Source = "SqlLinter"
            },

            // Hardcoded dates
            new LintingRule
            {
                Pattern = new Regex(@"'\d{4}-\d{2}-\d{2}'"),
                Message = "Hardcoded date detected - consider using parameters",
                Severity = LintingSeverity.Suggestion,
                Code = "SQL005",
                Source = "SqlLinter"
            }
        };
    }

    public SqlLinter(SyntaxLanguage language = SyntaxLanguage.MsSql)
    {
        _language = language;
    }

    public override SyntaxLanguage Language => _language;

    protected override IEnumerable<LintingRule> GetRules() => _rules;
}
