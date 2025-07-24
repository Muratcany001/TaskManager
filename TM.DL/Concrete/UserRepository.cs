using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Concrete
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        public UserRepository(Context context)
        {
            _context = context;
        }

        public User CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public User UpdatePasswordById(int id, string password)
        {
            var existedUser = _context.Users.Find(id);
            if (existedUser == null)
                throw new Exception("Kullanıcı bulunamadı");

            _context.SaveChanges();
            return existedUser;
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email);
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
        //public User UpdateUser(int id)
        //{
        //    var existedUser = _context.Users.FirstOrDefault(x => x.Id == id);
        //    return existedUser;
        //}
        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }
        public User DeleteUserById(int id)
        {
            var existedUser = _context.Users.FirstOrDefault(x => x.Id == id);
            _context.Users.Remove(existedUser);
            _context.SaveChanges();
            return existedUser;
        }
    }
}

