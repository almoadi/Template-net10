using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.RequestEmailVerification;

public sealed class RequestEmailVerificationCommandHandler
    : IRequestHandler<RequestEmailVerificationCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public RequestEmailVerificationCommandHandler(IAccountService account, ILocalizationService localization)
    {
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(
        RequestEmailVerificationCommand command, CancellationToken ct)
    {
        // Always report success so a caller cannot probe which emails exist.
        await _account.SendEmailVerificationAsync(command.Email, ct);

        return ApiResponseDto<MessageDto>.Success(
            MessageDto.Of(_localization.GetMessage(Resource.EmailVerificationSent)));
    }
}
