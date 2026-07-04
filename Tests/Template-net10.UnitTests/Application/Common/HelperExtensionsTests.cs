using FluentAssertions;
using NUnit.Framework;
using Template_net10.Application.Common.Extensions;
using Template_net10.Application.Common.Support;

namespace Template_net10.UnitTests.Application.Common;

[TestFixture]
public sealed class HelperExtensionsTests
{
    [TestCase("helloWorld", "hello_world")]
    [TestCase("Hello World", "hello_world")]
    [TestCase("already_snake", "already_snake")]
    public void Str_Snake_converts(string input, string expected)
        => Str.Snake(input).Should().Be(expected);

    [TestCase("helloWorld", "hello-world")]
    [TestCase("Hello World", "hello-world")]
    public void Str_Kebab_converts(string input, string expected)
        => Str.Kebab(input).Should().Be(expected);

    [TestCase("hello_world", "helloWorld")]
    [TestCase("Hello world", "helloWorld")]
    public void Str_Camel_converts(string input, string expected)
        => Str.Camel(input).Should().Be(expected);

    [TestCase("hello_world", "HelloWorld")]
    [TestCase("hello world", "HelloWorld")]
    public void Str_Studly_converts(string input, string expected)
        => Str.Studly(input).Should().Be(expected);

    [Test]
    public void Str_affix_helpers_work()
    {
        Str.Start("path", "/").Should().Be("/path");
        Str.Start("/path", "/").Should().Be("/path");
        Str.Finish("dir", "/").Should().Be("dir/");
        Str.Finish("dir/", "/").Should().Be("dir/");
        Str.Contains("Hello World", "world").Should().BeTrue();
    }

    [Test]
    public void StringExtensions_delegate_to_Str()
    {
        "Hello, World!".Slugify().Should().Be("hello-world");
        "HelloWorld".ToSnakeCase().Should().Be("hello_world");
        "abcdefgh".Truncate(4, "…").Should().Be("abcd…");
        "  ".IsBlank().Should().BeTrue();
        "x".HasValue().Should().BeTrue();
    }

    [Test]
    public void WhereIf_applies_predicate_only_when_condition_true()
    {
        var source = new[] { 1, 2, 3, 4 }.AsQueryable();

        source.WhereIf(true, x => x > 2).Should().BeEquivalentTo([3, 4]);
        source.WhereIf(false, x => x > 2).Should().BeEquivalentTo([1, 2, 3, 4]);
    }

    [Test]
    public void DateTime_boundary_helpers_work()
    {
        var value = new DateTime(2026, 7, 4, 13, 30, 0, DateTimeKind.Utc);

        value.StartOfDay().Should().Be(new DateTime(2026, 7, 4, 0, 0, 0, DateTimeKind.Utc));
        value.EndOfMonth().Date.Should().Be(new DateTime(2026, 7, 31));
        DateTimeExtensions.FromUnixTimeSeconds(value.ToUnixTimeSeconds()).Should().Be(value);
    }
}
