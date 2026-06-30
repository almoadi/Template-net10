using FluentAssertions;
using NUnit.Framework;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.UnitTests.Domain;

[TestFixture]
public sealed class UserTests
{
    private static User NewUser() => User.Create("Jane", "جين", "Jane@Example.com", "+966500000001", "hash");

    [Test]
    public void Create_sets_fields_normalizes_email_and_activates_user()
    {
        var user = NewUser();

        user.NameEn.Should().Be("Jane");
        user.NameAr.Should().Be("جين");
        user.Email.Should().Be("jane@example.com");
        user.Phone.Should().Be("+966500000001");
        user.PasswordHash.Should().Be("hash");
        user.IsActive.Should().BeTrue();
    }

    [Test]
    public void Update_only_overwrites_provided_values()
    {
        var user = NewUser();

        user.Update(nameEn: "Janet", nameAr: null, email: null, phone: null);

        user.NameEn.Should().Be("Janet");
        user.NameAr.Should().Be("جين");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public void AssignRole_is_idempotent_for_the_same_role()
    {
        var user = NewUser();
        var role = Role.Create("Admin", "مدير", isSystem: true);

        user.AssignRole(role);
        user.AssignRole(role);

        user.UserRoles.Should().HaveCount(1);
    }

    [Test]
    public void Deactivate_clears_active_flag()
    {
        var user = NewUser();

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }
}
