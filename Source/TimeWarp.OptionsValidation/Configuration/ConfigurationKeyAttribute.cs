namespace TimeWarp.OptionsValidation;

/// <summary>
/// Specifies the configuration key to bind the options class to.
/// Supports both simple keys ("Database") and hierarchical keys ("MyApp:Settings:Database").
/// </summary>
/// <remarks>
/// This attribute aligns with <see cref="Microsoft.Extensions.Configuration.IConfiguration.GetSection(string)"/>,
/// which accepts a configuration key that can represent both simple section names and nested paths using colon separators.
/// </remarks>
/// <example>
/// <code>
/// // Simple configuration key
/// [ConfigurationKey("Database")]
/// public class DatabaseOptions { }
///
/// // Hierarchical key using colon separator
/// [ConfigurationKey("MyApp:Settings:Database")]
/// public class DatabaseOptions { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ConfigurationKeyAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the ConfigurationKeyAttribute.
  /// </summary>
  /// <param name="key">
  /// The configuration key. Can be a simple key ("Database")
  /// or a hierarchical key using colon separators ("MyApp:Settings:Database").
  /// </param>
  public ConfigurationKeyAttribute(string key)
  {
    Key = key;
  }

  /// <summary>
  /// Gets the configuration key.
  /// </summary>
  public string Key { get; }
}
