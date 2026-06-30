namespace Template_net10.Application.Abstractions.Notifications;

/// <summary>A transport-agnostic email message handed to <see cref="IEmailSender"/>.</summary>
public sealed class EmailMessage
{
    public required IReadOnlyList<string> To { get; init; }

    public required string Subject { get; init; }

    public required string HtmlBody { get; init; }

    public string? PlainTextBody { get; init; }

    public static EmailMessage To1(string to, string subject, string htmlBody, string? plainTextBody = null)
        => new() { To = [to], Subject = subject, HtmlBody = htmlBody, PlainTextBody = plainTextBody };
}
