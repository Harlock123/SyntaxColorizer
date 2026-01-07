using System.Text.RegularExpressions;

namespace SyntaxColorizer.Linting;

/// <summary>
/// Base class for linters with common functionality.
/// </summary>
public abstract class BaseLinter : ILinter
{
    /// <inheritdoc/>
    public abstract SyntaxLanguage Language { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<LintingHint> Analyze(string text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        foreach (var rule in GetRules())
        {
            foreach (var hint in ApplyRule(text, rule))
            {
                yield return hint;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable<LintingHint> AnalyzeRange(string text, int startLine, int endLine)
    {
        return Analyze(text).Where(h => h.Line >= startLine && h.Line <= endLine);
    }

    /// <summary>
    /// Gets the linting rules for this linter.
    /// </summary>
    protected abstract IEnumerable<LintingRule> GetRules();

    /// <summary>
    /// Applies a single rule to the text.
    /// </summary>
    protected virtual IEnumerable<LintingHint> ApplyRule(string text, LintingRule rule)
    {
        var matches = rule.Pattern.Matches(text);
        var lines = text.Split('\n');
        var lineStarts = CalculateLineStarts(text);

        foreach (Match match in matches)
        {
            var (line, column) = GetLineAndColumn(match.Index, lineStarts);

            yield return new LintingHint
            {
                StartIndex = match.Index,
                Length = match.Length,
                Line = line,
                Column = column,
                Message = rule.Message,
                Severity = rule.Severity,
                Code = rule.Code,
                Source = rule.Source,
                SuggestedFix = rule.GetSuggestedFix?.Invoke(match)
            };
        }
    }

    /// <summary>
    /// Calculates the starting index of each line.
    /// </summary>
    protected static int[] CalculateLineStarts(string text)
    {
        var starts = new List<int> { 0 };
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
                starts.Add(i + 1);
        }
        return starts.ToArray();
    }

    /// <summary>
    /// Gets the line and column for a character index.
    /// </summary>
    protected static (int Line, int Column) GetLineAndColumn(int index, int[] lineStarts)
    {
        var line = Array.BinarySearch(lineStarts, index);
        if (line < 0)
            line = ~line - 1;

        var column = index - lineStarts[Math.Max(0, line)] + 1;
        return (line + 1, column);
    }
}

/// <summary>
/// Represents a linting rule.
/// </summary>
public class LintingRule
{
    /// <summary>
    /// Gets or sets the regex pattern to match.
    /// </summary>
    public required Regex Pattern { get; init; }

    /// <summary>
    /// Gets or sets the message to display when the rule matches.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets or sets the severity of matches.
    /// </summary>
    public LintingSeverity Severity { get; init; } = LintingSeverity.Warning;

    /// <summary>
    /// Gets or sets the code identifier for this rule.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Gets or sets the source identifier.
    /// </summary>
    public string? Source { get; init; }

    /// <summary>
    /// Gets or sets a function to generate suggested fixes.
    /// </summary>
    public Func<Match, string?>? GetSuggestedFix { get; init; }
}
