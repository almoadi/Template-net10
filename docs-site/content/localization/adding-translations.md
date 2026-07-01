# Adding Translations

Add a new user-facing message in three steps.

## Step 1 — Add Resource Key

Add an entry to the `Resource` enum in `Application/Abstractions/Localization/Resource.cs`:

```csharp
public enum Resource
{
    // ...
    OrderPlacedSuccessfully,
}
```

## Step 2 — Add English Translation

In `src/API/resources/lang/en.yml`:

```yaml
OrderPlacedSuccessfully: "Your order has been placed successfully."
```

## Step 3 — Add Arabic Translation

In `src/API/resources/lang/ar.yml`:

```yaml
OrderPlacedSuccessfully: "تم تقديم طلبك بنجاح."
```

> **Important:** Every key must exist in **every** language file. Missing keys fall back to the key name.

## Usage

```csharp
var message = _localization.GetMessage(Resource.OrderPlacedSuccessfully);
return ApiResponseDto<MessageDto>.Success(new MessageDto { Message = message });
```

## YAML Format

Keys use PascalCase matching the enum member name. Values are plain strings; use `{0}` placeholders for format arguments if supported by your call site.

## Related

- [Localization Overview](/docs/localization/overview)
