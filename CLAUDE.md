# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET Standard library for validating objects that follow the Options pattern, with specific support for ensuring that options are correctly validated.

The core functionality includes:
- Validation of objects implementing the Options pattern
- Support for custom validation logic specific to options usage
- Integration with built-in .NET validation capabilities

## Project Structure

The codebase has a clear separation between the main library and tests:

1. **src/OptionsPatternValidation/** - The core library project with:
   - The public entry points: `ServiceCollectionExtensions.cs` and `ConfigurationExtensions.cs`
   - `Extensions/OptionsBuilderDataAnnotationsExtensions.cs`, which adds `RecursivelyValidateDataAnnotations()` to `OptionsBuilder<T>`
   - `ValidationOptions/RecursiveDataAnnotationValidateOptions.cs`, the internal `IValidateOptions<T>` that calls the RecursiveDataAnnotationsValidation package
   - `SettingsSectionNameAttribute.cs`, which maps a POCO to a configuration section

2. **test/OptionsPatternValidation.Tests/** - Test project with:
   - Tests grouped by the class they cover, such as `ServiceCollectionExtensions/` and `ValidationOptions/`
   - Settings POCOs used as test models in `Settings/`
   - JSON test configuration in `Json/`

3. **examples/** - Two example web projects showing usage patterns

## Commands for Development

### Build
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

### Run Single Test
```bash
dotnet test test/OptionsPatternValidation.Tests/OptionsPatternValidation.Tests.csproj --filter "SpecificTestName"
```

### Run Tests with Coverage
```bash
dotnet test test/OptionsPatternValidation.Tests/OptionsPatternValidation.Tests.csproj --collect:"XPlat Code Coverage"
```

## Key Files to Understand

- `src/OptionsPatternValidation/ValidationOptions/RecursiveDataAnnotationValidateOptions.cs` - Runs the recursive validation and formats the error messages
- `src/OptionsPatternValidation/ServiceCollectionExtensions.cs` - The `AddSettings`, `AddValidatedSettings` and `AddEagerlyValidatedSettings` methods
- Settings POCOs in `test/OptionsPatternValidation.Tests/Settings/` show various usage patterns

## Target Frameworks

- Main library targets `.NET Standard 2.0`
- Test project targets `net8.0` and `net10.0`. Add `-p:IncludeNetFramework=true` to also target `net481`, which runs only on Windows. CI runs it there.

## Development Notes

The options validator handles:
1. Validation of objects following the Options pattern
2. Proper error message formatting that includes property paths
3. Integration with .NET built-in validation mechanisms
4. Support for custom validation attributes specific to options usage

The validator uses reflection to examine object properties and validate them according to the Options pattern requirements.

## Changelog

`CHANGELOG.md` follows the Keep a Changelog format. When you change the library's public API, validation behavior or NuGet package contents, add an entry under `Unreleased` in the same change. Use the headings Added, Changed, Deprecated, Removed, Fixed and Security.

Mark breaking changes with **BREAKING** and describe what callers must change. Breaking changes include changed configuration section naming, changed error messages that callers might parse, and removed or renamed public members. Keep entries short. Build, test and CI-only changes need an entry only when they affect what ships.

See `RELEASE-PROCESS.md` for how the changelog fits into a release.
