namespace SyntaxColorizer.Linting;

/// <summary>
/// Interface for language-specific linters.
/// </summary>
public interface ILinter
{
    /// <summary>
    /// Gets the language this linter handles.
    /// </summary>
    SyntaxLanguage Language { get; }

    /// <summary>
    /// Analyzes the text and returns linting hints.
    /// </summary>
    /// <param name="text">The source text to analyze.</param>
    /// <returns>A collection of linting hints.</returns>
    IEnumerable<LintingHint> Analyze(string text);

    /// <summary>
    /// Analyzes a specific range of text.
    /// </summary>
    /// <param name="text">The full source text.</param>
    /// <param name="startLine">The starting line (1-based).</param>
    /// <param name="endLine">The ending line (1-based).</param>
    /// <returns>A collection of linting hints for the range.</returns>
    IEnumerable<LintingHint> AnalyzeRange(string text, int startLine, int endLine);
}
