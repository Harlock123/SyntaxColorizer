using System.Text.RegularExpressions;

namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Base tokenizer for SQL dialects.
/// </summary>
public abstract class SqlTokenizerBase : LanguageTokenizerBase
{
    protected static readonly Dictionary<string, TokenType> CommonSqlKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        // DML keywords
        ["SELECT"] = TokenType.SqlKeyword,
        ["FROM"] = TokenType.SqlKeyword,
        ["WHERE"] = TokenType.SqlKeyword,
        ["AND"] = TokenType.SqlKeyword,
        ["OR"] = TokenType.SqlKeyword,
        ["NOT"] = TokenType.SqlKeyword,
        ["IN"] = TokenType.SqlKeyword,
        ["EXISTS"] = TokenType.SqlKeyword,
        ["BETWEEN"] = TokenType.SqlKeyword,
        ["LIKE"] = TokenType.SqlKeyword,
        ["IS"] = TokenType.SqlKeyword,
        ["NULL"] = TokenType.Constant,
        ["AS"] = TokenType.SqlKeyword,
        ["ON"] = TokenType.SqlKeyword,
        ["JOIN"] = TokenType.SqlKeyword,
        ["INNER"] = TokenType.SqlKeyword,
        ["LEFT"] = TokenType.SqlKeyword,
        ["RIGHT"] = TokenType.SqlKeyword,
        ["OUTER"] = TokenType.SqlKeyword,
        ["FULL"] = TokenType.SqlKeyword,
        ["CROSS"] = TokenType.SqlKeyword,
        ["NATURAL"] = TokenType.SqlKeyword,
        ["USING"] = TokenType.SqlKeyword,
        ["ORDER"] = TokenType.SqlKeyword,
        ["BY"] = TokenType.SqlKeyword,
        ["ASC"] = TokenType.SqlKeyword,
        ["DESC"] = TokenType.SqlKeyword,
        ["GROUP"] = TokenType.SqlKeyword,
        ["HAVING"] = TokenType.SqlKeyword,
        ["DISTINCT"] = TokenType.SqlKeyword,
        ["ALL"] = TokenType.SqlKeyword,
        ["UNION"] = TokenType.SqlKeyword,
        ["INTERSECT"] = TokenType.SqlKeyword,
        ["EXCEPT"] = TokenType.SqlKeyword,
        ["INSERT"] = TokenType.SqlKeyword,
        ["INTO"] = TokenType.SqlKeyword,
        ["VALUES"] = TokenType.SqlKeyword,
        ["UPDATE"] = TokenType.SqlKeyword,
        ["SET"] = TokenType.SqlKeyword,
        ["DELETE"] = TokenType.SqlKeyword,
        ["TRUNCATE"] = TokenType.SqlKeyword,
        ["MERGE"] = TokenType.SqlKeyword,

        // DDL keywords
        ["CREATE"] = TokenType.SqlKeyword,
        ["ALTER"] = TokenType.SqlKeyword,
        ["DROP"] = TokenType.SqlKeyword,
        ["TABLE"] = TokenType.SqlKeyword,
        ["VIEW"] = TokenType.SqlKeyword,
        ["INDEX"] = TokenType.SqlKeyword,
        ["PROCEDURE"] = TokenType.SqlKeyword,
        ["FUNCTION"] = TokenType.SqlKeyword,
        ["TRIGGER"] = TokenType.SqlKeyword,
        ["DATABASE"] = TokenType.SqlKeyword,
        ["SCHEMA"] = TokenType.SqlKeyword,
        ["CONSTRAINT"] = TokenType.SqlKeyword,
        ["PRIMARY"] = TokenType.SqlKeyword,
        ["KEY"] = TokenType.SqlKeyword,
        ["FOREIGN"] = TokenType.SqlKeyword,
        ["REFERENCES"] = TokenType.SqlKeyword,
        ["UNIQUE"] = TokenType.SqlKeyword,
        ["CHECK"] = TokenType.SqlKeyword,
        ["DEFAULT"] = TokenType.SqlKeyword,
        ["CASCADE"] = TokenType.SqlKeyword,
        ["RESTRICT"] = TokenType.SqlKeyword,

        // Control flow
        ["IF"] = TokenType.ControlKeyword,
        ["ELSE"] = TokenType.ControlKeyword,
        ["CASE"] = TokenType.ControlKeyword,
        ["WHEN"] = TokenType.ControlKeyword,
        ["THEN"] = TokenType.ControlKeyword,
        ["END"] = TokenType.ControlKeyword,
        ["BEGIN"] = TokenType.ControlKeyword,
        ["WHILE"] = TokenType.ControlKeyword,
        ["LOOP"] = TokenType.ControlKeyword,
        ["FOR"] = TokenType.ControlKeyword,
        ["RETURN"] = TokenType.ControlKeyword,
        ["BREAK"] = TokenType.ControlKeyword,
        ["CONTINUE"] = TokenType.ControlKeyword,

