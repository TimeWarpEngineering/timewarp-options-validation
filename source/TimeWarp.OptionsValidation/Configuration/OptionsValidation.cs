namespace TimeWarp.OptionsValidation;

public class OptionsValidation<TOptions, TOptionsValidator>
(
  TOptionsValidator OptionsValidator
) : IValidateOptions<TOptions>
  where TOptions : class
  where TOptionsValidator : AbstractValidator<TOptions>
{

  public ValidateOptionsResult Validate(string? name, TOptions options)
  {
    ValidationResult validationResult = OptionsValidator.Validate(options);
    if (validationResult.IsValid)
    {
      return ValidateOptionsResult.Success;
    }

    return ValidateOptionsResult.Fail
    (
      validationResult.Errors.Select(aValidationFailure => aValidationFailure.ErrorMessage)
    );
  }
}
