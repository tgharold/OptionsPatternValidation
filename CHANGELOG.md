# Changelog

All notable changes to this project are documented in this file.

The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project uses [Semantic Versioning](https://semver.org/).

Breaking changes are marked **BREAKING**.

## Unreleased

## 1.5.0 - 2026-10-02

### Changed

- The README now explains that an exception thrown by a validation attribute, or a regular expression timeout, reaches the caller unwrapped, and that the validator has no limit on nesting depth, so a very deep object graph can overflow the stack.
- Update to `RecursiveDataAnnotationsValidation` 2.3.3. The validator now tracks visited objects by reference, not by value. Two separate objects that are equal by value, such as two identical records in a list, are now each validated and each report their own errors. Before, only the first was validated. Settings that passed because of this gap can now fail at startup. An object that equals one of its own ancestors has its own attributes checked, but its properties are not walked.

### Fixed

- An object that points to itself and compares by value, such as a record or a class that overrides `Equals` and `GetHashCode`, no longer overflows the stack during validation on .NET. Two such objects that point to each other with equal values can still overflow it. On .NET Framework, a self-referencing record can still overflow it.
- A property of type `Uri` that holds a relative `Uri`, such as the value `/api` bound from configuration, no longer makes validation throw. The validator no longer walks the properties of `Uri`, `Type`, `Assembly`, delegates and a few other framework types. Attributes on the property that holds them, such as `[Required]`, still run.

## 1.4.3 - 2026-10-01

No library changes. The assemblies in the NuGet package are the same as v1.4.2.

### Changed

- The README now explains that the validator only sees what the configuration binder produces. An array element with an unknown enum name is dropped without an error, and validation passes. It also notes that `[MinLength]` throws on a `List<T>` property on .NET Framework.

## 1.4.2 - 2026-10-01

No library changes. The assemblies in the NuGet package are the same as v1.4.1.

### Added

- The package now declares its README, so nuget.org shows it on the package page. Before, the file was in the package but not listed in the package metadata.
- Each release now builds a symbol package (`.snupkg`) and attaches it to the GitHub release.

### Changed

- The package README is now the repository README, without a separate copy. The "Legacy" section is renamed "History".
- Release workflow: run the tests on .NET 8, .NET 10 and .NET Framework 4.8.1, on Linux and Windows, before publishing.
- Release workflow: build the package and create a draft GitHub release first. Then wait for approval before the nuget.org push and the release publish.

## 1.4.1 - 2026-10-01

### Changed

- `RecursiveDataAnnotationValidateOptions<T>.Validate` now checks `options` itself and throws `ArgumentNullException` for the `options` parameter when it is null. A skipped named validator still returns `Skip`.
- The zip file attached to each GitHub release now contains `LICENSE`, `README.md` and the `.nupkg`. It used to contain the `.dll`, `.pdb` and `.xml` files. The `.nupkg` is still attached to the release on its own.
- Release workflow: publish to nuget.org with Trusted Publishing instead of a stored API key.
- Release workflow: require release tags to be on `master`.
- Release workflow: create the GitHub release as a draft and publish it last.
- Release workflow: replace the archived release actions with the GitHub CLI, and pin every action to a commit SHA.
- The README now says that attribute error messages are copied as-is into `OptionsValidationException`.

## 1.4.0 - 2026-03-29

### Added

- The NuGet package now includes the README.

### Changed

- Update to `Microsoft.Extensions.*` 8.0 and `RecursiveDataAnnotationsValidation` 2.1.1.
- Update test and example project dependencies.

## 1.3.1 - 2026-03-28

The library still targets .NET Standard 2.0. The .NET 8 change applies to the build, tests and example project only.

### Changed

- Build and example projects now target .NET 8.
- Add `CLAUDE.md` for AI coding assistants.

## 1.3.0 - 2022-11-23

### Changed

- Update to `RecursiveDataAnnotationsValidation` 2.0.0. Member names for items inside an `IEnumerable` property now include the item index in square brackets, so validation messages change. Old: `Items.SimpleA.BoolC`. New: `Items[1].SimpleA.BoolC`. Update any code that parses or matches these messages.

## 1.2.0 - 2022-07-15

### Added

- More tests for `AddValidatedSettings`.

### Changed

- Update to `Microsoft.Extensions.*` 6.0.

## 1.1.4 - 2022-06-23

### Changed

- Update package dependencies. No other library changes.

## 1.1.3 - 2022-03-03

### Changed

- `OptionsBuilderDataAnnotationsExtensions.RecursivelyValidateDataAnnotations` is now public, so you can use it on your own `OptionsBuilder<T>`.

## 1.1.2 - 2022-01-11

### Fixed

- `GetValidatedConfigurationSection<T>` now creates a `new T()` when the configuration section is missing. Before, a missing section gave a null object. `T` now needs a public parameterless constructor. `AddEagerlyValidatedSettings<T>` uses this method, so it gets the same fix.

## 1.1.1 - 2022-01-10

### Changed

- Update to `RecursiveDataAnnotationsValidation` 1.1.0 and `Microsoft.Extensions.*` 3.1.20.

### Fixed

- Remove the unused `FluentMigrator.Runner.Core` package reference.

## 1.1.0 - 2022-01-10

### Added

- `GetValidatedConfigurationSection<T>()`, an `IConfiguration` extension method that fetches a section and validates it right away.

### Changed

- `AddEagerlyValidatedSettings<T>` now uses `GetValidatedConfigurationSection<T>()`.

## 1.0.2 - 2020-03-26

### Added

- XML documentation for the public types and methods, shipped with the package.

## 1.0.1 - 2020-03-26

### Added

- `AddEagerlyValidatedSettings<T>`, which binds a section, validates it right away and returns the options object. This method is experimental. It reads the values once, so it does not pick up later configuration changes.

## 1.0.0 - 2020-03-20

First stable release. Adds README and test improvements. No library changes.

## 0.9.1 - 2020-03-19

First automated push to nuget.org.

## 0.9.0 - 2020-03-11

First version ready for use. Includes:

- `AddSettings<T>` and `AddValidatedSettings<T>` to bind a configuration section to an options class.
- `AddValidatedSettings<T, TValidator>` to add your own `IValidateOptions<T>` validator.
- Recursive DataAnnotations validation of options objects.
- `[SettingsSectionName]` to choose the configuration section name. Without it, the section name is the class name.
