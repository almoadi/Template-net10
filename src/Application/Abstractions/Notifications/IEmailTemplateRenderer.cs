namespace Template_net10.Application.Abstractions.Notifications;

/// <summary>
/// Renders an HTML email body from a named template file, substituting <c>{{placeholder}}</c> tokens.
/// Templates live under <c>resources/email-templates/*.html</c>; implemented in Infrastructure.
/// Keeps email markup out of C# code (no inline HTML) without pulling in a Razor engine.
/// </summary>
public interface IEmailTemplateRenderer
{
    /// <summary>Loads <c>{templateName}.html</c> and replaces each <c>{{key}}</c> with its (HTML-encoded) value.</summary>
    string Render(string templateName, IReadOnlyDictionary<string, string> values);
}
