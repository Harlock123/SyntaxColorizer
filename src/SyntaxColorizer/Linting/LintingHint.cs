namespace SyntaxColorizer.Linting;

/// <summary>
/// Represents a linting hint to display in the editor.
/// </summary>
public class LintingHint
{
    /// <summary>
    /// Gets or sets the start index of the hint in the text.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// Gets or sets the length of the text span this hint applies to.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets the end index of the hint.
    /// </summary>
    public int EndIndex => StartIndex + Length;

    /// <summary>
    /// Gets or sets the line number (1-based).
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the column number (1-based).
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Gets or sets the message to display.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the severity of this hint.
    /// </summary>
    public LintingSeverity Severity { get; set; } = LintingSeverity.Information;

    /// <summary>
    /// Gets or sets the code or identifier for this hint type.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the source of this hint (e.g., "Compiler", "StyleCop", etc.).
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets a suggested fix for this hint.
    /// </summary>
    public string? SuggestedFix { get; set; }

    public override string ToString()
    {
        var severity = Severity.ToString().ToUpper();
        var code = string.IsNullOrEmpty(Code) ? "" : $"[{Code}] ";
        return $"{severity} ({Line}:{Column}): {code}{Message}";
    }
}
