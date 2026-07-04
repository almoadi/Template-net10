# Helpers & Extensions

A set of pure, dependency-free helpers to keep handlers and entities concise.

## Guard (Domain)

Guard clauses that enforce invariants inside entities. Each throws `BadRequestException` (HTTP 400)
and returns the validated value, so it reads cleanly in constructors:

```csharp
public static Product Create(string name, decimal price) => new()
{
    Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
    Price = Guard.AgainstNegative(price, nameof(price)),
};
```

| Method | Rule |
|--------|------|
| `Guard.AgainstNull(value, name)` | not null |
| `Guard.AgainstNullOrEmpty(value, name)` | not null/empty |
| `Guard.AgainstNullOrWhiteSpace(value, name)` | not blank |
| `Guard.AgainstNegative(value, name)` | `>= 0` |
| `Guard.AgainstNegativeOrZero(value, name)` | `> 0` |
| `Guard.AgainstMaxLength(value, max, name)` | length `<= max` |
| `Guard.Against(condition, message)` | custom |

## Str

String helpers (Laravel's `Str`) — see also [Str Helpers](/docs/utilities/str):

```csharp
Str.Snake("helloWorld");   // "hello_world"
Str.Kebab("helloWorld");   // "hello-world"
Str.Camel("hello_world");  // "helloWorld"
Str.Studly("hello_world"); // "HelloWorld"
Str.Title("hello world");  // "Hello World"
Str.Start("path", "/");    // "/path"
Str.Finish("dir", "/");    // "dir/"
Str.Contains("Hello", "ell"); // true (case-insensitive)
```

## String extensions

Instance-style versions for fluent call sites:

```csharp
"Hello, World!".Slugify();      // "hello-world"
"HelloWorld".ToSnakeCase();     // "hello_world"
"long text...".Truncate(10);
"4242424242424242".Mask();      // "************4242"
name.HasValue();                // !string.IsNullOrWhiteSpace
value.IsBlank();
```

## Queryable / Enumerable

`WhereIf` applies a filter only when a condition holds — ideal for optional search filters in query
handlers:

```csharp
var users = await _context.Users.AsNoTracking()
    .WhereIf(query.ActiveOnly, u => u.IsActive)
    .WhereIf(query.Search.HasValue(), u => u.Email.Contains(query.Search!))
    .ToPagedResponseAsync(query, ct);
```

Also: `source.ForEach(...)`, `source.AsReadOnlyList()`, `source.IsNullOrEmpty()`.

## DateTime extensions

```csharp
now.StartOfDay();
now.EndOfDay();
now.StartOfMonth();
now.EndOfMonth();
now.ToUnixTimeSeconds();
now.ToIso8601();
DateTimeExtensions.FromUnixTimeSeconds(1767532200);
```

## Related

- [Str Helpers](/docs/utilities/str)
- [Clock](/docs/utilities/clock)
