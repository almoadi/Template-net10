using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Template_net10.Application.Abstractions.Localization;
using YamlDotNet.Serialization;

namespace Template_net10.Infrastructure.Services.Localization;

/// <summary>
/// File-backed localization. Messages live in <c>resources/lang/{language}.yml</c> (e.g. <c>en.yml</c>,
/// <c>ar.yml</c>) and are resolved for the current request culture (set by request localization
/// middleware), falling back to English. Parsed language tables are cached after first load.
/// Handlers depend only on <see cref="ILocalizationService"/>.
/// </summary>
public sealed class LocalizationService : ILocalizationService
{
    private const string FallbackLanguage = "en";
    private const string LangDirectory = "resources/lang";

    // Shared across requests: each language file is read and parsed only once.
    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<Resource, string>> Cache = new();
    private static readonly IReadOnlyDictionary<Resource, string> EmptyTable = new Dictionary<Resource, string>();

    private readonly string _langRootPath;

    public LocalizationService(IHostEnvironment environment)
        => _langRootPath = Path.Combine(environment.ContentRootPath, LangDirectory);

    public string GetMessage(Resource resource)
    {
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (GetTable(language).TryGetValue(resource, out var message))
        {
            return message;
        }

        return GetTable(FallbackLanguage).GetValueOrDefault(resource, resource.ToString());
    }

    private IReadOnlyDictionary<Resource, string> GetTable(string language)
        => Cache.GetOrAdd(language, LoadTable);

    private IReadOnlyDictionary<Resource, string> LoadTable(string language)
    {
        var path = Path.Combine(_langRootPath, $"{language}.yml");
        if (!File.Exists(path))
        {
            return EmptyTable;
        }

        var raw = new DeserializerBuilder().Build()
            .Deserialize<Dictionary<string, string>>(File.ReadAllText(path));

        if (raw is null)
        {
            return EmptyTable;
        }

        var table = new Dictionary<Resource, string>();
        foreach (var (key, value) in raw)
        {
            if (Enum.TryParse<Resource>(key, ignoreCase: true, out var resource))
            {
                table[resource] = value;
            }
        }

        return table;
    }
}
