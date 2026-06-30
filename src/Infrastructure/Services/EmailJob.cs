using Template_net10.Application.Abstractions.Jobs;
using Template_net10.Application.Abstractions.Notifications;

namespace Template_net10.Infrastructure.Services;

/// <summary>Hangfire job that delegates to <see cref="IEmailSender"/> off the request thread.</summary>
public sealed class EmailJob : IEmailJob
{
    private readonly IEmailSender _emailSender;

    public EmailJob(IEmailSender emailSender) => _emailSender = emailSender;

    public Task SendAsync(EmailMessage message) => _emailSender.SendAsync(message);

    public Task SendBulkAsync(IReadOnlyList<EmailMessage> messages) => _emailSender.SendBulkAsync(messages);
}
