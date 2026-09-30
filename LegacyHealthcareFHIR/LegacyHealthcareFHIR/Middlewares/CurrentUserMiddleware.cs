using LegacyHealthcareFHIR.Core.Models;

namespace LegacyHealthcareFHIR.Web.Middlewares;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, CurrentUser currentUser)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var hospitalIdValue = context.User.FindFirst("hospitalId")?.Value;
            var userIdValue = context.User.FindFirst("sub")?.Value;
            var username = context.User.FindFirst("username")?.Value;
            var hospitalName = context.User.FindFirst("hospitalName")?.Value;

            if (!int.TryParse(hospitalIdValue, out var hospitalId) || hospitalId <= 0 || !int.TryParse(userIdValue, out var userId) || userId <= 0 || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(hospitalName))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid user claims." });
                return;
            }

            currentUser.SetHospitalId(hospitalId);
            currentUser.SetUserId(userId);
            currentUser.SetUsername(username);
            currentUser.SetHospitalName(hospitalName);
        }

        await _next(context);
    }
}
