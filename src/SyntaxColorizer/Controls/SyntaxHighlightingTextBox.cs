using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using SyntaxColorizer.Formatting;
using SyntaxColorizer.Linting;
using SyntaxColorizer.Themes;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Controls;

/// <summary>
/// A text box control with syntax highlighting support for multiple programming languages.
/// </summary>
public class SyntaxHighlightingTextBox : TemplatedControl
{
    private TextBox? _textBox;
    private SelectableTextBlock? _highlightedTextBlock;
    private ScrollViewer? _scrollViewer;
    private ItemsControl? _lineNumbers;
    private Canvas? _lintingOverlay;
    private Border? _lineNumberSeparator;
    private DispatcherTimer? _updateTimer;
    private string _lastHighlightedText = string.Empty;
    private bool _isUpdating;

    /// <summary>
    /// Defines the Text property.
    /// </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, string>(nameof(Text), string.Empty);

    /// <summary>
    /// Defines the Language property.
    /// </summary>
    public static readonly StyledProperty<SyntaxLanguage> LanguageProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, SyntaxLanguage>(nameof(Language), SyntaxLanguage.None);

    /// <summary>
    /// Defines the SyntaxTheme property.
    /// </summary>
    public static readonly StyledProperty<SyntaxTheme> SyntaxThemeProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, SyntaxTheme>(nameof(SyntaxTheme));

    /// <summary>
    /// Defines the ShowLineNumbers property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(ShowLineNumbers), true);

    /// <summary>
    /// Defines the IsReadOnly property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(IsReadOnly), false);

