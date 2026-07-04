using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Features;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Features;

/// <summary>
/// Configuration-driven <see cref="IFeatureFlags"/>. Reads flags from the <c>Features</c> section
/// (config/features.json, with per-environment overrides). Because config uses
/// <c>reloadOnChange</c>, toggling a flag on disk takes effect without a restart.
/// </summary>
public sealed class FeatureFlagService : IFeatureFlags
{
    private readonly IOptionsMonitor<FeatureFlagsOptions> _options;

    public FeatureFlagService(IOptionsMonitor<FeatureFlagsOptions> options) => _options = options;

    public bool IsEnabled(string feature)
        => _options.CurrentValue.Flags.TryGetValue(feature, out var enabled) && enabled;

    public bool IsDisabled(string feature) => !IsEnabled(feature);

    public IReadOnlyCollection<string> EnabledFeatures
        => _options.CurrentValue.Flags.Where(f => f.Value).Select(f => f.Key).ToArray();
}
