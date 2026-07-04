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

## Related

- [Feature Flags Configuration](/docs/configuration/features)
- [Configuration Overview](/docs/configuration/overview)
