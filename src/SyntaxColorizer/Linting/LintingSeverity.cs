namespace SyntaxColorizer.Linting;

/// <summary>
/// Severity levels for linting hints.
/// </summary>
public enum LintingSeverity
{
    /// <summary>
    /// Informational hint.
    /// </summary>
    Information,

    /// <summary>
    /// Warning that indicates a potential issue.
    /// </summary>
    Warning,

    /// <summary>
    /// Error that indicates a definite problem.
    /// </summary>
    Error,

    /// <summary>
    /// Suggestion for code improvement.
    /// </summary>
    Suggestion
}
