# Feature Flags

Toggle functionality on or off without a redeploy — the .NET analog of
[Laravel Pennant](https://laravel.com/docs/pennant). Flags are read from configuration, so changes
in `config/features.json` take effect at runtime (`reloadOnChange`).

## IFeatureFlags

```csharp
public interface IFeatureFlags
{
    bool IsEnabled(string feature);
    bool IsDisabled(string feature);
    IReadOnlyCollection<string> EnabledFeatures { get; }
}
```

Unknown features are treated as **off**.

## Injecting the service

```csharp
public sealed class CheckoutHandler(IFeatureFlags features)
{
    public async Task Handle(CheckoutCommand command, CancellationToken ct)
    {
        if (features.IsEnabled("new-checkout"))
        {
            // new flow
        }
    }
}
```

## Feature facade

```csharp
if (Feature.Active("new-checkout"))
{
    // ...
}
```

## Configuration

Flags live in the `Features` section of `config/features.json`:

```json
{
  "Features": {
    "Flags": {
      "new-checkout": false,
      "beta-reports": true
    }
  }
}
```

Override per environment in `config/{Environment}/features.json` — for example, enable a flag only
in Staging while keeping it off in Production.

## When & How to Use It

Feature flags let you turn functionality on or off from config — no redeploy needed. Use them when:

- **You want to ship code "dark"** — merge an unfinished feature behind a flag that's off, so it's
  in production but invisible until you're ready.
- **You're rolling out gradually** — enable a new flow in Staging (or for testing) before flipping
  it on in Production.
- **You run a beta** — keep experimental features (`beta-reports`) available only where you switch
  them on.
- **You need a kill switch** — if a new feature misbehaves, set its flag to `false` to disable it
  instantly instead of rolling back a deploy.

**How to use it:** check `Feature.Active("new-checkout")` (or inject `IFeatureFlags`) around the new
code path, and control the value in `config/features.json`. Unknown flags are treated as **off**,
so a missing entry fails safe.

## Related

- [Feature Flags Configuration](/docs/configuration/features)
- [Configuration Overview](/docs/configuration/overview)
