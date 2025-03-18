using TaskManager.Common.Models;
using TaskManager.Data.Entities;
using TaskManager.Data.Repositories;

namespace TaskManager.Service.Services
{
    public class CommentService(ICommentRepository commentRepository) : ICommentService
    {
        public async Task<IEnumerable<Comment>> GetCommentsAsync(int todoListItemId)
        {
            var commentEntities = await commentRepository.GetCommentsAsync(todoListItemId);
            return commentEntities.Select(entity => new Comment
            {
                Id = entity.Id,
                Text = entity.Text
                // Map other properties as needed
            });
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            var commentEntity = await commentRepository.GetCommentAsync(id);
            return new Comment
            {
                Id = commentEntity.Id,
                Text = commentEntity.Text
                // Map other properties as needed
            };
        }

        public async Task AddCommentAsync(int todoListItemId, CreateComment comment)
        {
            var commentEntity = new CommentEntity
            {
                Id = Guid.NewGuid(),
                Text = comment.Text,
                TodoItemId = todoListItemId
            };

            await commentRepository.AddCommentAsync(commentEntity);
        }

        public async Task UpdateCommentAsync(Guid id, Comment comment)
        {
            if (id != comment.Id)
            {
                throw new ArgumentException("ID mismatch");
            }

            if (!await commentRepository.CommentExistsAsync(id))
            {
                throw new KeyNotFoundException("Comment not found");
            }

            var commentEntity = new CommentEntity
            {
                Id = comment.Id,
                Text = comment.Text
                // Map other properties as needed
            };

            await commentRepository.UpdateCommentAsync(commentEntity);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            if (!await commentRepository.CommentExistsAsync(id))
            {
                throw new KeyNotFoundException("Comment not found");
            }

            await commentRepository.DeleteCommentAsync(id);
        }
    }
}
