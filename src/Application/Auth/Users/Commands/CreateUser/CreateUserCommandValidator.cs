using FluentValidation;
using Template_net10.Application.Common.Extensions;
using Template_net10.Domain.Common;

namespace Template_net10.Application.Auth.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.NameEn)
            .RequiredWithMaxLength(nameof(CreateUserCommand.NameEn), LengthConstants.L255);

        RuleFor(x => x.NameAr)
            .RequiredWithMaxLength(nameof(CreateUserCommand.NameAr), LengthConstants.L255);

        RuleFor(x => x.Email)
            .RequiredEmail(nameof(CreateUserCommand.Email));

        RuleFor(x => x.Phone)
            .RequiredSaudiMobileWithCountryCode(nameof(CreateUserCommand.Phone));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(10).WithMessage("Password must be at least 10 characters.")
            .MaximumLength(LengthConstants.L100);

        RuleForEach(x => x.RoleIds).GreaterThan(0);
    }
}
