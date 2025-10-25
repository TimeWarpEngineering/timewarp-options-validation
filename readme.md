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

### Basic Example

Here's a complete example showing how to validate database configuration settings:

#### 1. Define Your Options Class

```csharp
public class DatabaseOptions
{
  public string ConnectionString { get; set; } = string.Empty;
  public int MaxRetries { get; set; }
  public int CommandTimeout { get; set; }
}
```

#### 2. Create a FluentValidation Validator

```csharp
using FluentValidation;

public class DatabaseOptionsValidator : AbstractValidator<DatabaseOptions>
{
  public DatabaseOptionsValidator()
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
```

#### 3. Register in Your DI Container

```csharp
using Microsoft.Extensions.DependencyInjection;

// In your Program.cs or Startup.cs
services.ConfigureOptions<DatabaseOptions, DatabaseOptionsValidator>(configuration);
```

This will:
- Bind the `DatabaseOptions` section from your configuration (appsettings.json)
- Register the validator
- Automatically validate when the options are first accessed

#### 4. (Optional) Validate at Startup

To catch configuration errors immediately when your application starts:

```csharp
var serviceProvider = services.BuildServiceProvider();

// Validate all registered options
serviceProvider.ValidateOptions(services, logger);
```

If validation fails, this will throw an exception with detailed error messages, preventing your application from starting with invalid configuration.

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
services.ConfigureOptions<DatabaseOptions, DatabaseOptionsValidator>(configuration);
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

You can also configure options without IConfiguration:

```csharp
services.ConfigureOptions<DatabaseOptions, DatabaseOptionsValidator>(options =>
{
  options.ConnectionString = "Server=localhost;Database=myapp;";
  options.MaxRetries = 3;
  options.CommandTimeout = 30;
});
```

### Complete Startup Example

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Register validated options
builder.Services.ConfigureOptions<DatabaseOptions, DatabaseOptionsValidator>(builder.Configuration);
builder.Services.ConfigureOptions<CacheOptions, CacheOptionsValidator>(builder.Configuration);
builder.Services.ConfigureOptions<EmailOptions, EmailOptionsValidator>(builder.Configuration);

var app = builder.Build();

// Validate all options at startup (recommended)
app.Services.ValidateOptions(builder.Services, app.Logger);

app.Run();
```

If any configuration is invalid, the application will fail to start with clear error messages indicating exactly which settings are invalid and why.

## Features

- **Automatic Section Discovery**: Uses the class name as the configuration section name by default
- **Custom Section Mapping**: Use `[SectionName]` attribute to override the section name
- **Seamless Integration**: Works with Microsoft.Extensions.Options infrastructure
- **Startup Validation**: Optional helper method to validate all options when the application starts
- **Clear Error Messages**: FluentValidation provides detailed, actionable error messages
- **Type Safety**: Strongly-typed options with compile-time checking

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
