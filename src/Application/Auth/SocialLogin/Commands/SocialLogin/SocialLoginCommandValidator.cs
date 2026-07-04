using FluentValidation;

namespace Template_net10.Application.Auth.SocialLogin.Commands.SocialLogin;

public sealed class SocialLoginCommandValidator : AbstractValidator<SocialLoginCommand>
{
    public SocialLoginCommandValidator()
    {
        RuleFor(x => x.Provider).IsInEnum();
        RuleFor(x => x.AccessToken).NotEmpty();
    }
}
