# Security Headers

`SecurityHeadersMiddleware` adds a baseline set of hardening response headers to every response.
It runs early in the [request pipeline](/docs/architecture/mediatr-pipeline) and applies safe
defaults suitable for a JSON API.

## Headers applied

| Header | Value | Purpose |
|--------|-------|---------|
| `X-Content-Type-Options` | `nosniff` | Stops MIME-type sniffing |
| `X-Frame-Options` | `DENY` | Prevents clickjacking via framing |
| `Referrer-Policy` | `no-referrer` | Never leaks the referring URL |
| `X-Permitted-Cross-Domain-Policies` | `none` | Blocks Adobe cross-domain policies |
| `Cross-Origin-Opener-Policy` | `same-origin` | Isolates the browsing context |

The middleware also removes `X-Powered-By` and `Server` headers that leak implementation details.

## Customizing

Edit `SecurityHeadersMiddleware` in `src/Infrastructure/Middleware/` to tighten or extend the set per
deployment. HSTS is intentionally left to `UseHttpsRedirection` and host/reverse-proxy configuration.

## Related

- [Correlation ID](/docs/api/correlation-id)
- [Exception Handling](/docs/api/exception-handling)
