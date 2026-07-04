# Queue Configuration

Background job processing via Hangfire.

**File:** `src/API/config/queue.json`  
**Options class:** `QueueOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Queue:Driver` | `Hangfire` | Currently only `Hangfire` |
| `Queue:DashboardEnabled` | `true` | Expose Hangfire dashboard UI |
| `Queue:DashboardPath` | `/hangfire` | Dashboard URL path |
| `Queue:WorkerCount` | `0` | `0` = processor count × 5 |
| `Queue:Queues` | `["default", "emails"]` | Queues processed in priority order |
| `Queue:SchemaName` | `HangFire` | SQL schema for Hangfire tables |

## Dashboard

When enabled, visit `/hangfire` in the browser. Access requires an authenticated admin user via `HangfireDashboardAuthorizationFilter`.

## Storage

Hangfire uses the same SQL Server database as EF Core (from `database.json`).

## Related

- [Queue Overview](/docs/queue/overview)
- [Creating Jobs](/docs/queue/creating-jobs)
