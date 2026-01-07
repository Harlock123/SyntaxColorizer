using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Dockerfile configuration files.
/// </summary>
public class DockerfileTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static DockerfileTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            // Dockerfile instructions
            { "FROM", TokenType.Keyword },
            { "AS", TokenType.Keyword },
            { "RUN", TokenType.Keyword },
            { "CMD", TokenType.Keyword },
            { "LABEL", TokenType.Keyword },
            { "MAINTAINER", TokenType.Keyword },
            { "EXPOSE", TokenType.Keyword },
            { "ENV", TokenType.Keyword },
            { "ADD", TokenType.Keyword },
            { "COPY", TokenType.Keyword },
            { "ENTRYPOINT", TokenType.Keyword },
            { "VOLUME", TokenType.Keyword },
            { "USER", TokenType.Keyword },
            { "WORKDIR", TokenType.Keyword },
            { "ARG", TokenType.Keyword },
            { "ONBUILD", TokenType.Keyword },
            { "STOPSIGNAL", TokenType.Keyword },
            { "HEALTHCHECK", TokenType.Keyword },
            { "SHELL", TokenType.Keyword },

            // HEALTHCHECK options
            { "NONE", TokenType.Constant },

            // Common base images
            { "alpine", TokenType.TypeName },
            { "ubuntu", TokenType.TypeName },
            { "debian", TokenType.TypeName },
            { "centos", TokenType.TypeName },
            { "fedora", TokenType.TypeName },
            { "node", TokenType.TypeName },
            { "python", TokenType.TypeName },
            { "golang", TokenType.TypeName },
            { "rust", TokenType.TypeName },
            { "dotnet", TokenType.TypeName },
            { "openjdk", TokenType.TypeName },
            { "nginx", TokenType.TypeName },
            { "redis", TokenType.TypeName },
            { "postgres", TokenType.TypeName },
            { "mysql", TokenType.TypeName },
            { "mongo", TokenType.TypeName }
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Parser directives (must be at very beginning)
            new TokenPattern(@"^#\s*(?:syntax|escape)\s*=\s*[^\n]+", TokenType.Preprocessor, 100, RegexOptions.Multiline),

            // Dockerfile instructions (case-insensitive)
            new TokenPattern(@"^\s*(?:FROM|RUN|CMD|LABEL|MAINTAINER|EXPOSE|ENV|ADD|COPY|ENTRYPOINT|VOLUME|USER|WORKDIR|ARG|ONBUILD|STOPSIGNAL|HEALTHCHECK|SHELL)\b", TokenType.Keyword, 95, RegexOptions.Multiline | RegexOptions.IgnoreCase),

            // AS keyword in FROM
            new TokenPattern(@"\bAS\b", TokenType.Keyword, 90, RegexOptions.IgnoreCase),

            // --from flag in COPY
            new TokenPattern(@"--from=\w+", TokenType.Attribute, 85),

            // Flags/options
            new TokenPattern(@"--\w+(?:=\S+)?", TokenType.ShellOption, 85),

            // Environment variables
            new TokenPattern(@"\$\{[^}]+\}", TokenType.ShellVariable, 80),
            new TokenPattern(@"\$\w+", TokenType.ShellVariable, 80),

            // Double-quoted strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 75),

            // Single-quoted strings
            new TokenPattern(@"'[^']*'", TokenType.String, 75),

            // Image tags and versions
            new TokenPattern(@":\d+(?:\.\d+)*(?:-[\w.-]+)?", TokenType.Number, 70),
            new TokenPattern(@"@sha256:[a-f0-9]+", TokenType.Number, 70),

            // Port numbers
            new TokenPattern(@"\b\d{1,5}(?:/(?:tcp|udp))?\b", TokenType.Number, 65),

            // JSON arrays (for CMD, ENTRYPOINT)
            new TokenPattern(@"\[(?:[^\]]*)\]", TokenType.String, 60),

            // URLs and paths
            new TokenPattern(@"https?://[^\s]+", TokenType.String, 55),
            new TokenPattern(@"/[\w./-]+", TokenType.Identifier, 50),

            // Key=value pairs
            new TokenPattern(@"\b\w+=", TokenType.JsonKey, 45),

            // Backslash line continuation
            new TokenPattern(@"\\$", TokenType.Operator, 40, RegexOptions.Multiline),

            // Shell operators
            new TokenPattern(@"&&|\|\||[|;]", TokenType.Operator, 35),

            // Identifiers
            new TokenPattern(@"\b[a-zA-Z_][\w.-]*\b", TokenType.Identifier, 30),

            // Numbers
            new TokenPattern(@"\b\d+\b", TokenType.Number, 25),

            // Punctuation
            new TokenPattern(@"[{}\[\](),:]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Dockerfile;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
