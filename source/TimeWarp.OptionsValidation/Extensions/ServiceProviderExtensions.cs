namespace Microsoft.Extensions.DependencyInjection;

using Logging;

/// <summary>
/// Run Validation on all the IOptions that have validation
/// This will iterate through all the IConfigureOptions in the IServiceCollection
/// Then it will access each of those which will trigger the validation.
/// </summary>
public static class ServiceProviderExtensions
{
  public static void ValidateOptions
  (
    this IServiceProvider serviceProvider,
    IServiceCollection serviceCollection,
    ILogger logger
  )
  {
    using IServiceScope scope = serviceProvider.CreateScope();
    IServiceProvider scopedProvider = scope.ServiceProvider;
    ValidateOptionsInternal(scopedProvider, serviceCollection, logger);
  }

  private static void ValidateOptionsInternal
  (
    this IServiceProvider serviceProvider,
    IServiceCollection serviceCollection,
    ILogger logger
  )
  {
    IEnumerable<Type> optionTypes =
    serviceCollection
      .Where
      (
        serviceDescriptor =>
          serviceDescriptor.ServiceType.IsGenericType &&
          serviceDescriptor.ServiceType.GetGenericTypeDefinition() == typeof(IConfigureOptions<>)
      )
      .Select
      (
        serviceDescriptor => serviceDescriptor.ServiceType.GetGenericArguments()[0]
      ).Distinct();

    Func<Type, MemberInfo, LambdaExpression, string>? originalDisplayNameResolver = ValidatorOptions.Global.DisplayNameResolver;

    ValidatorOptions.Global.DisplayNameResolver =
      (type, memberInfo, _) =>
        type != null && memberInfo != null ? $"{type.Name}:{memberInfo.Name}" : null;


    foreach (Type optionType in optionTypes)
    {
      try
      {
        Type optionsAccessorType = typeof(IOptions<>).MakeGenericType(new Type[] { optionType });
        object? optionsAccessor = serviceProvider.GetService(optionsAccessorType);
        // Accessing the value triggers the validation.
        object? _ = optionsAccessor?.GetType().GetProperty(nameof(IOptions<object>.Value))?.GetValue(optionsAccessor);
      }
      catch (Exception e)
      {
        logger.LogWarning("Failed to validate options for {Name}: {Message}", optionType.Name, e.Message);
      }
    }

    ValidatorOptions.Global.DisplayNameResolver = originalDisplayNameResolver;
  }
}
