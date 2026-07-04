using FluentAssertions;
using NUnit.Framework;
using Template_net10.Domain.Auth.Entities;

namespace Template_net10.UnitTests.Domain.Auth;

[TestFixture]
public sealed class UserCreateFromSocialTests
{
    [Test]
    public void CreateFromSocial_provisions_a_passwordless_verified_active_user()
    {
        var user = User.CreateFromSocial("Ada Lovelace", "Ada Lovelace", "Ada@Example.com ");

        user.NameEn.Should().Be("Ada Lovelace");
        user.Email.Should().Be("ada@example.com");
        user.Phone.Should().BeEmpty();
        user.PasswordHash.Should().BeEmpty();
        user.IsActive.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerifiedAt.Should().NotBeNull();
    }
}
