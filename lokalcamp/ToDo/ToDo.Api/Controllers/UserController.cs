using Microsoft.AspNetCore.Mvc;
using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDto user)
        {
            return Ok(await userService.AddAsync(user));
        }
    }
}
