using Microsoft.AspNetCore.Mvc;
using ToDo.Business.Services;
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

        [HttpGet("leaderboard/{numberOfUsersToFetch}")]
        public async Task<IActionResult> GetLeaderboard(int numberOfUsersToFetch = 10)
        {
            return Ok(await service.GetLeaderboardAsync(numberOfUsersToFetch));
        }
    }
}