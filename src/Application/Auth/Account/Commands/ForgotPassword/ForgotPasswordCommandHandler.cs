using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;

namespace Template_net10.Application.Auth.Account.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public ForgotPasswordCommandHandler(IAccountService account, ILocalizationService localization)
    {
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(ForgotPasswordCommand command, CancellationToken ct)
    {
        // Always report success so a caller cannot enumerate registered emails.
        await _account.SendPasswordResetAsync(command.Email, ct);

        return ApiResponseDto<MessageDto>.Success(
            MessageDto.Of(_localization.GetMessage(Resource.PasswordResetSent)));
    }
}
