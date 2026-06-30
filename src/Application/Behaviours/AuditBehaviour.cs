using MediatR;
using Microsoft.Extensions.Logging;
using Template_net10.Application.Abstractions.Security;

namespace Template_net10.Application.Behaviours;

/// <summary>
/// Records an audit trail entry for write requests (commands), attributing the action to
/// the current user. Reads (queries) are skipped.
/// </summary>
public sealed class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AuditBehaviour<TRequest, TResponse>> _logger;

    public AuditBehaviour(
        ICurrentUserService currentUser, ILogger<AuditBehaviour<TRequest, TResponse>> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var isCommand = requestName.EndsWith("Command", StringComparison.Ordinal);

        var response = await next(cancellationToken);

        if (isCommand)
        {
            _logger.LogInformation(
                "Audit: {RequestName} executed by user {UserId}",
                requestName, _currentUser.UserId);
        }

        return response;
    }
}
