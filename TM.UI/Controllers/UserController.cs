using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;
using TM.DAL;
using Microsoft.AspNetCore.Identity.Data;
using TM.BLL.Services.UserService;
using Dtos;
using System.Threading.Tasks;
using TM.BLL.Services.AuthService;
using TM.BLL.Helpers;
namespace TM.UI.Controllers
{
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly IUserRepository _userRepository;
            private readonly IUserService _userService;
            private readonly ILogger _logger;
            private readonly IAuthService _authService;
            private readonly HashHelper _hashHelper;
            public UserController(IUserRepository userRepository, IUserService userService, IAuthService authService, HashHelper hashHelper)
            {
                _userRepository = userRepository;
                _userService = userService;
                _authService = authService;
                _hashHelper = hashHelper;
            }

            [HttpPost("users/CreateUser")] 
            public async Task<ActionResult> CreateUser(RegisterDto registerDto)
            {
                if (registerDto == null)
                {
                    return BadRequest("Geçersiz kullanıcı bilgileri");
                }
                if (string.IsNullOrEmpty(registerDto.Name))
                {
                    return BadRequest("Geçersiz kullanıcı adı");
                }
                if (string.IsNullOrEmpty(registerDto.Email))
                {
                    return BadRequest("Geçersiz email adresi");
                }
                if (string.IsNullOrEmpty(registerDto.Password))
                {
                    return BadRequest("Geçersiz şifre");
                }
                var existedUser = await _userService.GetByEmailAsync(registerDto.Email);
                if (existedUser != null)
                {
                    return BadRequest("Kullanıcı zaten mevcut");
                }
                registerDto.Password = _hashHelper.HashPasswordSHA256(registerDto.Password);
                var createdUser = _userService.CreateUserAsync(registerDto);
                return Ok(createdUser);
            }

            [HttpPatch("users/UpdateUser/{id}")]
            public async Task<ActionResult> UpdateUser(int id, UpdatePasswordDto updatePasswordDto) // DTO adı değişti
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var result = await _userService.UpdatePasswordAsync(id, updatePasswordDto);

                    if (!result.IsSuccess)
                    {
                        return result.StatusCode switch
                        {
                            404 => NotFound(result.Message),
                            400 => BadRequest(result.Message),
                            _ => StatusCode(500, "Bir hata oluştu")
                        };
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    // Log exception
                    return StatusCode(500, "Sunucu hatası oluştu");
                }
            }

            [HttpGet("users/GetAllUsers")]
            public async Task<ActionResult> GetAllUsers()
            {
                try
                {
                    var result = await _userService.GetAllUsersAsync();

                    if (!result.IsSuccess)
                    {
                        return StatusCode(result.StatusCode, result.Message);
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    // Log exception
                    return StatusCode(500, "Sunucu hatası oluştu");
                }
            }

        [HttpGet("users/GetUserById/{id}")]
        public async Task<ActionResult> GetUserById(int id)
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }

            [HttpGet("users/GetUserByEmail/{email}")]
            public ActionResult GetUserByEmail(string email)
            {
                return Ok(_userService.GetByEmailAsync(email));
            }

            [HttpDelete("users/DeleteUserById/{id}")]
            public ActionResult DeleteUserById(int id)
            {
                return Ok(_userService.DeleteUserAsync(id));
            }
            [HttpPost("users/Login")]
                public ActionResult Login([FromBody] LoginDto loginDto)
                {
                        var existedUser = _authService.Login(loginDto);
                    if (existedUser == null)
                        return Unauthorized("Eposta veya şifre yanlış");
                    return Ok(new { Message = "Giriş başarılı", UserId = existedUser.Id });
        }
    }
}

