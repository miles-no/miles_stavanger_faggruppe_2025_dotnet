using Microsoft.AspNetCore.Mvc;
using TaskManager.Common.Models;
using TaskManager.Service.Services;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(ICommentService commentService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int todoListItemId)
        {
            var comments = await commentService.GetCommentsAsync(todoListItemId);
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(Guid id)
        {
            var comment = await commentService.GetCommentAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(int todoListItemId, CreateComment comment)
        {
            await commentService.AddCommentAsync(todoListItemId, comment);
            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, Comment comment)
        {
            try
            {
                await commentService.UpdateCommentAsync(id, comment);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                await commentService.DeleteCommentAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
