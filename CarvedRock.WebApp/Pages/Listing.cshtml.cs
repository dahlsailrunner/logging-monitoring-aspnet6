using CarvedRock.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarvedRock.WebApp.Pages
{
    public class ListingModel : PageModel
    {
        private readonly HttpClient _apiClient;
        private readonly ILogger<ListingModel> _logger;

        public ListingModel(HttpClient apiClient, ILogger<ListingModel> logger)
        {
            _logger = logger;
            _apiClient = apiClient;            
            _apiClient.BaseAddress = new Uri("https://localhost:7213/");
        }

        public List<Product> Products { get; set; }
        public string CategoryName { get; set; } = "";

        public async Task OnGetAsync()
        {
            var cat = Request.Query["cat"].ToString();
            if (string.IsNullOrEmpty(cat))
            {
                throw new Exception("failed");
            }

            var response = await _apiClient.GetAsync($"Product?category={cat}");
            if (!response.IsSuccessStatusCode)
            {      
                 var fullPath = $"{_apiClient.BaseAddress}Product?category={cat}";                
                
                // trace id
                var details = await response.Content.ReadFromJsonAsync<ProblemDetails>() ??
                  new ProblemDetails();
                var traceId = details.Extensions["traceId"]?.ToString();

                _logger.LogWarning("API failure: {fullPath} Response: {response}, Trace: {trace}",
                  fullPath, (int) response.StatusCode, traceId);        

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
