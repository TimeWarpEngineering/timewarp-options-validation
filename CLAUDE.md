# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TimeWarp.OptionsValidation is a .NET library that integrates FluentValidation with Microsoft.Extensions.Options to validate configuration settings at startup. The library provides extension methods to configure and validate options using FluentValidation validators.

## Build and Test Commands

### Build
```bash
# Build the entire solution
dotnet build

# Build specific project
cd Source/TimeWarp.OptionsValidation/
dotnet build --configuration Debug
```

### Test
```bash
# Run tests using Fixie (requires tool restore)
cd Tests/TimeWarp.OptionsValidation.Tests/
dotnet tool restore
dotnet restore
dotnet fixie --configuration Debug

# Run tests from solution root
dotnet fixie --configuration Debug --project Tests/TimeWarp.OptionsValidation.Tests/
```

### Package
```bash
# Package is auto-generated on build (GeneratePackageOnBuild=true)
# Output: Source/TimeWarp.OptionsValidation/bin/Packages/

# Build and pack manually if needed
cd Source/TimeWarp.OptionsValidation/
dotnet pack --configuration Release
```

### Tools
```bash
# Restore local tools (defined in .config/dotnet-tools.json)
dotnet tool restore

# Check for outdated packages
dotnet outdated
```

## Architecture

### Core Components

**OptionsValidation<TOptions, TOptionsValidator>**
- Bridge between FluentValidation and Microsoft.Extensions.Options
- Implements `IValidateOptions<TOptions>`
- Invoked automatically by the options framework when options are accessed
- Located in [Source/TimeWarp.OptionsValidation/Configuration/OptionsValidation.cs](Source/TimeWarp.OptionsValidation/Configuration/OptionsValidation.cs)

**ServiceCollectionExtensions**
- Provides `ConfigureOptions<TOptions, TOptionsValidator>` extension methods
- Two overloads: one accepts `IConfiguration`, the other accepts `Action<TOptions>`
- Automatically discovers configuration section names via `SectionNameAttribute` or defaults to type name
- Registers both the validator and the `IValidateOptions<TOptions>` implementation
- Located in [Source/TimeWarp.OptionsValidation/Extensions/ServiceCollectionExtensions.cs](Source/TimeWarp.OptionsValidation/Extensions/ServiceCollectionExtensions.cs)

**SectionNameAttribute**
- Allows overriding the configuration section name
- Applied to options classes when the section name differs from the class name
- Located in [Source/TimeWarp.OptionsValidation/Configuration/SectionNameAttribute.cs](Source/TimeWarp.OptionsValidation/Configuration/SectionNameAttribute.cs)

### Usage Pattern

1. Define an options class (e.g., `MyOptions`)
2. Create a FluentValidation validator (e.g., `MyOptionsValidator : AbstractValidator<MyOptions>`)
3. Optionally decorate options class with `[SectionName("ConfigSectionName")]`
4. Register in DI: `services.ConfigureOptions<MyOptions, MyOptionsValidator>(configuration)`
5. Validation executes automatically when options are first accessed

### Project Structure

```
/Source/TimeWarp.OptionsValidation/     - Main library
  /Configuration/                       - Core validation logic
  /Extensions/                          - DI extension methods
/Tests/TimeWarp.OptionsValidation.Tests/ - Test project using Fixie
```

## Build Configuration

### Central Package Management
- Uses `Directory.Packages.props` for centralized version management
- `ManagePackageVersionsCentrally` is enabled
- Package references in .csproj files don't specify versions

### Common Properties (Directory.Build.props)
- **Target Framework**: net8.0
- **LangVersion**: preview
- **Nullable**: enabled
- **TreatWarningsAsErrors**: true
- **ImplicitUsings**: enabled
- **Package Version**: Defined in Directory.Build.props (1.0.0-beta.3)
- **Embedded Resources**: Auto-embeds `.scriban` and `.cstemplate` files

### Package Metadata
- NuGet packages include: logo.png, readme.md, and license files
- All asset references use lowercase filenames
- PackageOutputPath: `./bin/Packages`

## Testing Framework

Uses **Fixie** (not xUnit/NUnit/MSTest):
- Custom test discovery convention via TimeWarp.Fixie
- Test classes end with `_Should_` by convention
- Test methods are public static void methods
- Supports `[Skip]` attribute for skipping tests
- Supports `[TestTag]` for categorization
- Supports `[Input]` for parameterized tests
- FluentAssertions for assertions

## File Naming Conventions

All repository root files use **lowercase names**:
- `license` (not LICENSE)
- `readme.md` (not README.md)
- `assets/logo.png` (not Assets/Logo.png)

When updating package metadata, ensure references match these lowercase conventions.
