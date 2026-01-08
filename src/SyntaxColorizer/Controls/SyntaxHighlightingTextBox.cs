using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using SyntaxColorizer.Linting;
using SyntaxColorizer.Formatting;
using SyntaxColorizer.Themes;
using SyntaxColorizer.Tokenization;

namespace SyntaxColorizer.Controls;

/// <summary>
/// A text box control with syntax highlighting support for multiple programming languages.
/// Built on top of AvaloniaEdit for reliable text editing.
/// </summary>
public class SyntaxHighlightingTextBox : TemplatedControl
{
    private TextEditor? _editor;
    private SyntaxHighlightingTransformer? _highlightingTransformer;
    private ItemsControl? _lineNumbers;
    private Border? _lineNumberSeparator;

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
    private bool _isUpdating;

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
        get => _editor?.CaretOffset ?? 0;
        set
        {
            if (_editor != null)
                _editor.CaretOffset = value;
        }
    }

    /// <summary>
    /// Gets or sets the selection start.
    /// </summary>
    public int SelectionStart
    {
        get => _editor?.SelectionStart ?? 0;
        set
        {
            if (_editor != null)
                _editor.SelectionStart = value;
        }
    }

    /// <summary>
    /// Gets or sets the selection end.
    /// </summary>
    public int SelectionEnd
    {
        get => (_editor?.SelectionStart ?? 0) + (_editor?.SelectionLength ?? 0);
        set
        {
            if (_editor != null)
                _editor.SelectionLength = value - _editor.SelectionStart;
        }
    }

    /// <summary>
    /// Occurs when the text changes.
    /// </summary>
    public event EventHandler<SyntaxTextChangedEventArgs>? TextChanged;

    static SyntaxHighlightingTextBox()
    {
        TextProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, e) => x.OnTextPropertyChanged(e));
        LanguageProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.OnLanguageChanged());
        SyntaxThemeProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.OnThemeChanged());
        ShowLineNumbersProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.OnShowLineNumbersChanged());
        IsReadOnlyProperty.Changed.AddClassHandler<SyntaxHighlightingTextBox>((x, _) => x.OnIsReadOnlyChanged());

        // Set default font family for code editing
        FontFamilyProperty.OverrideDefaultValue<SyntaxHighlightingTextBox>(
            new FontFamily("Consolas, Menlo, Monaco, 'Courier New', monospace"));
        FontSizeProperty.OverrideDefaultValue<SyntaxHighlightingTextBox>(14.0);
    }

    /// <summary>
    /// Initializes a new instance of the SyntaxHighlightingTextBox.
    /// </summary>
    public SyntaxHighlightingTextBox()
    {
        SyntaxTheme = BuiltInThemes.Default;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _editor = e.NameScope.Find<TextEditor>("PART_Editor");
        _lineNumbers = e.NameScope.Find<ItemsControl>("PART_LineNumbers");
        _lineNumberSeparator = e.NameScope.Find<Border>("PART_LineNumberSeparator");

        if (_editor != null)
        {
            // Configure editor
            _editor.FontFamily = FontFamily;
            _editor.FontSize = FontSize;
            _editor.ShowLineNumbers = ShowLineNumbers;
            _editor.IsReadOnly = IsReadOnly;
            _editor.WordWrap = false;
            _editor.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
            _editor.VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;

            // Set up indentation
            _editor.Options.IndentationSize = IndentSize;
            _editor.Options.ConvertTabsToSpaces = UseSpacesForIndent;

            // Set initial text
            if (!string.IsNullOrEmpty(Text))
            {
                _editor.Text = Text;
            }

            // Create and add the syntax highlighting transformer
            _highlightingTransformer = new SyntaxHighlightingTransformer
            {
                Language = Language,
                Theme = SyntaxTheme ?? BuiltInThemes.Default
            };
            _editor.TextArea.TextView.LineTransformers.Add(_highlightingTransformer);

            // Subscribe to text changes
            _editor.TextChanged += OnEditorTextChanged;

            // Apply theme colors
            ApplyThemeColors();
        }
    }

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (_editor == null || _isUpdating)
            return;

        _isUpdating = true;
        Text = _editor.Text;
        _isUpdating = false;

        // Invalidate highlighting cache
        _highlightingTransformer?.InvalidateCache();

        TextChanged?.Invoke(this, new SyntaxTextChangedEventArgs(_editor.Text));
    }

    private void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_editor == null || _isUpdating)
            return;

        _isUpdating = true;
        _editor.Text = Text ?? string.Empty;
        _isUpdating = false;

        // Invalidate highlighting cache
        _highlightingTransformer?.InvalidateCache();
    }

    private void OnLanguageChanged()
    {
        if (_highlightingTransformer != null)
        {
            _highlightingTransformer.Language = Language;
            _highlightingTransformer.InvalidateCache();
            _editor?.TextArea.TextView.Redraw();
        }
    }

    private void OnThemeChanged()
    {
        if (_highlightingTransformer != null)
        {
            _highlightingTransformer.Theme = SyntaxTheme ?? BuiltInThemes.Default;
        }
        ApplyThemeColors();
        _editor?.TextArea.TextView.Redraw();
    }

    private void OnShowLineNumbersChanged()
    {
        if (_editor != null)
        {
            _editor.ShowLineNumbers = ShowLineNumbers;
        }
    }

    private void OnIsReadOnlyChanged()
    {
        if (_editor != null)
        {
            _editor.IsReadOnly = IsReadOnly;
        }
    }

    private void ApplyThemeColors()
    {
        var theme = SyntaxTheme ?? BuiltInThemes.Default;

        // Apply background
        Background = theme.DefaultBackground;

        // Apply line number foreground color
        LineNumberForeground = theme.LineNumberForeground;

        if (_editor != null)
        {
            // Apply foreground and background to editor
            _editor.Foreground = theme.DefaultForeground;
            _editor.Background = theme.DefaultBackground;

            // Apply line number margin colors
            if (_editor.TextArea.LeftMargins.Count > 0)
            {
                foreach (var margin in _editor.TextArea.LeftMargins)
                {
                    if (margin is LineNumberMargin lineNumberMargin)
                    {
                        // Note: LineNumberMargin styling may need to be done via styles
                    }
                }
            }
        }

        // Apply line number separator color
        if (_lineNumberSeparator != null)
        {
            _lineNumberSeparator.Background = theme.LineNumberSeparator;
        }
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
        _editor?.SelectAll();
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
        if (_editor != null)
        {
            _editor.SelectionLength = 0;
        }
    }

    /// <summary>
    /// Copies the selected text to the clipboard.
    /// </summary>
    public void Copy()
    {
        _editor?.Copy();
    }

    /// <summary>
    /// Cuts the selected text to the clipboard.
    /// </summary>
    public void Cut()
    {
        _editor?.Cut();
    }

    /// <summary>
    /// Pastes text from the clipboard.
    /// </summary>
    public void Paste()
    {
        _editor?.Paste();
    }

    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo()
    {
        _editor?.Undo();
    }

    /// <summary>
    /// Redoes the last undone action.
    /// </summary>
    public void Redo()
    {
        _editor?.Redo();
    }

    /// <summary>
    /// Scrolls to the specified line.
    /// </summary>
    /// <param name="line">The line number (1-based).</param>
    public void ScrollToLine(int line)
    {
        _editor?.ScrollToLine(line);
    }

    /// <summary>
    /// Gets the line number at the specified character index.
    /// </summary>
    /// <param name="charIndex">The character index.</param>
    /// <returns>The line number (1-based).</returns>
    public int GetLineFromCharIndex(int charIndex)
    {
        if (_editor?.Document == null || charIndex < 0)
            return 1;

        var safeOffset = Math.Min(charIndex, _editor.Document.TextLength);
        return _editor.Document.GetLineByOffset(safeOffset).LineNumber;
    }

    /// <summary>
    /// Gets the character index at the start of the specified line.
    /// </summary>
    /// <param name="line">The line number (1-based).</param>
    /// <returns>The character index.</returns>
    public int GetCharIndexFromLine(int line)
    {
        if (_editor?.Document == null || line < 1)
            return 0;

        var safeLine = Math.Min(line, _editor.Document.LineCount);
        return _editor.Document.GetLineByNumber(safeLine).Offset;
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

    /// <summary>
    /// Initializes a new instance of the SyntaxTextChangedEventArgs class.
    /// </summary>
    public SyntaxTextChangedEventArgs(string newText)
    {
        NewText = newText;
    }
}
