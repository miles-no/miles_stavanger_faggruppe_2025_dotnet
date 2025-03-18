using Microsoft.AspNetCore.Mvc;
using TaskManager.Common.Models;
using TaskManager.Service.Services;

namespace TaskManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController(ITodoItemService service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var items = await service.GetTodoItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetToDoItem(int id)
        {
            var toDoItem = await service.GetTodoItemAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return Ok(toDoItem);
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            await service.AddTodoItemAsync(todoItem);
            return CreatedAtAction(nameof(GetToDoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            if (!await service.TodoItemExistsAsync(id))
            {
                return NotFound();
            }

            await service.UpdateTodoItemAsync(todoItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            if (!await service.TodoItemExistsAsync(id))
            {
                return NotFound();
            }

            await service.DeleteTodoItemAsync(id);
            return NoContent();
        }
    }
}