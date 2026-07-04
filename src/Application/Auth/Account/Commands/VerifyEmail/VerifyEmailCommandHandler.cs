using MediatR;
using Template_net10.Application.Abstractions.Account;
using Template_net10.Application.Abstractions.Localization;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Application.Auth.Account.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponseDto<MessageDto>>
{
    private readonly IAccountService _account;
    private readonly ILocalizationService _localization;

    public VerifyEmailCommandHandler(IAccountService account, ILocalizationService localization)
    {
        _account = account;
        _localization = localization;
    }

    public async Task<ApiResponseDto<MessageDto>> Handle(VerifyEmailCommand command, CancellationToken ct)
    {
        var verified = await _account.VerifyEmailAsync(command.Email, command.Token, ct);

        return verified
            ? ApiResponseDto<MessageDto>.Success(MessageDto.Of(_localization.GetMessage(Resource.EmailVerified)))
            : throw new BadRequestException(_localization.GetMessage(Resource.InvalidOrExpiredCode));
    }
}
