using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SyntaxColorizer;
using SyntaxColorizer.Controls;
using SyntaxColorizer.Linting;
using SyntaxColorizer.Themes;

namespace SyntaxColorizer.Demo;

public partial class MainWindow : Window
{
    private SyntaxHighlightingTextBox? _editor;
    private ComboBox? _languageSelector;
    private ComboBox? _themeSelector;
    private CheckBox? _showLineNumbersCheckBox;
    private Button? _formatButton;
    private Button? _runLinterButton;
    private Button? _clearLintingButton;
    private TextBlock? _statusText;
    private TextBlock? _lineColumnText;
    private TextBlock? _charCountText;
    private TextBlock? _lintingCountText;

    public MainWindow()
    {
        InitializeComponent();

        _editor = this.FindControl<SyntaxHighlightingTextBox>("Editor");
        _languageSelector = this.FindControl<ComboBox>("LanguageSelector");
        _themeSelector = this.FindControl<ComboBox>("ThemeSelector");
        _showLineNumbersCheckBox = this.FindControl<CheckBox>("ShowLineNumbersCheckBox");
        _formatButton = this.FindControl<Button>("FormatButton");
        _runLinterButton = this.FindControl<Button>("RunLinterButton");
        _clearLintingButton = this.FindControl<Button>("ClearLintingButton");
        _statusText = this.FindControl<TextBlock>("StatusText");
        _lineColumnText = this.FindControl<TextBlock>("LineColumnText");
        _charCountText = this.FindControl<TextBlock>("CharCountText");
        _lintingCountText = this.FindControl<TextBlock>("LintingCountText");

        InitializeEditor();
        SetupEventHandlers();
    }

    private void InitializeEditor()
    {
        if (_editor == null) return;

        // Set default sample code
        _editor.Language = SyntaxLanguage.CSharp;
        _editor.SyntaxTheme = BuiltInThemes.VisualStudioLight;
        _editor.Text = GetSampleCode(SyntaxLanguage.CSharp);

        // Set C# as default in selector (index 3: None=0, Bash=1, C=2, C#=3)
        if (_languageSelector != null)
            _languageSelector.SelectedIndex = 3;
    }

    private void SetupEventHandlers()
    {
        if (_languageSelector != null)
            _languageSelector.SelectionChanged += OnLanguageChanged;

        if (_themeSelector != null)
            _themeSelector.SelectionChanged += OnThemeChanged;

        if (_showLineNumbersCheckBox != null)
            _showLineNumbersCheckBox.IsCheckedChanged += OnShowLineNumbersChanged;

        if (_formatButton != null)
            _formatButton.Click += OnFormatClicked;

        if (_runLinterButton != null)
            _runLinterButton.Click += OnRunLinterClicked;

        if (_clearLintingButton != null)
            _clearLintingButton.Click += OnClearLintingClicked;

        if (_editor != null)
            _editor.TextChanged += OnTextChanged;
    }

