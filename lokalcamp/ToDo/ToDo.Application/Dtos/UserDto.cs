using ToDo.Domain.Models;

namespace ToDo.Business.Dtos
{
    public class UserDto
    {
        public UserDto(User entity)
        {
            Id = entity.Id;
            UserName = entity.Username;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
    }
}
