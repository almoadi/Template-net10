namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Features</c> section (config/features.json).</summary>
public sealed class FeatureFlagsOptions
{
    public const string SectionName = "Features";

    /// <summary>Map of feature name to on/off state. Missing entries are treated as off.</summary>
    public Dictionary<string, bool> Flags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
