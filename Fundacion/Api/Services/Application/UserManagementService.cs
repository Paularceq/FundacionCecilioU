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
        public async Task<Result<IEnumerable<UserToListDto>>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userDtos = users.Select(u => new UserToListDto
            {
                Id = u.Id,
                Activo = u.Activo,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Nacionalidad = u.Nacionalidad,
                Identificacion = u.Identificacion
            });
            return Result<IEnumerable<UserToListDto>>.Success(userDtos);

        }
        public async Task<Result> AddUserAsync(NewUserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return Result.Failure("El correo electrónico ya está en uso.");
            }
            existingUser = await _userRepository.GetUserByIdentificacionAsync(userDto.Identificacion);
            if (existingUser != null)
            {
                return Result.Failure("La identificacion ya está en uso.");
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
        public async Task<Result<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            var roleDtos = roles.Select(r => new RoleDto
            {
                Name = r.Name,
                Description = r.Description
            });
            return Result<IEnumerable<RoleDto>>.Success(roleDtos);
        }
        public async Task<Result> UpdateUserAsync(UpdateUserDto userDto)
        {
            var userToUpdate = await _userRepository.GetUserByIdAsync(userDto.Id);
            if (userToUpdate == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }
            var otherUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (otherUser != null && otherUser.Email != userToUpdate.Email)
            {
                return Result.Failure("El correo electrónico ya está en uso.");
            }
            otherUser = await _userRepository.GetUserByIdentificacionAsync(userDto.Identificacion);
            if (otherUser != null && otherUser.Identificacion != userToUpdate.Identificacion)
            {
                return Result.Failure("La identificacion ya está en uso.");
            }
            userToUpdate.Nombre = userDto.Nombre;
            userToUpdate.Apellidos = userDto.Apellidos;
            userToUpdate.Email = userDto.Email;
            userToUpdate.Nacionalidad = userDto.Nacionalidad;
            userToUpdate.Identificacion = userDto.Identificacion;
            var roles = await _roleRepository.GetRolesByNamesAsync(userDto.Roles);
            userToUpdate.Roles = roles.ToList();
            await _userRepository.UpdateUserAsync(userToUpdate);
            return Result.Success();
        }
        public async Task<Result<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Result<UserDto>.Failure("Usuario no encontrado.");
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Activo = user.Activo,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                Email = user.Email,
                Nacionalidad = user.Nacionalidad,
                Identificacion = user.Identificacion,
                Roles = user.Roles.Select(r => new RoleDto
                {
                    Name = r.Name,
                    Description = r.Description
                }).ToList()
            };
            return Result<UserDto>.Success(userDto);
        }
        public async Task<Result> ChangeUserStatus(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }
            user.Activo = !user.Activo;
            await _userRepository.UpdateUserAsync(user);
            return Result.Success();

        }

        public async Task<Result<IEnumerable<UserToListDto>>> GetUsersByRole(string roleName)
        {
            var users = await _userRepository.GetUsersByRole(roleName);
            var userDtos = users.Select(u => new UserToListDto
            {
                Id = u.Id,
                Activo = u.Activo,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Nacionalidad = u.Nacionalidad,
                Identificacion = u.Identificacion
            });
            return Result<IEnumerable<UserToListDto>>.Success(userDtos);
        }
    }
}
