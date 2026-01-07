using Avalonia.Media;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Themes;

/// <summary>
/// Defines a color scheme for syntax highlighting.
/// </summary>
public class SyntaxTheme
{
    /// <summary>
    /// Gets or sets the name of this theme.
    /// </summary>
    public string Name { get; set; } = "Default";

    /// <summary>
    /// Gets or sets the default text color.
    /// </summary>
    public IBrush DefaultForeground { get; set; } = Brushes.Black;

    /// <summary>
    /// Gets or sets the default background color.
    /// </summary>
    public IBrush DefaultBackground { get; set; } = Brushes.White;

    /// <summary>
    /// Gets or sets the selection background color.
    /// </summary>
    public IBrush SelectionBackground { get; set; } = new SolidColorBrush(Color.FromRgb(173, 214, 255));

    /// <summary>
    /// Gets or sets the current line highlight color.
    /// </summary>
    public IBrush CurrentLineBackground { get; set; } = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));

    /// <summary>
    /// Gets or sets the line number foreground color.
    /// </summary>
    public IBrush LineNumberForeground { get; set; } = Brushes.Gray;

    /// <summary>
    /// Gets or sets the caret/cursor color.
    /// </summary>
    public IBrush CaretBrush { get; set; } = Brushes.Black;

    /// <summary>
    /// Gets or sets the line number separator color.
    /// </summary>
    public IBrush LineNumberSeparator { get; set; } = Brushes.LightGray;

    /// <summary>
    /// Gets the token style mappings.
    /// </summary>
    public Dictionary<TokenType, TokenStyle> TokenStyles { get; } = new();

    /// <summary>
    /// Gets the style for a specific token type.
    /// </summary>
    /// <param name="tokenType">The token type.</param>
    /// <returns>The token style, or a default style if not found.</returns>
    public TokenStyle GetStyle(TokenType tokenType)
    {
        if (TokenStyles.TryGetValue(tokenType, out var style))
            return style;

        return new TokenStyle { Foreground = DefaultForeground };
    }
}

/// <summary>
/// Defines the visual style for a token type.
/// </summary>
public class TokenStyle
{
    /// <summary>
    /// Gets or sets the foreground brush.
    /// </summary>
    public IBrush? Foreground { get; set; }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? Background { get; set; }

    /// <summary>
    /// Gets or sets whether the text should be bold.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Gets or sets whether the text should be italic.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Gets or sets whether the text should be underlined.
    /// </summary>
    public bool IsUnderline { get; set; }

    /// <summary>
    /// Gets the font weight based on the IsBold property.
    /// </summary>
    public FontWeight FontWeight => IsBold ? FontWeight.Bold : FontWeight.Normal;

    /// <summary>
    /// Gets the font style based on the IsItalic property.
    /// </summary>
    public FontStyle FontStyle => IsItalic ? FontStyle.Italic : FontStyle.Normal;
}
