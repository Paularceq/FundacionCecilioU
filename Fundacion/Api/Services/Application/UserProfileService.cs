using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Shared.Dtos;
using Shared.Models;

namespace Api.Services.Application
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        public UserProfileService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }
        public async Task<Result<UserProfileDto>> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<UserProfileDto>.Failure("Usuario no encontrado.");
            }
            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                Email = user.Email,
                Nacionalidad = user.Nacionalidad,
                Identificacion = user.Identificacion
            };
            return Result<UserProfileDto>.Success(userProfileDto);
        }
        public async Task<Result> UpdateUserProfileAsync(UserProfileUpdateDto updateDto)
        {
            var user = await _userRepository.GetUserByIdAsync(updateDto.Id);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }
            var otherUser = await _userRepository.GetUserByEmailAsync(updateDto.Email);
            if (otherUser != null && otherUser.Email != user.Email)
            {
                return Result.Failure("El correo electrónico ya está en uso.");
            }
            otherUser = await _userRepository.GetUserByIdentificacionAsync(updateDto.Identificacion);
            if (otherUser != null && otherUser.Identificacion != user.Identificacion)
            {
                return Result.Failure("La identificacion ya está en uso.");
            }
            user.Nombre = updateDto.Nombre;
            user.Apellidos = updateDto.Apellidos;
            user.Email = updateDto.Email;
            user.Nacionalidad = updateDto.Nacionalidad;
            user.Identificacion = updateDto.Identificacion;
            await _userRepository.UpdateUserAsync(user);
            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetUserByIdAsync(changePasswordDto.UserId);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }
            if (!_passwordService.VerifyPassword(changePasswordDto.OldPassword, user.PasswordHash))
            {
                return Result.Failure("La contraseña actual es incorrecta.");
            }
            user.PasswordHash = _passwordService.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateUserAsync(user);
            return Result.Success();
        }
    }
}
