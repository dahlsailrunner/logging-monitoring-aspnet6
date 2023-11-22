using System.Net.Http.Headers;
using CarvedRock.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarvedRock.WebApp.Pages
{
    public partial class ListingModel : PageModel
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<ListingModel> _logger;
        private readonly HttpContext? _httpCtx;
        
        public ListingModel(HttpClient apiClient, ILogger<ListingModel> logger, 
                IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _apiClient = apiClient;
            _apiClient.BaseAddress = new Uri("https://localhost:7213");
            _httpCtx = httpContextAccessor.HttpContext;
        }

        public List<Product> Products { get; set; } = new List<Product>();
        public string CategoryName { get; set; } = "";

        //[LoggerMessage(0, LogLevel.Warning, "API failure: {fullPath} Response: {statusCode}, Trace: {traceId}")]
        [LoggerMessage(LogLevel.Warning, "API failure: {fullPath} Response: {statusCode}, Trace: {traceId}")]
        partial void LogApiFailure(string fullPath, int statusCode, string traceId);
        public async Task OnGetAsync()
        {
           _logger.LogInformation("Making API call to get products...");
            var cat = Request.Query["cat"].ToString();
            if (string.IsNullOrEmpty(cat))
            {
                throw new Exception("failed");
            }

            if (_httpCtx != null)
            {
                var accessToken = await _httpCtx.GetTokenAsync("access_token");
                _apiClient.DefaultRequestHeaders.Authorization = 
                                new AuthenticationHeaderValue("Bearer", accessToken);
                // for a better way to include and manage access tokens for API calls:
                // https://identitymodel.readthedocs.io/en/latest/aspnetcore/web.html
            }

            var response = await _apiClient.GetAsync($"Product?category={cat}");
            if (!response.IsSuccessStatusCode)
            {      
                 var fullPath = $"{_apiClient.BaseAddress}Product?category={cat}";                
                
                // trace id
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>() ??
                  new ProblemDetails();
                var traceId = details.Extensions["traceId"]?.ToString();

                LogApiFailure(fullPath, (int) response.StatusCode, traceId ?? "");
               
                //_logger.LogWarning(
                //    "API failure: {fullPath} Response: {apiResponse}, Trace: {trace}, User: {user}",
                //  fullPath, (int) response.StatusCode, traceId, userName);        

                throw new Exception("API call failed!");
            }

            Products = await response.Content.ReadFromJsonAsync<List<Product>>() ?? new List<Product>();
            if (Products.Any())
            {
                CategoryName = Products.First().Category.First().ToString().ToUpper() +
                               Products.First().Category[1..];
            }
        }
    }
}
