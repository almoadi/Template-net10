# Localization Overview

Template-net10 localizes user-facing messages using YAML language files, similar to Laravel's `lang/` directory.

## Language Files

```
src/API/resources/lang/
├── en.yml
└── ar.yml
```

## Culture Resolution

Request culture is determined from (in order):

1. Query string (`?culture=ar`)
2. Cookie
3. `Accept-Language` header
4. `App:DefaultLocale` from config

Supported locales are listed in `App:SupportedLocales` (`en`, `ar` by default).

## Usage in Handlers

```csharp
_localization.GetMessage(Resource.UserNotFound)
```

Never hardcode user-facing strings — always use the `Resource` enum + YAML files.

## Components

| Component | Path |
|-----------|------|
| `Resource` enum | `Application/Abstractions/Localization/Resource.cs` |
| `ILocalizationService` | `Application/Abstractions/Localization/` |
| `LocalizationService` | `Infrastructure/Services/LocalizationService.cs` |

## Related

- [Adding Translations](/docs/localization/adding-translations)
- [App Configuration](/docs/configuration/app)
