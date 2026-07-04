using FluentValidation;

namespace Template_net10.Application.Auth.Account.Commands.RequestEmailVerification;

public sealed class RequestEmailVerificationCommandValidator : AbstractValidator<RequestEmailVerificationCommand>
{
    public RequestEmailVerificationCommandValidator()
        => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
