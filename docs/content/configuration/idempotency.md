# Idempotency Configuration

Settings for the idempotency middleware.

**File:** `src/API/config/idempotency.json`  
**Options class:** `IdempotencyOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Idempotency:Enabled` | `true` | Master switch for the middleware |
| `Idempotency:HeaderName` | `Idempotency-Key` | Request header carrying the key |
| `Idempotency:ExpirationMinutes` | `60` | How long a stored response is replayed |

## Usage

Clients send the configured header on mutating requests. See [Idempotency](/docs/api/idempotency).

## Related

- [Idempotency](/docs/api/idempotency)
- [Configuration Overview](/docs/configuration/overview)
