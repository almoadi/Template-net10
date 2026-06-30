namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>App</c> section (config/app.json) — general application settings.</summary>
public sealed class AppOptions
{
    public const string SectionName = "App";

    public string Name { get; set; } = "Template-net10";

    public string Url { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0.0";

    public string SupportEmail { get; set; } = string.Empty;

    public string TimeZone { get; set; } = "UTC";

    public string DefaultLocale { get; set; } = "en";

    public string[] SupportedLocales { get; set; } = ["en"];
}
