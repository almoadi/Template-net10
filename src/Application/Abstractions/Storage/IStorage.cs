namespace Template_net10.Application.Abstractions.Storage;

/// <summary>
/// File storage abstraction (Laravel: the <c>Storage</c> facade). Paths are relative to the
/// configured storage root and always use forward slashes. The default driver writes to local
/// disk; swap the Infrastructure implementation to target blob/object storage without touching
/// handlers. Implementations must reject path traversal (<c>..</c>) segments.
/// </summary>
public interface IStorage
{
    /// <summary>Writes <paramref name="content"/> to <paramref name="path"/>, overwriting. Returns the stored relative path. (Laravel: <c>Storage::put</c>)</summary>
    Task<string> PutAsync(string path, Stream content, CancellationToken cancellationToken = default);

    /// <summary>Writes raw bytes to <paramref name="path"/>, overwriting. Returns the stored relative path.</summary>
    Task<string> PutBytesAsync(string path, byte[] content, CancellationToken cancellationToken = default);

    /// <summary>Opens a readable stream for <paramref name="path"/>, or <c>null</c> when it does not exist. (Laravel: <c>Storage::get</c>)</summary>
    Task<Stream?> GetAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>True when a file exists at <paramref name="path"/>. (Laravel: <c>Storage::exists</c>)</summary>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>Deletes the file at <paramref name="path"/>. Safe when it is absent. (Laravel: <c>Storage::delete</c>)</summary>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>Builds the public URL for <paramref name="path"/>. (Laravel: <c>Storage::url</c>)</summary>
    string Url(string path);
}
