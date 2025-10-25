namespace Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Adds options with automatic configuration binding and FluentValidation.
  /// Returns OptionsBuilder for chaining (e.g., .ValidateOnStart())
  /// </summary>
  /// <typeparam name="TOptions">The options type to configure and validate</typeparam>
  /// <typeparam name="TValidator">The FluentValidation validator type</typeparam>
  /// <param name="services">The service collection</param>
  /// <param name="configuration">The configuration instance to bind from</param>
  /// <returns>The OptionsBuilder for method chaining (supports .ValidateOnStart())</returns>
  /// <remarks>
  /// Section name resolution:
  /// - Default: Uses class name (DatabaseOptions → "DatabaseOptions")
  /// - Custom: Uses [SectionName("Database")] → "Database"
  /// - Nested: Uses [SectionName("MyApp:Settings:Database")] → nested path
  /// </remarks>
  /// <example>
  /// <code>
  /// // Default: Binds from "DatabaseOptions" section
  /// services.AddFluentValidatedOptions&lt;DatabaseOptions, DatabaseOptions.Validator&gt;(configuration)
  ///     .ValidateOnStart();
  ///
  /// // Custom: [SectionName("Database")] binds from "Database" section
  /// services.AddFluentValidatedOptions&lt;DatabaseOptions, DatabaseOptions.Validator&gt;(configuration)
  ///     .ValidateOnStart();
  ///
  /// // Nested: [SectionName("MyApp:Settings:Database")] binds from nested path
  /// services.AddFluentValidatedOptions&lt;DatabaseOptions, DatabaseOptions.Validator&gt;(configuration)
  ///     .ValidateOnStart();
  /// </code>
  /// </example>
  public static OptionsBuilder<TOptions> AddFluentValidatedOptions<TOptions, TValidator>(
    this IServiceCollection services,
    IConfiguration configuration)
    where TOptions : class
    where TValidator : AbstractValidator<TOptions>
  {
    // Auto-discover section name using SectionNameAttribute or class name
    Type type = typeof(TOptions);
    var sectionNameAttribute = (SectionNameAttribute?)type
      .GetCustomAttributes(typeof(SectionNameAttribute), false)
      .FirstOrDefault();
    string sectionName = sectionNameAttribute?.SectionName ?? type.Name;

    return services.AddOptions<TOptions>()
      .Bind(configuration.GetSection(sectionName))
      .ValidateFluentValidation<TOptions, TValidator>();
  }

  /// <summary>
  /// Adds options with programmatic configuration and FluentValidation.
  /// Returns OptionsBuilder for chaining (e.g., .ValidateOnStart())
  /// </summary>
  /// <typeparam name="TOptions">The options type to configure and validate</typeparam>
  /// <typeparam name="TValidator">The FluentValidation validator type</typeparam>
  /// <param name="services">The service collection</param>
  /// <param name="configureOptions">Action to configure the options</param>
  /// <returns>The OptionsBuilder for method chaining (supports .ValidateOnStart())</returns>
  /// <example>
  /// <code>
  /// services.AddFluentValidatedOptions&lt;DatabaseOptions, DatabaseOptions.Validator&gt;(options =>
  /// {
  ///     options.ConnectionString = "Server=localhost;Database=myapp;";
  ///     options.MaxRetries = 3;
  /// })
  /// .ValidateOnStart();
  /// </code>
  /// </example>
  public static OptionsBuilder<TOptions> AddFluentValidatedOptions<TOptions, TValidator>(
    this IServiceCollection services,
    Action<TOptions> configureOptions)
    where TOptions : class
    where TValidator : AbstractValidator<TOptions>
  {
    return services.AddOptions<TOptions>()
      .Configure(configureOptions)
      .ValidateFluentValidation<TOptions, TValidator>();
  }

}

