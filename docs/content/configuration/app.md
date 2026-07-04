# App Configuration

General application settings — the .NET analog of Laravel's `config/app.php`.

**File:** `src/API/config/app.json`  
**Options class:** `AppOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `App:Name` | `Template-net10` | Display name in docs, emails, Swagger |
| `App:Url` | `https://localhost:5001` | Public base URL of the API |
| `App:Version` | `1.0.0` | Application version |
| `App:SupportEmail` | `support@template-net10.local` | Support contact |
| `App:TimeZone` | `Asia/Riyadh` | IANA time zone for app-level dates |
| `App:DefaultLocale` | `en` | Fallback language |
| `App:SupportedLocales` | `["en", "ar"]` | Languages for localized responses |

## Example

```json
{
  "App": {
    "Name": "Template-net10",
    "Url": "https://localhost:5001",
    "Version": "1.0.0",
    "SupportEmail": "support@template-net10.local",
    "TimeZone": "Asia/Riyadh",
    "DefaultLocale": "en",
    "SupportedLocales": [ "en", "ar" ]
  }
}
```

## Related

- [Localization Overview](/docs/localization/overview)
- [Configuration Overview](/docs/configuration/overview)
