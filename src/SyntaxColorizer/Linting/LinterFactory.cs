namespace SyntaxColorizer.Linting;

/// <summary>
/// Factory for creating language-specific linters.
/// </summary>
public static class LinterFactory
{
    private static readonly Dictionary<SyntaxLanguage, ILinter> _linters = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Gets a linter for the specified language.
    /// </summary>
    /// <param name="language">The programming language.</param>
    /// <returns>A linter for the language, or null for None.</returns>
    public static ILinter? GetLinter(SyntaxLanguage language)
    {
        if (language == SyntaxLanguage.None)
            return null;

        lock (_lock)
        {
            if (_linters.TryGetValue(language, out var linter))
                return linter;

            linter = CreateLinter(language);
            if (linter != null)
                _linters[language] = linter;

            return linter;
        }
    }

    /// <summary>
    /// Registers a custom linter for a language.
    /// </summary>
    /// <param name="language">The programming language.</param>
    /// <param name="linter">The linter to register.</param>
    public static void RegisterLinter(SyntaxLanguage language, ILinter linter)
    {
        lock (_lock)
        {
            _linters[language] = linter;
        }
    }

    /// <summary>
    /// Clears all cached linters.
    /// </summary>
    public static void ClearCache()
    {
        lock (_lock)
        {
            _linters.Clear();
        }
    }

    private static ILinter? CreateLinter(SyntaxLanguage language)
    {
        return language switch
        {
            SyntaxLanguage.CSharp => new CSharpLinter(),
            SyntaxLanguage.JavaScript => new JavaScriptLinter(SyntaxLanguage.JavaScript),
            SyntaxLanguage.TypeScript => new JavaScriptLinter(SyntaxLanguage.TypeScript),
            SyntaxLanguage.MsSql => new SqlLinter(SyntaxLanguage.MsSql),
            SyntaxLanguage.OracleSql => new SqlLinter(SyntaxLanguage.OracleSql),
            _ => new CommonLinter(language)
        };
    }
}
