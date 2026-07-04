using FluentValidation;

namespace Template_net10.Application.Auth.Account.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
        => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
