# CORS Configuration

Cross-Origin Resource Sharing settings for browser clients.

**File:** `src/API/config/cors.json`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `AllowedHosts` | `"*"` | ASP.NET allowed hosts |
| `Cors:AllowedOrigins` | `[]` | Origins permitted for CORS |

## Example

To allow a frontend at `http://localhost:3000`:

```json
{
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000", "https://app.example.com" ]
  }
}
```

CORS is wired in `CorsServiceExtensions.cs` during startup.

## Related

- [Configuration Overview](/docs/configuration/overview)
