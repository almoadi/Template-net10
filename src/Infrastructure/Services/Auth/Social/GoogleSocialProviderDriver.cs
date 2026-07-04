using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Domain.Auth.Enums;

namespace Template_net10.Infrastructure.Services.Auth.Social;

/// <summary>
/// Google social login driver. Validates the caller's OAuth access token by calling Google's
/// OpenID Connect userinfo endpoint, then maps the response into a <see cref="SocialUser"/>.
/// </summary>
public sealed class GoogleSocialProviderDriver : ISocialProviderDriver
{
    private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleSocialProviderDriver(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public SocialProvider Provider => SocialProvider.Google;

    public async Task<SocialUser> GetUserAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var profile = await response.Content.ReadFromJsonAsync<GoogleUserInfo>(cancellationToken)
            ?? throw new InvalidOperationException("Google returned an empty userinfo response.");

        if (string.IsNullOrWhiteSpace(profile.Subject) || string.IsNullOrWhiteSpace(profile.Email))
        {
            throw new InvalidOperationException("Google userinfo is missing the subject or email claim.");
        }

        return new SocialUser
        {
            Provider = SocialProvider.Google,
            ProviderUserId = profile.Subject,
            Email = profile.Email,
            Name = string.IsNullOrWhiteSpace(profile.Name) ? profile.Email : profile.Name,
            AvatarUrl = profile.Picture,
        };
    }

    private sealed record GoogleUserInfo
    {
        [JsonPropertyName("sub")]
        public string? Subject { get; init; }

        [JsonPropertyName("email")]
        public string? Email { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("picture")]
        public string? Picture { get; init; }
    }
}
