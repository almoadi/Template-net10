namespace Template_net10.Infrastructure.Options;

public enum StorageDriver
{
    /// <summary>Stores files on the local file system under <see cref="StorageOptions.Root"/>.</summary>
    Local,

    /// <summary>Stores files in an AWS S3 (or S3-compatible) bucket. See <see cref="StorageOptions.S3"/>.</summary>
    S3,
}

/// <summary>Bound from the <c>Storage</c> section (config/storage.json).</summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public StorageDriver Driver { get; set; } = StorageDriver.Local;

    /// <summary>Absolute or content-root-relative directory where files are stored (Local driver).</summary>
    public string Root { get; set; } = "storage/app";

    /// <summary>Public base URL used to build file links (e.g. served by a reverse proxy / CDN).</summary>
    public string PublicUrl { get; set; } = "/storage";

    /// <summary>Settings for the <see cref="StorageDriver.S3"/> driver.</summary>
    public S3StorageOptions S3 { get; set; } = new();
}

/// <summary>AWS S3 (or S3-compatible, e.g. MinIO) driver settings.</summary>
public sealed class S3StorageOptions
{
    /// <summary>Target bucket name.</summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>AWS region system name (e.g. <c>us-east-1</c>). Ignored when <see cref="ServiceUrl"/> is set.</summary>
    public string Region { get; set; } = "us-east-1";

    /// <summary>Optional access key. Leave empty to use the default AWS credential chain (IAM role, env vars, profile).</summary>
    public string? AccessKeyId { get; set; }

    /// <summary>Optional secret key. Leave empty to use the default AWS credential chain.</summary>
    public string? SecretAccessKey { get; set; }

    /// <summary>Optional custom endpoint for S3-compatible storage (e.g. MinIO: <c>http://localhost:9000</c>).</summary>
    public string? ServiceUrl { get; set; }

    /// <summary>Use path-style addressing (<c>host/bucket/key</c>). Required by most S3-compatible servers such as MinIO.</summary>
    public bool ForcePathStyle { get; set; }

    /// <summary>Optional CDN / public base URL for building file links. Falls back to the bucket's virtual-hosted URL.</summary>
    public string? PublicUrl { get; set; }
}

