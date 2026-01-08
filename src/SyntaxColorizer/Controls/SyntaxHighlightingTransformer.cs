using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using SyntaxColorizer.Themes;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Controls;

/// <summary>
/// A document colorizing transformer that applies syntax highlighting using our tokenizers.
/// </summary>
public class SyntaxHighlightingTransformer : DocumentColorizingTransformer
{
    private SyntaxLanguage _language = SyntaxLanguage.None;
    private SyntaxTheme _theme = BuiltInThemes.Default;
    private string? _cachedText;
    private List<Token>? _cachedTokens;

    /// <summary>
    /// Gets or sets the programming language for syntax highlighting.
    /// </summary>
    public SyntaxLanguage Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                InvalidateCache();
            }
        }
    }

    /// <summary>
    /// Gets or sets the syntax highlighting theme.
    /// </summary>
    public SyntaxTheme Theme
    {
        get => _theme;
        set
        {
            if (_theme != value)
            {
                _theme = value;
                // No need to invalidate cache, just need to re-render
            }
        }
    }

    /// <summary>
    /// Invalidates the token cache, forcing re-tokenization on next render.
    /// </summary>
    public void InvalidateCache()
    {
        _cachedText = null;
        _cachedTokens = null;
    }

    /// <inheritdoc/>
    protected override void ColorizeLine(DocumentLine line)
    {
        if (_language == SyntaxLanguage.None)
            return;

        var tokenizer = TokenizerFactory.GetTokenizer(_language);
        if (tokenizer == null)
            return;

        // Get the full document text for tokenization
        var document = CurrentContext.Document;
        var fullText = document.Text;

        // Check if we need to re-tokenize
        if (_cachedText != fullText || _cachedTokens == null)
        {
            try
            {
                _cachedTokens = tokenizer.Tokenize(fullText).ToList();
                _cachedText = fullText;
            }
            catch
            {
                // If tokenization fails, skip highlighting
                return;
            }
        }

        var lineStartOffset = line.Offset;
        var lineEndOffset = line.EndOffset;

        // Find tokens that overlap with this line
        foreach (var token in _cachedTokens)
        {
            // Skip tokens that don't overlap with this line
            if (token.EndIndex <= lineStartOffset || token.StartIndex >= lineEndOffset)
                continue;

            // Calculate the portion of the token that's on this line
            var startInLine = Math.Max(token.StartIndex, lineStartOffset);
            var endInLine = Math.Min(token.EndIndex, lineEndOffset);

            if (startInLine >= endInLine)
                continue;

            var style = _theme.GetStyle(token.Type);

            try
            {
                ChangeLinePart(startInLine, endInLine, element =>
                {
                    if (style.Foreground != null)
                    {
                        element.TextRunProperties.SetForegroundBrush(style.Foreground);
                    }

                    if (style.Background != null)
                    {
                        element.TextRunProperties.SetBackgroundBrush(style.Background);
                    }

                    if (style.FontWeight != FontWeight.Normal)
                    {
                        element.TextRunProperties.SetTypeface(new Typeface(
                            element.TextRunProperties.Typeface.FontFamily,
                            style.FontStyle,
                            style.FontWeight));
                    }
                    else if (style.FontStyle != FontStyle.Normal)
                    {
                        element.TextRunProperties.SetTypeface(new Typeface(
                            element.TextRunProperties.Typeface.FontFamily,
                            style.FontStyle,
                            element.TextRunProperties.Typeface.Weight));
                    }

                    if (style.IsUnderline)
                    {
                        element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
                    }
                });
            }
            catch
            {
                // Ignore any errors during colorization
            }
        }
    }
}
