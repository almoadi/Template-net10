# JWT Configuration

JWT access token and refresh token settings.

**File:** `src/API/config/jwt.json`  
**Options class:** `JwtOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Jwt:Issuer` | `Template-net10` | Token issuer |
| `Jwt:Audience` | `Template-net10.Client` | Token audience |
| `Jwt:SecretKey` | (dev placeholder) | HMAC-SHA256 signing key (≥ 32 chars) |
| `Jwt:ExpiryMinutes` | `60` | Access token lifetime |
| `Jwt:RefreshTokenExpiryDays` | `30` | Refresh token / session lifetime |

## Generate a Secret Key

```powershell
dotnet run --project tools/Do -- key:generate
```

This updates every `config/**/jwt.json` while preserving JSON comments.

## Security

> **Never commit production secrets.** Supply `Jwt:SecretKey` via user-secrets or environment variables:

```powershell
dotnet user-secrets set "Jwt:SecretKey" "YOUR_64_CHAR_SECRET_HERE"
```

## Related

- [Authentication Overview](/docs/authentication/overview)
- [Login](/docs/authentication/login)
