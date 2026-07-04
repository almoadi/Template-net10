using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Account.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public ResetPasswordCommandHandler(IAccountService account, ILocalizationService localization)
    {
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(ResetPasswordCommand command, CancellationToken ct)
    {
        var reset = await _account.ResetPasswordAsync(command.Email, command.Token, command.NewPassword, ct);

        return reset
            ? ApiResponseDto<MessageDto>.Success(MessageDto.Of(_localization.GetMessage(Resource.PasswordReset)))
            : throw new BadRequestException(_localization.GetMessage(Resource.InvalidOrExpiredCode));
    }
}
