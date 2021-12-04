using CarvedRock.Domain;
using CarvedRock.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarvedRock.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductLogic _productLogic;

    public ProductController(IProductLogic productLogic)
    {
        _productLogic = productLogic;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductModel>> Get(string category = "all")
    {
        return await _productLogic.GetProductsForCategory(category);
    }
}