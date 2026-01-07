namespace SyntaxColorizer.Tokenization;

/// <summary>
/// Interface for language-specific tokenizers.
/// </summary>
public interface ILanguageTokenizer
{
    /// <summary>
    /// Gets the language this tokenizer handles.
    /// </summary>
    SyntaxLanguage Language { get; }

    /// <summary>
    /// Tokenizes the given source text.
    /// </summary>
    /// <param name="text">The source text to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    IEnumerable<Token> Tokenize(string text);

    /// <summary>
    /// Tokenizes a specific range of the source text.
    /// Used for incremental parsing.
    /// </summary>
    /// <param name="text">The full source text.</param>
    /// <param name="startIndex">The start index of the range to tokenize.</param>
    /// <param name="length">The length of the range to tokenize.</param>
    /// <returns>An enumerable of tokens in the range.</returns>
    IEnumerable<Token> TokenizeRange(string text, int startIndex, int length);
}
