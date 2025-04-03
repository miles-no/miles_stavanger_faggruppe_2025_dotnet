using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Business.Dtos;
using ToDo.Domain.Models;

namespace ToDo.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AddAsync(UserDto user);
    }
}
