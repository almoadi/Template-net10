using Asp.Versioning;

namespace Template_net10.API.Extensions;

public static class ApiVersioningServiceExtensions
{
    /// <summary>
    /// Registers URL-segment API versioning (<c>api/v{version}/...</c>). The version can also be sent
    /// via the <c>X-Api-Version</c> header or the <c>api-version</c> query string. Requests without a
    /// version fall back to <c>1.0</c>, and responses advertise supported versions via the
    /// <c>api-supported-versions</c> header. The API explorer is version-aware so Swagger produces one
    /// document per version.
    /// </summary>
    public static IServiceCollection AddApiVersioningSupport(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new QueryStringApiVersionReader("api-version"));
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                // Group name format "'v'VVV" -> v1, v1.1, v2 … and swap {version} in reported routes.
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
