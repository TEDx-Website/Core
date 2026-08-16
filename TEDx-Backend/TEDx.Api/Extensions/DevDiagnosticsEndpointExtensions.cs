using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TEDx.Api.Common.Responses;

namespace TEDx.Api.Extensions;

/// <summary>
/// Development-only diagnostics. These endpoints are never mapped outside Development,
/// so the routes simply do not exist in any other environment.
/// </summary>
/// <remarks>
/// Temporary: T-AUTH-02 needs an <c>[Authorize]</c>-protected endpoint to prove the bearer
/// pipeline, and the real one (<c>GET /api/v1/me</c>) belongs to a later task. Delete this
/// file once that endpoint exists.
/// </remarks>
public static class DevDiagnosticsEndpointExtensions
{
    public static WebApplication MapDevDiagnostics(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.MapGet("/api/v1/auth/whoami", ([FromServices] IHttpContextAccessor accessor) =>
        {
            var user = accessor.HttpContext!.User;

            return Results.Ok(ApiResponse<object>.SuccessResult(new
            {
                accountId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                email = user.FindFirstValue(ClaimTypes.Email),
                globalRole = user.FindFirstValue(ClaimTypes.Role),
                claimTypes = user.Claims.Select(c => c.Type).Distinct().ToArray(),
            }));
        })
        .RequireAuthorization()
        .WithName("DevWhoAmI")
        .WithTags("Dev");

        return app;
    }
}
