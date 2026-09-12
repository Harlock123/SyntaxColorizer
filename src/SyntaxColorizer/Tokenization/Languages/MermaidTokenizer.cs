namespace SyntaxColorizer.Tokenization.Languages;

/// <summary>
/// Tokenizer for Mermaid diagram definitions.
///
/// Mermaid is a family of small languages rather than one - a flowchart, a sequence diagram
/// and a Gantt chart share a file format and very little else - so the words recognised here
/// are the union of them, and a word that means something in one diagram is coloured wherever
/// it appears. Colouring a keyword that happens to be a node name elsewhere is a far smaller
/// fault than leaving every diagram but flowcharts plain.
///
/// The link between two nodes is the part that carries the syntax: "--&gt;", "-.-&gt;", "==&gt;",
/// "&lt;|--", "||--o{" and a good many more. They are matched as a shaft of dashes, equals,
/// dots or tildes with an optional head at either end, which covers the lot without a list of
/// every arrangement.
/// </summary>
public class MermaidTokenizer : LanguageTokenizerBase
{
    private static readonly IReadOnlyList<TokenPattern> _patterns;
    private static readonly IReadOnlyDictionary<string, TokenType> _keywords;

    static MermaidTokenizer()
    {
        _keywords = new Dictionary<string, TokenType>(StringComparer.Ordinal)
        {
            // The word that says which kind of diagram this is.
            { "flowchart", TokenType.TypeDeclaration },
            { "graph", TokenType.TypeDeclaration },
            { "sequenceDiagram", TokenType.TypeDeclaration },
            { "classDiagram", TokenType.TypeDeclaration },
            { "stateDiagram", TokenType.TypeDeclaration },
            { "stateDiagram-v2", TokenType.TypeDeclaration },
            { "erDiagram", TokenType.TypeDeclaration },
            { "journey", TokenType.TypeDeclaration },
            { "gantt", TokenType.TypeDeclaration },
            { "pie", TokenType.TypeDeclaration },
            { "quadrantChart", TokenType.TypeDeclaration },
            { "requirementDiagram", TokenType.TypeDeclaration },
            { "gitGraph", TokenType.TypeDeclaration },
            { "mindmap", TokenType.TypeDeclaration },
            { "timeline", TokenType.TypeDeclaration },
            { "sankey-beta", TokenType.TypeDeclaration },
            { "xychart-beta", TokenType.TypeDeclaration },
            { "block-beta", TokenType.TypeDeclaration },
            { "packet-beta", TokenType.TypeDeclaration },
            { "architecture-beta", TokenType.TypeDeclaration },
            { "kanban", TokenType.TypeDeclaration },
            { "zenuml", TokenType.TypeDeclaration },
            { "C4Context", TokenType.TypeDeclaration },
            { "C4Container", TokenType.TypeDeclaration },
            { "C4Component", TokenType.TypeDeclaration },
            { "C4Dynamic", TokenType.TypeDeclaration },
            { "C4Deployment", TokenType.TypeDeclaration },

            // Structure.
            { "subgraph", TokenType.Keyword },
            { "end", TokenType.Keyword },
            { "direction", TokenType.Keyword },
            { "section", TokenType.Keyword },
            { "title", TokenType.Keyword },
            { "accTitle", TokenType.Keyword },
            { "accDescr", TokenType.Keyword },
            { "state", TokenType.Keyword },
            { "class", TokenType.Keyword },
            { "namespace", TokenType.Keyword },
            { "participant", TokenType.Keyword },
            { "actor", TokenType.Keyword },
            { "box", TokenType.Keyword },
            { "requirement", TokenType.Keyword },
            { "element", TokenType.Keyword },
            { "root", TokenType.Keyword },
            { "as", TokenType.Keyword },
            { "note", TokenType.Keyword },
            { "over", TokenType.Keyword },
            { "of", TokenType.Keyword },
            { "for", TokenType.Keyword },

            // Styling, and the things that hang off it.
            { "style", TokenType.Keyword },
            { "classDef", TokenType.Keyword },
            { "linkStyle", TokenType.Keyword },
            { "cssClass", TokenType.Keyword },
            { "click", TokenType.Keyword },
            { "callback", TokenType.Keyword },
            { "link", TokenType.Keyword },

            // Blocks that hold other statements.
            { "loop", TokenType.ControlKeyword },
            { "alt", TokenType.ControlKeyword },
            { "else", TokenType.ControlKeyword },
            { "opt", TokenType.ControlKeyword },
            { "par", TokenType.ControlKeyword },
            { "and", TokenType.ControlKeyword },
            { "critical", TokenType.ControlKeyword },
            { "option", TokenType.ControlKeyword },
            { "break", TokenType.ControlKeyword },
            { "rect", TokenType.ControlKeyword },
            { "activate", TokenType.ControlKeyword },
            { "deactivate", TokenType.ControlKeyword },
            { "autonumber", TokenType.ControlKeyword },
            { "create", TokenType.ControlKeyword },
            { "destroy", TokenType.ControlKeyword },

            // Source control diagrams.
            { "commit", TokenType.Keyword },
            { "branch", TokenType.Keyword },
            { "checkout", TokenType.Keyword },
            { "merge", TokenType.Keyword },
            { "cherry-pick", TokenType.Keyword },

            // Gantt charts.
            { "dateFormat", TokenType.Keyword },
            { "axisFormat", TokenType.Keyword },
            { "tickInterval", TokenType.Keyword },
            { "excludes", TokenType.Keyword },
            { "includes", TokenType.Keyword },
            { "todayMarker", TokenType.Keyword },
            { "weekday", TokenType.Keyword },
            { "showData", TokenType.Keyword },

            // The ways a diagram can be laid out.
            { "TB", TokenType.Constant },
            { "TD", TokenType.Constant },
            { "BT", TokenType.Constant },
            { "RL", TokenType.Constant },
            { "LR", TokenType.Constant },

            { "true", TokenType.Constant },
            { "false", TokenType.Constant },
        };

        // The heads a link can carry at either end, longest first so that "<|" is not read as
        // a bare "<" with the bar left over, and ">>" not as two arrows.
        const string LeftHead = @"(?:<\|:?|<<|\}o|\}\||\|\||\|o|<|[ox*(])?";
        const string RightHead = @"(?:\|>|>>|o\{|o\||\|\||\|\{|>|[ox*)])?";

        // Dashes, equals, dots or tildes: the shaft the heads sit on. One is enough, because a
        // sequence diagram draws its arrows with a single dash - "->>", "-x" and "-)" are all
        // links - and a lone dash elsewhere is punctuation either way. A hyphen inside a word
        // is safe: the word is read whole before the dash is ever reached.
        const string Shaft = @"[-=.~]+";

        _patterns = new List<TokenPattern>
        {
            // A directive, which configures the diagram rather than drawing it.
            new TokenPattern(@"%%\{[\s\S]*?\}%%", TokenType.Preprocessor, 110),

            // A comment, which takes the rest of its line.
            new TokenPattern(@"%%[^\r\n]*", TokenType.Comment, 100),

            // Text, which is where anything at all may appear and so outranks the rest.
            new TokenPattern(@"""(?:[^""\\\r\n]|\\.)*""", TokenType.String, 95),
            new TokenPattern(@"'(?:[^'\\\r\n]|\\.)*'", TokenType.String, 94),

            // A state diagram's start and finish.
            new TokenPattern(@"\[\*\]", TokenType.Constant, 90),

            // The line between two nodes, however it is drawn.
            new TokenPattern(LeftHead + Shaft + RightHead, TokenType.Operator, 85),

            // A colour, in a style or a class definition.
            new TokenPattern(@"#[0-9a-fA-F]{3,8}\b", TokenType.Number, 80),

            // What a style sets, and what it sets it to.
            new TokenPattern(@"\b(?:fill|stroke|stroke-width|stroke-dasharray|color|font-size|font-weight|font-family|opacity|padding|margin|rx|ry)(?=\s*:)",
                TokenType.CssProperty, 75),

            // A date or a duration, as a Gantt chart states them.
            new TokenPattern(@"\b\d{4}-\d{2}-\d{2}(?:[T ]\d{2}:\d{2}(?::\d{2})?)?\b", TokenType.Number, 70),
            new TokenPattern(@"\b\d+(?:\.\d+)?(?:px|em|rem|%|d|h|w|ms|s)\b", TokenType.Number, 68),
            new TokenPattern(@"\b\d+(?:\.\d+)?\b", TokenType.Number, 65),

            // A word. Hyphens are taken in when a letter follows, so that "stateDiagram-v2"
            // and "cherry-pick" stay whole while "a-->b" does not swallow its own arrow.
            new TokenPattern(@"\b[a-zA-Z_][a-zA-Z0-9_]*(?:-[a-zA-Z][a-zA-Z0-9_]*)*\b",
                TokenType.Identifier, 40),

            // The brackets that give a node its shape, and the bars round a link's words.
            // The slashes are shapes too: "[/this/]" is a parallelogram and "[/this\]" a
            // trapezium, so they belong with the brackets rather than being left plain.
            new TokenPattern(@"[(){}\[\]|/\\]", TokenType.Punctuation, 20),

            new TokenPattern(@"[:;,&+]", TokenType.Operator, 15),

            new TokenPattern(@"\s+", TokenType.PlainText, 0),
        };
    }

    /// <inheritdoc/>
    public override SyntaxLanguage Language => SyntaxLanguage.Mermaid;

    /// <inheritdoc/>
    protected override IReadOnlyList<TokenPattern> Patterns => _patterns;

    /// <inheritdoc/>
    protected override IReadOnlyDictionary<string, TokenType> Keywords => _keywords;
}
