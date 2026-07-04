# Correlation ID

`CorrelationIdMiddleware` ensures every request carries a correlation id so you can trace a single
request across all of its log entries.

## How it works

1. Reads the inbound `X-Correlation-ID` header, or generates a new id when absent.
2. Sets `HttpContext.TraceIdentifier` to that id.
3. Echoes the id back on the response `X-Correlation-ID` header.
4. Opens a logging scope so every log written during the request is tagged with `CorrelationId`.

It runs first in the [request pipeline](/docs/architecture/mediatr-pipeline) — before exception
handling — so even error logs carry the id.

## Client usage

Send your own id to correlate a call chain across services:

```http
GET /api/users HTTP/1.1
X-Correlation-ID: 9f8c7b6a5d4e3f2a1b0c
```

The same id is returned on the response and appears in the server logs for that request.

## Related

- [Security Headers](/docs/api/security-headers)
- [Exception Handling](/docs/api/exception-handling)
