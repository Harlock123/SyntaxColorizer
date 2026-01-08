using SyntaxColorizer.Tokenization.Languages;

namespace SyntaxColorizer.Tokenization;

/// <summary>
/// Factory for creating language-specific tokenizers.
/// </summary>
public static class TokenizerFactory
{
    private static readonly Dictionary<SyntaxLanguage, ILanguageTokenizer> _tokenizers = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Gets a tokenizer for the specified language.
    /// </summary>
    /// <param name="language">The programming language.</param>
    /// <returns>A tokenizer for the language, or null for None.</returns>
    public static ILanguageTokenizer? GetTokenizer(SyntaxLanguage language)
    {
        if (language == SyntaxLanguage.None)
            return null;

        lock (_lock)
        {
            if (_tokenizers.TryGetValue(language, out var tokenizer))
                return tokenizer;

            tokenizer = CreateTokenizer(language);
            if (tokenizer != null)
                _tokenizers[language] = tokenizer;

            return tokenizer;
        }
    }

    /// <summary>
    /// Registers a custom tokenizer for a language.
    /// </summary>
    /// <param name="language">The programming language.</param>
    /// <param name="tokenizer">The tokenizer to register.</param>
    public static void RegisterTokenizer(SyntaxLanguage language, ILanguageTokenizer tokenizer)
    {
        lock (_lock)
        {
            _tokenizers[language] = tokenizer;
        }
    }

    /// <summary>
    /// Clears all cached tokenizers.
    /// </summary>
    public static void ClearCache()
    {
        lock (_lock)
        {
            _tokenizers.Clear();
        }
    }

    private static ILanguageTokenizer? CreateTokenizer(SyntaxLanguage language)
    {
        return language switch
        {
            SyntaxLanguage.CSharp => new CSharpTokenizer(),
            SyntaxLanguage.VisualBasic => new VisualBasicTokenizer(),
            SyntaxLanguage.MsSql => new MsSqlTokenizer(),
            SyntaxLanguage.OracleSql => new OracleSqlTokenizer(),
            SyntaxLanguage.Java => new JavaTokenizer(),
            SyntaxLanguage.JavaScript => new JavaScriptTokenizer(),
            SyntaxLanguage.TypeScript => new TypeScriptTokenizer(),
            SyntaxLanguage.C => new CTokenizer(),
            SyntaxLanguage.Cpp => new CppTokenizer(),
            SyntaxLanguage.Php => new PhpTokenizer(),
            SyntaxLanguage.Python => new PythonTokenizer(),
            SyntaxLanguage.Rust => new RustTokenizer(),
            SyntaxLanguage.Html => new HtmlTokenizer(),
            SyntaxLanguage.Css => new CssTokenizer(),
            SyntaxLanguage.Markdown => new MarkdownTokenizer(),
            SyntaxLanguage.Json => new JsonTokenizer(),
            SyntaxLanguage.Yaml => new YamlTokenizer(),
            SyntaxLanguage.Xml => new XmlTokenizer(),
            SyntaxLanguage.Bash => new BashTokenizer(),
            SyntaxLanguage.PowerShell => new PowerShellTokenizer(),
            SyntaxLanguage.Go => new GoTokenizer(),
            SyntaxLanguage.Ruby => new RubyTokenizer(),
            SyntaxLanguage.Kotlin => new KotlinTokenizer(),
            SyntaxLanguage.Swift => new SwiftTokenizer(),
            SyntaxLanguage.Scala => new ScalaTokenizer(),
            SyntaxLanguage.Dockerfile => new DockerfileTokenizer(),
            SyntaxLanguage.Lua => new LuaTokenizer(),
            SyntaxLanguage.Dart => new DartTokenizer(),
            SyntaxLanguage.FSharp => new FSharpTokenizer(),
            SyntaxLanguage.R => new RTokenizer(),
            SyntaxLanguage.Groovy => new GroovyTokenizer(),
            SyntaxLanguage.Toml => new TomlTokenizer(),
            SyntaxLanguage.Haskell => new HaskellTokenizer(),
            SyntaxLanguage.GraphQL => new GraphQLTokenizer(),
            SyntaxLanguage.Scss => new ScssTokenizer(),
            SyntaxLanguage.ObjectiveC => new ObjectiveCTokenizer(),
            SyntaxLanguage.Elixir => new ElixirTokenizer(),
            _ => null
        };
    }
}
