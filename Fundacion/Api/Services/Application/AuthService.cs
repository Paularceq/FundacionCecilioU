using Api.Abstractions.Application;
using Api.Abstractions.Infrastructure;
using Api.Abstractions.Repositories;
using Api.Database.Entities;
using Shared.Constants;
using Shared.Dtos;
using Shared.Models;

namespace Api.Services.Application
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _configuration = configuration;
        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result.Failure("El correo electrónico ya está en uso.");
            }
            existingUser = await _userRepository.GetUserByIdentificacionAsync(registerDto.Identificacion);
            if (existingUser != null)
            {
                return Result.Failure("La identificación ya está en uso.");
            }

            var newUser = new User
            {
                Nombre = registerDto.Nombre,
                Apellidos = registerDto.Apellidos,
                Email = registerDto.Email,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                Nacionalidad = registerDto.Nacionalidad,
                Identificacion = registerDto.Identificacion,
                RequiereCambioDePassword = false,
                Activo = false // El usuario estará inactivo hasta que verifique su cuenta
            };
            var assignedRole = registerDto.Role == Roles.Voluntario ? Roles.Voluntario : Roles.Estudiante;
            var role = await _roleRepository.GetRoleByNameAsync(assignedRole);

            newUser.Roles.Add(role);

            await _userRepository.AddUserAsync(newUser);

            // Generar token de verificación
            var verificationToken = _jwtService.GenerateForgotPasswordToken(newUser.Id);

            // Enviar correo de verificación
            var subject = "Verificación de cuenta";
            var header = "Verifica tu cuenta";
            var body = $@"
                <p>Hola {newUser.Nombre} {newUser.Apellidos},</p>
                <p>Gracias por registrarte en nuestra plataforma. Antes de comenzar, necesitamos que verifiques tu cuenta.</p>
                <p>Haz clic en el siguiente enlace para verificar tu cuenta:</p>
                <p><a href='{_configuration["WebSettings:AccountVerificationUrl"]}?token={verificationToken}'>Verificar cuenta</a></p>
                <p>Si no realizaste este registro, puedes ignorar este correo.</p>";

            var emailContent = await _emailTemplateService.RenderTemplateAsync(subject, header, body);
            await _emailService.SendEmailAsync(newUser.Email, subject, emailContent);

            return Result.Success();
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Result<LoginResponseDto>.Failure("Credenciales inválidas.");
            }
            if (!user.Activo)
            {
                return Result<LoginResponseDto>.Failure("El usuario está inactivo o no ha verificado su cuenta.");
            }
            if (!_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Result<LoginResponseDto>.Failure("Credenciales inválidas.");
            }

            var userRoles = user.Roles.Select(r => r.Name).ToList();

            var token = _jwtService.GenerateAccessToken(user.Id, user.NombreCompleto, user.Email, userRoles);

            var response = new LoginResponseDto
            {
                AccessToken = token
            };

            return Result<LoginResponseDto>.Success(response);
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return Result.Failure("No existe un usuario con ese correo electrónico.");
            }

            var forgotPasswordToken = _jwtService.GenerateForgotPasswordToken(user.Id);

            var subject = "Restablecimiento de contraseña";
            var header = "Restablecimiento de contraseña";
            var body = $@"
                <p>Hola {user.NombreCompleto},</p>
                <p>Hemos recibido una solicitud para restablecer tu contraseña. Si no has realizado esta solicitud, puedes ignorar este correo.</p>
                <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
                <p><a href='{_configuration["WebSettings:ResetPasswordUrl"]}?token={forgotPasswordToken}'>Restablecer contraseña</a></p>";

            var emailContent = await _emailTemplateService.RenderTemplateAsync(subject, header, body);

            await _emailService.SendEmailAsync(user.Email, subject, emailContent);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var tokenValidationResult = _jwtService.ValidateVerificationToken(resetPasswordDto.Token);
            if (tokenValidationResult.IsFailure)
            {
                return Result.Failure(tokenValidationResult.Errors);
            }

            var userId = tokenValidationResult.Value;

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }

            if (_passwordService.VerifyPassword(resetPasswordDto.Password, user.PasswordHash))
            {
                return Result.Failure("La nueva contraseña no puede ser igual a la contraseña actual.");
            }

            user.PasswordHash = _passwordService.HashPassword(resetPasswordDto.Password);
            user.RequiereCambioDePassword = false;

            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }

        public async Task<Result> VerifyAccountAsync(string token)
        {
            var tokenValidationResult = _jwtService.ValidateVerificationToken(token);
            if (tokenValidationResult.IsFailure)
            {
                return Result.Failure("El token de verificación no es válido o ha expirado.");
            }

            var userId = tokenValidationResult.Value;
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("Usuario no encontrado.");
            }

            user.Activo = true;
            await _userRepository.UpdateUserAsync(user);

            return Result.Success();
        }
    }
}
