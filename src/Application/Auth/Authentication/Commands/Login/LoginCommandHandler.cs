using MediatR;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Abstractions.Security;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Authentication.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponseDto<AuthTokenDto>>
{
    private readonly IAuth _auth;
    private readonly ILocalizationService _localization;

    public LoginCommandHandler(IAuth auth, ILocalizationService localization)
    {
        _auth = auth;
        _localization = localization;
    }

    public async Task<ApiResponseDto<AuthTokenDto>> Handle(LoginCommand command, CancellationToken ct)
    {
        var token = await _auth.Attempt(command.Email, command.Password, ct);

        return token is null
            ? throw new BadRequestException(_localization.GetMessage(Resource.InvalidCredentials))
            : ApiResponseDto<AuthTokenDto>.Success(token);
    }
}
