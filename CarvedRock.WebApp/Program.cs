using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CarvedRock.WebApp;
// using NLog;
// using NLog.Web;
using Serilog;
using Serilog.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog.Enrichers.Span;
using System.Diagnostics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
//builder.Logging.AddJsonConsole();
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
        b.AddHttpClientInstrumentation();
        b.AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://localhost:4317"); });
    });

//NLog.LogManager.Setup().LoadConfigurationFromFile();
//builder.Host.UseNLog();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";    
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://demo.duendesoftware.com";
    options.ClientId = "interactive.confidential";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "email"
    };    
    options.SaveTokens = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks()
    .AddIdentityServer(new Uri("https://demo.duendesoftware.com"), failureStatus: HealthStatus.Degraded);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseExceptionHandler("/Error");

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//app.UseHsts();
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();
app.MapHealthChecks("health").AllowAnonymous();

app.Run();
