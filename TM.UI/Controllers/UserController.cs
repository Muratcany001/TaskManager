using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;
using TM.DAL;
using Microsoft.AspNetCore.Identity.Data;
namespace TM.UI.Controllers
{
        [ApiController]
        public class UserController : ControllerBase
        {
            private readonly IUserRepository _userRepository;
            public UserController(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            [HttpPost("users/CreateUser")]
            public ActionResult CreateUser(User user)
            {
                if (user == null)
                {
                    return BadRequest("Geçersiz kullanıcı bilgileri");
                }
                if (string.IsNullOrEmpty(user.Name))
                {
                    return BadRequest("Geçersiz kullanıcı adı");
                }
                if (string.IsNullOrEmpty(user.Email))
                {
                    return BadRequest("Geçersiz email adresi");
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    return BadRequest("Geçersiz şifre");
                }
                user.Password = HashPasswordSHA256(user.Password);
                var createdUser = _userRepository.CreateUser(user);
                return Ok(createdUser);
            }

            [HttpPatch("users/UpdateUser/{id}")]
            public ActionResult UpdateUser(int id, [FromBody] string password)
            {
                if (password == null)
                {
                    return BadRequest("Geçersiz girdi");
                }
                var existedUser = _userRepository.GetUserById(id);
                if (existedUser == null)
                    return BadRequest("Geçerli kullanıcı bulunamadı");

                existedUser.Password = HashPasswordSHA256(password);
                var updatedUser = _userRepository.UpdatePasswordById(id, password);
                return Ok(updatedUser);
            }

            [HttpGet("users/GetAllUsers")]
            public List<User> GetAllUsers()
            {
                return _userRepository.GetAllUsers().ToList();
            }

            [HttpGet("users/GetUserById/{id}")]
            public ActionResult GetUserById(int id)
            {
                return Ok(_userRepository.GetUserById(id));
            }

            [HttpGet("users/GetUserByEmail/{email}")]
            public ActionResult GetUserByEmail(string email)
            {
                return Ok(_userRepository.GetUserByEmail(email));
            }

            [HttpDelete("users/DeleteUserById/{id}")]
            public ActionResult DeleteUserById(int id)
            {
                return Ok(_userRepository.DeleteUserById(id));
            }
            [HttpPost("users/Login")]
                public ActionResult Login([FromBody] LoginRequest request)
                {
                        var existedUser = _userRepository.Login(request.Email, request.Password);
                    if (existedUser == null)
                        return Unauthorized("Eposta veya şifre yanlış");
                    return Ok(new { Message = "Giriş başarılı", UserId = existedUser.Id });
        }

        private string HashPasswordSHA256(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    return Convert.ToBase64String(hashBytes);
                }
            }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

