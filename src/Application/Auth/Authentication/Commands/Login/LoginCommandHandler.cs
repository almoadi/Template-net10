using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly IAuth _auth;
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public LoginCommandHandler(IAuth auth, IAccountService account, ILocalizationService localization)
    {
        _auth = auth;
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        // Optional gate: enforces email verification / triggers two-factor when enabled in config/auth.json.
        // No-op when those flags are off; silent when credentials are invalid.
        await _account.EnsureLoginAllowedAsync(command.Email, command.Password, ct);

        var token = await _auth.Attempt(command.Email, command.Password, ct);

        return token is null
            ? throw new BadRequestException(_localization.GetMessage(Resource.InvalidCredentials))
            : ApiResponseDto<AuthTokenDto>.Success(token);
    }
}
