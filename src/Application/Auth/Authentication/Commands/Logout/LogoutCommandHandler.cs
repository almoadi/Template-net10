using MediatR;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAuth _auth;
    private readonly ILocalizationService _localization;

    public LogoutCommandHandler(IAuth auth, ILocalizationService localization)
    {
        _auth = auth;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(LogoutCommand command, CancellationToken ct)
    {
        // Always report success so a caller cannot probe which refresh tokens exist.
        await _auth.Logout(command.RefreshToken, ct);

        return ApiResponseDto<MessageDto>.Success(
            MessageDto.Of(_localization.GetMessage(Resource.LoggedOut)));
    }
}