        // Transaction
        ["COMMIT"] = TokenType.SqlKeyword,
        ["ROLLBACK"] = TokenType.SqlKeyword,
        ["TRANSACTION"] = TokenType.SqlKeyword,
        ["SAVEPOINT"] = TokenType.SqlKeyword,

        // Aggregate functions
        ["COUNT"] = TokenType.SqlFunction,
        ["SUM"] = TokenType.SqlFunction,
        ["AVG"] = TokenType.SqlFunction,
        ["MIN"] = TokenType.SqlFunction,
        ["MAX"] = TokenType.SqlFunction,

        // Other functions
        ["COALESCE"] = TokenType.SqlFunction,
        ["NULLIF"] = TokenType.SqlFunction,
        ["CAST"] = TokenType.SqlFunction,
        ["CONVERT"] = TokenType.SqlFunction,

        // Data types
        ["INT"] = TokenType.TypeName,
        ["INTEGER"] = TokenType.TypeName,
        ["BIGINT"] = TokenType.TypeName,
        ["SMALLINT"] = TokenType.TypeName,
        ["TINYINT"] = TokenType.TypeName,
        ["BIT"] = TokenType.TypeName,
        ["DECIMAL"] = TokenType.TypeName,
        ["NUMERIC"] = TokenType.TypeName,
        ["FLOAT"] = TokenType.TypeName,
        ["REAL"] = TokenType.TypeName,
        ["MONEY"] = TokenType.TypeName,
        ["CHAR"] = TokenType.TypeName,
        ["VARCHAR"] = TokenType.TypeName,
        ["TEXT"] = TokenType.TypeName,
        ["NCHAR"] = TokenType.TypeName,
        ["NVARCHAR"] = TokenType.TypeName,
        ["NTEXT"] = TokenType.TypeName,
        ["DATE"] = TokenType.TypeName,
        ["TIME"] = TokenType.TypeName,
        ["DATETIME"] = TokenType.TypeName,
        ["TIMESTAMP"] = TokenType.TypeName,
        ["BINARY"] = TokenType.TypeName,
        ["VARBINARY"] = TokenType.TypeName,
        ["BLOB"] = TokenType.TypeName,
        ["CLOB"] = TokenType.TypeName,
        ["BOOLEAN"] = TokenType.TypeName,
        ["XML"] = TokenType.TypeName,
        ["JSON"] = TokenType.TypeName,

