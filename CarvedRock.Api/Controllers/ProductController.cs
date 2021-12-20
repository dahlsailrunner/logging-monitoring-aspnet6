using CarvedRock.Data.Entities;
using CarvedRock.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductLogic _productLogic;

    public ProductController(ILogger<ProductController> logger, IProductLogic productLogic)
    {
        _logger = logger;
        _productLogic = productLogic;
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> Get(string category = "all")
    {
        _logger.LogInformation("Getting products in API for {category}", category);
        return await _productLogic.GetProductsForCategoryAsync(category);
        //return _productLogic.GetProductsForCategory(category);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        //var product = await _productLogic.GetProductByIdAsync(id);
        var product = _productLogic.GetProductById(id);
        if (product != null)
        {
            return Ok(product);
        }
        return NotFound();
    }
}