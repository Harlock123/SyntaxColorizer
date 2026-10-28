# AGENTS.md

Notes for anyone, human or AI agent, changing this code. `README.md` covers using the control, its properties,
themes, linting and how to add a language; this file covers how the code fits together and how to change it
safely.

## What this is

`SyntaxColorizer`, a syntax-highlighting text box control for Avalonia 11, published as a NuGet package
(`PackageId` `SyntaxColorizer`, version 1.1.0 in the csproj), plus a demo app. Both target .NET 8 (`net8.0`). The
control is built on `Avalonia.AvaloniaEdit`: it wraps a `TextEditor` and colours it with its own regex
tokenizers. It runs on Windows, macOS and Linux.

## Commands

```bash
dotnet build SyntaxColorizer.sln                  # no global.json: any SDK that builds net8.0
dotnet run --project src/SyntaxColorizer.Demo     # the demo: switch languages and themes, run the linter
dotnet pack src/SyntaxColorizer -c Release -o dist   # the NuGet package and its .snupkg symbols
```

There are no tests yet. `src/SyntaxColorizer/SyntaxColorizer.sln` is a second solution holding only the library;
use the root `SyntaxColorizer.sln`, which has both projects.

## Layout

- `src/SyntaxColorizer/Controls/`:
  - `SyntaxHighlightingTextBox.cs`: the `TemplatedControl` users place. Its styled properties (`Text`, `Language`,
    `SyntaxTheme`, `ShowLineNumbers`, indenting, bracket closing, `UpdateDelay` and more) drive the inner editor.
  - `SyntaxHighlightingTextBox.axaml`: the control template, with the `TextEditor` named `PART_Editor`.
  - `SyntaxHighlightingTransformer.cs`: an AvaloniaEdit `DocumentColorizingTransformer`. It tokenizes the whole
    document, caches the tokens until the text changes, and colours each line from the cache.
- `src/SyntaxColorizer/Tokenization/`: `Token`, `TokenType`, `ILanguageTokenizer`, `LanguageTokenizerBase`
  (pattern- and keyword-driven, with shared `CommonPatterns`), and `TokenizerFactory`, which creates and caches
  one tokenizer per language.
  - `Languages/`: one tokenizer per language. Some files hold more than one, such as `SqlTokenizer.cs`, which has
    `MsSqlTokenizer` and `OracleSqlTokenizer`.
- `src/SyntaxColorizer/SyntaxLanguage.cs`: the language enum (`None` plus 38 languages).
- `src/SyntaxColorizer/Themes/`: `SyntaxTheme` (brushes per token type) and `BuiltInThemes` (14 themes;
  `Default` is Visual Studio Light).
- `src/SyntaxColorizer/Linting/`: `ILinter`, `BaseLinter`, `CommonLinter`, `LinterFactory` and the hint types.
- `src/SyntaxColorizer/Formatting/BasicCodeFormatter.cs`: a simple per-language re-indenter.
- `src/SyntaxColorizer.Demo/`: the demo window.
- `docs/images/`: screenshots used by the README.

## Rules that keep it working

- **A new language touches three places.** Add the tokenizer under `Tokenization/Languages/`, a value in
  `SyntaxLanguage`, and a case in `TokenizerFactory.CreateTokenizer`. If it should be linted or formatted, also
  check `LinterFactory` and `BasicCodeFormatter`. The README walks through the tokenizer part.
- **Keep the AvaloniaEdit styles in the control's own styles.** `SyntaxHighlightingTextBox.axaml` includes
  AvaloniaEdit's Fluent styles itself, so users only add one `StyleInclude`. Version 1.0.2 fixed broken keyboard
  input in the NuGet package this way; don't remove it.
- **The public API is documented.** `GenerateDocumentationFile` is on, so public members need XML doc comments,
  or the build warns.
- **The counts appear in several places.** The language and theme counts are in the csproj `Description` and the
  README. Update both when you add one.

## Making a change

- Check changes in the demo, since there are no tests. Try a few languages and both light and dark themes.
- Update `README.md` (supported languages, themes, properties) to match, and refresh `docs/images/` if the look
  changes.
- Bump `Version` and `PackageReleaseNotes` in `src/SyntaxColorizer/SyntaxColorizer.csproj` for a new package.
  The release workflow overrides `Version` from the tag.

## CI and releases

`.github/workflows/release.yml` is the only workflow. It runs for a `v*` tag, or by hand with a version number;
ordinary pushes are not built. It installs .NET 8 and publishes the demo as self-contained single files for
win-x64/x86/arm64, osx-x64/arm64 and linux-x64/arm64. It then packs the NuGet package, writes `SHA256SUMS`, and
attaches everything to the GitHub release `v<version>`. It does not push the package to nuget.org.
