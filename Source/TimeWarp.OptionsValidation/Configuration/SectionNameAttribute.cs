namespace TimeWarp.OptionsValidation;

/// <summary>
/// Specifies the configuration section path to bind the options class to.
/// Supports both simple names ("Database") and nested paths ("MyApp:Settings:Database").
/// </summary>
/// <example>
/// <code>
/// // Simple section name
/// [SectionName("Database")]
/// public class DatabaseOptions { }
///
/// // Nested path using colon separator
/// [SectionName("MyApp:Settings:Database")]
/// public class DatabaseOptions { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SectionNameAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the SectionNameAttribute.
  /// </summary>
  /// <param name="sectionName">
  /// The configuration section path. Can be a simple name ("Database")
  /// or a nested path using colon separators ("MyApp:Settings:Database").
  /// </param>
  public SectionNameAttribute(string sectionName)
  {
    SectionName = sectionName;
  }

  /// <summary>
  /// Gets the configuration section path.
  /// </summary>
  public string SectionName { get; }
}
