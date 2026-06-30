using System.Text.RegularExpressions;
using FluentValidation;
using Template_net10.Domain.Common;

namespace Template_net10.Application.Common.Extensions;

/// <summary>
/// Reusable FluentValidation rules so common constraints (required + length, phone format)
/// are declared once and applied consistently across all validators.
/// </summary>
public static partial class ValidationRuleExtensions
{
    [GeneratedRegex(@"^\+9665\d{8}$")]
    private static partial Regex SaudiMobileRegex();

    public static IRuleBuilderOptions<T, string> RequiredWithMaxLength<T>(
        this IRuleBuilder<T, string> rule, string propertyName, int maxLength)
        => rule
            .NotEmpty().WithMessage($"{propertyName} is required.")
            .MaximumLength(maxLength).WithMessage($"{propertyName} must not exceed {maxLength} characters.");

    public static IRuleBuilderOptions<T, string> RequiredSaudiMobileWithCountryCode<T>(
        this IRuleBuilder<T, string> rule, string propertyName)
        => rule
            .NotEmpty().WithMessage($"{propertyName} is required.")
            .Must(value => !string.IsNullOrWhiteSpace(value) && SaudiMobileRegex().IsMatch(value))
            .WithMessage($"{propertyName} must be a valid Saudi mobile number in the form +9665XXXXXXXX.");

    public static IRuleBuilderOptions<T, string> RequiredEmail<T>(
        this IRuleBuilder<T, string> rule, string propertyName)
        => rule
            .NotEmpty().WithMessage($"{propertyName} is required.")
            .EmailAddress().WithMessage($"{propertyName} must be a valid email address.")
            .MaximumLength(LengthConstants.L255);
}
