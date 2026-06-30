using MediatR;
using Microsoft.Extensions.Logging;

namespace Template_net10.Application.Behaviours;

/// <summary>Logs the start and successful completion of every request passing through the pipeline.</summary>
public sealed class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(cancellationToken);

        _logger.LogInformation("Handled {RequestName}", requestName);
        return response;
    }
}
