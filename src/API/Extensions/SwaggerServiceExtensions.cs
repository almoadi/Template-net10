using Microsoft.OpenApi;

namespace Template_net10.API.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        // One Swagger document per API version (v1, v2, …) — see ConfigureSwaggerOptions.
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter the JWT access token.",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
            });
        });

        return services;
    }
}
