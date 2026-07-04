using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Template_net10.Application.Abstractions.Notifications;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Mail;

/// <summary>
/// Sends email over SMTP using MailKit. When <see cref="MailDriver.Log"/> is configured (or no host
/// is set) it logs the message instead of sending — a safe local-development default. Bulk sends
/// reuse a single SMTP connection for all messages.
/// </summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly MailOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<MailOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        => SendBulkAsync([message], cancellationToken);

    public async Task SendBulkAsync(
        IReadOnlyList<EmailMessage> messages, CancellationToken cancellationToken = default)
    {
        if (messages.Count == 0)
        {
            return;
        }

        if (_options.Driver == MailDriver.Log || string.IsNullOrWhiteSpace(_options.Host))
        {
            foreach (var message in messages)
            {
                _logger.LogInformation(
                    "[Mail:Log] To: {To} | Subject: {Subject}\n{Body}",
                    string.Join(", ", message.To), message.Subject, message.HtmlBody);
            }

            return;
        }

        using var client = new SmtpClient { Timeout = _options.TimeoutSeconds * 1000 };

        var socketOptions = _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
        await client.ConnectAsync(_options.Host, _options.Port, socketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        }

        foreach (var message in messages)
        {
            await client.SendAsync(BuildMime(message), cancellationToken);
        }

        await client.DisconnectAsync(quit: true, cancellationToken);

        _logger.LogInformation("Sent {Count} email(s) over SMTP", messages.Count);
    }

    private MimeMessage BuildMime(EmailMessage message)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        foreach (var recipient in message.To)
        {
            mime.To.Add(MailboxAddress.Parse(recipient));
        }

        mime.Subject = message.Subject;
        mime.Body = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.PlainTextBody,
        }.ToMessageBody();

        return mime;
    }
}
