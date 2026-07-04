using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Infrastructure.Services.Auth.Social;

/// <summary>
/// Microsoft Entra ID (Azure AD) social login driver. Validates the caller's access token by calling
/// the Microsoft Graph <c>/me</c> endpoint, then maps the response into a <see cref="SocialUser"/>.
/// The supplied token must carry a Microsoft Graph scope (e.g. <c>User.Read</c>).
/// </summary>
public sealed class AzureSocialProviderDriver : ISocialProviderDriver
{
    private const string MeEndpoint = "https://graph.microsoft.com/v1.0/me";

    private readonly IHttpClientFactory _httpClientFactory;

    public AzureSocialProviderDriver(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SocialProvider Provider => SocialProvider.Azure;

    public async Task<SocialUser> GetUserAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, MeEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var profile = await response.Content.ReadFromJsonAsync<GraphUser>(cancellationToken)
            ?? throw new InvalidOperationException("Microsoft Graph returned an empty /me response.");

        var email = !string.IsNullOrWhiteSpace(profile.Mail) ? profile.Mail : profile.UserPrincipalName;

        if (string.IsNullOrWhiteSpace(profile.Id) || string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Microsoft Graph /me is missing the id or email.");
        }

        return new SocialUser
        {
            Provider = SocialProvider.Azure,
            ProviderUserId = profile.Id,
            Email = email,
            Name = string.IsNullOrWhiteSpace(profile.DisplayName) ? email : profile.DisplayName,
            AvatarUrl = null,
        };
    }

    private sealed record GraphUser
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; init; }

        [JsonPropertyName("mail")]
        public string? Mail { get; init; }

        [JsonPropertyName("userPrincipalName")]
        public string? UserPrincipalName { get; init; }
    }
}
