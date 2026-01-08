using Avalonia.Media;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Themes;

/// <summary>
/// Provides built-in syntax highlighting themes.
/// </summary>
public static class BuiltInThemes
{
    /// <summary>
    /// Gets a Visual Studio-like light theme.
    /// </summary>
    public static SyntaxTheme VisualStudioLight => CreateVisualStudioLight();

    /// <summary>
    /// Gets a Visual Studio-like dark theme.
    /// </summary>
    public static SyntaxTheme VisualStudioDark => CreateVisualStudioDark();

    /// <summary>
    /// Gets a Monokai-inspired dark theme.
    /// </summary>
    public static SyntaxTheme Monokai => CreateMonokai();

    /// <summary>
    /// Gets a GitHub-like light theme.
    /// </summary>
    public static SyntaxTheme GitHubLight => CreateGitHubLight();

    /// <summary>
    /// Gets a GitHub-like dark theme.
    /// </summary>
    public static SyntaxTheme GitHubDark => CreateGitHubDark();

    /// <summary>
    /// Gets a Solarized Light theme.
    /// </summary>
    public static SyntaxTheme SolarizedLight => CreateSolarizedLight();

    /// <summary>
    /// Gets a Solarized Dark theme.
    /// </summary>
    public static SyntaxTheme SolarizedDark => CreateSolarizedDark();

    /// <summary>
    /// Gets a Dracula dark theme.
    /// </summary>
    public static SyntaxTheme Dracula => CreateDracula();

    /// <summary>
    /// Gets a One Dark theme (Atom-style).
    /// </summary>
    public static SyntaxTheme OneDark => CreateOneDark();

    /// <summary>
    /// Gets a Nord dark theme.
    /// </summary>
    public static SyntaxTheme Nord => CreateNord();

    /// <summary>
    /// Gets a Gruvbox Dark theme.
    /// </summary>
    public static SyntaxTheme GruvboxDark => CreateGruvboxDark();

    /// <summary>
    /// Gets a Gruvbox Light theme.
    /// </summary>
    public static SyntaxTheme GruvboxLight => CreateGruvboxLight();

    /// <summary>
    /// Gets a One Light theme (Atom-style).
    /// </summary>
    public static SyntaxTheme OneLight => CreateOneLight();

    /// <summary>
    /// Gets a Quiet Light theme.
    /// </summary>
    public static SyntaxTheme QuietLight => CreateQuietLight();

    /// <summary>
    /// Gets the default theme (Visual Studio Light).
    /// </summary>
    public static SyntaxTheme Default => VisualStudioLight;

    private static SyntaxTheme CreateVisualStudioLight()
    {
        var theme = new SyntaxTheme
        {
            Name = "Visual Studio Light",
            DefaultForeground = Brushes.Black,
            DefaultBackground = Brushes.White,
            SelectionBackground = new SolidColorBrush(Color.FromRgb(173, 214, 255)),
            CurrentLineBackground = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(43, 145, 175)),
            CaretBrush = Brushes.Black,
            LineNumberSeparator = Brushes.LightGray
        };

