# OptionsPatternValidation

Extension methods (on `IServiceCollection` and `IConfiguration`) that make it easier to wire up `IOptions<T>` POCOs (plain old C# objects) with validation.

- [OptionsPatternValidation](#optionspatternvalidation)
- [Installation](#installation)
  - [.NET Core](#net-core)
  - [Package Manager](#package-manager)
- [Usage](#usage)
  - [Create POCOs](#create-pocos)
  - [Choose a validation](#choose-a-validation)
    - [No validation](#no-validation)
    - [DataAnnotation validation](#dataannotation-validation)
    - [IValidateOptions](#ivalidateoptions)
  - [Accessing Configuration in Startup](#accessing-configuration-in-startup)
  - [Experimental](#experimental)
    - [AddEagerlyValidatedSettings<T>(config, out x)](#addeagerlyvalidatedsettingstconfig-out-x)
- [Build Status](#build-status)
- [Nuget Page](#nuget-page)
- [Legacy](#legacy)

# Installation

## .NET Core

    $ dotnet add package OptionsPatternValidation

## Package Manager

    PM> Install-Package OptionsPatternValidation

# Usage

For most use cases where you want validation, I suggest using the [DataAnnotation](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) approach.  In my experience, most application settings can be validated with a simple `[Required]` or `[Range(N,M)]` attribute on the property.

## Create POCOs

Each "section" (top level) of your appsettings.json file will need its own POCO (Plain Old C# Object).  This is true no matter which validation approach you use.  

If the POCO class name is not identical to the appsettings.json section name, you can use the `[SettingsSectionNameAttribute("section-name")]` attribute on the POCO class definition to set the mapping.

Under the covers, this package is calling [GetSection() in Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/system.configuration.configuration.getsection) and [Bind() in Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationbinder.bind) with the same capabilities and restrictions.  Prior to .NET 6.0 (which adds the [ConfigurationKeyName](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationkeynameattribute) attribute) the property names on the POCO must match the names in the appsettings JSON or environment variables.

## Choose a validation

The POCOs for the options pattern will all get wired up in `Startup.ConfigureServices()` method in your application.

### No validation

If you do not want to wire up validation for the POCO, use the `AddSettings<T>(config)` method.

    services.AddSettings<ExampleAppSettings>(config);

### DataAnnotation validation

This method signature implements support for [DataAnnotation](https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1) validation of the POCO.  Including recursive validaton of all sub-objects and collections of objects on the POCO.

    services.AddValidatedSettings<ExampleAppSettings>(config);

Recursive validation of the object and its child objects is provided via [RecursiveDataAnnotationsValidation](https://www.nuget.org/packages/RecursiveDataAnnotationsValidation).

### IValidateOptions

This approach requires two classes.  One is the POCO for the settings.  The other is the class that derives from `IValidateOptions<T>` and implements the `Validate()` method.

    services.AddValidatedSettings<ExampleAppSettings, ExampleAppSettingsValidator>(config);

The `IValidationOptions<T>` approach is really powerful, but also tedious to use.

## Accessing Configuration in Startup

Sometimes in the Startup.ConfigureServices() method you want to access your settings POCOs with validation.  This can be performed using the `GetValidatedConfigurationSection<T>()` method. It does *not* register the settings class as `IOptions<T>` in the container, therefore you still need to make a call to `AddSettings<T>(config)` or `AddValidatedSettings<T>(config)`

    var appSettings = config.GetValidatedConfigurationSection<ExampleAppSettings>();

Note that if the section is completely missing from your configuration, the POCO will still be created and validation will run against the default values within the POCO class.

## Experimental

### AddEagerlyValidatedSettings<T>(config, out x)
                                                    
OBSOLETE: Will be removed at some point.  Use the `GetValidatedConfigurationSection<T>()` method instead.

There is an experimental extension method that will eagerly validate the object and also return a reference to the POCO.  But it is not compatible with situations where you are using `IOptionsMonitor<T>` in your code.  Because it wires up a snapshot of the configuration at the time of startup, the "on-change" listeners will not fire when underlying configuration values change.  

    services.AddEagerlyValidatedSettings<ExampleAppSettings>(
        configuration, 
        out var exampleAppSettings
        );

# Build Status

![.NET Core](https://github.com/tgharold/OptionsPatternValidation/workflows/.NET%20Core/badge.svg)

# Nuget Page

https://www.nuget.org/packages/OptionsPatternValidation/

# Legacy

This grew out of [experiments with the .NET Core options pattern](https://github.com/tgharold/DotNetCore-ConfigurationOptionsValidationExamples) and the desire to simplify how sections in the appsettings.json / .NET configuration system get wired up to POCOs and validation for those POCOs.
