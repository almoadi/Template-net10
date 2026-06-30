namespace Template_net10.Infrastructure.Options;

public enum MailDriver
{
    /// <summary>Writes the rendered message to the log instead of sending (great for local dev).</summary>
    Log,

    /// <summary>Sends over SMTP via MailKit.</summary>
    Smtp,
}

/// <summary>Bound from the <c>Mail</c> section (config/mail.json).</summary>
public sealed class MailOptions
{
    public const string SectionName = "Mail";

    public MailDriver Driver { get; set; } = MailDriver.Log;

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool UseStartTls { get; set; } = true;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromAddress { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;
}
