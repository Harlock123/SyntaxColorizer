using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for R programming language.
/// </summary>
public class RTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static RTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>
        {
            // Control flow
            { "if", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "for", TokenType.ControlKeyword },
            { "while", TokenType.ControlKeyword },
            { "repeat", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "next", TokenType.ControlKeyword },
            { "return", TokenType.ControlKeyword },
            { "switch", TokenType.ControlKeyword },
            { "tryCatch", TokenType.ControlKeyword },
            { "stop", TokenType.ControlKeyword },
            { "warning", TokenType.ControlKeyword },

            // Keywords
            { "function", TokenType.Keyword },
            { "in", TokenType.Keyword },
            { "library", TokenType.Keyword },
            { "require", TokenType.Keyword },
            { "source", TokenType.Keyword },
            { "setwd", TokenType.Keyword },
            { "getwd", TokenType.Keyword },

            // Constants
            { "TRUE", TokenType.Constant },
            { "FALSE", TokenType.Constant },
            { "T", TokenType.Constant },
            { "F", TokenType.Constant },
            { "NA", TokenType.Constant },
            { "NA_integer_", TokenType.Constant },
            { "NA_real_", TokenType.Constant },
            { "NA_complex_", TokenType.Constant },
            { "NA_character_", TokenType.Constant },
            { "NULL", TokenType.Constant },
            { "NaN", TokenType.Constant },
            { "Inf", TokenType.Constant },
            { "pi", TokenType.Constant },
            { "letters", TokenType.Constant },
            { "LETTERS", TokenType.Constant },
            { "month.name", TokenType.Constant },
            { "month.abb", TokenType.Constant },

            // Common functions
            { "print", TokenType.Method },
            { "cat", TokenType.Method },
            { "paste", TokenType.Method },
            { "paste0", TokenType.Method },
            { "sprintf", TokenType.Method },
            { "c", TokenType.Method },
            { "list", TokenType.Method },
            { "vector", TokenType.Method },
            { "matrix", TokenType.Method },
            { "array", TokenType.Method },
            { "data.frame", TokenType.Method },
            { "factor", TokenType.Method },
            { "length", TokenType.Method },
            { "nrow", TokenType.Method },
            { "ncol", TokenType.Method },
            { "dim", TokenType.Method },
            { "names", TokenType.Method },
            { "colnames", TokenType.Method },
            { "rownames", TokenType.Method },
            { "class", TokenType.Method },
            { "typeof", TokenType.Method },
            { "str", TokenType.Method },
            { "summary", TokenType.Method },
            { "head", TokenType.Method },
            { "tail", TokenType.Method },
            { "View", TokenType.Method },
            { "read.csv", TokenType.Method },
            { "write.csv", TokenType.Method },
            { "read.table", TokenType.Method },
            { "write.table", TokenType.Method },
            { "is.na", TokenType.Method },
            { "is.null", TokenType.Method },
            { "is.numeric", TokenType.Method },
            { "is.character", TokenType.Method },
            { "is.logical", TokenType.Method },
            { "is.factor", TokenType.Method },
            { "as.numeric", TokenType.Method },
            { "as.character", TokenType.Method },
            { "as.logical", TokenType.Method },
            { "as.factor", TokenType.Method },
            { "as.integer", TokenType.Method },
            { "as.data.frame", TokenType.Method },
            { "as.matrix", TokenType.Method },
            { "which", TokenType.Method },
            { "subset", TokenType.Method },
            { "merge", TokenType.Method },
            { "rbind", TokenType.Method },
            { "cbind", TokenType.Method },
            { "apply", TokenType.Method },
            { "lapply", TokenType.Method },
            { "sapply", TokenType.Method },
            { "mapply", TokenType.Method },
            { "tapply", TokenType.Method },
            { "aggregate", TokenType.Method },
            { "mean", TokenType.Method },
            { "median", TokenType.Method },
            { "sum", TokenType.Method },
            { "min", TokenType.Method },
            { "max", TokenType.Method },
            { "range", TokenType.Method },
            { "var", TokenType.Method },
            { "sd", TokenType.Method },
            { "cor", TokenType.Method },
            { "cov", TokenType.Method },
            { "abs", TokenType.Method },
            { "sqrt", TokenType.Method },
            { "log", TokenType.Method },
            { "log10", TokenType.Method },
            { "log2", TokenType.Method },
            { "exp", TokenType.Method },
            { "round", TokenType.Method },
            { "floor", TokenType.Method },
            { "ceiling", TokenType.Method },
            { "seq", TokenType.Method },
            { "rep", TokenType.Method },
            { "rev", TokenType.Method },
            { "sort", TokenType.Method },
            { "order", TokenType.Method },
            { "unique", TokenType.Method },
            { "table", TokenType.Method },
            { "cut", TokenType.Method },
            { "grep", TokenType.Method },
            { "grepl", TokenType.Method },
            { "gsub", TokenType.Method },
            { "sub", TokenType.Method },
            { "nchar", TokenType.Method },
            { "strsplit", TokenType.Method },
            { "substr", TokenType.Method },
            { "tolower", TokenType.Method },
            { "toupper", TokenType.Method },
            { "plot", TokenType.Method },
            { "hist", TokenType.Method },
            { "barplot", TokenType.Method },
            { "boxplot", TokenType.Method },
            { "ggplot", TokenType.Method },
            { "aes", TokenType.Method },
            { "geom_point", TokenType.Method },
            { "geom_line", TokenType.Method },
            { "geom_bar", TokenType.Method },
            { "geom_histogram", TokenType.Method },
            { "lm", TokenType.Method },
            { "glm", TokenType.Method },
            { "predict", TokenType.Method },
            { "coef", TokenType.Method },
            { "residuals", TokenType.Method },
            { "fitted", TokenType.Method },
            { "t.test", TokenType.Method },
            { "chisq.test", TokenType.Method },
            { "anova", TokenType.Method },
            { "install.packages", TokenType.Method },
            { "remove.packages", TokenType.Method },
        };

        _patterns = new List<TokenPattern>
        {
            // Comments
            new TokenPattern(@"#[^\n]*", TokenType.Comment, 100),

            // Raw strings (R 4.0+)
            new TokenPattern(@"[rR]""[^""]*""", TokenType.String, 90),
            new TokenPattern(@"[rR]'[^']*'", TokenType.String, 90),

            // Double-quoted strings
            new TokenPattern(@"""(?:[^""\\]|\\.)*""", TokenType.String, 85),

            // Single-quoted strings
            new TokenPattern(@"'(?:[^'\\]|\\.)*'", TokenType.String, 85),

            // Backtick identifiers
            new TokenPattern(@"`[^`]+`", TokenType.Identifier, 80),

            // Numbers (complex)
            new TokenPattern(@"\b\d+\.?\d*[eE][+-]?\d+i?\b", TokenType.Number, 70),

            // Numbers (hex)
            new TokenPattern(@"\b0[xX][0-9a-fA-F]+L?\b", TokenType.Number, 70),

            // Numbers (integer/decimal)
            new TokenPattern(@"\b\d+\.?\d*L?\b", TokenType.Number, 70),

            // Operators
            new TokenPattern(@"<-|<<-|->|->>|%%|%/%|%\*%|%in%|%o%|%x%|\|\||&&|::|:::|\$|@|[+\-*/%^<>=!&|:~?]+", TokenType.Operator, 40),

            // Identifiers (can include dots)
            new TokenPattern(@"\b[a-zA-Z][a-zA-Z0-9._]*\b", TokenType.Identifier, 30),
            new TokenPattern(@"\.[a-zA-Z][a-zA-Z0-9._]*\b", TokenType.Identifier, 30),

            // Punctuation
            new TokenPattern(@"[{}()\[\];,]+", TokenType.Punctuation, 20),

            // Whitespace
            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    public override SyntaxLanguage Language => SyntaxLanguage.R;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
