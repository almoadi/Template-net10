using FluentAssertions;
using NUnit.Framework;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.UnitTests.Domain;

[TestFixture]
public sealed class RoleSoftDeleteTests
{
    [Test]
    public void SoftDelete_succeeds_for_custom_role()
    {
        var role = Role.Create("Custom", "مخصص");

        role.SoftDelete();

        role.IsDeleted.Should().BeTrue();
        role.DeletedAt.Should().NotBeNull();
    }

    [Test]
    public void SoftDelete_throws_for_system_role()
    {
        var role = Role.Create("Admin", "مدير", isSystem: true);

        var act = () => role.SoftDelete();

        act.Should().Throw<BadRequestException>();
    }
}
