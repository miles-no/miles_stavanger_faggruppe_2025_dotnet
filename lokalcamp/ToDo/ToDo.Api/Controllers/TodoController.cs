using Microsoft.AspNetCore.Mvc;
using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController(ITodoService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto todo)
        {
            return Ok(await service.AddAsync(todo));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto todo)
        {
            return Ok(await service.UpdateAsync(id, todo));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            service.Delete(id);
            return Ok();
        }
    }
}