using Template_net10.Application.Abstractions.Features;

namespace Template_net10.Application.Common.Facades;

/// <summary>
/// Laravel-style static <c>Feature</c> facade (Pennant). Proxies to the request-scoped
/// <see cref="IFeatureFlags"/> resolved through a delegate set at startup (<see cref="SetResolver"/>).
/// Prefer injecting <see cref="IFeatureFlags"/> in handlers/services; use this facade for ergonomics
/// (<c>Feature.Active("new-checkout")</c>). Only valid inside an HTTP request scope.
/// </summary>
public static class Feature
{
    private static Func<IFeatureFlags>? _resolver;

    /// <summary>Wires the facade to the DI container. Called once during application startup.</summary>
    public static void SetResolver(Func<IFeatureFlags> resolver) => _resolver = resolver;

    private static IFeatureFlags Instance => (_resolver ?? throw new InvalidOperationException(
        "Feature facade is not initialized. Call app.UseFacades() during startup."))();

    public static bool Active(string feature) => Instance.IsEnabled(feature);

    public static bool Inactive(string feature) => Instance.IsDisabled(feature);

    public static IReadOnlyCollection<string> All => Instance.EnabledFeatures;
}