    /// <summary>
    /// Defines the AcceptsReturn property.
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(AcceptsReturn), true);

    /// <summary>
    /// Defines the AcceptsTab property.
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsTabProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(AcceptsTab), true);

    /// <summary>
    /// Defines the UpdateDelay property (in milliseconds).
    /// </summary>
    public static readonly StyledProperty<int> UpdateDelayProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, int>(nameof(UpdateDelay), 100);

    /// <summary>
    /// Defines the AutoIndent property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoIndentProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(AutoIndent), true);

    /// <summary>
    /// Defines the IndentSize property (number of spaces per indent level).
    /// </summary>
    public static readonly StyledProperty<int> IndentSizeProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, int>(nameof(IndentSize), 4);

    /// <summary>
    /// Defines the UseSpacesForIndent property.
    /// </summary>
    public static readonly StyledProperty<bool> UseSpacesForIndentProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(UseSpacesForIndent), true);

    /// <summary>
    /// Defines the AutoCloseBrackets property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoCloseBracketsProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, bool>(nameof(AutoCloseBrackets), true);

    /// <summary>
    /// Defines the LineNumberForeground property (set from theme).
    /// </summary>
    public static readonly StyledProperty<IBrush> LineNumberForegroundProperty =
        AvaloniaProperty.Register<SyntaxHighlightingTextBox, IBrush>(nameof(LineNumberForeground), Brushes.Gray);

    /// <summary>
    /// Defines the LintingHints property.
    /// </summary>
    public static readonly DirectProperty<SyntaxHighlightingTextBox, ObservableCollection<LintingHint>> LintingHintsProperty =
        AvaloniaProperty.RegisterDirect<SyntaxHighlightingTextBox, ObservableCollection<LintingHint>>(
            nameof(LintingHints),
            o => o.LintingHints);

    private ObservableCollection<LintingHint> _lintingHints = new();

    // Bracket pairs for auto-closing
    private static readonly Dictionary<char, char> BracketPairs = new()
    {
        { '(', ')' },
        { '[', ']' },
        { '{', '}' },
        { '"', '"' },
        { '\'', '\'' },
        { '`', '`' }
    };

    private static readonly HashSet<char> ClosingBrackets = new() { ')', ']', '}', '"', '\'', '`' };

    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the programming language for syntax highlighting.
    /// </summary>
    public SyntaxLanguage Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets the syntax highlighting theme.
    /// </summary>
    public SyntaxTheme SyntaxTheme
    {
        get => GetValue(SyntaxThemeProperty);
        set => SetValue(SyntaxThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether line numbers are shown.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the text is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control accepts return keys.
    /// </summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control accepts tab keys.
    /// </summary>
    public bool AcceptsTab
    {
        get => GetValue(AcceptsTabProperty);
        set => SetValue(AcceptsTabProperty, value);
    }

    /// <summary>
    /// Gets or sets the delay before updating syntax highlighting (in milliseconds).
    /// </summary>
    public int UpdateDelay
    {
        get => GetValue(UpdateDelayProperty);
        set => SetValue(UpdateDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether automatic indentation is enabled.
    /// </summary>
    public bool AutoIndent
    {
        get => GetValue(AutoIndentProperty);
        set => SetValue(AutoIndentProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of spaces per indent level.
    /// </summary>
    public int IndentSize
    {
        get => GetValue(IndentSizeProperty);
        set => SetValue(IndentSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to use spaces instead of tabs for indentation.
    /// </summary>
    public bool UseSpacesForIndent
    {
        get => GetValue(UseSpacesForIndentProperty);
        set => SetValue(UseSpacesForIndentProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to automatically close brackets and quotes.
    /// </summary>
    public bool AutoCloseBrackets
    {
        get => GetValue(AutoCloseBracketsProperty);
        set => SetValue(AutoCloseBracketsProperty, value);
    }

    /// <summary>
    /// Gets or sets the line number foreground color (typically set by theme).
    /// </summary>
    public IBrush LineNumberForeground
    {
        get => GetValue(LineNumberForegroundProperty);
        set => SetValue(LineNumberForegroundProperty, value);
    }

    /// <summary>
    /// Gets the collection of linting hints to display.
    /// </summary>
    public ObservableCollection<LintingHint> LintingHints
    {
        get => _lintingHints;
        private set => SetAndRaise(LintingHintsProperty, ref _lintingHints, value);
    }

    /// <summary>
    /// Gets or sets the caret index.
    /// </summary>
    public int CaretIndex
    {
        get => _textBox?.CaretIndex ?? 0;
        set
        {
            if (_textBox != null)
                _textBox.CaretIndex = value;
        }
    }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    public int SelectionStart
    {
        get => _textBox?.SelectionStart ?? 0;
        set
        {
            if (_textBox != null)
                _textBox.SelectionStart = value;
        }
    }

    /// <summary>
    /// Gets or sets the selection end.
    /// </summary>
    public int SelectionEnd
    {
        get => _textBox?.SelectionEnd ?? 0;
        set
        {
            if (_textBox != null)
                _textBox.SelectionEnd = value;
        }
    }

    /// <summary>
    /// Occurs when the text changes.
    /// </summary>
    public event EventHandler<SyntaxTextChangedEventArgs>? TextChanged;

    static SyntaxHighlightingTextBox()
    {
        TextProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, e) => x.OnTextChanged(e));
        LanguageProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.ScheduleHighlightUpdate());
        SyntaxThemeProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.OnThemeChanged());
        ShowLineNumbersProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.UpdateLineNumbers());

        // Set default font family for code editing
        FontFamilyProperty.OverrideDefaultValue<SyntaxHighlightingTextBox>(
            new FontFamily("Consolas, Menlo, Monaco, 'Courier New', monospace"));
        FontSizeProperty.OverrideDefaultValue<SyntaxHighlightingTextBox>(14.0);
    }

    public SyntaxHighlightingTextBox()
    {
        SyntaxTheme = BuiltInThemes.Default;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _highlightedTextBlock = e.NameScope.Find<SelectableTextBlock>("PART_HighlightedText");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        _lineNumbers = e.NameScope.Find<ItemsControl>("PART_LineNumbers");
        _lintingOverlay = e.NameScope.Find<Canvas>("PART_LintingOverlay");
        _lineNumberSeparator = e.NameScope.Find<Border>("PART_LineNumberSeparator");

        if (_textBox != null)
        {
            _textBox.TextChanging += OnInnerTextBoxTextChanging;
            // Subscribe to actual text changes (after the change is applied)
            _textBox.PropertyChanged += OnInnerTextBoxPropertyChanged;
            // Use tunneling (Preview) to intercept before TextBox handles the event
            _textBox.AddHandler(KeyDownEvent, OnInnerTextBoxKeyDown, Avalonia.Interactivity.RoutingStrategies.Tunnel);
            _textBox.AddHandler(TextInputEvent, OnInnerTextBoxTextInput, Avalonia.Interactivity.RoutingStrategies.Tunnel);
        }

        _updateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(UpdateDelay)
        };
        _updateTimer.Tick += (_, _) =>
        {
            _updateTimer.Stop();
            UpdateHighlighting();
        };

        ApplyThemeColors();
        UpdateHighlighting();
        UpdateLineNumbers();
    }

    private void OnThemeChanged()
    {
        ApplyThemeColors();
        _lastHighlightedText = string.Empty; // Force re-render with new theme colors
        ScheduleHighlightUpdate();
    }

    private void ApplyThemeColors()
    {
        var theme = SyntaxTheme ?? BuiltInThemes.Default;

        // Apply background
        Background = theme.DefaultBackground;

        // Apply line number foreground color
        LineNumberForeground = theme.LineNumberForeground;

        // Apply caret color
        if (_textBox != null)
        {
            _textBox.CaretBrush = theme.CaretBrush;
        }

        // Apply line number separator color
        if (_lineNumberSeparator != null)
        {
            _lineNumberSeparator.Background = theme.LineNumberSeparator;
        }
    }

    private void SyncText()
    {
        if (_textBox != null && !_isUpdating)
        {
            _isUpdating = true;
            Text = _textBox.Text ?? string.Empty;
            _isUpdating = false;
        }
    }

    private void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"OnTextChanged: _textBox={_textBox != null}, _isUpdating={_isUpdating}, newLen={((e.NewValue as string) ?? "").Length}");
        if (_textBox != null && !_isUpdating)
        {
            _isUpdating = true;
            _textBox.Text = Text;
            System.Diagnostics.Debug.WriteLine($"OnTextChanged: Set _textBox.Text, actual={_textBox.Text?.Length}");
            _isUpdating = false;
        }

        ScheduleHighlightUpdate();
        TextChanged?.Invoke(this, new SyntaxTextChangedEventArgs(e.NewValue as string ?? string.Empty));
    }

    private void OnInnerTextBoxTextChanging(object? sender, TextChangingEventArgs e)
    {
        // Note: TextChanging fires BEFORE the text changes, so we don't sync here
        // The actual sync happens in OnInnerTextBoxTextChanged
    }

    private void OnInnerTextBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        // Only react to Text property changes
        if (e.Property == TextBox.TextProperty)
        {
            // This fires AFTER the text has actually changed
            SyncText();
            ScheduleHighlightUpdate();
        }
    }

    private void OnInnerTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (_textBox == null || IsReadOnly)
            return;

        if (e.Key == Key.Enter && AcceptsReturn && AutoIndent)
        {
            e.Handled = true;
            HandleEnterKey();
        }
        else if (e.Key == Key.Tab && AcceptsTab && AutoIndent)
        {
            e.Handled = true;
            HandleTabKey(e.KeyModifiers.HasFlag(KeyModifiers.Shift));
        }
        else if (e.Key == Key.Back && AutoCloseBrackets)
        {
            // Check if we should delete a bracket pair
            if (HandleBackspaceForBracketPair())
            {
                e.Handled = true;
            }
        }
        else if (AutoCloseBrackets)
        {
            // Handle bracket keys in KeyDown to intercept before character insertion
            var bracketChar = GetBracketCharFromKey(e.Key, e.KeyModifiers);
            if (bracketChar.HasValue && BracketPairs.TryGetValue(bracketChar.Value, out var closingChar))
            {
                // For quotes typed on their own (not as part of opening bracket)
                if (bracketChar.Value == closingChar)
                {
                    if (HandleQuoteKey(bracketChar.Value))
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    // Opening bracket - auto close
                    e.Handled = true;
                    HandleAutoCloseBracket(bracketChar.Value, closingChar);
                }
            }
            else if (bracketChar.HasValue && ClosingBrackets.Contains(bracketChar.Value))
            {
                // Closing bracket - check for skip-over
                if (HandleClosingBracketKey(bracketChar.Value))
                {
                    e.Handled = true;
                }
            }
        }
    }

    private static char? GetBracketCharFromKey(Key key, KeyModifiers modifiers)
    {
        var shift = modifiers.HasFlag(KeyModifiers.Shift);
        return key switch
        {
            Key.D9 when shift => '(',
            Key.D0 when shift => ')',
            Key.OemOpenBrackets when !shift => '[',
            Key.OemOpenBrackets when shift => '{',
            Key.OemCloseBrackets when !shift => ']',
            Key.OemCloseBrackets when shift => '}',
            Key.OemQuotes when !shift => '\'',
            Key.OemQuotes when shift => '"',
            Key.Oem3 when !shift => '`', // Backtick
            _ => null
        };
    }

    private bool HandleQuoteKey(char quote)
    {
        if (_textBox == null) return false;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Skip over if next char is the same quote
        if (caretIndex < text.Length && text[caretIndex] == quote)
        {
            _textBox.CaretIndex = caretIndex + 1;
            return true;
        }

        // Don't auto-close apostrophe if inside a word
        if (quote == '\'' && caretIndex > 0)
        {
            var prevChar = text[caretIndex - 1];
            if (char.IsLetterOrDigit(prevChar))
            {
                return false; // Let it insert normally
            }
        }

        // Auto-close the quote
        HandleAutoCloseBracket(quote, quote);
        return true;
    }

    private bool HandleClosingBracketKey(char closingBracket)
    {
        if (_textBox == null) return false;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Skip over if next char is the same closing bracket
        if (caretIndex < text.Length && text[caretIndex] == closingBracket)
        {
            _textBox.CaretIndex = caretIndex + 1;
            return true;
        }

        // Not skipping - let it insert, then handle auto-dedent
        if (AutoIndent && closingBracket is '}' or ']' or ')')
        {
            // Insert the bracket manually and handle dedent
            InsertClosingBracketWithDedent(closingBracket);
            return true;
        }

        return false;
    }

    private void InsertClosingBracketWithDedent(char bracket)
    {
        if (_textBox == null) return;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Find the start of the current line
        var lineStart = text.LastIndexOf('\n', Math.Max(0, caretIndex - 1)) + 1;
        var textBeforeCaret = text.Substring(lineStart, caretIndex - lineStart);

        // Check if the line only contains whitespace before the cursor
        if (string.IsNullOrWhiteSpace(textBeforeCaret))
        {
            // Find matching opening bracket's indentation
            var matchingIndent = FindMatchingBracketIndentForInsertion(text, caretIndex, bracket);
            if (matchingIndent != null && textBeforeCaret != matchingIndent)
            {
                // Replace whitespace with correct indentation and add bracket
                _isUpdating = true;
                var newText = text.Remove(lineStart, textBeforeCaret.Length).Insert(lineStart, matchingIndent + bracket);
                _textBox.Text = newText;
                _textBox.CaretIndex = lineStart + matchingIndent.Length + 1;
                _isUpdating = false;

                SyncText();
                ScheduleHighlightUpdate();
                return;
            }
        }

        // Default: just insert the bracket
        _isUpdating = true;
        _textBox.Text = text.Insert(caretIndex, bracket.ToString());
        _textBox.CaretIndex = caretIndex + 1;
        _isUpdating = false;

        SyncText();
        ScheduleHighlightUpdate();
    }

    private string? FindMatchingBracketIndentForInsertion(string text, int insertPosition, char closingBracket)
    {
        var openingBracket = closingBracket switch
        {
            '}' => '{',
            ']' => '[',
            ')' => '(',
            _ => '\0'
        };

        if (openingBracket == '\0') return null;

        var depth = 1;
        for (var i = insertPosition - 1; i >= 0; i--)
        {
            var c = text[i];
            if (c == closingBracket) depth++;
            else if (c == openingBracket)
            {
                depth--;
                if (depth == 0)
                {
                    // Found matching bracket, get its line's indentation
                    var lineStart = text.LastIndexOf('\n', i) + 1;
                    var lineContent = text.Substring(lineStart, i - lineStart);
                    return GetLeadingWhitespace(lineContent);
                }
            }
        }

        return null;
    }

    private bool HandleBackspaceForBracketPair()
    {
        if (_textBox == null) return false;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Need at least one char before and after caret
        if (caretIndex < 1 || caretIndex >= text.Length)
            return false;

        var charBefore = text[caretIndex - 1];
        var charAfter = text[caretIndex];

        // Check if we're between a matching bracket pair
        if (BracketPairs.TryGetValue(charBefore, out var expectedClose) && charAfter == expectedClose)
        {
            // Delete both characters
            _isUpdating = true;
            _textBox.Text = text.Remove(caretIndex - 1, 2);
            _textBox.CaretIndex = caretIndex - 1;
            _isUpdating = false;

            SyncText();
            ScheduleHighlightUpdate();
            return true;
        }

        return false;
    }

    private void OnInnerTextBoxTextInput(object? sender, TextInputEventArgs e)
    {
        if (_textBox == null || IsReadOnly || string.IsNullOrEmpty(e.Text))
            return;

        var inputChar = e.Text[0];

        // Handle auto-close brackets (fallback if KeyDown didn't catch it)
        if (AutoCloseBrackets && BracketPairs.TryGetValue(inputChar, out var closingChar))
        {
            var text = _textBox.Text ?? string.Empty;
            var caretIndex = _textBox.CaretIndex;

            // For quotes, check if we should skip over
            if (inputChar == closingChar)
            {
                if (caretIndex < text.Length && text[caretIndex] == inputChar)
                {
                    e.Handled = true;
                    _textBox.CaretIndex = caretIndex + 1;
                    return;
                }

                // Don't auto-close apostrophe inside words
                if (inputChar == '\'' && caretIndex > 0 && char.IsLetterOrDigit(text[caretIndex - 1]))
                {
                    return;
                }
            }

            // Auto-close
            e.Handled = true;
            HandleAutoCloseBracket(inputChar, closingChar);
            return;
        }

        // Handle skip-over for closing brackets
        if (AutoCloseBrackets && ClosingBrackets.Contains(inputChar))
        {
            var text = _textBox.Text ?? string.Empty;
            var caretIndex = _textBox.CaretIndex;

            if (caretIndex < text.Length && text[caretIndex] == inputChar)
            {
                e.Handled = true;
                _textBox.CaretIndex = caretIndex + 1;
                return;
            }

            // Handle auto-dedent for closing braces
            if (AutoIndent)
            {
                e.Handled = true;
                InsertClosingBracketWithDedent(inputChar);
                return;
            }
        }
    }

    private void HandleAutoCloseBracket(char openChar, char closeChar)
    {
        if (_textBox == null) return;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Insert both brackets
        var insertText = $"{openChar}{closeChar}";

        _isUpdating = true;
        _textBox.Text = text.Insert(caretIndex, insertText);
        _textBox.CaretIndex = caretIndex + 1; // Position between the brackets
        _isUpdating = false;

        SyncText();
        ScheduleHighlightUpdate();
    }

    private void HandleEnterKey()
    {
        if (_textBox == null) return;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;

        // Get the current line
        var lineStart = text.LastIndexOf('\n', Math.Max(0, caretIndex - 1)) + 1;
        var currentLine = text.Substring(lineStart, caretIndex - lineStart);

        // Get the leading whitespace from the current line
        var leadingWhitespace = GetLeadingWhitespace(currentLine);

        // Check if we need to increase indent (line ends with opening bracket)
        var trimmedLine = currentLine.TrimEnd();
        var shouldIncreaseIndent = trimmedLine.EndsWith('{') ||
                                   trimmedLine.EndsWith('[') ||
                                   trimmedLine.EndsWith('(') ||
                                   trimmedLine.EndsWith(':'); // For Python, etc.

        // Build the new line content
        var indent = GetIndentString();
        var newLineContent = "\n" + leadingWhitespace;
        if (shouldIncreaseIndent)
        {
            newLineContent += indent;
        }

        // Check if we're between brackets (e.g., {|}) and need to add extra line
        var addExtraLine = false;
        if (caretIndex < text.Length)
        {
            var nextChar = text[caretIndex];
            if (shouldIncreaseIndent && IsClosingBracket(nextChar) && IsMatchingBracket(trimmedLine[^1], nextChar))
            {
                addExtraLine = true;
            }
        }

        // Insert the new line
        _isUpdating = true;
        var insertText = addExtraLine ? newLineContent + "\n" + leadingWhitespace : newLineContent;
        var newCaretPos = caretIndex + newLineContent.Length;

        _textBox.Text = text.Insert(caretIndex, insertText);
        _textBox.CaretIndex = newCaretPos;
        _isUpdating = false;

        SyncText();
        ScheduleHighlightUpdate();
    }

    private void HandleTabKey(bool isShiftPressed)
    {
        if (_textBox == null) return;

        var text = _textBox.Text ?? string.Empty;
        var caretIndex = _textBox.CaretIndex;
        var selectionStart = _textBox.SelectionStart;
        var selectionEnd = _textBox.SelectionEnd;

        // If there's a selection spanning multiple lines, indent/dedent all selected lines
        if (selectionStart != selectionEnd)
        {
            HandleBlockIndent(isShiftPressed);
            return;
        }

        if (isShiftPressed)
        {
            // Dedent current line
            var lineStart = text.LastIndexOf('\n', Math.Max(0, caretIndex - 1)) + 1;
            var lineContent = caretIndex > lineStart ? text.Substring(lineStart, caretIndex - lineStart) : "";
            var leadingWs = GetLeadingWhitespace(lineContent);

            if (leadingWs.Length > 0)
            {
                var removeCount = Math.Min(IndentSize, leadingWs.Length);
                // Check if leading whitespace is tabs
                if (leadingWs.StartsWith('\t'))
                {
                    removeCount = 1;
                }

                _isUpdating = true;
                _textBox.Text = text.Remove(lineStart, removeCount);
                _textBox.CaretIndex = Math.Max(lineStart, caretIndex - removeCount);
                _isUpdating = false;

                SyncText();
                ScheduleHighlightUpdate();
            }
        }
        else
        {
            // Insert tab/spaces at caret
            var indent = GetIndentString();
            _isUpdating = true;
            _textBox.Text = text.Insert(caretIndex, indent);
            _textBox.CaretIndex = caretIndex + indent.Length;
            _isUpdating = false;

            SyncText();
            ScheduleHighlightUpdate();
        }
    }

    private void HandleBlockIndent(bool dedent)
    {
        if (_textBox == null) return;

        var text = _textBox.Text ?? string.Empty;
        var selStart = Math.Min(_textBox.SelectionStart, _textBox.SelectionEnd);
        var selEnd = Math.Max(_textBox.SelectionStart, _textBox.SelectionEnd);

        // Find line boundaries
        var firstLineStart = text.LastIndexOf('\n', Math.Max(0, selStart - 1)) + 1;
        var lastLineEnd = text.IndexOf('\n', selEnd);
        if (lastLineEnd == -1) lastLineEnd = text.Length;

        var selectedText = text.Substring(firstLineStart, lastLineEnd - firstLineStart);
        var lines = selectedText.Split('\n');
        var indent = GetIndentString();
        var modifiedLines = new List<string>();
        var totalChange = 0;

        foreach (var line in lines)
        {
            if (dedent)
            {
                var ws = GetLeadingWhitespace(line);
                if (ws.Length > 0)
                {
                    var removeCount = ws.StartsWith('\t') ? 1 : Math.Min(IndentSize, ws.Length);
                    modifiedLines.Add(line.Substring(removeCount));
                    totalChange -= removeCount;
                }
                else
                {
                    modifiedLines.Add(line);
                }
            }
            else
            {
                modifiedLines.Add(indent + line);
                totalChange += indent.Length;
            }
        }

        var newText = string.Join("\n", modifiedLines);

        _isUpdating = true;
        _textBox.Text = text.Substring(0, firstLineStart) + newText + text.Substring(lastLineEnd);
        _textBox.SelectionStart = firstLineStart;
        _textBox.SelectionEnd = firstLineStart + newText.Length;
        _isUpdating = false;

        SyncText();
        ScheduleHighlightUpdate();
    }

    private string GetIndentString()
    {
        return UseSpacesForIndent ? new string(' ', IndentSize) : "\t";
    }

    private static string GetLeadingWhitespace(string line)
    {
        var count = 0;
        foreach (var c in line)
        {
            if (c == ' ' || c == '\t')
                count++;
            else
                break;
        }
        return line.Substring(0, count);
    }

    private static bool IsClosingBracket(char c) => c is '}' or ']' or ')';

    private static bool IsMatchingBracket(char open, char close)
    {
        return (open == '{' && close == '}') ||
               (open == '[' && close == ']') ||
               (open == '(' && close == ')');
    }

    private void ScheduleHighlightUpdate()
    {
        _updateTimer?.Stop();
        _updateTimer?.Start();
    }

    private void UpdateHighlighting()
    {
        if (_highlightedTextBlock == null)
            return;

        var text = Text ?? string.Empty;
        if (text == _lastHighlightedText && _highlightedTextBlock.Inlines?.Count > 0)
            return;

        _lastHighlightedText = text;

        var inlines = new InlineCollection();
        var tokenizer = TokenizerFactory.GetTokenizer(Language);
        var theme = SyntaxTheme ?? BuiltInThemes.Default;

        try
        {
            if (tokenizer == null || string.IsNullOrEmpty(text))
            {
                inlines.Add(new Run(text) { Foreground = theme.DefaultForeground });
            }
            else
            {
                var tokens = tokenizer.Tokenize(text).ToList();
                var lastEnd = 0;

                foreach (var token in tokens)
                {
                    // Add any text between tokens as plain text
                    if (token.StartIndex > lastEnd)
                    {
                        var gapText = text.Substring(lastEnd, token.StartIndex - lastEnd);
                        inlines.Add(new Run(gapText) { Foreground = theme.DefaultForeground });
                    }

                    var style = theme.GetStyle(token.Type);
                    var tokenText = token.GetText(text);

                    var run = new Run(tokenText)
                    {
                        Foreground = style.Foreground ?? theme.DefaultForeground,
                        Background = style.Background,
                        FontWeight = style.FontWeight,
                        FontStyle = style.FontStyle
                    };

                    if (style.IsUnderline)
                    {
                        run.TextDecorations = TextDecorations.Underline;
                    }

                    inlines.Add(run);
                    lastEnd = token.EndIndex;
                }

                // Add any remaining text
                if (lastEnd < text.Length)
                {
                    inlines.Add(new Run(text.Substring(lastEnd)) { Foreground = theme.DefaultForeground });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR in UpdateHighlighting: {ex.Message}");
            // Fallback to plain text on error
            inlines.Clear();
            inlines.Add(new Run(text) { Foreground = theme.DefaultForeground });
        }

        _highlightedTextBlock.Inlines = inlines;
        UpdateLineNumbers();
    }

    private void UpdateLineNumbers()
    {
        if (_lineNumbers == null)
            return;

        if (!ShowLineNumbers)
        {
            _lineNumbers.ItemsSource = null;
            return;
        }

        var text = Text ?? string.Empty;
        var lineCount = text.Split('\n').Length;
        if (lineCount == 0) lineCount = 1;

        var lines = Enumerable.Range(1, lineCount).ToList();
        _lineNumbers.ItemsSource = lines;
    }

    /// <summary>
    /// Adds a linting hint to display.
    /// </summary>
    /// <param name="hint">The linting hint to add.</param>
    public void AddLintingHint(LintingHint hint)
    {
        LintingHints.Add(hint);
    }

    /// <summary>
    /// Removes a linting hint.
    /// </summary>
    /// <param name="hint">The linting hint to remove.</param>
    public void RemoveLintingHint(LintingHint hint)
    {
        LintingHints.Remove(hint);
    }

    /// <summary>
    /// Clears all linting hints.
    /// </summary>
    public void ClearLintingHints()
    {
        LintingHints.Clear();
    }

    /// <summary>
    /// Selects all text.
    /// </summary>
    public void SelectAll()
    {
        _textBox?.SelectAll();
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
        if (_textBox != null)
        {
            _textBox.SelectionStart = _textBox.CaretIndex;
            _textBox.SelectionEnd = _textBox.CaretIndex;
        }
    }

    /// <summary>
    /// Copies the selected text to the clipboard.
    /// </summary>
    public void Copy()
    {
        _textBox?.Copy();
    }

    /// <summary>
    /// Cuts the selected text to the clipboard.
    /// </summary>
    public void Cut()
    {
        _textBox?.Cut();
    }

    /// <summary>
    /// Pastes text from the clipboard.
    /// </summary>
    public void Paste()
    {
        _textBox?.Paste();
    }

    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo()
    {
        _textBox?.Undo();
    }

    /// <summary>
    /// Redoes the last undone action.
    /// </summary>
    public void Redo()
    {
        _textBox?.Redo();
    }

    /// <summary>
    /// Scrolls to the specified line.
    /// </summary>
    /// <param name="line">The line number (1-based).</param>
    public void ScrollToLine(int line)
    {
        if (_scrollViewer == null || string.IsNullOrEmpty(Text))
            return;

        var lines = Text.Split('\n');
        if (line < 1 || line > lines.Length)
            return;

        var lineHeight = FontSize * 1.2;
        var offset = (line - 1) * lineHeight;
        _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, offset);
    }

    /// <summary>
    /// Gets the line number at the specified character index.
    /// </summary>
    /// <param name="charIndex">The character index.</param>
    /// <returns>The line number (1-based).</returns>
    public int GetLineFromCharIndex(int charIndex)
    {
        if (string.IsNullOrEmpty(Text) || charIndex < 0)
            return 1;

        var text = Text.Substring(0, Math.Min(charIndex, Text.Length));
        return text.Count(c => c == '\n') + 1;
    }

    /// <summary>
    /// Gets the character index at the start of the specified line.
    /// </summary>
    /// <param name="line">The line number (1-based).</param>
    /// <returns>The character index.</returns>
    public int GetCharIndexFromLine(int line)
    {
        if (string.IsNullOrEmpty(Text) || line < 1)
            return 0;

        var currentLine = 1;
        for (var i = 0; i < Text.Length; i++)
        {
            if (currentLine == line)
                return i;
            if (Text[i] == '\n')
                currentLine++;
        }

        return Text.Length;
    }

    /// <summary>
    /// Formats the document with proper indentation based on the current language.
    /// </summary>
    public void FormatDocument()
    {
        if (string.IsNullOrEmpty(Text))
            return;

        var formatted = BasicCodeFormatter.Format(Text, Language, IndentSize, UseSpacesForIndent);

        if (formatted != Text)
        {
            // Preserve caret position approximately
            var caretLine = GetLineFromCharIndex(CaretIndex);

            Text = formatted;

            // Try to restore caret to same line
            CaretIndex = GetCharIndexFromLine(caretLine);
        }
    }
}

/// <summary>
/// Event arguments for syntax text changed events.
/// </summary>
public class SyntaxTextChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new text value.
    /// </summary>
    public string NewText { get; }

    public SyntaxTextChangedEventArgs(string newText)
    {
        NewText = newText;
    }
}
