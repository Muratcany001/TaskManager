using Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.Helpers;
using TM.BLL.Services.HashService;
using TM.BLL.Services.TokenService;
using TM.BLL.Services.UserService;
using TM.BLL.ViewModels;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly ITokenService _tokenService;
        private readonly JwtHelper _jwtHelper;
        public AuthService(IUserRepository userRepository, IUserService userService, IHashService hashService, JwtHelper jwtHelper, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _userService = userService;
            _hashService = hashService;
            _jwtHelper = jwtHelper;
            _tokenService = tokenService;
        }

        public async Task<ResultViewModel<string>> Login(LoginDto loginDto)
        {
            var user = await _userService.GetByEmailAsync(loginDto.Email);
            if(user == null)
            {
                return ResultViewModel<string>.NotFound("User not found",404);
            }
            var password = _hashService.VerifyPassword(loginDto.Password , user.Data.Password);
            if(password == false)
            {
                return ResultViewModel<string>.NotFound("User not found", 404);
            }

            var tokenString = await _jwtHelper.GenerateJwtToken(user.Data);

            var tokenEntity = new Token
            {
                UserId = user.Data.Id,
                TokenString = tokenString,
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddHours(1),
                IsActive = true
            };
            await _tokenService.CreateTokenAsync(tokenEntity);
            return ResultViewModel<string>.Success(tokenString,"",200);
            
        }  
    }
}
