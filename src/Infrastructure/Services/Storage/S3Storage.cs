using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Storage;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Storage;

/// <summary>
/// AWS S3 (or S3-compatible, e.g. MinIO) <see cref="IStorage"/> driver. Object keys mirror the
/// relative path and always use forward slashes. All incoming paths are normalized and validated to
/// reject path-traversal (<c>..</c>) segments.
/// </summary>
public sealed class S3Storage : IStorage, IDisposable
{
    private readonly S3StorageOptions _options;
    private readonly IAmazonS3 _client;

    public S3Storage(IOptions<StorageOptions> options)
    {
        _options = options.Value.S3;

        if (string.IsNullOrWhiteSpace(_options.Bucket))
        {
            throw new InvalidOperationException("Storage:S3:Bucket must be configured when the S3 driver is selected.");
        }

        var config = new AmazonS3Config();
        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            config.ServiceURL = _options.ServiceUrl;
            config.ForcePathStyle = _options.ForcePathStyle;
        }
        else
        {
            config.RegionEndpoint = RegionEndpoint.GetBySystemName(_options.Region);
        }

        _client = !string.IsNullOrWhiteSpace(_options.AccessKeyId) && !string.IsNullOrWhiteSpace(_options.SecretAccessKey)
            ? new AmazonS3Client(_options.AccessKeyId, _options.SecretAccessKey, config)
            : new AmazonS3Client(config);
    }

    public async Task<string> PutAsync(string path, Stream content, CancellationToken cancellationToken = default)
    {
        var key = ResolveKey(path);
        await _client.PutObjectAsync(
            new PutObjectRequest
            {
                BucketName = _options.Bucket,
                Key = key,
                InputStream = content,
                AutoCloseStream = false,
            },
            cancellationToken);

        return key;
    }

    public async Task<string> PutBytesAsync(string path, byte[] content, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(content, writable: false);
        return await PutAsync(path, stream, cancellationToken);
    }

    public async Task<Stream?> GetAsync(string path, CancellationToken cancellationToken = default)
    {
        var key = ResolveKey(path);
        try
        {
            var response = await _client.GetObjectAsync(_options.Bucket, key, cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        var key = ResolveKey(path);
        try
        {
            await _client.GetObjectMetadataAsync(_options.Bucket, key, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var key = ResolveKey(path);
        await _client.DeleteObjectAsync(_options.Bucket, key, cancellationToken);
    }

    public string Url(string path)
    {
        var key = ResolveKey(path);

        if (!string.IsNullOrWhiteSpace(_options.PublicUrl))
        {
            return $"{_options.PublicUrl.TrimEnd('/')}/{key}";
        }

        if (!string.IsNullOrWhiteSpace(_options.ServiceUrl))
        {
            return $"{_options.ServiceUrl.TrimEnd('/')}/{_options.Bucket}/{key}";
        }

        return $"https://{_options.Bucket}.s3.{_options.Region}.amazonaws.com/{key}";
    }

    public void Dispose() => _client.Dispose();

    /// <summary>Normalizes the path to a forward-slash S3 key and rejects path-traversal segments.</summary>
    private static string ResolveKey(string path)
    {
        var key = path.Replace('\\', '/').TrimStart('/');

        if (key.Split('/').Any(segment => segment == ".."))
        {
            throw new InvalidOperationException($"Storage path may not contain traversal segments: '{path}'.");
        }

        return key;
    }
}
