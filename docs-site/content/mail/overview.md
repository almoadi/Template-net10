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

## Related

- [Mail Configuration](/docs/configuration/mail)
- [Sending Email](/docs/mail/sending-email)
- [Creating Jobs](/docs/queue/creating-jobs)
