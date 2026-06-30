using FluentValidation;

namespace Template_net10.Application.Auth.Users.Commands.AssignRoles;

public sealed class AssignRolesCommandValidator : AbstractValidator<AssignRolesCommand>
{
    public AssignRolesCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleForEach(x => x.RoleIds).GreaterThan(0);
    }
}
