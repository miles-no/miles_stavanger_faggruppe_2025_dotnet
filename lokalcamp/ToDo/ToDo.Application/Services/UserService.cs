using ToDo.Business.Dtos;
using ToDo.Business.Services.Interfaces;
using ToDo.Data.Repositories.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Business.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<UserDto> AddAsync(UserDto user)
        {
            var entity = new User { Id = user.Id, Username = user.UserName };
            await userRepository.AddAsync(entity);
            return new UserDto(entity);
        }
    }
}
