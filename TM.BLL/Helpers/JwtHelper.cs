using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.Services.UserService;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Helpers
{
    public class JwtHelper
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        public JwtHelper(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }
        public async Task<string> GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("userId", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}