using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Template_net10.Application.Common.Models;
using Template_net10.Domain.Common.Exceptions;

namespace Template_net10.Infrastructure.Middleware;

/// <summary>
/// Translates exceptions thrown anywhere downstream into the standard
/// <see cref="ApiResponseDto{T}"/> failure envelope with the right HTTP status code.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, message, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors.Select(e => e.ErrorMessage).ToList()),
            BadRequestException br => (HttpStatusCode.BadRequest, br.Message, (List<string>?)null),
            ItemNotFoundException nf => (HttpStatusCode.NotFound, nf.Message, null),
            ForbiddenAccessException fa => (HttpStatusCode.Forbidden, fa.Message, null),
            TooManyRequestsException tm => (HttpStatusCode.TooManyRequests, tm.Message, null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null),
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var body = ApiResponseDto<object>.Failed(message, errors);
        await context.Response.WriteAsJsonAsync(body);
    }
}
