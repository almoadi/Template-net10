using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Storage;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Storage;

/// <summary>
/// Local file-system <see cref="IStorage"/> driver. Files live under <see cref="StorageOptions.Root"/>
/// (resolved against the content root when relative). All incoming paths are normalized and validated
/// to stay inside the root, rejecting path-traversal attempts.
/// </summary>
public sealed class LocalStorage : IStorage
{
    private readonly StorageOptions _options;
    private readonly string _rootPath;

    public LocalStorage(IOptions<StorageOptions> options, IHostEnvironment environment)
    {
        _options = options.Value;
        _rootPath = Path.IsPathRooted(_options.Root)
            ? _options.Root
            : Path.Combine(environment.ContentRootPath, _options.Root);
    }

    public async Task<string> PutAsync(string path, Stream content, CancellationToken cancellationToken = default)
    {
        var fullPath = ResolveAndEnsureDirectory(path);
        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, cancellationToken);
        return Normalize(path);
    }

    public async Task<string> PutBytesAsync(string path, byte[] content, CancellationToken cancellationToken = default)
    {
        var fullPath = ResolveAndEnsureDirectory(path);
        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        return Normalize(path);
    }

    public Task<Stream?> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Resolve(path);
        Stream? stream = File.Exists(fullPath)
            ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null;

        return Task.FromResult(stream);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(File.Exists(Resolve(path)));

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Resolve(path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public string Url(string path)
        => $"{_options.PublicUrl.TrimEnd('/')}/{Normalize(path).TrimStart('/')}";

    private string ResolveAndEnsureDirectory(string path)
    {
        var fullPath = Resolve(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return fullPath;
    }

    private string Resolve(string path)
    {
        var normalized = Normalize(path).TrimStart('/');
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, normalized));
        var rootFull = Path.GetFullPath(_rootPath);

        if (!fullPath.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Resolved storage path escapes the storage root: '{path}'.");
        }

        return fullPath;
    }

    private static string Normalize(string path)
        => path.Replace('\\', '/');
}
