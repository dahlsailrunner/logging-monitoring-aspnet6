using Microsoft.Data.Sqlite;

namespace CarvedRock.Api;
public class CriticalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CriticalExceptionMiddleware> _logger;

    public CriticalExceptionMiddleware(RequestDelegate next, ILogger<CriticalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SqliteException sqlex)
        {
            if (sqlex.SqliteErrorCode == 551)
            {
                _logger.LogCritical(sqlex, "Fatal error occurred in database!!");
            }            
        }       
    }
}