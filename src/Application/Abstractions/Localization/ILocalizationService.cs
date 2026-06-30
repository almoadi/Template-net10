namespace Template_net10.Application.Abstractions.Localization;

/// <summary>Resolves a <see cref="Resource"/> key into a localized, user-facing string.</summary>
public interface ILocalizationService
{
    string GetMessage(Resource resource);
}
