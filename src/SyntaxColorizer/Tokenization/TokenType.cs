namespace SyntaxColorizer.Tokenization;

/// <summary>
/// Types of tokens that can be recognized during syntax highlighting.
/// </summary>
public enum TokenType
{
    /// <summary>Plain text with no special highlighting.</summary>
    PlainText,

    /// <summary>Language keyword (if, else, class, etc.).</summary>
    Keyword,

    /// <summary>Control flow keyword (if, else, while, for, etc.).</summary>
    ControlKeyword,

    /// <summary>Type name (int, string, class names, etc.).</summary>
    TypeName,

    /// <summary>String literal ("hello", 'c', etc.).</summary>
    String,

    /// <summary>Character literal ('a', '\n', etc.).</summary>
    Character,

    /// <summary>Numeric literal (123, 3.14, 0xFF, etc.).</summary>
    Number,

    /// <summary>Single-line comment (// or #).</summary>
    Comment,

    /// <summary>Multi-line comment (/* ... */).</summary>
    MultiLineComment,

    /// <summary>Documentation comment (/// or /** ... */).</summary>
    DocComment,

    /// <summary>Operator (+, -, *, /, etc.).</summary>
    Operator,

    /// <summary>Punctuation (parentheses, braces, semicolons, etc.).</summary>
    Punctuation,

    /// <summary>Preprocessor directive (#include, #define, etc.).</summary>
    Preprocessor,

    /// <summary>Identifier (variable names, function names).</summary>
    Identifier,

    /// <summary>Method or function name.</summary>
    Method,

    /// <summary>Class or type declaration.</summary>
    TypeDeclaration,

    /// <summary>Attribute or annotation (@Override, [Attribute], etc.).</summary>
    Attribute,

    /// <summary>Namespace or module name.</summary>
    Namespace,

    /// <summary>Parameter name.</summary>
    Parameter,

    /// <summary>Property name.</summary>
    Property,

    /// <summary>Field or variable.</summary>
    Field,

    /// <summary>Constant value.</summary>
    Constant,

    /// <summary>SQL keyword (SELECT, FROM, WHERE, etc.).</summary>
    SqlKeyword,

    /// <summary>SQL function (COUNT, SUM, etc.).</summary>
    SqlFunction,

    /// <summary>Regular expression literal.</summary>
    Regex,

    /// <summary>XML/HTML tag name.</summary>
    XmlTag,

    /// <summary>XML/HTML attribute name.</summary>
    XmlAttribute,

    /// <summary>XML/HTML attribute value.</summary>
    XmlAttributeValue,

    /// <summary>CSS selector (class, id, element).</summary>
    CssSelector,

    /// <summary>CSS property name.</summary>
    CssProperty,

    /// <summary>CSS property value.</summary>
    CssValue,

    /// <summary>CSS unit (px, em, %, etc.).</summary>
    CssUnit,

    /// <summary>Markdown heading (# ## ###).</summary>
    MarkdownHeading,

    /// <summary>Markdown bold text.</summary>
    MarkdownBold,

    /// <summary>Markdown italic text.</summary>
    MarkdownItalic,

    /// <summary>Markdown link or URL.</summary>
    MarkdownLink,

    /// <summary>Markdown code (inline or block).</summary>
    MarkdownCode,

    /// <summary>Markdown list marker.</summary>
    MarkdownList,

    /// <summary>JSON/YAML property key.</summary>
    JsonKey,

    /// <summary>YAML anchor (&amp;name).</summary>
    YamlAnchor,

    /// <summary>YAML alias (*name).</summary>
    YamlAlias,

    /// <summary>YAML tag (!tag).</summary>
    YamlTag,

    /// <summary>Shell variable ($VAR, ${VAR}).</summary>
    ShellVariable,

    /// <summary>Shell command/builtin.</summary>
    ShellCommand,

    /// <summary>Shell option/flag (-x, --option).</summary>
    ShellOption,

    /// <summary>PowerShell cmdlet (Verb-Noun).</summary>
    PowerShellCmdlet,

    /// <summary>PowerShell parameter (-Parameter).</summary>
    PowerShellParameter,

    /// <summary>Error or invalid syntax.</summary>
    Error,

    /// <summary>Warning indicator.</summary>
    Warning
}
