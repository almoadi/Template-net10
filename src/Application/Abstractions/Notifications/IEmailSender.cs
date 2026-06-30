namespace Template_net10.Application.Abstractions.Notifications;

/// <summary>Sends emails. Implemented in Infrastructure (SMTP via MailKit, or a log driver for dev).</summary>
public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>Sends multiple messages, reusing a single SMTP connection where possible.</summary>
    Task SendBulkAsync(IReadOnlyList<EmailMessage> messages, CancellationToken cancellationToken = default);
}
