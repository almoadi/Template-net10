using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Template_net10.Infrastructure.Options;

namespace Template_net10.API.Extensions;

public static class LocalizationServiceExtensions
{
    /// <summary>
    /// Configures request localization from the <c>App</c> config section: supported cultures come
    /// from <c>App:SupportedLocales</c> and the default from <c>App:DefaultLocale</c>. The culture is
    /// resolved per request (query string <c>?culture=ar</c>, cookie, or Accept-Language header).
    /// </summary>
    public static IServiceCollection AddAppLocalization(
        this IServiceCollection services, IConfiguration configuration)
    {
        var app = configuration.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();

        var supported = app.SupportedLocales is { Length: > 0 }
            ? app.SupportedLocales.Select(c => new CultureInfo(c)).ToList()
            : [new CultureInfo(app.DefaultLocale)];

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(app.DefaultLocale);
            options.SupportedCultures = supported;
            options.SupportedUICultures = supported;
        });

        return services;
    }
}
