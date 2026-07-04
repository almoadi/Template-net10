# Storage Configuration

File storage driver settings.

**File:** `src/API/config/storage.json`  
**Options class:** `StorageOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Storage:Driver` | `Local` | Storage driver — `Local` (local file system) or `S3` (AWS S3 / S3-compatible) |
| `Storage:Root` | `storage/app` | Directory for stored files (relative to the content root, or absolute) — `Local` driver |
| `Storage:PublicUrl` | `/storage` | Base URL used by `Storage.Url(...)` to build public links — `Local` driver |
| `Storage:S3:Bucket` | _(empty)_ | Target bucket name — **required** for the `S3` driver |
| `Storage:S3:Region` | `us-east-1` | AWS region system name. Ignored when `ServiceUrl` is set |
| `Storage:S3:AccessKeyId` | _(empty)_ | Access key. Leave empty to use the default AWS credential chain (IAM role, env vars, profile) |
| `Storage:S3:SecretAccessKey` | _(empty)_ | Secret key. Supply via user-secrets / env vars — never commit it |
| `Storage:S3:ServiceUrl` | _(empty)_ | Custom endpoint for S3-compatible storage (e.g. MinIO `http://localhost:9000`) |
| `Storage:S3:ForcePathStyle` | `false` | Path-style addressing (`host/bucket/key`) — required by most S3-compatible servers |
| `Storage:S3:PublicUrl` | _(empty)_ | Optional CDN / public base URL. Falls back to the bucket's virtual-hosted URL |

## Example — Local (default)

```json
{
  "Storage": {
    "Driver": "Local",
    "Root": "storage/app",
    "PublicUrl": "/storage"
  }
}
```

## Example — AWS S3

```json
{
  "Storage": {
    "Driver": "S3",
    "S3": {
      "Bucket": "my-app-uploads",
      "Region": "eu-west-1",
      "PublicUrl": "https://cdn.example.com"
    }
  }
}
```

Leave `AccessKeyId` / `SecretAccessKey` empty to authenticate with the default AWS credential chain
(IAM role, environment variables, or a shared profile). For S3-compatible storage such as MinIO, set
`ServiceUrl` and `ForcePathStyle: true`.

> **Secrets:** never commit real access keys. Provide them via `dotnet user-secrets`, environment
> variables (`Storage__S3__SecretAccessKey`), or a vault.


## Usage

Inject `IStorage` or use the static `Storage` facade. See [File Storage](/docs/storage/overview).

## Related

- [File Storage](/docs/storage/overview)
- [Configuration Overview](/docs/configuration/overview)
