using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Bash/Shell scripting language.
/// </summary>
public class BashTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static BashTokenizer()
    {
        // Shell keywords
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "then", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "elif", TokenType.ControlKeyword },
            { "fi", TokenType.ControlKeyword },
            { "case", TokenType.ControlKeyword },
            { "esac", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "until", TokenType.ControlKeyword },
            { "do", TokenType.ControlKeyword },
            { "done", TokenType.ControlKeyword },
            { "in", TokenType.ControlKeyword },
            { "select", TokenType.ControlKeyword },

            // Keywords
            { "function", TokenType.Keyword },
            { "return", TokenType.Keyword },
            { "exit", TokenType.Keyword },
            { "break", TokenType.Keyword },
            { "continue", TokenType.Keyword },
            { "local", TokenType.Keyword },
            { "export", TokenType.Keyword },
            { "readonly", TokenType.Keyword },
            { "declare", TokenType.Keyword },
            { "typeset", TokenType.Keyword },
            { "unset", TokenType.Keyword },
            { "shift", TokenType.Keyword },
            { "source", TokenType.Keyword },
            { "trap", TokenType.Keyword },
            { "eval", TokenType.Keyword },
            { "exec", TokenType.Keyword },
            { "set", TokenType.Keyword },

            // Common builtins
            { "echo", TokenType.ShellCommand },
            { "printf", TokenType.ShellCommand },
            { "read", TokenType.ShellCommand },
            { "cd", TokenType.ShellCommand },
            { "pwd", TokenType.ShellCommand },
            { "pushd", TokenType.ShellCommand },
            { "popd", TokenType.ShellCommand },
            { "dirs", TokenType.ShellCommand },
            { "test", TokenType.ShellCommand },
            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
            { "alias", TokenType.ShellCommand },
            { "unalias", TokenType.ShellCommand },
            { "type", TokenType.ShellCommand },
            { "which", TokenType.ShellCommand },
            { "whereis", TokenType.ShellCommand },
            { "command", TokenType.ShellCommand },
            { "builtin", TokenType.ShellCommand },
            { "hash", TokenType.ShellCommand },
            { "help", TokenType.ShellCommand },
            { "history", TokenType.ShellCommand },
            { "jobs", TokenType.ShellCommand },
            { "fg", TokenType.ShellCommand },
            { "bg", TokenType.ShellCommand },
            { "wait", TokenType.ShellCommand },
            { "kill", TokenType.ShellCommand },
            { "disown", TokenType.ShellCommand },
            { "suspend", TokenType.ShellCommand },
            { "logout", TokenType.ShellCommand },
            { "times", TokenType.ShellCommand },
            { "umask", TokenType.ShellCommand },
            { "ulimit", TokenType.ShellCommand },
            { "getopts", TokenType.ShellCommand },
            { "bind", TokenType.ShellCommand },
            { "complete", TokenType.ShellCommand },
            { "compgen", TokenType.ShellCommand },
            { "shopt", TokenType.ShellCommand },
            { "let", TokenType.ShellCommand },

            // Common external commands
            { "grep", TokenType.ShellCommand },
            { "sed", TokenType.ShellCommand },
            { "awk", TokenType.ShellCommand },
            { "cat", TokenType.ShellCommand },
            { "head", TokenType.ShellCommand },
            { "tail", TokenType.ShellCommand },
            { "sort", TokenType.ShellCommand },
            { "uniq", TokenType.ShellCommand },
            { "wc", TokenType.ShellCommand },
            { "cut", TokenType.ShellCommand },
            { "tr", TokenType.ShellCommand },
            { "find", TokenType.ShellCommand },
            { "xargs", TokenType.ShellCommand },
            { "ls", TokenType.ShellCommand },
            { "cp", TokenType.ShellCommand },
            { "mv", TokenType.ShellCommand },
            { "rm", TokenType.ShellCommand },
            { "mkdir", TokenType.ShellCommand },
            { "rmdir", TokenType.ShellCommand },
            { "touch", TokenType.ShellCommand },
            { "chmod", TokenType.ShellCommand },
            { "chown", TokenType.ShellCommand },
            { "ln", TokenType.ShellCommand },
            { "tar", TokenType.ShellCommand },
            { "gzip", TokenType.ShellCommand },
            { "gunzip", TokenType.ShellCommand },
            { "zip", TokenType.ShellCommand },
            { "unzip", TokenType.ShellCommand },
            { "curl", TokenType.ShellCommand },
            { "wget", TokenType.ShellCommand },
            { "ssh", TokenType.ShellCommand },
            { "scp", TokenType.ShellCommand },
            { "rsync", TokenType.ShellCommand },
            { "git", TokenType.ShellCommand },
            { "docker", TokenType.ShellCommand },
            { "sudo", TokenType.ShellCommand },
            { "su", TokenType.ShellCommand },
            { "apt", TokenType.ShellCommand },
            { "yum", TokenType.ShellCommand },
            { "dnf", TokenType.ShellCommand },
            { "pacman", TokenType.ShellCommand },
            { "brew", TokenType.ShellCommand },
            { "npm", TokenType.ShellCommand },
            { "pip", TokenType.ShellCommand },
            { "python", TokenType.ShellCommand },
            { "node", TokenType.ShellCommand },
            { "make", TokenType.ShellCommand },
            { "cmake", TokenType.ShellCommand },
            { "gcc", TokenType.ShellCommand },
            { "clang", TokenType.ShellCommand }
        };

        _patterns = new List<TokenPattern>
        {
            // Shebang
            new TokenPattern(@"^#!.*$", TokenType.Preprocessor, 100, RegexOptions.Multiline),

            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 95),

            // Here-documents
            new TokenPattern(@"<<-?\s*['""]?(\w+)['""]?[\s\S]*?\n\1\b", TokenType.String, 90),

            // Double-quoted strings (with variable interpolation)
            new TokenPattern(@"""(?:[^""\\$]|\\.|(\$(?:\{[^}]+\}|\([^)]+\)|[\w@#?$!*-])))*""", TokenType.String, 85),

            // Single-quoted strings (literal)
            new TokenPattern(@"'[^']*'", TokenType.String, 85),

            // ANSI-C quoted strings
            new TokenPattern(@"\$'(?:[^'\\]|\\.)*'", TokenType.String, 85),

            // Command substitution $(...)
            new TokenPattern(@"\$\([^)]+\)", TokenType.Method, 80),

            // Arithmetic expansion $((...))
            new TokenPattern(@"\$\(\([^)]+\)\)", TokenType.Number, 80),

            // Backtick command substitution
            new TokenPattern(@"`[^`]+`", TokenType.Method, 80),

            // Variables with braces ${VAR}
            new TokenPattern(@"\$\{[^}]+\}", TokenType.ShellVariable, 75),

            // Special variables $@, $*, $#, $?, $$, $!, $0-$9
            new TokenPattern(@"\$[@*#?$!0-9-]", TokenType.ShellVariable, 75),

            // Regular variables $VAR
            new TokenPattern(@"\$[a-zA-Z_][a-zA-Z0-9_]*", TokenType.ShellVariable, 75),

            // Long options --option
            new TokenPattern(@"--[a-zA-Z][a-zA-Z0-9-]*", TokenType.ShellOption, 70),

            // Short options -x or combined -xyz
            new TokenPattern(@"(?<=\s)-[a-zA-Z]+", TokenType.ShellOption, 70),

            // Numbers
            new TokenPattern(@"\b\d+\b", TokenType.Number, 60),

            // Function definitions
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*\s*\(\s*\)", TokenType.Method, 55),

            // Test operators
            new TokenPattern(@"\[\[|\]\]|\[|\]", TokenType.Keyword, 50),

            // Redirection operators
            new TokenPattern(@">>|>&|<&|<<|<>|[<>]", TokenType.Operator, 45),

            // Pipe and logical operators
            new TokenPattern(@"\|\||&&|\|&?|&", TokenType.Operator, 45),

            // Comparison operators
            new TokenPattern(@"-(?:eq|ne|lt|le|gt|ge|z|n|e|f|d|r|w|x|s|L|O|G|N|nt|ot|ef)\b", TokenType.Operator, 45),

            // Assignment and arithmetic operators
            new TokenPattern(@"[+\-*/%]=?|==?|!=", TokenType.Operator, 40),

            // Identifiers (words)
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}();,]", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0)
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.Bash;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
