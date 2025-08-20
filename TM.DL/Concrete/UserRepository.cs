using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Concrete
{
    public class UserRepository  : BaseRepository<User>, IUserRepository
    {
        private readonly Context _context;

        public UserRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await GetAsync(x => x.Email == email && x.IsActive);
        }

        public async Task<List<User>> GetActiveUsersAsync()
        {
            return await GetListAsync(x => x.IsActive);
        }

        public async Task<User> GetUserWithRolesAsync(int id)
        {
            return await GetAsync(
                x => x.Id == id,
                includeFunc: query => query.Include(u => u.Roles).ThenInclude(ur => ur.Role)
            );
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await ExistsAsync(x => x.Email == email);
        }
    }
}

