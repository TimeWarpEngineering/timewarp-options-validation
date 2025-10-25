[![Dotnet](https://img.shields.io/badge/dotnet-10.0-blue)](https://dotnet.microsoft.com)
[![Stars](https://img.shields.io/github/stars/TimeWarpEngineering/timewarp-options-validation?logo=github)](https://github.com/TimeWarpEngineering/timewarp-options-validation)
[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)
[![workflow](https://github.com/TimeWarpEngineering/timewarp-options-validation/actions/workflows/release-build.yml/badge.svg)](https://github.com/TimeWarpEngineering/timewarp-options-validation/actions)
[![nuget](https://img.shields.io/nuget/v/TimeWarp.OptionsValidation?logo=nuget)](https://www.nuget.org/packages/TimeWarp.OptionsValidation/)
[![nuget](https://img.shields.io/nuget/dt/TimeWarp.OptionsValidation?logo=nuget)](https://www.nuget.org/packages/TimeWarp.OptionsValidation/)
[![Issues Open](https://img.shields.io/github/issues/TimeWarpEngineering/timewarp-options-validation.svg?logo=github)](https://github.com/TimeWarpEngineering/timewarp-options-validation/issues)
[![Forks](https://img.shields.io/github/forks/TimeWarpEngineering/timewarp-options-validation)](https://github.com/TimeWarpEngineering/timewarp-options-validation)
[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-options-validation.svg?style=flat-square&logo=github)](https://github.com/TimeWarpEngineering/timewarp-options-validation/issues)
[![Twitter](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Fgithub.com%2FTimeWarpEngineering%2Ftimewarp-options-validation)](https://twitter.com/intent/tweet?url=https://github.com/TimeWarpEngineering/timewarp-options-validation)

[![Twitter](https://img.shields.io/twitter/follow/StevenTCramer.svg)](https://twitter.com/intent/follow?screen_name=StevenTCramer)
[![Twitter](https://img.shields.io/twitter/follow/TheFreezeTeam1.svg)](https://twitter.com/intent/follow?screen_name=TheFreezeTeam1)

# TimeWarp.OptionsValidation

![TimeWarp Logo](assets/logo.png)

TimeWarp.OptionsValidation integrates FluentValidation with Microsoft.Extensions.Options to provide automatic validation of your configuration settings at application startup.

## Why Use This Library?

Configuration errors are a common source of runtime failures. TimeWarp.OptionsValidation helps you **fail fast** by validating all configuration settings when your application starts, rather than discovering errors when the configuration is first accessed (which could be hours or days later in production).

**Key Benefits:**
- Validates configuration settings using FluentValidation rules
- Integrates seamlessly with Microsoft.Extensions.Options
- Catches configuration errors at startup, not at runtime
- Provides clear, actionable error messages
- Supports both IConfiguration binding and programmatic configuration

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Installation

```console
dotnet add package TimeWarp.OptionsValidation
```

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [TimeWarp.OptionsValidation](https://www.nuget.org/packages/TimeWarp.OptionsValidation/) [![nuget](https://img.shields.io/nuget/v/TimeWarp.OptionsValidation?logo=nuget)](https://www.nuget.org/packages/TimeWarp.OptionsValidation/)

## Usage

### Basic Setup with Automatic Startup Validation

Use `AddFluentValidatedOptions()` which returns `OptionsBuilder<T>`, allowing you to chain with `.ValidateOnStart()` for automatic startup validation.

#### 1. Define Your Options Class with Nested Validator

```csharp
using FluentValidation;

public class DatabaseOptions
{
  public string ConnectionString { get; set; } = string.Empty;
  public int MaxRetries { get; set; }
  public int CommandTimeout { get; set; }

  // Nested validator - sealed and only used here
  public sealed class Validator : AbstractValidator<DatabaseOptions>
  {
    public Validator()
    {
      RuleFor(x => x.ConnectionString)
        .NotEmpty()
        .WithMessage("Database connection string is required");

      RuleFor(x => x.MaxRetries)
        .GreaterThan(0)
        .LessThanOrEqualTo(10)
        .WithMessage("MaxRetries must be between 1 and 10");

      RuleFor(x => x.CommandTimeout)
        .GreaterThanOrEqualTo(30)
        .WithMessage("CommandTimeout must be at least 30 seconds");
    }
  }
}
```

#### 2. Register with Automatic Startup Validation

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register options with automatic startup validation
builder.Services
  .AddFluentValidatedOptions<DatabaseOptions, DatabaseOptions.Validator>(builder.Configuration)
  .ValidateOnStart(); // ✅ Validates when host starts, throws on error

var app = builder.Build();
app.Run(); // Validation happens automatically before this runs
```

**What this does:**
- Binds the `DatabaseOptions` section from appsettings.json
- Registers the FluentValidation validator
- **Validates configuration at startup** (before `app.Run()`)
- **Fails fast with clear error messages** if configuration is invalid
- No manual validation calls needed!

### Configuration Binding

#### Using appsettings.json

By default, the library looks for a configuration section with the same name as your options class:

```json
{
  "DatabaseOptions": {
    "ConnectionString": "Server=localhost;Database=myapp;",
    "MaxRetries": 3,
    "CommandTimeout": 30
  }
}
```

```csharp
services
  .AddFluentValidatedOptions<DatabaseOptions, DatabaseOptions.Validator>(configuration)
  .ValidateOnStart();
```

#### Custom Section Names

Use the `[SectionName]` attribute to map to a different configuration section:

```csharp
using TimeWarp.OptionsValidation;

[SectionName("Database")]
public class DatabaseOptions
{
  public string ConnectionString { get; set; } = string.Empty;
  // ...
}
```

Now it will bind to the "Database" section instead of "DatabaseOptions":

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=myapp;",
    "MaxRetries": 3,
    "CommandTimeout": 30
  }
}
```

### Programmatic Configuration

You can also configure options programmatically without IConfiguration:

```csharp
services
  .AddFluentValidatedOptions<DatabaseOptions, DatabaseOptions.Validator>(options =>
  {
    options.ConnectionString = "Server=localhost;Database=myapp;";
    options.MaxRetries = 3;
    options.CommandTimeout = 30;
  })
  .ValidateOnStart();
```

### Complete Startup Example

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register multiple validated options with automatic startup validation
builder.Services
  .AddFluentValidatedOptions<DatabaseOptions, DatabaseOptions.Validator>(builder.Configuration)
  .ValidateOnStart();

builder.Services
  .AddFluentValidatedOptions<CacheOptions, CacheOptions.Validator>(builder.Configuration)
  .ValidateOnStart();

builder.Services
  .AddFluentValidatedOptions<EmailOptions, EmailOptions.Validator>(builder.Configuration)
  .ValidateOnStart();

var app = builder.Build();
app.Run(); // All options validated before this runs
```

If any configuration is invalid, the application will **fail to start** with clear error messages indicating exactly which settings are invalid and why.

### Without Startup Validation

If you don't need automatic startup validation, simply omit `.ValidateOnStart()`:

```csharp
// Validates on first access instead of at startup
services.AddFluentValidatedOptions<DatabaseOptions, DatabaseOptions.Validator>(configuration);
// No .ValidateOnStart() call - validation happens lazily
```

This approach validates options when they're first accessed rather than at application startup.

## Features

- **Automatic Startup Validation**: Use `.ValidateOnStart()` to fail fast on invalid configuration
- **Automatic Section Discovery**: Uses the class name as the configuration section name by default
- **Custom Section Mapping**: Use `[SectionName]` attribute to override the section name
- **Seamless Integration**: Works with Microsoft.Extensions.Options infrastructure and `OptionsBuilder<T>`
- **FluentValidation Power**: Rich validation rules, custom validators, conditional validation
- **Clear Error Messages**: Detailed, actionable error messages from FluentValidation
- **Type Safety**: Strongly-typed options with compile-time checking
- **Flexible API**: Choose between fluent API (with `.ValidateOnStart()`) or simple registration

## Releases

See the [Release Notes](./documentation/releases.md)
## Unlicense

[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-options-validation.svg?style=flat-square&logo=github)](https://unlicense.org)

## Contributing

Time is of the essence.  Before developing a Pull Request I recommend opening a [discussion](https://github.com/TimeWarpEngineering/timewarp-options-validation/discussions).

Please feel free to make suggestions and help out with the [documentation](https://timewarpengineering.github.io/timewarp-options-validation/).
Please refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.

## Contact

Sometimes the github notifications get lost in the shuffle.  If you file an [issue](https://github.com/TimeWarpEngineering/timewarp-options-validation/issues) and don't get a response in a timely manner feel free to ping on our [Discord server](https://discord.gg/A55JARGKKP).

[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)
