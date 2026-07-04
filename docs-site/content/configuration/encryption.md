# Encryption Configuration

Application encryption key settings.

**File:** `src/API/config/encryption.json`  
**Options class:** `EncryptionOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Encryption:Key` | `""` | Base64-encoded 32-byte key. Any other non-empty string is hashed to a 256-bit key |

## Generating a key

```bash
openssl rand -base64 32
```

Supply the value via user-secrets (development) or environment variables / a vault (production).
Never commit real keys.

## Usage

Inject `IEncryptor` or use the static `Crypt` facade. See [Encryption](/docs/security/encryption).

## Related

- [Encryption](/docs/security/encryption)
- [Configuration Overview](/docs/configuration/overview)
