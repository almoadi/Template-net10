# Sending Email

Send email from handlers or background jobs using `IEmailSender`.

## Single Email

```csharp
public sealed class WelcomeUserHandler(IEmailSender emailSender, ILocalizationService localization)
{
    public async Task Handle(WelcomeUserCommand command, CancellationToken ct)
    {
        await emailSender.SendAsync(new EmailMessage
        {
            To = command.Email,
            Subject = localization.GetMessage(Resource.WelcomeEmailSubject),
            Body = localization.GetMessage(Resource.WelcomeEmailBody),
            IsHtml = true,
        }, ct);
    }
}
```

## Bulk Email

```csharp
var messages = users.Select(u => new EmailMessage
{
    To = u.Email,
    Subject = subject,
    Body = body,
});

await _emailSender.SendBulkAsync(messages, ct);
```

## Background Jobs

For large sends, enqueue via Hangfire:

```csharp
_jobScheduler.Enqueue<IEmailJob>(j => j.SendAsync(emailMessage));
```

See [Creating Jobs](/docs/queue/creating-jobs).

## Local Development

With `Mail:Driver` set to `Log`, emails appear in the application log instead of being sent — no SMTP server required.

## Related

- [Mail Overview](/docs/mail/overview)
- [Mail Configuration](/docs/configuration/mail)
