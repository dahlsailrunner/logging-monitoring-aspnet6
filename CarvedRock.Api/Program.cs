using System.IdentityModel.Tokens.Jwt;
using CarvedRock.Data;
using CarvedRock.Domain;
//using Hellang.Middleware.ProblemDetails;
using Microsoft.Data.Sqlite;
using CarvedRock.Api;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
// using NLog;
// using NLog.Web;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LocalContext>();

builder.Logging.ClearProviders();
// builder.Logging.AddDebug();
//builder.Logging.AddSimpleConsole();
//builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((context, loggerConfig) => {
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .WriteTo.Console()
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Seq("http://localhost:5341");
});

builder.Services.AddOpenTelemetry()
    .WithTracing(b => {
        b.ConfigureResource(r => r.AddService(builder.Environment.ApplicationName));
        b.AddAspNetCoreInstrumentation();
        b.AddEntityFrameworkCoreInstrumentation();
        b.AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
    });

//NLog.LogManager.Setup().LoadConfigurationFromFile();
//builder.Host.UseNLog();

// https://bit.ly/aspnetcore-problemdetails
builder.Services.AddProblemDetails(opts => // built-in problem details support
    opts.CustomizeProblemDetails = (ctx) =>
    {
        // exception will hold the thrown exception
        var exception = ctx.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (ctx.ProblemDetails.Status == 500)
        {
            ctx.ProblemDetails.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    }
);   

//builder.Services.AddProblemDetails(opts => // hellang problem details support
//{
//    opts.IncludeExceptionDetails = (ctx, ex) => false;
    
//    opts.OnBeforeWriteDetails = (ctx, dtls) => {
//        if (dtls.Status == 500)
//        {
//            dtls.Detail = "An error occurred in our API. Use the trace id when contacting us.";
//        }
//    }; 
//    opts.Rethrow<SqliteException>(); 
//    opts.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
//});

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        options.Audience = "api";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "email"
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<ICarvedRockRepository, CarvedRockRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LocalContext>();
    context.MigrateAndCreateData();
}
app.UseExceptionHandler();  // built-in problem details support
//app.UseMiddleware<CriticalExceptionMiddleware>();  // hellang problem details support
//app.UseProblemDetails(); // hellang problem details support

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("interactive.public.short");
        options.OAuthAppName("CarvedRock API");
        options.OAuthUsePkce();
    });
}
app.MapFallback(() => Results.Redirect("/swagger"));
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.MapHealthChecks("health").AllowAnonymous();

app.Run();
