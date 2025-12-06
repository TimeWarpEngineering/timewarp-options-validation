using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TimeWarp.OptionsValidation;

namespace AotTest;

internal static class Program
{
  public static void Main()
  {
    // Create configuration
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["TestOptions:Name"] = "Test",
            ["TestOptions:Value"] = "42"
        })
        .Build();

    // Setup DI with options validation
    ServiceCollection services = new();
    services.AddFluentValidatedOptions<TestOptions, TestOptionsValidator>(configuration)
        .ValidateOnStart();

    ServiceProvider serviceProvider = services.BuildServiceProvider();
    IOptions<TestOptions> options = serviceProvider.GetRequiredService<IOptions<TestOptions>>();

    Console.WriteLine($"Name: {options.Value.Name}, Value: {options.Value.Value}");
  }
}

[ConfigurationKey("TestOptions")]
internal sealed class TestOptions
{
  public string Name { get; set; } = string.Empty;
  public int Value { get; set; }
}

internal sealed class TestOptionsValidator : AbstractValidator<TestOptions>
{
  public TestOptionsValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
    RuleFor(x => x.Value).GreaterThan(0);
  }
}
