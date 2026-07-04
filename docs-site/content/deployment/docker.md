# Docker

The template ships a production-style container setup: a multi-stage `Dockerfile` for the API and a
`docker-compose.yml` that also starts SQL Server.

## Quick start (Compose)

```bash
docker compose up --build
```

This builds the API image, starts SQL Server, waits until it is healthy, then runs the API with
migrations + seeding applied. The API is available at **http://localhost:8080** (Swagger at
`/swagger` in Development).

## Image only

```bash
docker build -t template-net10-api .
docker run -p 8080:8080 \
  -e Database__ConnectionString="Server=host.docker.internal,1433;Database=Template_net10;User Id=sa;Password=Your_strong!Passw0rd;TrustServerCertificate=true;Encrypt=false;" \
  -e Jwt__SecretKey="change-me-at-least-32-bytes-long-please" \
  template-net10-api
```

## How it works

- **Multi-stage build** — restores against `Directory.Packages.props` + the project files (cached
  layer), publishes the API, then copies the output onto the smaller ASP.NET runtime image.
- **PDF support** — installs `libfontconfig1` so QuestPDF/SkiaSharp can render on Linux.
- **Non-root** — runs as the image's `app` user; `/app/storage/app` is created and owned for the
  local file [Storage](/docs/storage/overview) driver.
- **Port** — listens on `8080` (the runtime default).

## Configuration via environment variables

Environment variables are applied **after** the `config/*.json` files, so they override any setting
using the `Section__Key` convention:

| Variable | Overrides |
|----------|-----------|
| `Database__ConnectionString` | `Database:ConnectionString` |
| `Jwt__SecretKey` | `Jwt:SecretKey` |
| `Encryption__Key` | `Encryption:Key` |
| `ASPNETCORE_ENVIRONMENT` | Hosting environment (`Development`/`Staging`/`Production`) |

> The passwords and keys in `docker-compose.yml` are for local development only — replace them (and
> source real secrets from your orchestrator/vault) before any real deployment.

## Related

- [Configuration Overview](/docs/configuration/overview)
- [Database Migrations](/docs/database/migrations)
