using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Hosting;
using Template_net10.Application.Abstractions.Notifications;

namespace Template_net10.Infrastructure.Services.Mail;

/// <summary>
/// File-backed <see cref="IEmailTemplateRenderer"/>. Loads HTML templates from
/// <c>resources/email-templates/{name}.html</c> (relative to the content root), caches the raw markup,
/// and replaces <c>{{key}}</c> tokens with HTML-encoded values. Falls back to a minimal built-in
/// layout when a template file is missing.
/// </summary>
public sealed class FileEmailTemplateRenderer : IEmailTemplateRenderer
{
    private const string TemplateDirectory = "resources/email-templates";

    private static readonly ConcurrentDictionary<string, string> Cache = new();

    private readonly string _templateRoot;

    public FileEmailTemplateRenderer(IHostEnvironment environment)
        => _templateRoot = Path.Combine(environment.ContentRootPath, TemplateDirectory);

    public string Render(string templateName, IReadOnlyDictionary<string, string> values)
    {
        var template = Cache.GetOrAdd(templateName, LoadTemplate);

        foreach (var (key, value) in values)
        {
            template = template.Replace($"{{{{{key}}}}}", WebUtility.HtmlEncode(value ?? string.Empty));
        }

        return template;
    }

    private string LoadTemplate(string templateName)
    {
        var path = Path.Combine(_templateRoot, $"{templateName}.html");
        return File.Exists(path)
            ? File.ReadAllText(path)
            : "<html><body><h2>{{title}}</h2><p>{{intro}}</p><p><strong>{{code}}</strong></p></body></html>";
    }
}
