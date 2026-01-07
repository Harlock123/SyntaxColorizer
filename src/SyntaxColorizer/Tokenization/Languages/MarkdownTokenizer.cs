using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Markdown markup.
/// </summary>
public class MarkdownTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static MarkdownTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>();

        _patterns = new List<TokenPattern>
        {
            // Fenced code blocks with language
            new TokenPattern(@"```[\w]*\n[\s\S]*?```", TokenType.MarkdownCode, 100),

            // Fenced code blocks
            new TokenPattern(@"~~~[\s\S]*?~~~", TokenType.MarkdownCode, 100),

            // Inline code
            new TokenPattern(@"`[^`\n]+`", TokenType.MarkdownCode, 95),

            // Headings (ATX style)
            new TokenPattern(@"^#{1,6}\s+[^\n]+", TokenType.MarkdownHeading, 90, RegexOptions.Multiline),

            // Setext-style headings (underlined)
            new TokenPattern(@"^[^\n]+\n[=]+\s*$", TokenType.MarkdownHeading, 88, RegexOptions.Multiline),
            new TokenPattern(@"^[^\n]+\n[-]+\s*$", TokenType.MarkdownHeading, 87, RegexOptions.Multiline),

            // Horizontal rules
            new TokenPattern(@"^(?:[*\-_]\s*){3,}\s*$", TokenType.Punctuation, 85, RegexOptions.Multiline),

            // Block quotes
            new TokenPattern(@"^>\s*[^\n]*", TokenType.Comment, 80, RegexOptions.Multiline),

            // Unordered list items
            new TokenPattern(@"^[\t ]*[*\-+]\s+", TokenType.MarkdownList, 75, RegexOptions.Multiline),

            // Ordered list items
            new TokenPattern(@"^[\t ]*\d+\.\s+", TokenType.MarkdownList, 75, RegexOptions.Multiline),

            // Images ![alt](url)
            new TokenPattern(@"!\[[^\]]*\]\([^)]+\)", TokenType.MarkdownLink, 70),

            // Links [text](url)
            new TokenPattern(@"\[[^\]]+\]\([^)]+\)", TokenType.MarkdownLink, 70),

            // Reference links [text][ref]
            new TokenPattern(@"\[[^\]]+\]\[[^\]]*\]", TokenType.MarkdownLink, 70),

            // Link references [ref]: url
            new TokenPattern(@"^\[[^\]]+\]:\s+\S+", TokenType.MarkdownLink, 68, RegexOptions.Multiline),

            // Autolinks <url> or <email>
            new TokenPattern(@"<(?:https?://[^>]+|[^@>]+@[^>]+)>", TokenType.MarkdownLink, 65),

            // Bold with asterisks
            new TokenPattern(@"\*\*[^*\n]+\*\*", TokenType.MarkdownBold, 60),

            // Bold with underscores
            new TokenPattern(@"__[^_\n]+__", TokenType.MarkdownBold, 60),

            // Italic with asterisk
            new TokenPattern(@"\*[^*\n]+\*", TokenType.MarkdownItalic, 55),

            // Italic with underscore
            new TokenPattern(@"_[^_\n]+_", TokenType.MarkdownItalic, 55),

            // Strikethrough
            new TokenPattern(@"~~[^~\n]+~~", TokenType.Comment, 50),

            // HTML tags (inline)
            new TokenPattern(@"</?[\w-]+(?:\s+[\w-]+(?:\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+))?)*\s*/?>", TokenType.XmlTag, 45),

            // Escape sequences
            new TokenPattern(@"\\[\\`*_{}[\]()#+\-.!]", TokenType.String, 40),

            // Plain text (word boundaries)
            new TokenPattern(@"[\w]+", TokenType.PlainText, 10),

            // Punctuation
            new TokenPattern(@"[^\w\s]", TokenType.Punctuation, 5),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Markdown;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
