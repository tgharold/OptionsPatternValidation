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
   - Main validator implementation (`OptionsValidator.cs`)
   - Options pattern validation logic
   - Extension methods and interfaces

2. **test/OptionsPatternValidation.Tests/** - Test project with:
   - Tests for various validation scenarios
   - Example test models showing options pattern usage

3. **examples/** - Example project showing usage patterns

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

- `OptionsValidator.cs` - Main implementation that handles options pattern validation
- Test models in `test/OptionsPatternValidation.Tests/TestModels/` show various usage patterns

## Target Frameworks

- Main library targets `.NET Standard 2.0`
- Test project targets `net8.0`

## Development Notes

The options validator handles:
1. Validation of objects following the Options pattern
2. Proper error message formatting that includes property paths
3. Integration with .NET built-in validation mechanisms
4. Support for custom validation attributes specific to options usage

The validator uses reflection to examine object properties and validate them according to the Options pattern requirements.