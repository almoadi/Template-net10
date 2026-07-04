using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Template_net10.Application.Abstractions.Auth.Social;
using Template_net10.Application.Abstractions.Data;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Domain.Auth.Entities;
using Template_net10.Domain.Auth.Enums;
using Template_net10.Domain.Common.Exceptions;
using Template_net10.Infrastructure.Options;

namespace Template_net10.Infrastructure.Services.Auth.Social;

/// <summary>
/// Backing implementation of the Laravel-style <see cref="ISocialite"/> facade. Selects the provider
/// driver, resolves the caller's profile from the supplied access token, then finds or provisions the
/// local user (linking a <see cref="SocialAccount"/>) and issues this application's own token pair via
/// <see cref="ITokenIssuer"/>. Provider access is opt-in through <see cref="SocialiteOptions"/>.
/// </summary>
public sealed class SocialiteService : ISocialite
{
    private readonly IEnumerable<ISocialProviderDriver> _drivers;
    private readonly IApplicationDbContext _context;
    private readonly ITokenIssuer _tokenIssuer;
    private readonly ILocalizationService _localization;
    private readonly SocialiteOptions _options;

    public SocialiteService(
        IEnumerable<ISocialProviderDriver> drivers,
        IApplicationDbContext context,
        ITokenIssuer tokenIssuer,
        ILocalizationService localization,
        IOptions<SocialiteOptions> options)
    {
        _drivers = drivers;
        _context = context;
        _tokenIssuer = tokenIssuer;
        _localization = localization;
        _options = options.Value;
    }

    public async Task<AuthTokenDto> LoginWithTokenAsync(
        SocialProvider provider, string accessToken, CancellationToken cancellationToken = default)
    {
        if (!ProviderOptions(provider).Enabled)
        {
            throw new BadRequestException(_localization.GetMessage(Resource.SocialProviderNotConfigured));
        }

        var driver = _drivers.FirstOrDefault(d => d.Provider == provider)
            ?? throw new BadRequestException(_localization.GetMessage(Resource.SocialProviderNotConfigured));

        SocialUser socialUser;
        try
        {
            socialUser = await driver.GetUserAsync(accessToken, cancellationToken);
        }
        catch (Exception ex) when (ex is not BadRequestException)
        {
            throw new BadRequestException(_localization.GetMessage(Resource.SocialLoginFailed));
        }

        var userId = await ResolveUserIdAsync(socialUser, cancellationToken);

        return await _tokenIssuer.IssueAsync(userId, cancellationToken)
            ?? throw new BadRequestException(_localization.GetMessage(Resource.SocialLoginFailed));
    }

    /// <summary>Finds the linked account, else matches by email, else provisions a new user — then links the social account.</summary>
    private async Task<int> ResolveUserIdAsync(SocialUser socialUser, CancellationToken cancellationToken)
    {
        var email = socialUser.Email.Trim().ToLowerInvariant();

        // 1. Already linked to this exact provider identity — refresh the cached profile and reuse it.
        var account = await _context.SocialAccounts
            .FirstOrDefaultAsync(
                a => a.Provider == socialUser.Provider && a.ProviderUserId == socialUser.ProviderUserId,
                cancellationToken);

        if (account is not null)
        {
            account.UpdateProfile(email, socialUser.Name, socialUser.AvatarUrl);
            await _context.SaveChangesAsync(cancellationToken);
            return account.UserId;
        }

        // 2. A local account with the same email — link the new provider to it.
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        // 3. First time we see this person — provision a passwordless, email-verified user.
        if (user is null)
        {
            user = User.CreateFromSocial(socialUser.Name, socialUser.Name, email);
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        _context.SocialAccounts.Add(
            SocialAccount.Create(user.Id, socialUser.Provider, socialUser.ProviderUserId, email, socialUser.Name, socialUser.AvatarUrl));
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private SocialProviderOptions ProviderOptions(SocialProvider provider) => provider switch
    {
        SocialProvider.Google => _options.Google,
        SocialProvider.Azure => _options.Azure,
        _ => new SocialProviderOptions(),
    };
}
