using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Dtos;
using Shared.Models;

namespace Api.Services.Application
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IRoleRepository _roleRepository;

        private readonly IUserRepository _userRepository;
        public UserManagementService(IUserRepository userRepository, IPasswordService passwordService, IEmailService emailService, IEmailTemplateService emailTemplateService, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _roleRepository = roleRepository;
        }
        public async Task<Result<IEnumerable<UsertoListDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(u => new UsertoListDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Nacionalidad = u.Nacionalidad,
                Identificacion = u.Identificacion
            });
            return Result<IEnumerable<UsertoListDto>>.Success(userDtos);

        }
        public async Task<Result> AddUserAsync(NewUserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return Result.Failure("El correo electrónico ya está en uso.");
            }
            var temporaryPassword = _passwordService.GeneratePassword(8);
            var newUser = new User
            {
                Nombre = userDto.Nombre,
                Apellidos = userDto.Apellidos,
                Email = userDto.Email,
                PasswordHash = _passwordService.HashPassword(temporaryPassword),
                Nacionalidad = userDto.Nacionalidad,
                Identificacion = userDto.Identificacion,
                RequiereCambioDePassword = true,
            };
            var roles = await _roleRepository.GetRolesByNamesAsync(userDto.Roles);
            newUser.Roles = roles.ToList();
            await _userRepository.AddUserAsync(newUser);
            var subject = "Creacion de Usuario";
            var header = "Creacion de Usuario";
            var body = $@"
                <p>Hola {newUser.NombreCompleto},</p>
                <p>¡Bienvenido! Hemos creado tu cuenta exitosamente.</p>
                <p>Para acceder por primera vez, utiliza la siguiente contraseña temporal:</p>
                <p><strong>{temporaryPassword}</strong></p>
                <p>Te recomendamos cambiar esta contraseña después de iniciar sesión por primera vez.</p>
                <p>Si no solicitaste esta cuenta, puedes ignorar este mensaje.</p>
                <p>Gracias,<br>El equipo de soporte</p>";

            var emailContent = await _emailTemplateService.RenderTemplateAsync(subject, header, body);
            await _emailService.SendEmailAsync(newUser.Email, subject, emailContent);
            return Result.Success();
        }
    }
}
