using FluentAssertions;
using NUnit.Framework;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.UnitTests.Domain;

[TestFixture]
public sealed class UserTests
{
    [Test]
    public void Create_sets_fields_and_activates_user()
    {
        var user = User.Create("Jane", "جين", "+966500000001", "hash");

        user.NameEn.Should().Be("Jane");
        user.NameAr.Should().Be("جين");
        user.Phone.Should().Be("+966500000001");
        user.PasswordHash.Should().Be("hash");
        user.IsActive.Should().BeTrue();
    }

    [Test]
    public void Update_only_overwrites_provided_values()
    {
        var user = User.Create("Jane", "جين", "+966500000001", "hash");

        user.Update(nameEn: "Janet", nameAr: null, phone: null);

        user.NameEn.Should().Be("Janet");
        user.NameAr.Should().Be("جين");
        user.UpdatedAt.Should().NotBeNull();
    }

    [Test]
    public void Deactivate_clears_active_flag()
    {
        var user = User.Create("Jane", "جين", "+966500000001", "hash");

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }
}
