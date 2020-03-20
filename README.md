# OptionsPatternValidation

Extension methods (on `IServiceCollection`) that make it easier to wire up `IOptions<T>` POCOs (plain old C# objects) and validation.

- [OptionsPatternValidation](#optionspatternvalidation)
  - [Installation](#installation)
    - [.NET Core](#net-core)
    - [Package Manager](#package-manager)
  - [Usage](#usage)
    - [Create POCOs](#create-pocos)
    - [Choose a validation](#choose-a-validation)
      - [No validation](#no-validation)
      - [Recursive DataAnnotation validation](#recursive-dataannotation-validation)
      - [IValidateOptions](#ivalidateoptions)
  - [Build Status](#build-status)
  - [Nuget Page](#nuget-page)
  - [Legacy](#legacy)

## Installation

### .NET Core

    $ dotnet add package OptionsPatternValidation

### Package Manager

    PM> Install-Package OptionsPatternValidation

## Usage

For most use cases where you want validation, I suggest using the [DataAnnotation](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) approach.  In my experience, most application settings can be validated with a simple `[Required]` or `[Range(N,M)]` attribute on the property.

### Create POCOs

Each "section" (top level) of your appsettings.json file will need its own POCO.  This is true no matter which validation approach you use.  If the POCO class is not identical to the appsettings.json section name, you can use the `[SettingsSectionNameAttribute("section-name")]` attribute on the POCO class definition to do the mapping.

### Choose a validation

The POCOs for the options pattern will all get wired up in `Startup.ConfigureServices()` in your application.

#### No validation

If you do not want to wire up validation for the POCO, use the `AddSettings<T>(config)` method.

    services.AddSettings<ExampleAppSettings>(Configuration);

#### Recursive DataAnnotation validation

This method signature implements support for [DataAnnotation](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) validation of the POCO.  Including recursive validaton of all sub-objects on the POCO.

    services.AddValidatedSettings<ExampleAppSettings>(Configuration);

#### IValidateOptions

This approach requires two classes.  One is the POCO for the settings.  The other is the class that derives from `IValidateOptions<T>` and implements the `Validate()` method.

    services.AddValidatedSettings<ExampleAppSettings, ExampleAppSettingsValidator>(Configuration);

The `IValidationOptions<T>` approach is really powerful, but also tedious to use.

## Build Status

![.NET Core](https://github.com/tgharold/OptionsPatternValidation/workflows/.NET%20Core/badge.svg)

## Nuget Page

https://www.nuget.org/packages/OptionsPatternValidation/

## Legacy

This grew out of [experiments with the .NET Core options pattern](https://github.com/tgharold/DotNetCore-ConfigurationOptionsValidationExamples) and the desire to simplify how sections in the appsettings.json / .NET configuration system get wired up to POCOs and validation for those POCOs.