        // Boolean literals
        ["TRUE"] = TokenType.Constant,
        ["FALSE"] = TokenType.Constant,
    };

    protected static List<TokenPattern> CreateCommonSqlPatterns()
    {
        return new List<TokenPattern>
        {
            // Single-line comments
            new(@"--[^\r\n]*", TokenType.Comment, 10),

            // Multi-line comments
            new(CommonPatterns.MultiLineComment, TokenType.MultiLineComment, 10),

            // Strings (single-quoted)
            new(@"'(?:[^']|'')*'", TokenType.String, 8),

            // Strings (double-quoted for identifiers in some dialects)
            new(@"""(?:[^""]|"""")*""", TokenType.String, 8),

            // Square bracket identifiers
            new(@"\[[^\]]+\]", TokenType.Identifier, 7),

            // Numbers
            new(CommonPatterns.FloatingPoint, TokenType.Number, 5),
            new(CommonPatterns.Integer, TokenType.Number, 4),

            // Parameters
            new(@"@[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Parameter, 6),
            new(@":[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Parameter, 6),

            // Identifiers
            new(CommonPatterns.Identifier, TokenType.Identifier, 2),

            // Operators
            new(@"[+\-*/%=<>!|]+|<>|<=|>=|!=|\|\|", TokenType.Operator, 1),

            // Punctuation
            new(@"[(){}\[\];,.]", TokenType.Punctuation, 0),

            // Whitespace
            new(CommonPatterns.Whitespace, TokenType.PlainText, -1),
        };
    }
}

/// <summary>
/// Tokenizer for Microsoft SQL Server dialect.
/// </summary>
public class MsSqlTokenizer : SqlTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static MsSqlTokenizer()
    {
        var keywords = new Dictionary<string, TokenType>(CommonSqlKeywords, StringComparer.OrdinalIgnoreCase)
        {
            // MSSQL-specific keywords
            ["GO"] = TokenType.SqlKeyword,
            ["TOP"] = TokenType.SqlKeyword,
            ["OFFSET"] = TokenType.SqlKeyword,
            ["FETCH"] = TokenType.SqlKeyword,
            ["NEXT"] = TokenType.SqlKeyword,
            ["ROWS"] = TokenType.SqlKeyword,
            ["ONLY"] = TokenType.SqlKeyword,
            ["PIVOT"] = TokenType.SqlKeyword,
            ["UNPIVOT"] = TokenType.SqlKeyword,
            ["OUTPUT"] = TokenType.SqlKeyword,
            ["INSERTED"] = TokenType.SqlKeyword,
            ["DELETED"] = TokenType.SqlKeyword,
            ["DECLARE"] = TokenType.SqlKeyword,
            ["EXEC"] = TokenType.SqlKeyword,
            ["EXECUTE"] = TokenType.SqlKeyword,
            ["PRINT"] = TokenType.SqlKeyword,
            ["RAISERROR"] = TokenType.SqlKeyword,
            ["THROW"] = TokenType.SqlKeyword,
            ["TRY"] = TokenType.ControlKeyword,
            ["CATCH"] = TokenType.ControlKeyword,
            ["WITH"] = TokenType.SqlKeyword,
            ["CTE"] = TokenType.SqlKeyword,
            ["NOLOCK"] = TokenType.SqlKeyword,
            ["HOLDLOCK"] = TokenType.SqlKeyword,
            ["ROWLOCK"] = TokenType.SqlKeyword,
            ["TABLOCK"] = TokenType.SqlKeyword,
            ["IDENTITY"] = TokenType.SqlKeyword,
            ["NEWID"] = TokenType.SqlFunction,
            ["GETDATE"] = TokenType.SqlFunction,
            ["GETUTCDATE"] = TokenType.SqlFunction,
            ["DATEADD"] = TokenType.SqlFunction,
            ["DATEDIFF"] = TokenType.SqlFunction,
            ["DATEPART"] = TokenType.SqlFunction,
            ["ISNULL"] = TokenType.SqlFunction,
            ["LEN"] = TokenType.SqlFunction,
            ["SUBSTRING"] = TokenType.SqlFunction,
            ["CHARINDEX"] = TokenType.SqlFunction,
            ["REPLACE"] = TokenType.SqlFunction,
            ["UPPER"] = TokenType.SqlFunction,
            ["LOWER"] = TokenType.SqlFunction,
            ["LTRIM"] = TokenType.SqlFunction,
            ["RTRIM"] = TokenType.SqlFunction,
            ["ROW_NUMBER"] = TokenType.SqlFunction,
            ["RANK"] = TokenType.SqlFunction,
            ["DENSE_RANK"] = TokenType.SqlFunction,
            ["NTILE"] = TokenType.SqlFunction,
            ["OVER"] = TokenType.SqlKeyword,
            ["PARTITION"] = TokenType.SqlKeyword,

            // MSSQL-specific types
            ["DATETIME2"] = TokenType.TypeName,
            ["DATETIMEOFFSET"] = TokenType.TypeName,
            ["SMALLDATETIME"] = TokenType.TypeName,
            ["SMALLMONEY"] = TokenType.TypeName,
            ["IMAGE"] = TokenType.TypeName,
            ["UNIQUEIDENTIFIER"] = TokenType.TypeName,
            ["SQL_VARIANT"] = TokenType.TypeName,
            ["HIERARCHYID"] = TokenType.TypeName,
            ["GEOGRAPHY"] = TokenType.TypeName,
            ["GEOMETRY"] = TokenType.TypeName,
        };

        _keywords = keywords;
        _patterns = CreateCommonSqlPatterns();
    }

    public override SyntaxLanguage Language => SyntaxLanguage.MsSql;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}

