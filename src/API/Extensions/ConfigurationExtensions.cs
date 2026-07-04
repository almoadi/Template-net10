using Microsoft.Extensions.Configuration;

namespace Template_net10.API.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Config concerns loaded at startup. Each name maps to <c>config/{name}.json</c> with an optional
    /// per-environment override in <c>config/{Environment}/{name}.json</c>. Add a new concern here.
    /// </summary>
    private static readonly string[] ConfigFiles =
    [
        "app", "database", "cache", "mail", "jwt", "queue", "logging",
        "cors", "storage", "features", "encryption", "idempotency", "auth", "socialite",
    ];

    /// <summary>
    /// Loads the Laravel-style split configuration: each concern lives in its own <c>config/*.json</c>
    /// file, with an optional per-environment override in <c>config/{Environment}/*.json</c>
    /// (e.g. <c>config/Production/mail.json</c>). Environment variables are applied last so container /
    /// host vars (e.g. <c>Database__ConnectionString</c>, <c>Jwt__SecretKey</c>) override the JSON files.
    /// </summary>
    public static WebApplicationBuilder AddSplitConfiguration(this WebApplicationBuilder builder)
    {
        foreach (var file in ConfigFiles)
        {
            builder.Configuration
                .AddJsonFile($"config/{file}.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"config/{builder.Environment.EnvironmentName}/{file}.json", optional: true, reloadOnChange: true);
        }

        // Re-apply environment variables last so container/host env vars win (12-factor / Docker).
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }
}
