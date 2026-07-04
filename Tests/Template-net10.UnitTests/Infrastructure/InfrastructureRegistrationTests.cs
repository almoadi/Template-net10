using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Template_net10.Application.Abstractions.Caching;
using Template_net10.Application.Abstractions.Encryption;
using Template_net10.Application.Abstractions.Excel;
using Template_net10.Application.Abstractions.Features;
using Template_net10.Application.Abstractions.Jobs;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Messaging;
using Template_net10.Application.Abstractions.Notifications;
using Template_net10.Application.Abstractions.Pdf;
using Template_net10.Application.Abstractions.Realtime;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Abstractions.Storage;
using Template_net10.Application.Abstractions.Time;
using Template_net10.Infrastructure;
using Template_net10.Infrastructure.Services.Pdf;

namespace Template_net10.UnitTests.Infrastructure;

/// <summary>
/// Guards the Infrastructure Scrutor scan. Every service lives under a feature subfolder of
/// <c>Infrastructure/Services</c>; the namespace-prefix scan must register each one regardless of
/// which subfolder it sits in. Asserts on service descriptors (registration), not resolution, so no
/// host/database is required.
/// </summary>
[TestFixture]
public sealed class InfrastructureRegistrationTests
{
    private static IServiceCollection BuildInfrastructure()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Server=(local);Database=Test;Trusted_Connection=True;TrustServerCertificate=True",
                ["Cache:Driver"] = "Memory",
                ["Encryption:Key"] = "unit-test-encryption-key",
            })
            .Build();

        return new ServiceCollection().AddInfrastructure(configuration);
    }

    [TestCase(typeof(IAuth))]
    [TestCase(typeof(ICurrentUserService))]
    [TestCase(typeof(IJwtTokenService))]
    [TestCase(typeof(IPasswordHasher))]
    [TestCase(typeof(ILocalizationService))]
    [TestCase(typeof(IEmailSender))]
    [TestCase(typeof(IEmailJob))]
    [TestCase(typeof(IMaintenanceJob))]
    [TestCase(typeof(IDomainEventDispatcher))]
    [TestCase(typeof(ICacheService))]
    [TestCase(typeof(IStorage))]
    [TestCase(typeof(IFeatureFlags))]
    [TestCase(typeof(IEncryptor))]
    [TestCase(typeof(IClock))]
    [TestCase(typeof(IExcelWriter))]
    [TestCase(typeof(IExcelReader))]
    [TestCase(typeof(IPdfGenerator))]
    [TestCase(typeof(IPdfRenderer))]
    [TestCase(typeof(IRealtimeNotifier))]
    public void Registers_service_from_every_feature_subfolder(Type serviceType)
    {
        var services = BuildInfrastructure();

        services.Should().Contain(
            descriptor => descriptor.ServiceType == serviceType,
            "the Services namespace-prefix scan must register {0}", serviceType.Name);
    }
}