/// <summary>
/// Tokenizer for Oracle SQL dialect.
/// </summary>
public class OracleSqlTokenizer : SqlTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static OracleSqlTokenizer()
    {
        var keywords = new Dictionary<string, TokenType>(CommonSqlKeywords, StringComparer.OrdinalIgnoreCase)
        {
            // Oracle-specific keywords
            ["ROWNUM"] = TokenType.SqlKeyword,
            ["ROWID"] = TokenType.SqlKeyword,
            ["LEVEL"] = TokenType.SqlKeyword,
            ["CONNECT"] = TokenType.SqlKeyword,
            ["PRIOR"] = TokenType.SqlKeyword,
            ["START"] = TokenType.SqlKeyword,
            ["MINUS"] = TokenType.SqlKeyword,
            ["DUAL"] = TokenType.SqlKeyword,
            ["SEQUENCE"] = TokenType.SqlKeyword,
            ["SYNONYM"] = TokenType.SqlKeyword,
            ["TABLESPACE"] = TokenType.SqlKeyword,
            ["GRANT"] = TokenType.SqlKeyword,
            ["REVOKE"] = TokenType.SqlKeyword,
            ["PACKAGE"] = TokenType.SqlKeyword,
            ["BODY"] = TokenType.SqlKeyword,
            ["TYPE"] = TokenType.SqlKeyword,
            ["CURSOR"] = TokenType.SqlKeyword,
            ["OPEN"] = TokenType.ControlKeyword,
            ["CLOSE"] = TokenType.ControlKeyword,
            ["FETCH"] = TokenType.ControlKeyword,
            ["EXIT"] = TokenType.ControlKeyword,
            ["EXCEPTION"] = TokenType.ControlKeyword,
            ["RAISE"] = TokenType.ControlKeyword,
            ["DECLARE"] = TokenType.SqlKeyword,
            ["PRAGMA"] = TokenType.SqlKeyword,
            ["AUTONOMOUS_TRANSACTION"] = TokenType.SqlKeyword,
            ["BULK"] = TokenType.SqlKeyword,
            ["COLLECT"] = TokenType.SqlKeyword,
            ["FORALL"] = TokenType.ControlKeyword,
            ["LIMIT"] = TokenType.SqlKeyword,

            // Oracle functions
            ["NVL"] = TokenType.SqlFunction,
            ["NVL2"] = TokenType.SqlFunction,
            ["DECODE"] = TokenType.SqlFunction,
            ["SYSDATE"] = TokenType.SqlFunction,
            ["SYSTIMESTAMP"] = TokenType.SqlFunction,
            ["TO_DATE"] = TokenType.SqlFunction,
            ["TO_CHAR"] = TokenType.SqlFunction,
            ["TO_NUMBER"] = TokenType.SqlFunction,
            ["TRUNC"] = TokenType.SqlFunction,
            ["ROUND"] = TokenType.SqlFunction,
            ["SUBSTR"] = TokenType.SqlFunction,
            ["INSTR"] = TokenType.SqlFunction,
            ["LENGTH"] = TokenType.SqlFunction,
            ["LPAD"] = TokenType.SqlFunction,
            ["RPAD"] = TokenType.SqlFunction,
            ["TRIM"] = TokenType.SqlFunction,
            ["INITCAP"] = TokenType.SqlFunction,
            ["CONCAT"] = TokenType.SqlFunction,
            ["GREATEST"] = TokenType.SqlFunction,
            ["LEAST"] = TokenType.SqlFunction,
            ["MONTHS_BETWEEN"] = TokenType.SqlFunction,
            ["ADD_MONTHS"] = TokenType.SqlFunction,
            ["LAST_DAY"] = TokenType.SqlFunction,
            ["NEXT_DAY"] = TokenType.SqlFunction,
            ["EXTRACT"] = TokenType.SqlFunction,
            ["LISTAGG"] = TokenType.SqlFunction,
            ["RANK"] = TokenType.SqlFunction,
            ["DENSE_RANK"] = TokenType.SqlFunction,
            ["ROW_NUMBER"] = TokenType.SqlFunction,
            ["OVER"] = TokenType.SqlKeyword,
            ["PARTITION"] = TokenType.SqlKeyword,
            ["WITHIN"] = TokenType.SqlKeyword,

            // Oracle types
            ["NUMBER"] = TokenType.TypeName,
            ["VARCHAR2"] = TokenType.TypeName,
            ["NVARCHAR2"] = TokenType.TypeName,
            ["RAW"] = TokenType.TypeName,
            ["LONG"] = TokenType.TypeName,
            ["ROWID"] = TokenType.TypeName,
            ["UROWID"] = TokenType.TypeName,
            ["BFILE"] = TokenType.TypeName,
            ["INTERVAL"] = TokenType.TypeName,
            ["YEAR"] = TokenType.TypeName,
            ["MONTH"] = TokenType.TypeName,
            ["DAY"] = TokenType.TypeName,
            ["HOUR"] = TokenType.TypeName,
            ["MINUTE"] = TokenType.TypeName,
            ["SECOND"] = TokenType.TypeName,
            ["XMLTYPE"] = TokenType.TypeName,
            ["SDO_GEOMETRY"] = TokenType.TypeName,
        };

        _keywords = keywords;

        var patterns = CreateCommonSqlPatterns();
        // Add Oracle-specific patterns
        patterns.Insert(0, new TokenPattern(@"&[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Parameter, 6));
        _patterns = patterns;
    }

    public override SyntaxLanguage Language => SyntaxLanguage.OracleSql;
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;
    protected override IReadOnlyDictionary<string, TokenType>? Keywords => _keywords;
}
