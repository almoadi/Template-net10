# Feature Flags Configuration

Feature-flag settings, toggled without a redeploy.

**File:** `src/API/config/features.json`  
**Options class:** `FeatureFlagsOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Features:Flags` | `{}` | Map of feature name to on/off state. Missing entries are treated as off |

## Example

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

Because config is loaded with `reloadOnChange`, edits take effect at runtime. Override per environment
in `config/{Environment}/features.json`.

## Usage

Inject `IFeatureFlags` or use the static `Feature` facade. See [Feature Flags](/docs/features/overview).

## Related

- [Feature Flags](/docs/features/overview)
- [Configuration Overview](/docs/configuration/overview)
