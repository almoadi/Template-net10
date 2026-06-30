using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Template_net10.Application.Behaviours;

/// <summary>Times each request and logs a warning when it exceeds the slow-request threshold.</summary>
public sealed class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;

    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();

        var response = await next(cancellationToken);

        timer.Stop();

        if (timer.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Long running request {RequestName} took {ElapsedMilliseconds} ms",
                typeof(TRequest).Name, timer.ElapsedMilliseconds);
        }

        return response;
    }
}
