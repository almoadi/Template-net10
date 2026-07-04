# Str Helpers

Pure, dependency-free string utilities — the .NET analog of Laravel's `Str`. Use anywhere; no DI required.

## Methods

| Method | Description |
|--------|-------------|
| `Str.Random(length = 32)` | Cryptographically-strong random alphanumeric string |
| `Str.Uuid()` | New GUID as a 32-char lowercase hex string (no dashes) |
| `Str.Slug(value, separator = '-')` | URL-friendly slug from arbitrary text |
| `Str.Limit(value, limit = 100, end = "…")` | Truncates and appends an ellipsis |
| `Str.Mask(value, visible = 4, mask = '*')` | Masks all but the last N characters |
| `Str.UcFirst(value)` | Upper-cases the first character |

## Examples

```csharp
Str.Random(16);                       // "a8Kd0Qb3RtVz9LmP"
Str.Slug("Hello, World!");            // "hello-world"
Str.Limit("A long description", 6);   // "A long…"
Str.Mask("4242424242424242");         // "************4242"
```

## Related

- [Clock](/docs/utilities/clock)
