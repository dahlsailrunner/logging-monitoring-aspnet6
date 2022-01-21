using System.Text.RegularExpressions;

namespace CarvedRock.WebApp;
public class UserScopeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserScopeMiddleware> _logger;

    public UserScopeMiddleware(RequestDelegate next, ILogger<UserScopeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var user = context.User;
            var pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
            var maskedUsername = Regex.Replace(user.Identity.Name??"", pattern, m => new string('*', m.Length));

            var subjectId = user.Claims.First(c=> c.Type == "sub")?.Value;
                        
            using (_logger.BeginScope("User:{user}, SubjectId:{subject}", maskedUsername, subjectId))
            {
                await _next(context);    
            }
        }
        else
        {
            await _next(context);
        }
    }
}