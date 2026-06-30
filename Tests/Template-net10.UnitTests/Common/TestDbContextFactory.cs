using Microsoft.EntityFrameworkCore;
using Template_net10.Infrastructure;

namespace Template_net10.UnitTests.Common;

/// <summary>Builds an isolated in-memory <see cref="ApplicationDbContext"/> for handler tests.</summary>
internal static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
