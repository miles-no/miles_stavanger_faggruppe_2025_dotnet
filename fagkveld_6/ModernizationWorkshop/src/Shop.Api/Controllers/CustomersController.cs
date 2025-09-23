using Microsoft.AspNetCore.Mvc;
using Shop.Domain;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customers;

    public CustomersController(ICustomerRepository customers)
    {
        _customers = customers;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Customer>> Get() => Ok(_customers.GetAll());

    [HttpPost]
    public ActionResult<Customer> Create([FromBody] Customer customer)
    {
        _customers.Add(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpGet("{id}")]
    public ActionResult<Customer> GetById(Guid id)
    {
        var c = _customers.Get(id);
        return c is null ? NotFound() : Ok(c);
    }
}
