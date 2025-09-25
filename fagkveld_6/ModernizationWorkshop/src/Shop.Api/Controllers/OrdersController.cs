using Microsoft.AspNetCore.Mvc;
using Shop.Domain;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;

    public OrdersController(OrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Order>> Get() => Ok(_orders.GetAll());

    [HttpGet("{id}")]
    public ActionResult<Order> GetById(Guid id)
    {
        var o = _orders.Get(id);
        return o is null ? NotFound() : Ok(o);
    }

    [HttpPost]
    public ActionResult<Order> Create([FromBody] CreateOrderRequest request)
    {
        var created = _orders.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id}/process")]
    public async Task<ActionResult<IEnumerable<string>>> Process(Guid id, CancellationToken ct)
    {
        var steps = await _orders.ProcessOrderAsync(id, ct);
        return Ok(steps);
    }
}
