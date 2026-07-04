# Mail Overview

Email is sent through the `IEmailSender` abstraction — there is no HTTP endpoint for mail. Call it from handlers, services, or background jobs.

## Abstraction

```csharp
public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
    Task SendBulkAsync(IEnumerable<EmailMessage> messages, CancellationToken ct = default);
}
```

## Drivers

Configured via `Mail:Driver` in `config/mail.json`:

| Driver | Implementation | Use case |
|--------|----------------|----------|
| `Log` | Logs the rendered message | Local development |
| `Smtp` | MailKit SMTP sender | Production |

Implementations are registered in `Infrastructure/DependencyInjection.cs` based on the driver setting.

## EmailMessage

```csharp
new EmailMessage
{
    To = "user@example.com",
    Subject = "Welcome",
    Body = "<p>Hello!</p>",
    IsHtml = true
}
```

## When & How to Use It

Email is how your app talks to people outside a live request. Because it's an abstraction (not an
endpoint), you call it from inside your own use cases whenever something noteworthy happens:

- **Welcome a new user** — send a greeting right after an account is created.
- **Password reset / verification** — email a one-time link or code.
- **Notifications & receipts** — order confirmations, invoices, alerts.
- **Send in the background** — for anything non-urgent, enqueue a job so the HTTP response returns
  instantly and the email goes out on a worker (see [Creating Jobs](/docs/queue/creating-jobs)).
- **Test safely on your machine** — keep `Mail:Driver = "Log"` in development so emails are
  written to the log instead of actually being sent, then switch to `Smtp` in production.

**Typical flow:** inject `IEmailSender` → build an `EmailMessage` → `SendAsync` directly for a
quick note, or enqueue an email job when volume or latency matters.

## Related

- [Mail Configuration](/docs/configuration/mail)
- [Sending Email](/docs/mail/sending-email)
- [Creating Jobs](/docs/queue/creating-jobs)