        var blue = new SolidColorBrush(Color.FromRgb(0, 0, 255));
        var darkBlue = new SolidColorBrush(Color.FromRgb(0, 0, 139));
        var green = new SolidColorBrush(Color.FromRgb(0, 128, 0));
        var darkGreen = new SolidColorBrush(Color.FromRgb(0, 100, 0));
        var red = new SolidColorBrush(Color.FromRgb(163, 21, 21));
        var purple = new SolidColorBrush(Color.FromRgb(128, 0, 128));
        var teal = new SolidColorBrush(Color.FromRgb(43, 145, 175));
        var orange = new SolidColorBrush(Color.FromRgb(163, 73, 0));
        var gray = new SolidColorBrush(Color.FromRgb(128, 128, 128));
        var maroon = new SolidColorBrush(Color.FromRgb(128, 0, 0));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = darkGreen };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = Brushes.Black };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = maroon };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = maroon };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = maroon };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = maroon, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = Brushes.Black, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = Brushes.Black, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = maroon };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = Brushes.Red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateVisualStudioDark()
    {
        var theme = new SyntaxTheme
        {
            Name = "Visual Studio Dark",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(38, 79, 120)),
            CurrentLineBackground = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(43, 145, 175)),
            CaretBrush = Brushes.White,
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(60, 60, 60))
        };

        var blue = new SolidColorBrush(Color.FromRgb(86, 156, 214));
        var lightBlue = new SolidColorBrush(Color.FromRgb(156, 220, 254));
        var green = new SolidColorBrush(Color.FromRgb(87, 166, 74));
        var lightGreen = new SolidColorBrush(Color.FromRgb(181, 206, 168));
        var orange = new SolidColorBrush(Color.FromRgb(206, 145, 120));
        var purple = new SolidColorBrush(Color.FromRgb(197, 134, 192));
        var teal = new SolidColorBrush(Color.FromRgb(78, 201, 176));
        var yellow = new SolidColorBrush(Color.FromRgb(220, 220, 170));
        var gray = new SolidColorBrush(Color.FromRgb(128, 128, 128));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = lightGreen };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = lightGreen };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = blue, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = theme.DefaultForeground, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = lightBlue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100)), IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = new SolidColorBrush(Color.FromRgb(255, 200, 100)), IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateMonokai()
    {
        var theme = new SyntaxTheme
        {
            Name = "Monokai",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(248, 248, 242)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(39, 40, 34)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(73, 72, 62)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(60, 60, 50)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(144, 144, 138)),
            CaretBrush = Brushes.White,
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(73, 72, 62))
        };

        var pink = new SolidColorBrush(Color.FromRgb(249, 38, 114));
        var green = new SolidColorBrush(Color.FromRgb(166, 226, 46));
        var yellow = new SolidColorBrush(Color.FromRgb(230, 219, 116));
        var orange = new SolidColorBrush(Color.FromRgb(253, 151, 31));
        var purple = new SolidColorBrush(Color.FromRgb(174, 129, 255));
        var cyan = new SolidColorBrush(Color.FromRgb(102, 217, 239));
        var comment = new SolidColorBrush(Color.FromRgb(117, 113, 94));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = cyan, IsItalic = true };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = orange, IsItalic = true };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = pink, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = theme.DefaultForeground, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = cyan, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = orange, IsItalic = true };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = pink, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateGitHubLight()
    {
        var theme = new SyntaxTheme
        {
            Name = "GitHub Light",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(36, 41, 47)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(180, 215, 255)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(246, 248, 250)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(140, 149, 159)),
            CaretBrush = Brushes.Black,
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(225, 228, 232))
        };

        var red = new SolidColorBrush(Color.FromRgb(207, 34, 46));
        var blue = new SolidColorBrush(Color.FromRgb(5, 80, 174));
        var purple = new SolidColorBrush(Color.FromRgb(130, 80, 223));
        var green = new SolidColorBrush(Color.FromRgb(17, 99, 41));
        var orange = new SolidColorBrush(Color.FromRgb(149, 56, 0));
        var teal = new SolidColorBrush(Color.FromRgb(0, 92, 197));
        var gray = new SolidColorBrush(Color.FromRgb(106, 115, 125));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = red, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = theme.DefaultForeground, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateGitHubDark()
    {
        var theme = new SyntaxTheme
        {
            Name = "GitHub Dark",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(201, 209, 217)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(13, 17, 23)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(56, 139, 253)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(22, 27, 34)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(110, 118, 129)),
            CaretBrush = Brushes.White,
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(48, 54, 61))
        };

        var red = new SolidColorBrush(Color.FromRgb(255, 123, 114));
        var blue = new SolidColorBrush(Color.FromRgb(121, 192, 255));
        var purple = new SolidColorBrush(Color.FromRgb(210, 168, 255));
        var green = new SolidColorBrush(Color.FromRgb(126, 231, 135));
        var orange = new SolidColorBrush(Color.FromRgb(255, 166, 87));
        var cyan = new SolidColorBrush(Color.FromRgb(165, 214, 255));
        var gray = new SolidColorBrush(Color.FromRgb(139, 148, 158));
        var lightBlue = new SolidColorBrush(Color.FromRgb(165, 214, 255));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = lightBlue };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = red, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = theme.DefaultForeground, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateSolarizedLight()
    {
        // Solarized color palette - light variant
        var base03 = new SolidColorBrush(Color.FromRgb(0, 43, 54));
        var base02 = new SolidColorBrush(Color.FromRgb(7, 54, 66));
        var base01 = new SolidColorBrush(Color.FromRgb(88, 110, 117));
        var base00 = new SolidColorBrush(Color.FromRgb(101, 123, 131));
        var base0 = new SolidColorBrush(Color.FromRgb(131, 148, 150));
        var base1 = new SolidColorBrush(Color.FromRgb(147, 161, 161));
        var base2 = new SolidColorBrush(Color.FromRgb(238, 232, 213));
        var base3 = new SolidColorBrush(Color.FromRgb(253, 246, 227));

        var yellow = new SolidColorBrush(Color.FromRgb(181, 137, 0));
        var orange = new SolidColorBrush(Color.FromRgb(203, 75, 22));
        var red = new SolidColorBrush(Color.FromRgb(220, 50, 47));
        var magenta = new SolidColorBrush(Color.FromRgb(211, 54, 130));
        var violet = new SolidColorBrush(Color.FromRgb(108, 113, 196));
        var blue = new SolidColorBrush(Color.FromRgb(38, 139, 210));
        var cyan = new SolidColorBrush(Color.FromRgb(42, 161, 152));
        var green = new SolidColorBrush(Color.FromRgb(133, 153, 0));

        var theme = new SyntaxTheme
        {
            Name = "Solarized Light",
            DefaultForeground = base00,
            DefaultBackground = base3,
            SelectionBackground = new SolidColorBrush(Color.FromRgb(217, 210, 190)),
            CurrentLineBackground = base2,
            LineNumberForeground = base1,
            CaretBrush = base00,
            LineNumberSeparator = base2
        };

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = magenta };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = base1, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = base1, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = base1, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = base00 };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = base00 };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = base00 };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = base00 };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = magenta };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = base00, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = base00, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = base1 };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateSolarizedDark()
    {
        // Solarized color palette - dark variant
        var base03 = new SolidColorBrush(Color.FromRgb(0, 43, 54));
        var base02 = new SolidColorBrush(Color.FromRgb(7, 54, 66));
        var base01 = new SolidColorBrush(Color.FromRgb(88, 110, 117));
        var base00 = new SolidColorBrush(Color.FromRgb(101, 123, 131));
        var base0 = new SolidColorBrush(Color.FromRgb(131, 148, 150));
        var base1 = new SolidColorBrush(Color.FromRgb(147, 161, 161));
        var base2 = new SolidColorBrush(Color.FromRgb(238, 232, 213));
        var base3 = new SolidColorBrush(Color.FromRgb(253, 246, 227));

        var yellow = new SolidColorBrush(Color.FromRgb(181, 137, 0));
        var orange = new SolidColorBrush(Color.FromRgb(203, 75, 22));
        var red = new SolidColorBrush(Color.FromRgb(220, 50, 47));
        var magenta = new SolidColorBrush(Color.FromRgb(211, 54, 130));
        var violet = new SolidColorBrush(Color.FromRgb(108, 113, 196));
        var blue = new SolidColorBrush(Color.FromRgb(38, 139, 210));
        var cyan = new SolidColorBrush(Color.FromRgb(42, 161, 152));
        var green = new SolidColorBrush(Color.FromRgb(133, 153, 0));

        var theme = new SyntaxTheme
        {
            Name = "Solarized Dark",
            DefaultForeground = base0,
            DefaultBackground = base03,
            SelectionBackground = new SolidColorBrush(Color.FromRgb(7, 54, 66)),
            CurrentLineBackground = base02,
            LineNumberForeground = base01,
            CaretBrush = base0,
            LineNumberSeparator = base02
        };

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = magenta };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = base01, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = base01, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = base01, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = base0 };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = base0 };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = base0 };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = base0 };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = magenta };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = base0, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = base0, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = violet };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = base01 };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateDracula()
    {
        var theme = new SyntaxTheme
        {
            Name = "Dracula",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(248, 248, 242)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(40, 42, 54)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(68, 71, 90)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(68, 71, 90)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(98, 114, 164)),
            CaretBrush = new SolidColorBrush(Color.FromRgb(248, 248, 242)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(68, 71, 90))
        };

        var pink = new SolidColorBrush(Color.FromRgb(255, 121, 198));
        var purple = new SolidColorBrush(Color.FromRgb(189, 147, 249));
        var cyan = new SolidColorBrush(Color.FromRgb(139, 233, 253));
        var green = new SolidColorBrush(Color.FromRgb(80, 250, 123));
        var yellow = new SolidColorBrush(Color.FromRgb(241, 250, 140));
        var orange = new SolidColorBrush(Color.FromRgb(255, 184, 108));
        var red = new SolidColorBrush(Color.FromRgb(255, 85, 85));
        var comment = new SolidColorBrush(Color.FromRgb(98, 114, 164));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = cyan, IsItalic = true };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = green, IsItalic = true };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = orange, IsItalic = true };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = purple, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = yellow, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = cyan, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = pink };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = orange, IsItalic = true };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateOneDark()
    {
        var theme = new SyntaxTheme
        {
            Name = "One Dark",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(171, 178, 191)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(40, 44, 52)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(62, 68, 81)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(44, 49, 58)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(76, 82, 99)),
            CaretBrush = new SolidColorBrush(Color.FromRgb(82, 139, 255)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(53, 59, 69))
        };

        var red = new SolidColorBrush(Color.FromRgb(224, 108, 117));
        var green = new SolidColorBrush(Color.FromRgb(152, 195, 121));
        var yellow = new SolidColorBrush(Color.FromRgb(229, 192, 123));
        var blue = new SolidColorBrush(Color.FromRgb(97, 175, 239));
        var purple = new SolidColorBrush(Color.FromRgb(198, 120, 221));
        var cyan = new SolidColorBrush(Color.FromRgb(86, 182, 194));
        var orange = new SolidColorBrush(Color.FromRgb(209, 154, 102));
        var comment = new SolidColorBrush(Color.FromRgb(92, 99, 112));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = red, IsItalic = true };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = red, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = purple, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = red, IsItalic = true };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = yellow, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateNord()
    {
        // Nord color palette
        var theme = new SyntaxTheme
        {
            Name = "Nord",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(216, 222, 233)), // nord4
            DefaultBackground = new SolidColorBrush(Color.FromRgb(46, 52, 64)),    // nord0
            SelectionBackground = new SolidColorBrush(Color.FromRgb(67, 76, 94)),  // nord2
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(59, 66, 82)), // nord1
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(76, 86, 106)), // nord3
            CaretBrush = new SolidColorBrush(Color.FromRgb(216, 222, 233)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(59, 66, 82))
        };

        // Nord frost colors
        var nord7 = new SolidColorBrush(Color.FromRgb(143, 188, 187)); // frost - cyan
        var nord8 = new SolidColorBrush(Color.FromRgb(136, 192, 208)); // frost - light blue
        var nord9 = new SolidColorBrush(Color.FromRgb(129, 161, 193)); // frost - blue
        var nord10 = new SolidColorBrush(Color.FromRgb(94, 129, 172)); // frost - dark blue

        // Nord aurora colors
        var nord11 = new SolidColorBrush(Color.FromRgb(191, 97, 106));  // aurora - red
        var nord12 = new SolidColorBrush(Color.FromRgb(208, 135, 112)); // aurora - orange
        var nord13 = new SolidColorBrush(Color.FromRgb(235, 203, 139)); // aurora - yellow
        var nord14 = new SolidColorBrush(Color.FromRgb(163, 190, 140)); // aurora - green
        var nord15 = new SolidColorBrush(Color.FromRgb(180, 142, 173)); // aurora - purple

        var comment = new SolidColorBrush(Color.FromRgb(97, 110, 136)); // between nord3 and nord4

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = nord7 };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = nord7 };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = nord14 };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = nord14 };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = nord15 };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = nord10 };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = nord12 };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = nord15 };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = nord13 };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = nord14 };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = nord14 };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = nord15 };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = nord9, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = nord13, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = nord8, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = nord7 };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = nord9 };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = nord15 };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = nord15 };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = nord7 };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = nord8 };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = nord12 };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = nord11, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = nord13, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateGruvboxDark()
    {
        var theme = new SyntaxTheme
        {
            Name = "Gruvbox Dark",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(235, 219, 178)), // fg
            DefaultBackground = new SolidColorBrush(Color.FromRgb(40, 40, 40)),    // bg
            SelectionBackground = new SolidColorBrush(Color.FromRgb(80, 73, 69)),  // bg2
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(60, 56, 54)), // bg1
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(124, 111, 100)), // gray
            CaretBrush = new SolidColorBrush(Color.FromRgb(235, 219, 178)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(60, 56, 54))
        };

        var red = new SolidColorBrush(Color.FromRgb(251, 73, 52));
        var green = new SolidColorBrush(Color.FromRgb(184, 187, 38));
        var yellow = new SolidColorBrush(Color.FromRgb(250, 189, 47));
        var blue = new SolidColorBrush(Color.FromRgb(131, 165, 152));
        var purple = new SolidColorBrush(Color.FromRgb(211, 134, 155));
        var aqua = new SolidColorBrush(Color.FromRgb(142, 192, 124));
        var orange = new SolidColorBrush(Color.FromRgb(254, 128, 25));
        var gray = new SolidColorBrush(Color.FromRgb(146, 131, 116));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = yellow, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateGruvboxLight()
    {
        var theme = new SyntaxTheme
        {
            Name = "Gruvbox Light",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(60, 56, 54)),    // fg
            DefaultBackground = new SolidColorBrush(Color.FromRgb(251, 241, 199)), // bg
            SelectionBackground = new SolidColorBrush(Color.FromRgb(213, 196, 161)), // bg2
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(235, 219, 178)), // bg1
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(124, 111, 100)), // gray
            CaretBrush = new SolidColorBrush(Color.FromRgb(60, 56, 54)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(213, 196, 161))
        };

        var red = new SolidColorBrush(Color.FromRgb(204, 36, 29));
        var green = new SolidColorBrush(Color.FromRgb(152, 151, 26));
        var yellow = new SolidColorBrush(Color.FromRgb(215, 153, 33));
        var blue = new SolidColorBrush(Color.FromRgb(69, 133, 136));
        var purple = new SolidColorBrush(Color.FromRgb(177, 98, 134));
        var aqua = new SolidColorBrush(Color.FromRgb(104, 157, 106));
        var orange = new SolidColorBrush(Color.FromRgb(214, 93, 14));
        var gray = new SolidColorBrush(Color.FromRgb(146, 131, 116));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = gray, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = yellow, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = gray };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = aqua };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateOneLight()
    {
        var theme = new SyntaxTheme
        {
            Name = "One Light",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(56, 58, 66)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(157, 157, 161)),
            CaretBrush = new SolidColorBrush(Color.FromRgb(82, 139, 255)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(230, 230, 230))
        };

        var red = new SolidColorBrush(Color.FromRgb(228, 86, 73));
        var green = new SolidColorBrush(Color.FromRgb(80, 161, 79));
        var yellow = new SolidColorBrush(Color.FromRgb(193, 132, 1));
        var blue = new SolidColorBrush(Color.FromRgb(64, 120, 242));
        var purple = new SolidColorBrush(Color.FromRgb(166, 38, 164));
        var cyan = new SolidColorBrush(Color.FromRgb(1, 132, 188));
        var orange = new SolidColorBrush(Color.FromRgb(152, 104, 1));
        var comment = new SolidColorBrush(Color.FromRgb(160, 161, 167));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = red, IsItalic = true };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = cyan };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = red, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = orange, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = purple, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = yellow };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = yellow, IsUnderline = true };

        return theme;
    }

    private static SyntaxTheme CreateQuietLight()
    {
        var theme = new SyntaxTheme
        {
            Name = "Quiet Light",
            DefaultForeground = new SolidColorBrush(Color.FromRgb(51, 51, 51)),
            DefaultBackground = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
            SelectionBackground = new SolidColorBrush(Color.FromRgb(196, 217, 242)),
            CurrentLineBackground = new SolidColorBrush(Color.FromRgb(233, 241, 248)),
            LineNumberForeground = new SolidColorBrush(Color.FromRgb(170, 175, 186)),
            CaretBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            LineNumberSeparator = new SolidColorBrush(Color.FromRgb(225, 228, 232))
        };

        var red = new SolidColorBrush(Color.FromRgb(170, 55, 49));
        var green = new SolidColorBrush(Color.FromRgb(68, 140, 39));
        var blue = new SolidColorBrush(Color.FromRgb(55, 80, 158));
        var purple = new SolidColorBrush(Color.FromRgb(127, 55, 153));
        var orange = new SolidColorBrush(Color.FromRgb(171, 102, 0));
        var teal = new SolidColorBrush(Color.FromRgb(0, 128, 128));
        var darkGray = new SolidColorBrush(Color.FromRgb(119, 119, 119));
        var comment = new SolidColorBrush(Color.FromRgb(170, 170, 170));

        theme.TokenStyles[TokenType.Keyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.ControlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.TypeName] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.TypeDeclaration] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.String] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Character] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.Number] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Comment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.MultiLineComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.DocComment] = new TokenStyle { Foreground = comment, IsItalic = true };
        theme.TokenStyles[TokenType.Operator] = new TokenStyle { Foreground = darkGray };
        theme.TokenStyles[TokenType.Punctuation] = new TokenStyle { Foreground = darkGray };
        theme.TokenStyles[TokenType.Preprocessor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.Attribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.Method] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Namespace] = new TokenStyle { Foreground = theme.DefaultForeground };
        theme.TokenStyles[TokenType.Parameter] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Property] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Field] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Constant] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.SqlKeyword] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.SqlFunction] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.Regex] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.XmlTag] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.XmlAttribute] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.XmlAttributeValue] = new TokenStyle { Foreground = green };
        theme.TokenStyles[TokenType.CssSelector] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.CssProperty] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.CssValue] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.CssUnit] = new TokenStyle { Foreground = orange };
        theme.TokenStyles[TokenType.MarkdownHeading] = new TokenStyle { Foreground = blue, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownBold] = new TokenStyle { Foreground = theme.DefaultForeground, IsBold = true };
        theme.TokenStyles[TokenType.MarkdownItalic] = new TokenStyle { Foreground = theme.DefaultForeground, IsItalic = true };
        theme.TokenStyles[TokenType.MarkdownLink] = new TokenStyle { Foreground = blue, IsUnderline = true };
        theme.TokenStyles[TokenType.MarkdownCode] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.MarkdownList] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.JsonKey] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.YamlAnchor] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlAlias] = new TokenStyle { Foreground = purple };
        theme.TokenStyles[TokenType.YamlTag] = new TokenStyle { Foreground = teal };
        theme.TokenStyles[TokenType.ShellVariable] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.ShellCommand] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.ShellOption] = new TokenStyle { Foreground = comment };
        theme.TokenStyles[TokenType.PowerShellCmdlet] = new TokenStyle { Foreground = blue };
        theme.TokenStyles[TokenType.PowerShellParameter] = new TokenStyle { Foreground = red };
        theme.TokenStyles[TokenType.Error] = new TokenStyle { Foreground = red, IsUnderline = true };
        theme.TokenStyles[TokenType.Warning] = new TokenStyle { Foreground = orange, IsUnderline = true };

        return theme;
    }
}
