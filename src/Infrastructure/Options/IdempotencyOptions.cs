namespace Template_net10.Infrastructure.Options;

/// <summary>Bound from the <c>Idempotency</c> section (config/idempotency.json).</summary>
public sealed class IdempotencyOptions
{
    public const string SectionName = "Idempotency";

    /// <summary>Master switch for the idempotency middleware.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Request header carrying the client-supplied idempotency key.</summary>
    public string HeaderName { get; set; } = "Idempotency-Key";

    /// <summary>How long a stored response is replayed for repeat keys.</summary>
    public int ExpirationMinutes { get; set; } = 60;
}
