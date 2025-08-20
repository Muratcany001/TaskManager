using AutoMapper;
using Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.Services.HashService;
using TM.BLL.ViewModels;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.UserService
{
    public class UserService : IUserService
    {
     private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterDto> _createUserValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<UpdatePasswordDto> _updatePasswordValidator;
        private readonly IHashService _hashService;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IValidator<RegisterDto> createUserValidator,
            IValidator<UpdateUserDto> updateUserValidator,
            IValidator<UpdatePasswordDto> updatePasswordValidator,
            IHashService hashService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _updatePasswordValidator = updatePasswordValidator;
            _hashService = hashService;
        }

        public async Task<ResultViewModel<UserDto>> CreateUserAsync(RegisterDto createUserDto)
        {
            // 1. Validation
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultViewModel<UserDto>.Failure("Lütfen girdiğiniz bilgileri kontrol edin.", errorMessages, 400);
            }

            // 2. Email kontrolü
            var emailExists = await _userRepository.EmailExistsAsync(createUserDto.Email);
            if (emailExists)
            {
                return ResultViewModel<UserDto>.Failure("Bu email adresi zaten kullanılıyor.");
            }

            // 3. User oluştur
            var user = _mapper.Map<User>(createUserDto);
            user.Password = _hashService.HashPassword(createUserDto.Password);
            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.AddAsync(user);

            var userDto = _mapper.Map<UserDto>(user);
            return ResultViewModel<UserDto>.Success(userDto, "Kullanıcı başarıyla oluşturuldu.", 201);
        }

        public async Task<ResultViewModel<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return ResultViewModel<UserDto>.Failure("Kullanıcı bulunamadı.", null, 404);
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ResultViewModel<UserDto>.Success(userDto);
        }

        public async Task<ResultViewModel<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetActiveUsersAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return ResultViewModel<List<UserDto>>.Success(userDtos);
        }

        public async Task<ResultViewModel<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            // 1. Validation
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultViewModel<UserDto>.Failure("Lütfen girdiğiniz bilgileri kontrol edin.", errorMessages, 400);
            }

            // 2. User kontrolü
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return ResultViewModel<UserDto>.Failure("Kullanıcı bulunamadı.", null, 404);
            }

            // 3. Email değişmişse kontrol et
            if (user.Email != updateUserDto.Email)
            {
                var emailExists = await _userRepository.EmailExistsAsync(updateUserDto.Email);
                if (emailExists)
                {
                    return ResultViewModel<UserDto>.Failure("Bu email adresi zaten kullanılıyor.");
                }
            }

            // 4. Update
            _mapper.Map(updateUserDto, user);
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);

            var userDto = _mapper.Map<UserDto>(user);
            return ResultViewModel<UserDto>.Success(userDto, "Kullanıcı başarıyla güncellendi.");
        }

        public async Task<ResultViewModel<object>> UpdatePasswordAsync(int id, UpdatePasswordDto updatePasswordDto)
        {
            // 1. Validation
            var validationResult = await _updatePasswordValidator.ValidateAsync(updatePasswordDto);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ResultViewModel<object>.Failure("Lütfen girdiğiniz bilgileri kontrol edin.", errorMessages, 400);
            }

            // 2. User kontrolü
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return ResultViewModel<object>.Failure("Kullanıcı bulunamadı.", null, 404);
            }

            // 3. Mevcut şifre kontrolü
            if (!_hashService.VerifyPassword(updatePasswordDto.CurrentPassowrd, user.Password))
            {
                return ResultViewModel<object>.Failure("Mevcut şifre hatalı.", null, 400);
            }

            // 4. Şifre güncelle
            user.Password = _hashService.HashPassword(updatePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);

            return ResultViewModel<object>.Success("Şifre başarıyla güncellendi.");
        }

        public async Task<ResultViewModel<object>> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || !user.IsActive)
            {
                return ResultViewModel<object>.Failure("Kullanıcı bulunamadı.", null, 404);
            }

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            await _userRepository.UpdateAsync(user);

            return ResultViewModel<object>.Success("Kullanıcı başarıyla silindi.");
        }

        public ResultViewModel<object> ValidateUser(string email, string password)
        {
            // Email format kontrolü
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                return ResultViewModel<object>.Failure("Geçersiz email formatı");
            }

            // Şifre güçlülük kontrolü
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return ResultViewModel<object>.Failure("Şifre en az 6 karakter olmalıdır");
            }

            return ResultViewModel<object>.Success("Validation başarılı");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
