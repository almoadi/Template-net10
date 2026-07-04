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

## When & How to Use It

Localization is for any text a real person will read. Use it when:

- **Your app serves more than one language** — English and Arabic are wired up out of the box, and
  you add more locales the same way.
- **You return an error or message** — instead of hardcoding `"User not found"`, add a `Resource`
  enum key and translate it in every `*.yml` file, then call `_localization.GetMessage(...)`.
- **Users should choose their language** — the request culture is picked from `?culture=ar`, a
  cookie, or the `Accept-Language` header, so the same endpoint responds in the caller's language.

**Rule of thumb:** if a string is ever shown to a user, it belongs in the YAML lang files — never
inline in code. Add the key to **every** language file so no locale is left with a missing message.

## Related

- [Adding Translations](/docs/localization/adding-translations)
- [App Configuration](/docs/configuration/app)
