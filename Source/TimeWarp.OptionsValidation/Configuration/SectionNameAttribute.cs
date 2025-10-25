namespace TimeWarp.OptionsValidation;

/// <summary>
/// The section name in appsettings.json to which the class should be mapped
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SectionNameAttribute : Attribute
{
  public SectionNameAttribute(string sectionName)
  {
    SectionName = sectionName;
  }

  public string SectionName { get; }
}
