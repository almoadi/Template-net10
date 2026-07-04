# Idempotency

`IdempotencyMiddleware` makes mutating requests safe to retry. When a client sends an
`Idempotency-Key` header on a `POST`/`PUT`/`PATCH`/`DELETE`, the first successful response is cached
and **replayed** for any repeat with the same key — so a retried payment or order never runs twice.

## How it works

1. The middleware runs after authentication/authorization.
2. On the first request for a key, the response is buffered, returned, and cached (2xx only).
3. On a repeat with the same key, the cached status, content type, and body are replayed with an
   `Idempotency-Replayed: true` response header — the handler never runs again.

Responses are stored in the configured distributed cache (Memory or Redis) via `ICacheService`.

## Client usage

```http
POST /api/orders HTTP/1.1
Idempotency-Key: 6f9c2b1a-...-unique-per-operation
Content-Type: application/json

{ "sku": "ABC", "qty": 2 }
```

Send the **same** key when retrying the same logical operation; use a **new** key for a new operation.

## Configuration

`config/idempotency.json`:

| Key | Default | Description |
|-----|---------|-------------|
| `Idempotency:Enabled` | `true` | Master switch |
| `Idempotency:HeaderName` | `Idempotency-Key` | Header carrying the key |
| `Idempotency:ExpirationMinutes` | `60` | How long a response is replayed |

## Related

- [Cache Service](/docs/caching/cache-service)
- [Correlation ID](/docs/api/correlation-id)
