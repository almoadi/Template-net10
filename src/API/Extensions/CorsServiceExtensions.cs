namespace Template_net10.API.Extensions;

public static class CorsServiceExtensions
{
    public const string ClientCorsPolicy = "ClientCors";

    /// <summary>
    /// Allows the configured client origins (<c>Cors:AllowedOrigins</c>). In Development,
    /// falls back to allowing any origin to ease local work.
    /// </summary>
    public static IServiceCollection AddClientCors(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(ClientCorsPolicy, policy =>
        {
            if (environment.IsDevelopment() || allowedOrigins.Length == 0)
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        }));

        return services;
    }
}
