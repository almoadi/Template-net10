using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Template_net10.API.Controllers;

/// <summary>
/// Base controller: exposes the MediatR <see cref="Sender"/> and the authenticated
/// <see cref="CurrentUserId"/>. Controllers only dispatch requests — no business logic.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;

    protected ISender Sender =>
        _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected int? CurrentUserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
