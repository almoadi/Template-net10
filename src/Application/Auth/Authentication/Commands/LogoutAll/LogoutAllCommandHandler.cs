using MediatR;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Authentication.Commands.LogoutAll;

public sealed class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAuth _auth;
    private readonly ILocalizationService _localization;

    public LogoutAllCommandHandler(IAuth auth, ILocalizationService localization)
    {
        _auth = auth;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(LogoutAllCommand command, CancellationToken ct)
    {
        await _auth.LogoutAll(ct);

        return ApiResponseDto<MessageDto>.Success(
            MessageDto.Of(_localization.GetMessage(Resource.AllSessionsRevoked)));
    }
}
