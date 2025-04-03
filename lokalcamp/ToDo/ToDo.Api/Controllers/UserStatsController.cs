using Microsoft.AspNetCore.Mvc;
using ToDo.Business.Services.Interfaces;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserStatsController(IUserStatsService service) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await service.GetByUserIdAsync(id));
        }
    }
}