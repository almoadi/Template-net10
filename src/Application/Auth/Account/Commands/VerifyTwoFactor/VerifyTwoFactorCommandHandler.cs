using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Auth.Authentication;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Account.Commands.VerifyTwoFactor;

public sealed class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public VerifyTwoFactorCommandHandler(IAccountService account, ILocalizationService localization)
    {
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(VerifyTwoFactorCommand command, CancellationToken ct)
    {
        var token = await _account.VerifyTwoFactorAsync(command.Email, command.Code, ct);

        return token is null
            ? throw new BadRequestException(_localization.GetMessage(Resource.InvalidOrExpiredCode))
            : ApiResponseDto<AuthTokenDto>.Success(token);
    }
}
