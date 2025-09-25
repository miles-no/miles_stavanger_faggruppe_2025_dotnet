using Microsoft.AspNetCore.Mvc;
using Shop.Domain;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;

    public ProductsController(IProductRepository products)
    {
        _products = products;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get() => Ok(_products.GetAll());

    [HttpPost]
    public ActionResult<Product> Create([FromBody] Product product)
    {
        _products.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetById(Guid id)
    {
        var p = _products.Get(id);
        return p is null ? NotFound() : Ok(p);
    }
}
