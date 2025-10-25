namespace Microsoft.Extensions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentValidation;
using TimeWarp.OptionsValidation;

/// <summary>
/// Extension methods for OptionsBuilder to integrate FluentValidation
/// </summary>
public static class OptionsBuilderExtensions
{
  /// <summary>
  /// Adds FluentValidation to the options configuration.
  /// This enables validation using FluentValidation rules and allows chaining with .ValidateOnStart()
  /// </summary>
  /// <typeparam name="TOptions">The options type to validate</typeparam>
  /// <typeparam name="TValidator">The FluentValidation validator type</typeparam>
  /// <param name="optionsBuilder">The options builder</param>
  /// <returns>The options builder for method chaining</returns>
  /// <example>
  /// <code>
  /// services.AddOptions&lt;DatabaseOptions&gt;()
  ///     .Bind(configuration.GetSection("Database"))
  ///     .ValidateFluentValidation&lt;DatabaseOptions, DatabaseOptionsValidator&gt;()
  ///     .ValidateOnStart();
  /// </code>
  /// </example>
  public static OptionsBuilder<TOptions> ValidateFluentValidation<TOptions, TValidator>(
    this OptionsBuilder<TOptions> optionsBuilder)
    where TOptions : class
    where TValidator : AbstractValidator<TOptions>
  {
    // Register the FluentValidation validator as a singleton
    optionsBuilder.Services.TryAddSingleton<TValidator>();

    // Register the bridge that connects FluentValidation to IValidateOptions<T>
    optionsBuilder.Services.TryAddEnumerable(
      ServiceDescriptor.Singleton<IValidateOptions<TOptions>,
        OptionsValidation<TOptions, TValidator>>()
    );

    return optionsBuilder;
  }
}
