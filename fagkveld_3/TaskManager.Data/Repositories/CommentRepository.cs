using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Entities;

namespace TaskManager.Data.Repositories;

public class CommentRepository(TaskManagerContext context) : ICommentRepository
{

    public async Task<IEnumerable<CommentEntity>> GetCommentsAsync(int todoItemId)
    {
        return await context.Comments.Where(c => c.TodoItemId == todoItemId).ToListAsync();
    }

    public async Task<CommentEntity> GetCommentAsync(Guid id)
    {
        return await context.Comments.FindAsync(id);
    }

    public async Task AddCommentAsync(CommentEntity comment)
    {
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCommentAsync(CommentEntity comment)
    {
        context.Entry(comment).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(Guid id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment != null)
        {
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> CommentExistsAsync(Guid id)
    {
        return await context.Comments.AnyAsync(e => e.Id == id);
    }
}