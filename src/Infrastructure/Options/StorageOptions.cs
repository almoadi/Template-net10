namespace Template_net10.Infrastructure.Options;

public enum StorageDriver
{
    /// <summary>Stores files on the local file system under <see cref="StorageOptions.Root"/>.</summary>
    Local,
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
}