    private void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_languageSelector?.SelectedItem is not ComboBoxItem item) return;
        if (_editor == null) return;

        var languageTag = item.Tag?.ToString() ?? "None";
        var parseSuccess = Enum.TryParse<SyntaxLanguage>(languageTag, out var result);
        var language = parseSuccess ? result : SyntaxLanguage.None;

        System.Diagnostics.Debug.WriteLine($"DEBUG: Tag='{languageTag}', ParseSuccess={parseSuccess}, Language={language}");

        var sampleCode = GetSampleCode(language);
        _editor.Text = sampleCode;  // Set text BEFORE language
        _editor.Language = language;
        _editor.ClearLintingHints();

        // Check what's actually in the editor after setting
        var actualText = _editor.Text;
        UpdateStatus($"{language}: set={sampleCode?.Length}, actual={actualText?.Length}");
    }

    private void OnThemeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_themeSelector?.SelectedItem is not ComboBoxItem item) return;
        if (_editor == null) return;

        var themeTag = item.Tag?.ToString() ?? "VSLight";
        var theme = themeTag switch
        {
            "VSLight" => BuiltInThemes.VisualStudioLight,
            "VSDark" => BuiltInThemes.VisualStudioDark,
            "Monokai" => BuiltInThemes.Monokai,
            "GitHubLight" => BuiltInThemes.GitHubLight,
            "GitHubDark" => BuiltInThemes.GitHubDark,
            "SolarizedLight" => BuiltInThemes.SolarizedLight,
            "SolarizedDark" => BuiltInThemes.SolarizedDark,
            _ => BuiltInThemes.Default
        };

        _editor.SyntaxTheme = theme;

        // Update editor background based on theme
        _editor.Background = theme.DefaultBackground;

        UpdateStatus($"Theme changed to {item.Content}");
    }

    private void OnShowLineNumbersChanged(object? sender, RoutedEventArgs e)
    {
        if (_editor == null || _showLineNumbersCheckBox == null) return;
        _editor.ShowLineNumbers = _showLineNumbersCheckBox.IsChecked ?? true;
    }

    private void OnFormatClicked(object? sender, RoutedEventArgs e)
    {
        if (_editor == null) return;

        try
        {
            _editor.FormatDocument();
            UpdateStatus("Document formatted");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Format error: {ex.Message}");
        }
    }

    private void OnRunLinterClicked(object? sender, RoutedEventArgs e)
    {
        if (_editor == null) return;

        _editor.ClearLintingHints();

        var linter = LinterFactory.GetLinter(_editor.Language);
        if (linter == null)
        {
            UpdateStatus("No linter available for this language");
            return;
        }

        var hints = linter.Analyze(_editor.Text).ToList();
        foreach (var hint in hints)
        {
            _editor.AddLintingHint(hint);
        }

        UpdateLintingCount(hints.Count);
        UpdateStatus($"Linting complete: {hints.Count} hint(s) found");
    }

    private void OnClearLintingClicked(object? sender, RoutedEventArgs e)
    {
        _editor?.ClearLintingHints();
        UpdateLintingCount(0);
        UpdateStatus("Linting hints cleared");
    }

    private void OnTextChanged(object? sender, SyntaxTextChangedEventArgs e)
    {
        UpdateCharCount();
    }

    private void UpdateStatus(string message)
    {
        if (_statusText != null)
            _statusText.Text = message;
    }

    private void UpdateCharCount()
    {
        if (_charCountText == null || _editor == null) return;
        var count = _editor.Text?.Length ?? 0;
        _charCountText.Text = $"{count:N0} characters";
    }

    private void UpdateLintingCount(int count)
    {
        if (_lintingCountText != null)
            _lintingCountText.Text = $"{count} hint(s)";
    }

    private static string GetSampleCode(SyntaxLanguage language)
    {
        return language switch
        {
            SyntaxLanguage.CSharp => """
                using System;
                using System.Collections.Generic;

                namespace SyntaxColorizer.Demo
                {
                    /// <summary>
                    /// A sample class demonstrating C# syntax highlighting.
                    /// </summary>
                    public class Calculator
                    {
                        private readonly List<double> _history = new();

                        // TODO: Add more operations
                        public double Add(double a, double b)
                        {
                            var result = a + b;
                            _history.Add(result);
                            return result;
                        }

                        public async Task<double> CalculateAsync(string expression)
                        {
                            await Task.Delay(100);

                            // Parse and calculate
                            if (string.IsNullOrEmpty(expression))
                                throw new ArgumentException("Expression cannot be empty");

                            return 42.0; // Magic number for demo
                        }

                        public IEnumerable<double> GetHistory() => _history;
                    }
                }
                """,

            SyntaxLanguage.VisualBasic => """
                Imports System
                Imports System.Collections.Generic

                Namespace SyntaxColorizer.Demo
                    ''' <summary>
                    ''' A sample class demonstrating VB.NET syntax highlighting.
                    ''' </summary>
                    Public Class Calculator
                        Private ReadOnly _history As New List(Of Double)()

                        ' TODO: Add more operations
                        Public Function Add(a As Double, b As Double) As Double
                            Dim result As Double = a + b
                            _history.Add(result)
                            Return result
                        End Function

                        Public Async Function CalculateAsync(expression As String) As Task(Of Double)
                            Await Task.Delay(100)

                            If String.IsNullOrEmpty(expression) Then
                                Throw New ArgumentException("Expression cannot be empty")
                            End If

                            Return 42.0
                        End Function
                    End Class
                End Namespace
                """,

            SyntaxLanguage.MsSql or SyntaxLanguage.OracleSql => """
                -- Sample SQL query demonstrating syntax highlighting
                SELECT
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    d.DepartmentName,
                    COUNT(p.ProjectId) AS ProjectCount,
                    SUM(p.Budget) AS TotalBudget
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
                LEFT JOIN ProjectAssignments pa ON e.EmployeeId = pa.EmployeeId
                LEFT JOIN Projects p ON pa.ProjectId = p.ProjectId
                WHERE e.IsActive = 1
                    AND d.DepartmentName IN ('Engineering', 'Sales', 'Marketing')
                GROUP BY e.EmployeeId, e.FirstName, e.LastName, d.DepartmentName
                HAVING COUNT(p.ProjectId) > 0
                ORDER BY TotalBudget DESC;

                -- TODO: Add stored procedure
                CREATE PROCEDURE GetEmployeeDetails
                    @EmployeeId INT
                AS
                BEGIN
                    SELECT * FROM Employees WHERE EmployeeId = @EmployeeId;
                END
                """,

            SyntaxLanguage.Java => """
                package com.syntaxcolorizer.demo;

                import java.util.ArrayList;
                import java.util.List;

                /**
                 * A sample class demonstrating Java syntax highlighting.
                 */
                public class Calculator {
                    private final List<Double> history = new ArrayList<>();

                    // TODO: Add more operations
                    public double add(double a, double b) {
                        double result = a + b;
                        history.add(result);
                        return result;
                    }

                    public double calculate(String expression) throws IllegalArgumentException {
                        if (expression == null || expression.isEmpty()) {
                            throw new IllegalArgumentException("Expression cannot be empty");
                        }

                        // Parse and calculate
                        return 42.0; // Magic number for demo
                    }

                    @Override
                    public String toString() {
                        return "Calculator{history=" + history + "}";
                    }
                }
                """,

            SyntaxLanguage.JavaScript => """
                // Sample JavaScript code demonstrating syntax highlighting

                class Calculator {
                    constructor() {
                        this.history = [];
                    }

                    // TODO: Add more operations
                    add(a, b) {
                        const result = a + b;
                        this.history.push(result);
                        return result;
                    }

                    async calculate(expression) {
                        if (!expression) {
                            throw new Error('Expression cannot be empty');
                        }

                        // Simulate async operation
                        await new Promise(resolve => setTimeout(resolve, 100));

                        console.log(`Calculating: ${expression}`);
                        return 42; // Magic number for demo
                    }

                    getHistory() {
                        return [...this.history];
                    }
                }

                const calc = new Calculator();
                const sum = calc.add(10, 20);
                console.log(`Sum: ${sum}`);
                """,

            SyntaxLanguage.TypeScript => """
                // Sample TypeScript code demonstrating syntax highlighting

                interface ICalculator {
                    add(a: number, b: number): number;
                    calculate(expression: string): Promise<number>;
                }

                class Calculator implements ICalculator {
                    private history: number[] = [];

                    // TODO: Add more operations
                    public add(a: number, b: number): number {
                        const result: number = a + b;
                        this.history.push(result);
                        return result;
                    }

                    public async calculate(expression: string): Promise<number> {
                        if (!expression) {
                            throw new Error('Expression cannot be empty');
                        }

                        await new Promise<void>(resolve => setTimeout(resolve, 100));

                        console.log(`Calculating: ${expression}`);
                        return 42;
                    }

                    public getHistory(): readonly number[] {
                        return this.history;
                    }
                }

                type Operation = 'add' | 'subtract' | 'multiply' | 'divide';

                const calc = new Calculator();
                """,

            SyntaxLanguage.C => """
                #include <stdio.h>
                #include <stdlib.h>
                #include <string.h>

                #define MAX_HISTORY 100

                /* A simple calculator structure */
                typedef struct {
                    double history[MAX_HISTORY];
                    int count;
                } Calculator;

                // TODO: Add more operations
                double add(Calculator* calc, double a, double b) {
                    double result = a + b;

                    if (calc->count < MAX_HISTORY) {
                        calc->history[calc->count++] = result;
                    }

                    return result;
                }

                int main(int argc, char* argv[]) {
                    Calculator calc = {0};

                    double sum = add(&calc, 10.0, 20.0);
                    printf("Sum: %.2f\n", sum);

                    return 0;
                }
                """,

            SyntaxLanguage.Cpp => """
                #include <iostream>
                #include <vector>
                #include <string>
                #include <memory>

                /**
                 * A sample class demonstrating C++ syntax highlighting.
                 */
                class Calculator {
                private:
                    std::vector<double> history;

                public:
                    Calculator() = default;
                    ~Calculator() = default;

                    // TODO: Add more operations
                    double add(double a, double b) {
                        auto result = a + b;
                        history.push_back(result);
                        return result;
                    }

                    template<typename T>
                    T calculate(const std::string& expression) {
                        if (expression.empty()) {
                            throw std::invalid_argument("Expression cannot be empty");
                        }
                        return static_cast<T>(42.0);
                    }

                    const std::vector<double>& getHistory() const noexcept {
                        return history;
                    }
                };

                int main() {
                    auto calc = std::make_unique<Calculator>();
                    std::cout << "Sum: " << calc->add(10, 20) << std::endl;
                    return 0;
                }
                """,

            SyntaxLanguage.Php => """
                <?php

                namespace SyntaxColorizer\Demo;

                /**
                 * A sample class demonstrating PHP syntax highlighting.
                 */
                class Calculator
                {
                    private array $history = [];

                    // TODO: Add more operations
                    public function add(float $a, float $b): float
                    {
                        $result = $a + $b;
                        $this->history[] = $result;
                        return $result;
                    }

                    public function calculate(string $expression): float
                    {
                        if (empty($expression)) {
                            throw new \InvalidArgumentException('Expression cannot be empty');
                        }

                        // Parse and calculate
                        return 42.0; // Magic number for demo
                    }

                    #[Pure]
                    public function getHistory(): array
                    {
                        return $this->history;
                    }
                }

                $calc = new Calculator();
                $sum = $calc->add(10, 20);
                echo "Sum: {$sum}\n";
                """,

            SyntaxLanguage.Python => @"# A sample module demonstrating Python syntax highlighting.

from typing import List, Optional
from dataclasses import dataclass
import asyncio


@dataclass
class CalculationResult:
    value: float
    operation: str


class Calculator:
    ''A simple calculator class.''

    def __init__(self):
        self._history: List[float] = []

    # TODO: Add more operations
    def add(self, a: float, b: float) -> float:
        result = a + b
        self._history.append(result)
        return result

    async def calculate(self, expression: str) -> float:
        if not expression:
            raise ValueError('Expression cannot be empty')

        await asyncio.sleep(0.1)
        return 42.0  # Magic number for demo

    @property
    def history(self) -> List[float]:
        return self._history.copy()


if __name__ == '__main__':
    calc = Calculator()
    total = calc.add(10, 20)
    print(f'Sum: {total}')
",

            SyntaxLanguage.Rust => """
                //! A sample module demonstrating Rust syntax highlighting.

                use std::collections::HashMap;

                /// A simple calculator struct.
                #[derive(Debug, Default)]
                pub struct Calculator {
                    history: Vec<f64>,
                }

                impl Calculator {
                    /// Creates a new calculator.
                    pub fn new() -> Self {
                        Self::default()
                    }

                    // TODO: Add more operations
                    pub fn add(&mut self, a: f64, b: f64) -> f64 {
                        let result = a + b;
                        self.history.push(result);
                        result
                    }

                    pub async fn calculate(&self, expression: &str) -> Result<f64, &'static str> {
                        if expression.is_empty() {
                            return Err("Expression cannot be empty");
                        }

                        // Parse and calculate
                        Ok(42.0) // Magic number for demo
                    }

                    pub fn history(&self) -> &[f64] {
                        &self.history
                    }
                }

                fn main() {
                    let mut calc = Calculator::new();
                    let sum = calc.add(10.0, 20.0);
                    println!("Sum: {}", sum);
                }
                """,

            SyntaxLanguage.Html => """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Sample HTML Page</title>
                    <link rel="stylesheet" href="styles.css">
                </head>
                <body>
                    <!-- Main content area -->
                    <header class="site-header">
                        <nav id="main-nav">
                            <ul>
                                <li><a href="#home">Home</a></li>
                                <li><a href="#about">About</a></li>
                                <li><a href="#contact">Contact</a></li>
                            </ul>
                        </nav>
                    </header>

                    <main>
                        <article>
                            <h1>Welcome to SyntaxColorizer</h1>
                            <p>This is a <strong>sample</strong> HTML document demonstrating syntax highlighting.</p>
                            <img src="image.png" alt="Sample Image" width="300" height="200">
                        </article>

                        <section id="features">
                            <h2>Features</h2>
                            <ul>
                                <li>Multi-language support</li>
                                <li>Customizable themes</li>
                                <li>Code formatting</li>
                            </ul>
                        </section>
                    </main>

                    <footer>
                        <p>&copy; 2024 SyntaxColorizer</p>
                    </footer>

                    <script src="app.js"></script>
                </body>
                </html>
                """,

            SyntaxLanguage.Css => """
                /* Main stylesheet for SyntaxColorizer demo */

                :root {
                    --primary-color: #0078d7;
                    --secondary-color: #6c757d;
                    --font-size-base: 16px;
                }

                /* Reset and base styles */
                *, *::before, *::after {
                    box-sizing: border-box;
                    margin: 0;
                    padding: 0;
                }

                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    font-size: var(--font-size-base);
                    line-height: 1.6;
                    color: #333;
                    background-color: #f5f5f5;
                }

                /* Header styles */
                .site-header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    padding: 1rem 2rem;
                    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                }

                #main-nav ul {
                    display: flex;
                    list-style: none;
                    gap: 2rem;
                }

                #main-nav a {
                    color: #fff;
                    text-decoration: none;
                    font-weight: 500;
                    transition: opacity 0.3s ease;
                }

                #main-nav a:hover {
                    opacity: 0.8;
                }

                /* Button styles */
                .btn {
                    display: inline-block;
                    padding: 0.5em 1.5em;
                    border: none;
                    border-radius: 4px;
                    cursor: pointer;
                }

                .btn-primary {
                    background-color: var(--primary-color);
                    color: #fff;
                }

                /* Media queries */
                @media screen and (max-width: 768px) {
                    .site-header {
                        padding: 0.5rem 1rem;
                    }

                    #main-nav ul {
                        flex-direction: column;
                        gap: 1rem;
                    }
                }

                /* Keyframe animation */
                @keyframes fadeIn {
                    from { opacity: 0; }
                    to { opacity: 1; }
                }
                """,

            SyntaxLanguage.Markdown => """
                # SyntaxColorizer Documentation

                Welcome to **SyntaxColorizer**, a powerful syntax highlighting library for Avalonia applications.

                ## Features

                - Multi-language support
                - Customizable themes
                - *Code formatting*
                - Line numbers
                - Linting integration

                ### Supported Languages

                1. C#
                2. Visual Basic
                3. JavaScript
                4. TypeScript
                5. Python
                6. And many more!

                ## Getting Started

                Install the package via NuGet:

                ```bash
                dotnet add package SyntaxColorizer
                ```

                Then add the control to your XAML:

                ```xml
                <controls:SyntaxHighlightingTextBox
                    Language="CSharp"
                    ShowLineNumbers="True" />
                ```

                ## Code Example

                Here's a simple C# example:

                ```csharp
                var editor = new SyntaxHighlightingTextBox();
                editor.Language = SyntaxLanguage.CSharp;
                editor.Text = "Console.WriteLine(\"Hello!\");";
                ```

                ## Links

                - [GitHub Repository](https://github.com/example/syntaxcolorizer)
                - [Documentation](https://docs.example.com)
                - [Issue Tracker](https://github.com/example/syntaxcolorizer/issues)

                ---

                > **Note:** This library is actively maintained and welcomes contributions!

                ![Logo](./images/logo.png)

                | Feature | Status |
                |---------|--------|
                | Highlighting | ✅ |
                | Formatting | ✅ |
                | Linting | ✅ |
                """,

            SyntaxLanguage.Json => """
                {
                    "name": "SyntaxColorizer",
                    "version": "1.0.0",
                    "description": "A syntax highlighting library for Avalonia",
                    "author": {
                        "name": "Developer",
                        "email": "dev@example.com"
                    },
                    "repository": {
                        "type": "git",
                        "url": "https://github.com/example/syntaxcolorizer"
                    },
                    "keywords": [
                        "syntax",
                        "highlighting",
                        "avalonia",
                        "editor"
                    ],
                    "features": {
                        "languages": 18,
                        "themes": 7,
                        "formatting": true,
                        "linting": true
                    },
                    "dependencies": {
                        "Avalonia": "^11.0.0"
                    },
                    "settings": {
                        "tabSize": 4,
                        "useSpaces": true,
                        "showLineNumbers": true,
                        "wordWrap": false,
                        "autoIndent": true
                    },
                    "enabled": true,
                    "maxFileSize": 1048576,
                    "supportedExtensions": [".cs", ".vb", ".js", ".ts", ".py"]
                }
                """,

            SyntaxLanguage.Yaml => """
                # SyntaxColorizer Configuration
                name: SyntaxColorizer
                version: 1.0.0
                description: A syntax highlighting library for Avalonia

                author:
                  name: Developer
                  email: dev@example.com

                repository:
                  type: git
                  url: https://github.com/example/syntaxcolorizer

                keywords:
                  - syntax
                  - highlighting
                  - avalonia
                  - editor

                features:
                  languages: 18
                  themes: 7
                  formatting: true
                  linting: true

                dependencies:
                  Avalonia: "^11.0.0"

                settings:
                  tabSize: 4
                  useSpaces: true
                  showLineNumbers: true
                  wordWrap: false
                  autoIndent: true

                # Anchors and aliases example
                defaults: &defaults
                  adapter: postgres
                  host: localhost

                development:
                  <<: *defaults
                  database: dev_db

                production:
                  <<: *defaults
                  database: prod_db
                  host: db.example.com

                enabled: true
                maxFileSize: 1048576

                supportedExtensions:
                  - .cs
                  - .vb
                  - .js
                  - .ts
                  - .py
                """,

            SyntaxLanguage.Xml => """
                <?xml version="1.0" encoding="UTF-8"?>
                <!-- SyntaxColorizer Configuration File -->
                <configuration xmlns="http://example.com/config"
                               xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

                    <appSettings>
                        <add key="ApplicationName" value="SyntaxColorizer" />
                        <add key="Version" value="1.0.0" />
                        <add key="Debug" value="false" />
                    </appSettings>

                    <syntaxHighlighting>
                        <languages>
                            <language name="CSharp" extensions=".cs" enabled="true">
                                <keywords>class,interface,struct,enum,namespace</keywords>
                                <comment singleLine="//" multiLineStart="/*" multiLineEnd="*/" />
                            </language>
                            <language name="JavaScript" extensions=".js,.jsx" enabled="true">
                                <keywords>function,const,let,var,class</keywords>
                                <comment singleLine="//" multiLineStart="/*" multiLineEnd="*/" />
                            </language>
                            <language name="Python" extensions=".py" enabled="true">
                                <keywords>def,class,import,from,as</keywords>
                                <comment singleLine="#" />
                            </language>
                        </languages>

                        <themes>
                            <theme name="VisualStudioLight" default="true">
                                <color token="Keyword" foreground="#0000FF" />
                                <color token="String" foreground="#A31515" />
                                <color token="Comment" foreground="#008000" />
                            </theme>
                            <theme name="Monokai">
                                <color token="Keyword" foreground="#F92672" />
                                <color token="String" foreground="#E6DB74" />
                                <color token="Comment" foreground="#75715E" />
                            </theme>
                        </themes>
                    </syntaxHighlighting>

                    <logging level="Information">
                        <target type="File" path="logs/app.log" />
                        <target type="Console" />
                    </logging>

                    <![CDATA[
                        This is a CDATA section containing special characters: <>&"'
                        It can contain any text without escaping.
                    ]]>

                </configuration>
                """,

            SyntaxLanguage.Bash => """
                #!/bin/bash
                # SyntaxColorizer Build Script
                # This script builds and packages the application

                set -euo pipefail

                # Configuration variables
                PROJECT_NAME="SyntaxColorizer"
                VERSION="${1:-1.0.0}"
                BUILD_DIR="./build"
                OUTPUT_DIR="./dist"

                # Colors for output
                RED='\033[0;31m'
                GREEN='\033[0;32m'
                YELLOW='\033[1;33m'
                NC='\033[0m' # No Color

                # Functions
                log_info() {
                    echo -e "${GREEN}[INFO]${NC} $1"
                }

                log_warn() {
                    echo -e "${YELLOW}[WARN]${NC} $1"
                }

                log_error() {
                    echo -e "${RED}[ERROR]${NC} $1" >&2
                }

                check_dependencies() {
                    local deps=("dotnet" "git" "zip")
                    for dep in "${deps[@]}"; do
                        if ! command -v "$dep" &> /dev/null; then
                            log_error "Missing dependency: $dep"
                            exit 1
                        fi
                    done
                    log_info "All dependencies satisfied"
                }

                clean_build() {
                    log_info "Cleaning build directories..."
                    rm -rf "$BUILD_DIR" "$OUTPUT_DIR"
                    mkdir -p "$BUILD_DIR" "$OUTPUT_DIR"
                }

                build_project() {
                    log_info "Building $PROJECT_NAME v$VERSION..."

                    dotnet restore
                    dotnet build --configuration Release --output "$BUILD_DIR"

                    if [[ $? -eq 0 ]]; then
                        log_info "Build completed successfully"
                    else
                        log_error "Build failed"
                        exit 1
                    fi
                }

                run_tests() {
                    log_info "Running tests..."
                    dotnet test --no-build --configuration Release

                    case $? in
                        0)
                            log_info "All tests passed"
                            ;;
                        1)
                            log_warn "Some tests failed"
                            ;;
                        *)
                            log_error "Test execution error"
                            exit 1
                            ;;
                    esac
                }

                create_package() {
                    local package_name="${PROJECT_NAME}-${VERSION}.zip"
                    log_info "Creating package: $package_name"

                    cd "$BUILD_DIR" && zip -r "../$OUTPUT_DIR/$package_name" . && cd ..

                    echo "Package created: $OUTPUT_DIR/$package_name"
                    echo "Size: $(du -h "$OUTPUT_DIR/$package_name" | cut -f1)"
                }

                # Main execution
                main() {
                    echo "========================================"
                    echo "  $PROJECT_NAME Build Script"
                    echo "  Version: $VERSION"
                    echo "========================================"

                    check_dependencies
                    clean_build
                    build_project
                    run_tests
                    create_package

                    log_info "Build process completed!"
                }

                # Run main function
                main "$@"
                """,

            SyntaxLanguage.PowerShell => """
                <#
                .SYNOPSIS
                    SyntaxColorizer Build Script for PowerShell
                .DESCRIPTION
                    This script builds, tests, and packages the SyntaxColorizer application.
                .PARAMETER Version
                    The version number for the build (default: 1.0.0)
                .PARAMETER Configuration
                    Build configuration: Debug or Release (default: Release)
                #>

                [CmdletBinding()]
                param(
                    [Parameter(Position = 0)]
                    [string]$Version = "1.0.0",

                    [ValidateSet("Debug", "Release")]
                    [string]$Configuration = "Release"
                )

                # Script configuration
                $ErrorActionPreference = "Stop"
                $ProjectName = "SyntaxColorizer"
                $BuildDir = "./build"
                $OutputDir = "./dist"

                # Helper functions
                function Write-Step {
                    param([string]$Message)
                    Write-Host "`n=== $Message ===" -ForegroundColor Cyan
                }

                function Write-Success {
                    param([string]$Message)
                    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
                }

                function Write-Warning {
                    param([string]$Message)
                    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
                }

                function Test-Prerequisites {
                    Write-Step "Checking Prerequisites"

                    $required = @("dotnet", "git")
                    foreach ($cmd in $required) {
                        if (-not (Get-Command $cmd -ErrorAction SilentlyContinue)) {
                            throw "Required command not found: $cmd"
                        }
                    }

                    Write-Success "All prerequisites satisfied"
                }

                function Initialize-BuildDirectory {
                    Write-Step "Initializing Build Directory"

                    if (Test-Path $BuildDir) {
                        Remove-Item $BuildDir -Recurse -Force
                    }
                    if (Test-Path $OutputDir) {
                        Remove-Item $OutputDir -Recurse -Force
                    }

                    New-Item -ItemType Directory -Path $BuildDir | Out-Null
                    New-Item -ItemType Directory -Path $OutputDir | Out-Null

                    Write-Success "Build directories created"
                }

                function Invoke-Build {
                    Write-Step "Building $ProjectName v$Version"

                    dotnet restore
                    dotnet build --configuration $Configuration --output $BuildDir

                    if ($LASTEXITCODE -eq 0) {
                        Write-Success "Build completed successfully"
                    } else {
                        throw "Build failed with exit code $LASTEXITCODE"
                    }
                }

                function Invoke-Tests {
                    Write-Step "Running Tests"

                    $testResult = dotnet test --no-build --configuration $Configuration

                    switch ($LASTEXITCODE) {
                        0 { Write-Success "All tests passed" }
                        1 { Write-Warning "Some tests failed" }
                        default { throw "Test execution error" }
                    }
                }

                function New-Package {
                    Write-Step "Creating Package"

                    $packageName = "$ProjectName-$Version.zip"
                    $packagePath = Join-Path $OutputDir $packageName

                    Compress-Archive -Path "$BuildDir/*" -DestinationPath $packagePath

                    $fileInfo = Get-Item $packagePath
                    Write-Host "Package: $packagePath"
                    Write-Host "Size: $([math]::Round($fileInfo.Length / 1MB, 2)) MB"
                }

                # Main execution
                try {
                    Write-Host "`n========================================"
                    Write-Host "  $ProjectName Build Script"
                    Write-Host "  Version: $Version | Config: $Configuration"
                    Write-Host "========================================`n"

                    Test-Prerequisites
                    Initialize-BuildDirectory
                    Invoke-Build
                    Invoke-Tests
                    New-Package

                    Write-Success "`nBuild process completed!"
                }
                catch {
                    Write-Host "[ERROR] $_" -ForegroundColor Red
                    exit 1
                }
                """,

            SyntaxLanguage.Go => """
                // Package main demonstrates Go syntax highlighting
                package main

                import (
                    "context"
                    "encoding/json"
                    "fmt"
                    "log"
                    "net/http"
                    "sync"
                    "time"
                )

                // Calculator represents a simple calculator with history
                type Calculator struct {
                    history []float64
                    mu      sync.RWMutex
                }

                // Result represents a calculation result
                type Result struct {
                    Value     float64   `json:"value"`
                    Operation string    `json:"operation"`
                    Timestamp time.Time `json:"timestamp"`
                }

                // NewCalculator creates a new Calculator instance
                func NewCalculator() *Calculator {
                    return &Calculator{
                        history: make([]float64, 0),
                    }
                }

                // Add adds two numbers and stores the result
                func (c *Calculator) Add(a, b float64) float64 {
                    result := a + b

                    c.mu.Lock()
                    defer c.mu.Unlock()
                    c.history = append(c.history, result)

                    return result
                }

                // Calculate performs an async calculation
                func (c *Calculator) Calculate(ctx context.Context, expression string) (float64, error) {
                    if expression == "" {
                        return 0, fmt.Errorf("expression cannot be empty")
                    }

                    // Simulate async work
                    select {
                    case <-time.After(100 * time.Millisecond):
                        return 42.0, nil
                    case <-ctx.Done():
                        return 0, ctx.Err()
                    }
                }

                // History returns a copy of the calculation history
                func (c *Calculator) History() []float64 {
                    c.mu.RLock()
                    defer c.mu.RUnlock()

                    result := make([]float64, len(c.history))
                    copy(result, c.history)
                    return result
                }

                // HTTP handler for calculator operations
                func handleCalculation(w http.ResponseWriter, r *http.Request) {
                    if r.Method != http.MethodPost {
                        http.Error(w, "Method not allowed", http.StatusMethodNotAllowed)
                        return
                    }

                    var req struct {
                        A float64 `json:"a"`
                        B float64 `json:"b"`
                    }

                    if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
                        http.Error(w, err.Error(), http.StatusBadRequest)
                        return
                    }

                    calc := NewCalculator()
                    result := calc.Add(req.A, req.B)

                    resp := Result{
                        Value:     result,
                        Operation: "add",
                        Timestamp: time.Now(),
                    }

                    w.Header().Set("Content-Type", "application/json")
                    json.NewEncoder(w).Encode(resp)
                }

                func main() {
                    calc := NewCalculator()

                    // Basic operations
                    sum := calc.Add(10.0, 20.0)
                    fmt.Printf("Sum: %.2f\n", sum)

                    // Async calculation with context
                    ctx, cancel := context.WithTimeout(context.Background(), time.Second)
                    defer cancel()

                    result, err := calc.Calculate(ctx, "10 + 20")
                    if err != nil {
                        log.Fatalf("Calculation failed: %v", err)
                    }
                    fmt.Printf("Result: %.2f\n", result)

                    // Print history
                    for i, v := range calc.History() {
                        fmt.Printf("History[%d]: %.2f\n", i, v)
                    }

                    // Start HTTP server
                    http.HandleFunc("/calculate", handleCalculation)
                    log.Println("Server starting on :8080")
                    log.Fatal(http.ListenAndServe(":8080", nil))
                }
                """,

            SyntaxLanguage.Ruby => """
                # A sample module demonstrating Ruby syntax highlighting.

                require 'json'
                require 'net/http'

                # Calculator class with history tracking
                class Calculator
                  attr_reader :history

                  def initialize
                    @history = []
                  end

                  # Add two numbers and store result
                  def add(a, b)
                    result = a + b
                    @history << result
                    result
                  end

                  # Subtract two numbers
                  def subtract(a, b)
                    result = a - b
                    @history << result
                    result
                  end

                  # Calculate with block
                  def calculate(expression)
                    raise ArgumentError, 'Expression cannot be empty' if expression.nil? || expression.empty?

                    # Yield to block if given
                    if block_given?
                      yield expression
                    else
                      eval(expression)
                    end
                  end

                  # Clear history
                  def clear_history!
                    @history.clear
                  end
                end

                # Module for math utilities
                module MathUtils
                  PI = 3.14159265359
                  E = 2.71828182845

                  def self.circle_area(radius)
                    PI * radius ** 2
                  end

                  def self.factorial(n)
                    return 1 if n <= 1
                    n * factorial(n - 1)
                  end
                end

                # Using symbols and hashes
                config = {
                  name: 'SyntaxColorizer',
                  version: '1.0.0',
                  features: [:highlighting, :formatting, :linting],
                  settings: {
                    tab_size: 4,
                    use_spaces: true
                  }
                }

                # Array operations with blocks
                numbers = [1, 2, 3, 4, 5]
                squared = numbers.map { |n| n ** 2 }
                evens = numbers.select(&:even?)

                # String interpolation
                name = "Ruby"
                puts "Hello, #{name}! Version: #{RUBY_VERSION}"

                # Here document
                help_text = <<~HEREDOC
                  Welcome to the Calculator!
                  Commands: add, subtract, multiply, divide
                  Type 'exit' to quit.
                HEREDOC

                # Main execution
                if __FILE__ == $PROGRAM_NAME
                  calc = Calculator.new
                  sum = calc.add(10, 20)
                  puts "Sum: #{sum}"

                  calc.calculate('5 + 3') do |expr|
                    puts "Evaluating: #{expr}"
                    eval(expr)
                  end

                  area = MathUtils.circle_area(5)
                  puts "Circle area: #{area.round(2)}"
                end
                """,

            SyntaxLanguage.Kotlin => """
                // A sample file demonstrating Kotlin syntax highlighting.

                package com.syntaxcolorizer.demo

                import kotlinx.coroutines.*

                // Data class for results
                data class Result(val value: Double, val operation: String)

                // Calculator class with history
                class Calculator {
                    private val history = mutableListOf<Double>()

                    fun add(a: Double, b: Double): Double {
                        val result = a + b
                        history.add(result)
                        return result
                    }

                    fun getHistory(): List<Double> = history.toList()
                }

                // Sealed class example
                sealed class Operation {
                    data class Add(val a: Double, val b: Double) : Operation()
                    data object Clear : Operation()
                }

                // Main function
                fun main() {
                    val calc = Calculator()
                    val sum = calc.add(10.0, 20.0)
                    println("Sum: $sum")
                    println("History: ${calc.getHistory()}")
                }
                """,

            SyntaxLanguage.Swift => @"// A sample file demonstrating Swift syntax highlighting.

import Foundation

/// A struct representing a calculation result.
struct Result: Codable {
    let value: Double
    let operation: String
    let timestamp: Date

    init(value: Double, operation: String) {
        self.value = value
        self.operation = operation
        self.timestamp = Date()
    }
}

/// Calculator class with history tracking.
class Calculator {
    private var history: [Double] = []

    // Add two numbers
    func add(_ a: Double, _ b: Double) -> Double {
        let result = a + b
        history.append(result)
        return result
    }

    // Subtract two numbers
    func subtract(_ a: Double, _ b: Double) -> Double {
        let result = a - b
        history.append(result)
        return result
    }

    // Async calculation
    func calculate(expression: String) async throws -> Result {
        guard !expression.isEmpty else {
            throw CalculatorError.emptyExpression
        }

        try await Task.sleep(nanoseconds: 100_000_000)
        return Result(value: 42.0, operation: ""calculate"")
    }

    func getHistory() -> [Double] {
        return history
    }
}

// Error enum
enum CalculatorError: Error {
    case emptyExpression
    case invalidOperation
    case divisionByZero
}

// Enum with associated values
enum Operation {
    case add(Double, Double)
    case subtract(Double, Double)
    case multiply(Double, Double)
    case divide(Double, Double)

    func execute() -> Double? {
        switch self {
        case .add(let a, let b):
            return a + b
        case .subtract(let a, let b):
            return a - b
        case .multiply(let a, let b):
            return a * b
        case .divide(let a, let b):
            guard b != 0 else { return nil }
            return a / b
        }
    }
}

// Protocol with extension
protocol Printable {
    func printDescription()
}

extension Printable {
    func printDescription() {
        print(""Description: \(self)"")
    }
}

// Generic function
func findMax<T: Comparable>(_ array: [T]) -> T? {
    guard !array.isEmpty else { return nil }
    return array.max()
}

// Main execution
@main
struct CalculatorApp {
    static func main() async {
        let calc = Calculator()

        // Basic operations
        let sum = calc.add(10.0, 20.0)
        print(""Sum: \(sum)"")

        // Optional binding
        let optionalValue: Int? = 42
        if let value = optionalValue {
            print(""Value: \(value)"")
        }

        // Guard statement
        func process(input: String?) {
            guard let input = input, !input.isEmpty else {
                print(""Invalid input"")
                return
            }
            print(""Processing: \(input)"")
        }

        // Collection operations
        let numbers = [1, 2, 3, 4, 5]
        let squared = numbers.map { $0 * $0 }
        let evens = numbers.filter { $0 % 2 == 0 }

        print(""Squared: \(squared)"")
        print(""Evens: \(evens)"")
    }
}
",

            SyntaxLanguage.Scala => @"// A sample file demonstrating Scala syntax highlighting.

package com.syntaxcolorizer.demo

import scala.concurrent.{Future, ExecutionContext}
import scala.util.{Try, Success, Failure}

/**
 * A case class representing a calculation result.
 */
case class Result(
  value: Double,
  operation: String,
  timestamp: Long = System.currentTimeMillis()
)

/**
 * Calculator class with history tracking.
 */
class Calculator {
  private var history: List[Double] = Nil

  // Add two numbers
  def add(a: Double, b: Double): Double = {
    val result = a + b
    history = result :: history
    result
  }

  // Subtract two numbers
  def subtract(a: Double, b: Double): Double = {
    val result = a - b
    history = result :: history
    result
  }

  // Calculate with expression
  def calculate(expression: String)(implicit ec: ExecutionContext): Future[Result] = {
    require(expression.nonEmpty, ""Expression cannot be empty"")

    Future {
      Thread.sleep(100)
      Result(42.0, ""calculate"")
    }
  }

  def getHistory: List[Double] = history.reverse
}

// Sealed trait for operations
sealed trait Operation
case class Add(a: Double, b: Double) extends Operation
case class Subtract(a: Double, b: Double) extends Operation
case class Multiply(a: Double, b: Double) extends Operation
case object Clear extends Operation

// Object declaration (singleton)
object MathUtils {
  val PI: Double = 3.14159

  def circleArea(radius: Double): Double = PI * radius * radius

  def factorial(n: Int): Long = n match {
    case _ if n <= 1 => 1L
    case _ => n * factorial(n - 1)
  }

  // Higher-order function
  def applyTwice[A](f: A => A)(x: A): A = f(f(x))
}

// Trait with default implementation
trait Printable {
  def print(): Unit
  def prettyPrint(): Unit = println(s""Pretty: $this"")
}

// Enum (Scala 3 style)
enum Theme(val isDark: Boolean) {
  case Light extends Theme(false)
  case Dark extends Theme(true)
  case Monokai extends Theme(true)

  def toggle: Theme = if (isDark) Theme.Light else Theme.Dark
}

// Generic class with type bounds
class Container[T <: AnyRef](value: T) {
  def get: T = value
  def map[U <: AnyRef](f: T => U): Container[U] = new Container(f(value))
}

// Main object
object Main extends App {
  implicit val ec: ExecutionContext = ExecutionContext.global

  val calc = new Calculator

  // Basic operations
  val sum = calc.add(10.0, 20.0)
  println(s""Sum: $sum"")

  // Pattern matching
  val operation: Operation = Add(5.0, 3.0)
  val result = operation match {
    case Add(a, b) => a + b
    case Subtract(a, b) => a - b
    case Multiply(a, b) => a * b
    case Clear => 0.0
  }

  // Option handling
  val maybeValue: Option[Int] = Some(42)
  val extracted = maybeValue.getOrElse(0)

  // For comprehension
  val numbers = List(1, 2, 3, 4, 5)
  val processed = for {
    n <- numbers
    if n % 2 == 0
  } yield n * n

  // Collection operations
  val squared = numbers.map(n => n * n)
  val evens = numbers.filter(_ % 2 == 0)

  println(s""Result: $result"")
  println(s""History: ${calc.getHistory}"")
}
",

            SyntaxLanguage.Dockerfile => @"# Dockerfile demonstrating syntax highlighting
# Build stage for the application

# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
LABEL maintainer=""developer@example.com""
LABEL version=""1.0""
LABEL description=""SyntaxColorizer Demo Application""

# Set working directory
WORKDIR /src

# Copy project files
COPY [""src/SyntaxColorizer/SyntaxColorizer.csproj"", ""SyntaxColorizer/""]
COPY [""src/SyntaxColorizer.Demo/SyntaxColorizer.Demo.csproj"", ""SyntaxColorizer.Demo/""]

# Restore dependencies
RUN dotnet restore ""SyntaxColorizer.Demo/SyntaxColorizer.Demo.csproj""

# Copy remaining source code
COPY src/ .

# Build arguments
ARG BUILD_CONFIGURATION=Release
ARG VERSION=1.0.0

# Environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=true \
    ASPNETCORE_ENVIRONMENT=Production

# Build the application
RUN dotnet build ""SyntaxColorizer.Demo/SyntaxColorizer.Demo.csproj"" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    /p:Version=$VERSION

# Publish stage
FROM build AS publish
RUN dotnet publish ""SyntaxColorizer.Demo/SyntaxColorizer.Demo.csproj"" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080/tcp

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT [""dotnet"", ""SyntaxColorizer.Demo.dll""]
CMD [""--urls"", ""http://+:8080""]
",

            _ => "// Select a language to see sample code with syntax highlighting"
        };
    }
}
