using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface IUserRepository
    {
        User CreateUser(User user);
        User DeleteUserById(int id);
        User UpdatePasswordById(int id, string password);
        List<User> GetAllUsers();
        User GetUserById(int id);
        User Login(string username, string password);
        User GetUserByEmail(string email);
    }
}
