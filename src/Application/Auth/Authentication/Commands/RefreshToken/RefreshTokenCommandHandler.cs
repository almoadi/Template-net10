using MediatR;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly IAuth _auth;
    private readonly ILocalizationService _localization;

    public RefreshTokenCommandHandler(IAuth auth, ILocalizationService localization)
    {
        _auth = auth;
        _localization = localization;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(RefreshTokenCommand command, CancellationToken ct)
    {
        var token = await _auth.Refresh(command.RefreshToken, ct);

        return token is null
            ? throw new BadRequestException(_localization.GetMessage(Resource.InvalidRefreshToken))
            : ApiResponseDto<AuthTokenDto>.Success(token);
    }
}
