using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Shared.Dtos;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Web.Http;
using Web.Models.Auth;

namespace Web.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ApiClient apiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> LoginAsync(LoginViewModel model)
        {
            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var result = await _apiClient.PostAsync<LoginDto, LoginResponseDto>("auth/login", dto);

            if (result.IsFailure)
                return Result.Failure(result.Errors);

            var accessToken = result.Value.AccessToken;
            var jwtToken = ReadJwtToken(accessToken);
            var principal = CreatePrincipalFromJwt(jwtToken);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = jwtToken.ValidTo
                });

            _httpContextAccessor.HttpContext.Session.SetString("AccessToken", accessToken);

            return Result.Success();
        }

        public async Task<Result> RegisterAsync(RegisterViewModel model)
        {
            var dto = new RegisterDto
            {
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                Nacionalidad = model.Nacionalidad,
                Identificacion = model.Identificacion,
                Email = model.Email,
                Password = model.Password,
            };

            var result = await _apiClient.PostAsync("auth/register", dto);
            if (!result.IsSuccess)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var dto = new ForgotPasswordDto
            {
                Email = model.Email
            };

            var result = await _apiClient.PostAsync("auth/forgot-password", dto);
            if (!result.IsSuccess)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var dto = new ResetPasswordDto
            {
                Token = model.Token,
                Password = model.Password
            };

            var result = await _apiClient.PostAsync("auth/reset-password", dto);
            if (!result.IsSuccess)
                return Result.Failure(result.Errors);

            return Result.Success();
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _httpContextAccessor.HttpContext.Session.Remove("AccessToken");
        }

        #region Private methods
        private static JwtSecurityToken ReadJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }

        private static ClaimsPrincipal CreatePrincipalFromJwt(JwtSecurityToken jwtToken)
        {
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
        #endregion
    }
}