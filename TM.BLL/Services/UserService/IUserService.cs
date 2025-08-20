using Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ResultViewModel<UserDto>> CreateUserAsync(RegisterDto registerDto);
        Task<ResultViewModel<UserDto>> GetUserByIdAsync(int id);
        Task<ResultViewModel<List<UserDto>>> GetAllUsersAsync();
        Task<ResultViewModel<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<ResultViewModel<object>> DeleteUserAsync(int id);
        Task<ResultViewModel<object>> UpdatePasswordAsync(int id, UpdatePasswordDto updatePasswordDto);
        ResultViewModel<object> ValidateUser(string email, string password);
    }
}
