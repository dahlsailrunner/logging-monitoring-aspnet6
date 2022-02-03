using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CarvedRock.WebApp;
// using NLog;
// using NLog.Web;
using Serilog;
using Serilog.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
//builder.Logging.AddSimpleConsole();
builder.Services.AddApplicationInsightsTelemetry();

builder.Host.UseSerilog((context, loggerConfig) => {
    loggerConfig
    .WriteTo.Console()
    .Enrich.WithExceptionDetails()
    .WriteTo.Seq("http://localhost:5341");
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

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
