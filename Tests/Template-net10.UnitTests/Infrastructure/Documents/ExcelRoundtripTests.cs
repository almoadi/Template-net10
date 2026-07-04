using FluentAssertions;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Excel;
using Template_net10.Infrastructure.Services.Excel;

namespace Template_net10.UnitTests.Infrastructure.Documents;

[TestFixture]
public sealed class ExcelRoundtripTests
{
    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public bool IsActive { get; set; }
    }

    [Test]
    public void Write_then_Read_round_trips_typed_rows()
    {
        IExcelWriter writer = new ClosedXmlExcelWriter();
        IExcelReader reader = new ClosedXmlExcelReader();

        var people = new List<Person>
        {
            new() { Name = "Alice", Age = 30, IsActive = true },
            new() { Name = "Bob", Age = 25, IsActive = false },
        };

        var bytes = writer.Write(people, "People");
        bytes.Should().NotBeEmpty();

        using var stream = new MemoryStream(bytes);
        var read = reader.Read<Person>(stream);

        read.Should().HaveCount(2);
        read[0].Name.Should().Be("Alice");
        read[0].Age.Should().Be(30);
        read[0].IsActive.Should().BeTrue();
        read[1].Name.Should().Be("Bob");
        read[1].Age.Should().Be(25);
        read[1].IsActive.Should().BeFalse();
    }

    [Test]
    public void Write_supports_explicit_columns()
    {
        IExcelWriter writer = new ClosedXmlExcelWriter();

        var bytes = writer.Write(
            [new Person { Name = "Carol", Age = 40 }],
            "Sheet1",
            [new ExcelColumn<Person>("Full Name", p => p.Name)]);

        bytes.Should().NotBeEmpty();
    }
}
