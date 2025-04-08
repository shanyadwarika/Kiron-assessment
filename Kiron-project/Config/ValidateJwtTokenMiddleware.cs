using Microsoft.Extensions.Primitives;

public static class ValidateJwtTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseValidateJwtToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidateJwtTokenMiddleware>();
    }
}

public class ValidateJwtTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAppSettingsService _appSettingsService;

    public ValidateJwtTokenMiddleware(RequestDelegate next, IAppSettingsService appSettingsService)
    {
        _next = next;
        _appSettingsService = appSettingsService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        bool isValid = false;
        string message = String.Empty;

        context.Request.Headers.TryGetValue("token", out StringValues token);

        if (String.IsNullOrEmpty(token))
        {
            message = "No token in http headers";
        }
        else
        {
            isValid = true;
        }

        if (isValid)
        {
            await this._next(context);
        }
        else
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(message);
        }
    }
}
