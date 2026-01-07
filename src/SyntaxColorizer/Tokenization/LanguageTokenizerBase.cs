using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization;

/// <summary>
/// Base class for language tokenizers using regex-based pattern matching.
/// </summary>
public abstract class LanguageTokenizerBase : ILanguageTokenizer
{
    /// <summary>
    /// Represents a token pattern with its regex and resulting token type.
    /// </summary>
    protected readonly struct TokenPattern
    {
        public Regex Regex { get; }
        public TokenType Type { get; }
        public int Priority { get; }

        public TokenPattern(string pattern, TokenType type, int priority = 0, RegexOptions options = RegexOptions.None)
        {
            Regex = new Regex(pattern, options | RegexOptions.Compiled);
            Type = type;
            Priority = priority;
        }
    }

    /// <inheritdoc/>
    public abstract SyntaxLanguage Language { get; }

    /// <summary>
    /// Gets the token patterns for this language, ordered by priority.
    /// Patterns are matched in order; first match wins.
    /// </summary>
    protected abstract IReadOnlyList<TokenPattern> Patterns { get; }

    /// <summary>
    /// Gets the keywords for this language mapped to their token types.
    /// </summary>
    protected virtual IReadOnlyDictionary<string, TokenType>? Keywords => null;

    /// <inheritdoc/>
    public virtual IEnumerable<Token> Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        var tokens = new List<Token>();
        var position = 0;

        while (position < text.Length)
        {
            var matched = false;
            Token? bestMatch = null;
            var bestPriority = int.MinValue;

            foreach (var pattern in Patterns)
            {
                var match = pattern.Regex.Match(text, position);
                if (match.Success && match.Index == position && match.Length > 0)
                {
                    if (pattern.Priority > bestPriority ||
                        (pattern.Priority == bestPriority && (!bestMatch.HasValue || match.Length > bestMatch.Value.Length)))
                    {
                        var tokenType = pattern.Type;

                        // Check if it's an identifier that might be a keyword
                        if (tokenType == TokenType.Identifier && Keywords != null)
                        {
                            var tokenText = match.Value;
                            if (Keywords.TryGetValue(tokenText, out var keywordType))
                            {
                                tokenType = keywordType;
                            }
                        }

                        bestMatch = new Token(position, match.Length, tokenType);
                        bestPriority = pattern.Priority;
                        matched = true;
                    }
                }
            }

            if (matched && bestMatch.HasValue)
            {
                yield return bestMatch.Value;
                position += bestMatch.Value.Length;
            }
            else
            {
                // No pattern matched, emit a single character as plain text
                yield return new Token(position, 1, TokenType.PlainText);
                position++;
            }
        }
    }

    /// <inheritdoc/>
    public virtual IEnumerable<Token> TokenizeRange(string text, int startIndex, int length)
    {
        // For now, retokenize the entire text and filter
        // A more sophisticated implementation could use incremental parsing
        return Tokenize(text).Where(t =>
            t.EndIndex > startIndex && t.StartIndex < startIndex + length);
    }

    /// <summary>
    /// Creates a combined regex pattern from a list of keywords.
    /// </summary>
    protected static string CreateKeywordPattern(IEnumerable<string> keywords)
    {
        return $@"\b({string.Join("|", keywords.Select(Regex.Escape))})\b";
    }

    /// <summary>
    /// Common regex patterns used across multiple languages.
    /// </summary>
    protected static class CommonPatterns
    {
        public const string SingleLineComment = @"//[^\r\n]*";
        public const string HashComment = @"#[^\r\n]*";
        public const string MultiLineComment = @"/\*[\s\S]*?\*/";
        public const string DoubleQuotedString = @"""(?:[^""\\]|\\.)*""";
        public const string SingleQuotedString = @"'(?:[^'\\]|\\.)*'";
        public const string BacktickString = @"`(?:[^`\\]|\\.)*`";
        public const string Integer = @"\b\d+[lLuU]?\b";
        public const string HexNumber = @"\b0[xX][0-9a-fA-F]+[lLuU]?\b";
        public const string BinaryNumber = @"\b0[bB][01]+[lLuU]?\b";
        public const string FloatingPoint = @"\b\d+\.\d+(?:[eE][+-]?\d+)?[fFdDmM]?\b";
        public const string ScientificNotation = @"\b\d+[eE][+-]?\d+[fFdDmM]?\b";
        public const string Identifier = @"\b[a-zA-Z_][a-zA-Z0-9_]*\b";
        public const string Operator = @"[+\-*/%=<>!&|^~?:]+|\.{2,3}";
        public const string Punctuation = @"[(){}\[\];,.]";
        public const string Whitespace = @"\s+";
    }
}
