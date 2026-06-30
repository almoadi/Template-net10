using Template_net10.Application.Abstractions.Notifications;

namespace Template_net10.Application.Abstractions.Jobs;

/// <summary>
/// Background job that sends email off the request thread. Enqueue via
/// <see cref="IJobScheduler"/>; the implementation runs inside a Hangfire worker.
/// </summary>
public interface IEmailJob
{
    Task SendAsync(EmailMessage message);

    /// <summary>Sends many messages in one job, reusing a single SMTP connection.</summary>
    Task SendBulkAsync(IReadOnlyList<EmailMessage> messages);
}
