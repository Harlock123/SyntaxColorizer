namespace SyntaxColorizer.Tokenization;

/// <summary>
/// Represents a single token in the source text.
/// </summary>
public readonly struct Token
{
    /// <summary>
    /// Gets the start index of the token in the source text.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Gets the length of the token.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the end index of the token (exclusive).
    /// </summary>
    public int EndIndex => StartIndex + Length;

    /// <summary>
    /// Gets the type of this token.
    /// </summary>
    public TokenType Type { get; }

    /// <summary>
    /// Creates a new token.
    /// </summary>
    /// <param name="startIndex">The start index in the source text.</param>
    /// <param name="length">The length of the token.</param>
    /// <param name="type">The type of token.</param>
    public Token(int startIndex, int length, TokenType type)
    {
        StartIndex = startIndex;
        Length = length;
        Type = type;
    }

    /// <summary>
    /// Gets the text of this token from the source.
    /// </summary>
    /// <param name="source">The source text.</param>
    /// <returns>The token text.</returns>
    public string GetText(string source)
    {
        if (StartIndex < 0 || StartIndex + Length > source.Length)
            return string.Empty;
        return source.Substring(StartIndex, Length);
    }

    /// <summary>
    /// Gets the text of this token from the source as a span.
    /// </summary>
    /// <param name="source">The source text.</param>
    /// <returns>The token text as a read-only span.</returns>
    public ReadOnlySpan<char> GetTextSpan(ReadOnlySpan<char> source)
    {
        if (StartIndex < 0 || StartIndex + Length > source.Length)
            return ReadOnlySpan<char>.Empty;
        return source.Slice(StartIndex, Length);
    }

    public override string ToString() => $"Token({Type}, {StartIndex}..{EndIndex})";
}
