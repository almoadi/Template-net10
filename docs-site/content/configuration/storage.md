# Storage Configuration

File storage driver settings.

**File:** `src/API/config/storage.json`  
**Options class:** `StorageOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Storage:Driver` | `Local` | Storage driver — `Local` (local file system) |
| `Storage:Root` | `storage/app` | Directory for stored files (relative to the content root, or absolute) |
| `Storage:PublicUrl` | `/storage` | Base URL used by `Storage.Url(...)` to build public links |

## Example

```json
{
  "Storage": {
    "Driver": "Local",
    "Root": "storage/app",
    "PublicUrl": "/storage"
  }
}
```

## Usage

Inject `IStorage` or use the static `Storage` facade. See [File Storage](/docs/storage/overview).

## Related

- [File Storage](/docs/storage/overview)
- [Configuration Overview](/docs/configuration/overview)
