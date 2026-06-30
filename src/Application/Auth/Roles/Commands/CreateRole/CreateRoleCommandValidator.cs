using FluentValidation;
using Template_net10.Application.Common.Extensions;
using Template_net10.Domain.Common;

namespace Template_net10.Application.Auth.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.NameEn)
            .RequiredWithMaxLength(nameof(CreateRoleCommand.NameEn), LengthConstants.L100);

        RuleFor(x => x.NameAr)
            .RequiredWithMaxLength(nameof(CreateRoleCommand.NameAr), LengthConstants.L100);

        RuleForEach(x => x.PermissionIds).GreaterThan(0);
    }
}
