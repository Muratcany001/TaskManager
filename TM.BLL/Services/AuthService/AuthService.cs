using Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.Services.UserService;
using TM.BLL.ViewModels;
using TM.DAL.Abstract;

namespace TM.BLL.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        public Task<ResultViewModel<string>> Login(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultViewModel<string>> Register(RegisterDto registerDto)
        {
            throw new NotImplementedException();
        }
    }
}
