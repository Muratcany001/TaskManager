using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetActiveUsersAsync();
        Task<User> GetUserWithRolesAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
