# File Storage

A Laravel-style `Storage` abstraction for reading and writing files behind a swappable driver.
Handlers depend on `IStorage` (or the static `Storage` facade) and never touch the file system directly.

## IStorage

```csharp
public interface IStorage
{
    Task<string> PutAsync(string path, Stream content, CancellationToken ct = default);
    Task<string> PutBytesAsync(string path, byte[] content, CancellationToken ct = default);
    Task<Stream?> GetAsync(string path, CancellationToken ct = default);
    Task<bool> ExistsAsync(string path, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
    string Url(string path);
}
```

Paths are relative to the configured storage root and always use forward slashes. The driver
rejects path-traversal (`..`) segments that would escape the root.

## Injecting the service

```csharp
public sealed class UploadAvatarHandler(IStorage storage)
{
    public async Task<string> Handle(UploadAvatarCommand command, CancellationToken ct)
    {
        var path = await storage.PutAsync($"avatars/{command.UserId}.png", command.Content, ct);
        return storage.Url(path); // -> /storage/avatars/1.png
    }
}
```

## Storage facade

For Laravel-like ergonomics, use the static `Storage` facade inside a request scope:

```csharp
await Storage.Put("invoices/2026/01.pdf", stream, ct);
var url = Storage.Url("invoices/2026/01.pdf");
```

Prefer injecting `IStorage` in handlers, jobs, and services — the facade is for convenience and is
only valid inside an HTTP request.

## Drivers

| Driver | Config | Use case |
|--------|--------|----------|
| `Local` | `Storage:Driver = "Local"` | Files on local disk under `Root` (default) |

Swap the Infrastructure `LocalStorage` implementation for blob/object storage without changing any handler.

## When & How to Use It

Use `IStorage` any time you're dealing with files instead of database rows:

- **Accept uploads** — profile avatars, attachments, imported spreadsheets. Save the stream with
  `PutAsync` and store the returned path on your entity.
- **Generate files** — invoices, PDF reports, CSV exports. Write the bytes, then hand the user a
  link with `Url(path)`.
- **Serve a file back** — `GetAsync` to stream a stored file, `ExistsAsync` to check first.
- **Clean up** — `DeleteAsync` when the owning record is removed.
- **Move to the cloud later** — develop against the `Local` disk driver, then swap in blob/object
  storage in Infrastructure without touching a single handler.

**Why go through the abstraction?** Handlers never touch the file system directly, paths are
validated against traversal (`..`), and the storage backend stays swappable.

## Related

- [Storage Configuration](/docs/configuration/storage)
- [Facades](/docs/architecture/overview)
