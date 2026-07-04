namespace Template_net10.Application.Abstractions.Features;

/// <summary>
/// Feature-flag abstraction (Laravel: Pennant's <c>Feature</c>). Lets code branch on features that
/// can be toggled via configuration (config/features.json) without a redeploy. Inject this in
/// handlers/services, or use the static <c>Feature</c> facade for ergonomics.
/// </summary>
public interface IFeatureFlags
{
    /// <summary>True when the named feature is turned on. Unknown features are treated as off. (Laravel: <c>Feature::active</c>)</summary>
    bool IsEnabled(string feature);

    /// <summary>True when the named feature is turned off or unknown. (Laravel: <c>Feature::inactive</c>)</summary>
    bool IsDisabled(string feature);

    /// <summary>All currently enabled feature names.</summary>
    IReadOnlyCollection<string> EnabledFeatures { get; }
}
