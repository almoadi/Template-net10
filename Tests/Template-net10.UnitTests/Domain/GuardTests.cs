using FluentAssertions;
using NUnit.Framework;
using Template_net10.Domain.Common;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.UnitTests.Domain;

[TestFixture]
public sealed class GuardTests
{
    [Test]
    public void AgainstNullOrWhiteSpace_returns_value_when_valid()
        => Guard.AgainstNullOrWhiteSpace("Alice", "name").Should().Be("Alice");

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void AgainstNullOrWhiteSpace_throws_when_blank(string? value)
    {
        var act = () => Guard.AgainstNullOrWhiteSpace(value, "name");
        act.Should().Throw<BadRequestException>().WithMessage("*name*");
    }

    [Test]
    public void AgainstNegativeOrZero_throws_for_non_positive()
    {
        var act = () => Guard.AgainstNegativeOrZero(0, "quantity");
        act.Should().Throw<BadRequestException>();
    }

    [Test]
    public void Against_throws_when_condition_true()
    {
        var act = () => Guard.Against(1 > 0, "boom");
        act.Should().Throw<BadRequestException>().WithMessage("boom");
    }
}
