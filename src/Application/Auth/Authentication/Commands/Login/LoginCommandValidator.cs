using FluentValidation;
using Template_net10.Application.Common.Extensions;

namespace Template_net10.Application.Auth.Authentication.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).RequiredEmail(nameof(LoginCommand.Email));
        RuleFor(x => x.Password).NotEmpty();
    }
}
